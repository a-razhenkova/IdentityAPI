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
        // client
        [Description("clientId")]
        ClientId,
        [Description("isInternalClient")]
        IsInternalClient,

        // user
        [Description("userId")]
        UserPublicId,
        [Description("username")]
        Username,
        [Description("userRole")]
        UserRole,
        [Description("userStatus")]
        UserStatus,

        // rights
        [Description("canNotify")]
        CanNotify
    }
}