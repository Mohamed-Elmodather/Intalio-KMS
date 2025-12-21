using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFC27.KMS.Notifications;

/// <summary>
/// Notifications module configuration
/// </summary>
public static class NotificationsModule
{
    /// <summary>
    /// Add Notifications module services
    /// </summary>
    public static IServiceCollection AddNotificationsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure options
        services.Configure<NotificationOptions>(
            configuration.GetSection("Notifications"));

        services.Configure<EmailOptions>(
            configuration.GetSection("Notifications:Email"));

        services.Configure<PushOptions>(
            configuration.GetSection("Notifications:Push"));

        services.Configure<SmsOptions>(
            configuration.GetSection("Notifications:Sms"));

        // Register services
        // services.AddScoped<INotificationService, NotificationService>();
        // services.AddScoped<INotificationPreferencesService, NotificationPreferencesService>();
        // services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
        // services.AddScoped<INotificationDeliveryService, NotificationDeliveryService>();

        // Register delivery channel handlers
        // services.AddScoped<IDeliveryChannelHandler, InAppDeliveryHandler>();
        // services.AddScoped<IDeliveryChannelHandler, EmailDeliveryHandler>();
        // services.AddScoped<IDeliveryChannelHandler, PushDeliveryHandler>();
        // services.AddScoped<IDeliveryChannelHandler, SmsDeliveryHandler>();

        // Register repositories
        // services.AddScoped<INotificationRepository, NotificationRepository>();
        // services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        // services.AddScoped<INotificationPreferencesRepository, NotificationPreferencesRepository>();
        // services.AddScoped<IDeviceRegistrationRepository, DeviceRegistrationRepository>();

        // Register SignalR hub
        // services.AddSignalR();

        // Register background services
        // services.AddHostedService<NotificationDigestService>();
        // services.AddHostedService<NotificationCleanupService>();

        // Configure authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("NotificationAdmin", policy =>
                policy.RequireRole("Admin", "NotificationAdmin"));

        return services;
    }
}

/// <summary>
/// Notification system options
/// </summary>
public class NotificationOptions
{
    /// <summary>
    /// Enable notifications system
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Default timezone for notifications
    /// </summary>
    public string DefaultTimeZone { get; set; } = "Asia/Riyadh";

    /// <summary>
    /// Maximum batch size for bulk operations
    /// </summary>
    public int MaxBatchSize { get; set; } = 1000;

    /// <summary>
    /// Number of retry attempts for failed deliveries
    /// </summary>
    public int RetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts in seconds
    /// </summary>
    public int RetryDelaySeconds { get; set; } = 60;

    /// <summary>
    /// Notification retention period in days
    /// </summary>
    public int RetentionDays { get; set; } = 90;

    /// <summary>
    /// Enable notification grouping
    /// </summary>
    public bool EnableGrouping { get; set; } = true;

    /// <summary>
    /// Group notification window in minutes
    /// </summary>
    public int GroupingWindowMinutes { get; set; } = 5;

    /// <summary>
    /// Default quiet hours start time
    /// </summary>
    public string DefaultQuietHoursStart { get; set; } = "22:00";

    /// <summary>
    /// Default quiet hours end time
    /// </summary>
    public string DefaultQuietHoursEnd { get; set; } = "07:00";

    /// <summary>
    /// Digest schedule cron expression (default: 9 AM daily)
    /// </summary>
    public string DigestSchedule { get; set; } = "0 9 * * *";

    /// <summary>
    /// Enable real-time notifications via SignalR
    /// </summary>
    public bool EnableRealTime { get; set; } = true;
}

/// <summary>
/// Email delivery options
/// </summary>
public class EmailOptions
{
    /// <summary>
    /// Enable email notifications
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Email provider type (smtp, sendgrid, ses)
    /// </summary>
    public string Provider { get; set; } = "smtp";

    /// <summary>
    /// SMTP host
    /// </summary>
    public string SmtpHost { get; set; } = string.Empty;

    /// <summary>
    /// SMTP port
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// SMTP username
    /// </summary>
    public string SmtpUsername { get; set; } = string.Empty;

