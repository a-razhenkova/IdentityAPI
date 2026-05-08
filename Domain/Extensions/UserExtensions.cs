namespace Domain
{
    public static class UserExtensions
    {
        extension(User user)
        {
            public void Activate()
            {
                user.Status.Value = UserStatuses.Active;
                user.Status.Reason = UserStatusReasons.None;
                user.Status.Note = null;
            }

            public void Block(UserStatusReasons reason)
            {
                user.Status.Value = UserStatuses.Disabled;
                user.Status.Reason = reason;
                user.Status.Note = null;
            }
        }
    }
}