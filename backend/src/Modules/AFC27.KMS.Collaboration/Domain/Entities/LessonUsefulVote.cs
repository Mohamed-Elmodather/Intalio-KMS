namespace AFC27.KMS.Collaboration.Domain.Entities;

public class LessonUsefulVote
{
    public Guid LessonLearnedId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public virtual LessonLearned? LessonLearned { get; private set; }

    private LessonUsefulVote() { }

    public static LessonUsefulVote Create(Guid lessonLearnedId, Guid userId)
    {
        return new LessonUsefulVote
        {
            LessonLearnedId = lessonLearnedId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
