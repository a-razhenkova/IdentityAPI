namespace Domain
{
    public enum ClientStatuses
    {
        Disabled = 0,
        Active = 1,
        Blocked = 2
    }

    public enum ClientStatusReasons
    {
        None = 0,
        MaxWrongLoginAttemptsReached,
        ExpiredSubscription
    }

    public enum UserRoles
    {
        Administrator = 0,
        Employee = 1,
        Customer = 2
    }

    public enum UserStatuses
    {
        Disabled = 0,
        Active = 1,
        Blocked = 2,
        Restricted = 3
    }

    public enum UserStatusReasons
    {
        None = 0,
        MaxWrongLoginAttemptsReached = 1,
        NewUser = 2,
        EmailChanged = 3,
    }

    public enum DocumentTypes
    {
        SubscriptionContract
    }
}