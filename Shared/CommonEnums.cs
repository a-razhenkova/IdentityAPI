using System.ComponentModel;

namespace Shared
{
    public enum LoggerContextProperty
    {
        Environment,
        Version,
        MachineName,
        ActionType,
        ActionId,
        TraceId,
        CorrelationId
    }

    public enum LoggerContext
    {
        WebApi,
        HealthCheck,
        DbMigration,
        DbUp
    }

    public enum HealthCheckImpactTag
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum AuthorizationSchema
    {
        Basic,
        Bearer
    }

    public enum TokenClaim
    {
        [Description("iss")]
        Issuer,
        [Description("aud")]
        Audience,

        // client
        [Description("clientId")]
        ClientId,
        [Description("clientStatus")]
        ClientStatus,
        [Description("isInternalClient")]
        IsInternalClient,
        [Description("canNotify")]
        CanClientNotify,

        // user
        [Description("userId")]
        UserPublicId,
        [Description("username")]
        Username,
        [Description("userRole")]
        UserRole,
        [Description("userStatus")]
        UserStatus
    }

    public enum ResiliencePipelineType
    {
        RabbitMQ_PublishFastEvent,
        RabbitMQ_PublishEventInBackground
    }
}