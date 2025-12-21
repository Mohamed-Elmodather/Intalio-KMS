using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.Interfaces;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service for CRDT (Conflict-free Replicated Data Type) operations.
/// This implementation provides a simplified CRDT-like structure for
/// collaborative editing. In production, consider using Y.js or Automerge
/// via interop for full CRDT capabilities.
/// </summary>
public class CRDTService : ICRDTService
{
    private readonly ILogger<CRDTService> _logger;

    public CRDTService(ILogger<CRDTService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Create a new empty CRDT document.
    /// </summary>
    public byte[] CreateDocument()
    {
        var document = new CRDTDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            Version = 1,
            Blocks = new List<CRDTBlock>(),
            StateVector = new Dictionary<string, long> { { "server", 0 } },
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        return SerializeDocument(document);
    }

    /// <summary>
    /// Apply an update to a document.
    /// </summary>
    public byte[] ApplyUpdate(byte[] documentBytes, byte[] updateBytes)
    {
        var document = DeserializeDocument(documentBytes);
        var update = DeserializeUpdate(updateBytes);

        // Apply operations from the update
        foreach (var operation in update.Operations)
        {
            ApplyOperation(document, operation);
        }

        // Update state vector
        foreach (var (clientId, clock) in update.StateVector)
        {
            if (!document.StateVector.ContainsKey(clientId) ||
                document.StateVector[clientId] < clock)
            {
                document.StateVector[clientId] = clock;
            }
        }

        document.Version++;
        document.ModifiedAt = DateTime.UtcNow;

        _logger.LogDebug(
            "Applied update with {OperationCount} operations to document {DocumentId}",
            update.Operations.Count, document.Id);

        return SerializeDocument(document);
    }

    /// <summary>
    /// Merge two documents.
    /// </summary>
    public byte[] MergeDocuments(byte[] doc1Bytes, byte[] doc2Bytes)
    {
        var doc1 = DeserializeDocument(doc1Bytes);
        var doc2 = DeserializeDocument(doc2Bytes);

        // Merge blocks using last-writer-wins strategy based on timestamps
        var mergedBlocks = new Dictionary<string, CRDTBlock>();

        foreach (var block in doc1.Blocks)
        {
            mergedBlocks[block.Id] = block;
        }

        foreach (var block in doc2.Blocks)
        {
            if (!mergedBlocks.ContainsKey(block.Id) ||
                block.ModifiedAt > mergedBlocks[block.Id].ModifiedAt)
            {
                mergedBlocks[block.Id] = block;
            }
        }

        // Merge state vectors (take max for each client)
        var mergedStateVector = new Dictionary<string, long>(doc1.StateVector);
        foreach (var (clientId, clock) in doc2.StateVector)
        {
            if (!mergedStateVector.ContainsKey(clientId) ||
                mergedStateVector[clientId] < clock)
            {
                mergedStateVector[clientId] = clock;
            }
        }

        var mergedDoc = new CRDTDocument
        {
            Id = doc1.Id,
            Version = Math.Max(doc1.Version, doc2.Version) + 1,
            Blocks = mergedBlocks.Values
                .Where(b => !b.IsDeleted)
                .OrderBy(b => b.Order)
                .ToList(),
            StateVector = mergedStateVector,
            CreatedAt = doc1.CreatedAt,
            ModifiedAt = DateTime.UtcNow
        };

        _logger.LogDebug(
            "Merged documents, result has {BlockCount} blocks",
            mergedDoc.Blocks.Count);

        return SerializeDocument(mergedDoc);
    }

    /// <summary>
    /// Get the current state vector for a document.
    /// </summary>
    public byte[] GetStateVector(byte[] documentBytes)
    {
        var document = DeserializeDocument(documentBytes);
        return JsonSerializer.SerializeToUtf8Bytes(document.StateVector);
    }

    /// <summary>
    /// Compute a diff update from a state vector.
    /// Returns operations that the requester is missing.
    /// </summary>
    public byte[] ComputeUpdate(byte[] documentBytes, byte[] stateVectorBytes)
    {
        var document = DeserializeDocument(documentBytes);
        var clientStateVector = JsonSerializer.Deserialize<Dictionary<string, long>>(stateVectorBytes)
            ?? new Dictionary<string, long>();

        // Find operations the client is missing
        var missingOperations = new List<CRDTOperation>();

        foreach (var block in document.Blocks)
        {
            var clientClock = clientStateVector.GetValueOrDefault(block.ClientId, -1);

            if (block.Clock > clientClock)
            {
                // Client is missing this block's latest state
                missingOperations.Add(new CRDTOperation
                {
                    Type = block.IsDeleted ? OperationType.Delete : OperationType.Update,
                    BlockId = block.Id,
                    Block = block.IsDeleted ? null : block,
                    Clock = block.Clock,
                    ClientId = block.ClientId
                });
            }
        }

        var update = new CRDTUpdate
        {
            Operations = missingOperations,
            StateVector = document.StateVector
        };

        return SerializeUpdate(update);
    }

    /// <summary>
    /// Extract text content from a CRDT document.
    /// </summary>
    public string GetTextContent(byte[] documentBytes, string key)
    {
        var document = DeserializeDocument(documentBytes);

        if (key == "all")
        {
            return string.Join("\n\n", document.Blocks
                .Where(b => !b.IsDeleted)
                .OrderBy(b => b.Order)
                .Select(b => b.Content));
        }

        var block = document.Blocks.FirstOrDefault(b => b.Id == key && !b.IsDeleted);
        return block?.Content ?? string.Empty;
    }

    /// <summary>
    /// Get all content blocks from a CRDT document.
    /// </summary>
    public IReadOnlyList<CRDTBlockData> GetBlocks(byte[] documentBytes)
    {
        var document = DeserializeDocument(documentBytes);

        return document.Blocks
            .Where(b => !b.IsDeleted)
            .OrderBy(b => b.Order)
            .Select(b => new CRDTBlockData
            {
                Id = b.Id,
                Type = b.Type,
                Content = b.Content,
                Order = b.Order,
                Metadata = b.Metadata,
                ParentId = b.ParentId
            })
            .ToList();
    }

    /// <summary>
    /// Create an update for inserting a new block.
    /// </summary>
    public byte[] CreateInsertUpdate(
        string clientId,
        long clock,
        string blockId,
        string blockType,
        string content,
        int order,
        string? metadata = null,
        string? parentId = null)
    {
        var operation = new CRDTOperation
        {
            Type = OperationType.Insert,
            BlockId = blockId,
            Block = new CRDTBlock
            {
                Id = blockId,
                Type = blockType,
                Content = content,
                Order = order,
                Metadata = metadata,
                ParentId = parentId,
                ClientId = clientId,
                Clock = clock,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            },
            Clock = clock,
            ClientId = clientId
        };

        var update = new CRDTUpdate
        {
            Operations = new List<CRDTOperation> { operation },
            StateVector = new Dictionary<string, long> { { clientId, clock } }
        };

        return SerializeUpdate(update);
    }

    /// <summary>
    /// Create an update for modifying a block.
    /// </summary>
    public byte[] CreateUpdateUpdate(
        string clientId,
        long clock,
        string blockId,
        string? content = null,
        string? metadata = null,
        int? order = null)
    {
        var operation = new CRDTOperation
        {
            Type = OperationType.Update,
            BlockId = blockId,
            Block = new CRDTBlock
            {
                Id = blockId,
                Content = content ?? string.Empty,
                Metadata = metadata,
                Order = order ?? 0,
                ClientId = clientId,
                Clock = clock,
                ModifiedAt = DateTime.UtcNow
            },
            Clock = clock,
            ClientId = clientId
        };

        var update = new CRDTUpdate
        {
            Operations = new List<CRDTOperation> { operation },
            StateVector = new Dictionary<string, long> { { clientId, clock } }
        };

        return SerializeUpdate(update);
    }

    /// <summary>
    /// Create an update for deleting a block.
    /// </summary>
    public byte[] CreateDeleteUpdate(string clientId, long clock, string blockId)
    {
        var operation = new CRDTOperation
        {
            Type = OperationType.Delete,
            BlockId = blockId,
            Clock = clock,
            ClientId = clientId
        };

        var update = new CRDTUpdate
        {
            Operations = new List<CRDTOperation> { operation },
            StateVector = new Dictionary<string, long> { { clientId, clock } }
        };

        return SerializeUpdate(update);
    }

    private void ApplyOperation(CRDTDocument document, CRDTOperation operation)
    {
        switch (operation.Type)
        {
            case OperationType.Insert:
                if (operation.Block != null)
                {
                    // Check for duplicate
                    var existing = document.Blocks.FirstOrDefault(b => b.Id == operation.BlockId);
                    if (existing == null)
                    {
                        document.Blocks.Add(operation.Block);
                    }
                }
                break;

            case OperationType.Update:
                var blockToUpdate = document.Blocks.FirstOrDefault(b => b.Id == operation.BlockId);
                if (blockToUpdate != null && operation.Block != null)
                {
                    // Only apply if this update is newer
                    if (operation.Clock > blockToUpdate.Clock ||
                        (operation.Clock == blockToUpdate.Clock &&
                         string.Compare(operation.ClientId, blockToUpdate.ClientId, StringComparison.Ordinal) > 0))
                    {
                        if (!string.IsNullOrEmpty(operation.Block.Content))
                            blockToUpdate.Content = operation.Block.Content;
                        if (operation.Block.Metadata != null)
                            blockToUpdate.Metadata = operation.Block.Metadata;
                        if (operation.Block.Order != 0)
                            blockToUpdate.Order = operation.Block.Order;

                        blockToUpdate.Clock = operation.Clock;
                        blockToUpdate.ClientId = operation.ClientId;
                        blockToUpdate.ModifiedAt = DateTime.UtcNow;
                    }
                }
                break;

            case OperationType.Delete:
                var blockToDelete = document.Blocks.FirstOrDefault(b => b.Id == operation.BlockId);
                if (blockToDelete != null)
                {
                    blockToDelete.IsDeleted = true;
                    blockToDelete.Clock = operation.Clock;
                    blockToDelete.ClientId = operation.ClientId;
                    blockToDelete.ModifiedAt = DateTime.UtcNow;
                }
                break;
        }
    }

    private static byte[] SerializeDocument(CRDTDocument document)
    {
        return JsonSerializer.SerializeToUtf8Bytes(document, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private static CRDTDocument DeserializeDocument(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return new CRDTDocument
            {
                Id = Guid.NewGuid().ToString("N"),
                Version = 1,
                Blocks = new List<CRDTBlock>(),
                StateVector = new Dictionary<string, long>(),
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
        }

        return JsonSerializer.Deserialize<CRDTDocument>(bytes, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }) ?? throw new InvalidOperationException("Failed to deserialize CRDT document");
    }

    private static byte[] SerializeUpdate(CRDTUpdate update)
    {
        return JsonSerializer.SerializeToUtf8Bytes(update, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private static CRDTUpdate DeserializeUpdate(byte[] bytes)
    {
        return JsonSerializer.Deserialize<CRDTUpdate>(bytes, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }) ?? throw new InvalidOperationException("Failed to deserialize CRDT update");
    }
}

/// <summary>
/// Internal CRDT document structure.
/// </summary>
internal class CRDTDocument
{
    public string Id { get; set; } = string.Empty;
    public int Version { get; set; }
    public List<CRDTBlock> Blocks { get; set; } = new();
    public Dictionary<string, long> StateVector { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

/// <summary>
/// Internal CRDT block structure.
/// </summary>
internal class CRDTBlock
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = "Paragraph";
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? Metadata { get; set; }
    public string? ParentId { get; set; }
    public string ClientId { get; set; } = "server";
    public long Clock { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

/// <summary>
/// CRDT update containing operations to apply.
/// </summary>
internal class CRDTUpdate
{
    public List<CRDTOperation> Operations { get; set; } = new();
    public Dictionary<string, long> StateVector { get; set; } = new();
}

/// <summary>
/// A single CRDT operation.
/// </summary>
internal class CRDTOperation
{
    public OperationType Type { get; set; }
    public string BlockId { get; set; } = string.Empty;
    public CRDTBlock? Block { get; set; }
    public long Clock { get; set; }
    public string ClientId { get; set; } = string.Empty;
}

/// <summary>
/// Types of CRDT operations.
/// </summary>
internal enum OperationType
{
    Insert,
    Update,
    Delete
}