    /// <summary>
    /// SMTP password
    /// </summary>
    public string SmtpPassword { get; set; } = string.Empty;

    /// <summary>
    /// Enable SSL/TLS
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// From email address
    /// </summary>
    public string FromAddress { get; set; } = "noreply@afc27.sa";

    /// <summary>
    /// From display name
    /// </summary>
    public string FromName { get; set; } = "AFC Asian Cup 2027";

    /// <summary>
    /// Reply-to email address
    /// </summary>
    public string? ReplyToAddress { get; set; }

    /// <summary>
    /// SendGrid API key (if using SendGrid)
    /// </summary>
    public string? SendGridApiKey { get; set; }

    /// <summary>
    /// AWS SES region (if using SES)
    /// </summary>
    public string? AwsSesRegion { get; set; }

    /// <summary>
    /// Maximum emails per hour (rate limiting)
    /// </summary>
    public int MaxEmailsPerHour { get; set; } = 1000;

    /// <summary>
    /// Email queue batch size
    /// </summary>
    public int BatchSize { get; set; } = 50;
}

/// <summary>
/// Push notification options
/// </summary>
public class PushOptions
{
    /// <summary>
    /// Enable push notifications
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Push provider type (firebase, apns, webpush)
    /// </summary>
    public string Provider { get; set; } = "firebase";

    /// <summary>
    /// Firebase server key
    /// </summary>
    public string? FirebaseServerKey { get; set; }

    /// <summary>
    /// Firebase project ID
    /// </summary>
    public string? FirebaseProjectId { get; set; }

    /// <summary>
    /// Firebase service account JSON path
    /// </summary>
    public string? FirebaseServiceAccountPath { get; set; }

    /// <summary>
    /// APNS key ID
    /// </summary>
    public string? ApnsKeyId { get; set; }

    /// <summary>
    /// APNS team ID
    /// </summary>
    public string? ApnsTeamId { get; set;  }

    /// <summary>
    /// APNS bundle ID
    /// </summary>
    public string? ApnsBundleId { get; set; }

    /// <summary>
    /// APNS private key path
    /// </summary>
    public string? ApnsPrivateKeyPath { get; set; }

    /// <summary>
    /// Use APNS sandbox (development)
    /// </summary>
    public bool ApnsUseSandbox { get; set; } = false;

    /// <summary>
    /// VAPID public key (for web push)
    /// </summary>
    public string? VapidPublicKey { get; set; }

    /// <summary>
    /// VAPID private key (for web push)
    /// </summary>
    public string? VapidPrivateKey { get; set; }

    /// <summary>
    /// VAPID subject (email or URL)
    /// </summary>
    public string? VapidSubject { get; set; }

    /// <summary>
    /// Push notification TTL in seconds
    /// </summary>
    public int TtlSeconds { get; set; } = 86400;

    /// <summary>
    /// Maximum push notifications per minute
    /// </summary>
    public int MaxPushPerMinute { get; set; } = 500;
}

/// <summary>
/// SMS notification options
/// </summary>
public class SmsOptions
{
    /// <summary>
    /// Enable SMS notifications
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// SMS provider type (twilio, unifonic)
    /// </summary>
    public string Provider { get; set; } = "twilio";

    /// <summary>
    /// Twilio account SID
    /// </summary>
    public string? TwilioAccountSid { get; set; }

    /// <summary>
    /// Twilio auth token
    /// </summary>
    public string? TwilioAuthToken { get; set; }

    /// <summary>
    /// Twilio phone number
    /// </summary>
    public string? TwilioPhoneNumber { get; set; }

    /// <summary>
    /// Unifonic app SID
    /// </summary>
    public string? UnifonicAppSid { get; set; }

    /// <summary>
    /// Unifonic sender ID
    /// </summary>
    public string? UnifonicSenderId { get; set; }

    /// <summary>
    /// Maximum SMS per day (rate limiting)
    /// </summary>
    public int MaxSmsPerDay { get; set; } = 100;

    /// <summary>
    /// SMS only for urgent notifications
    /// </summary>
    public bool UrgentOnly { get; set; } = true;
}
