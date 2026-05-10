namespace Domain
{
    public static class UserExtensions
    {
        extension(User user)
        {
            public bool IsActivate() => user.Status.Value == UserStatuses.Active;
            public bool IsRestrict() => user.Status.Value == UserStatuses.Restricted;
            public bool IsBlock() => user.Status.Value == UserStatuses.Blocked;
            public bool IsDisable() => user.Status.Value == UserStatuses.Disabled;

            public void Activate()
            {
                if (user.Status.Value == UserStatuses.Restricted && user.Status.Reason == UserStatusReasons.EmailChanged)
                    user.IsVerified = true;

                if (!user.IsVerified)
                {
                    user.Restrict(UserStatusReasons.EmailChanged);
                    return;
                }

                user.Status.Value = UserStatuses.Active;
                user.Status.Reason = UserStatusReasons.None;
                user.Status.Note = null;
            }

            public void Restrict(UserStatusReasons reason = UserStatusReasons.None, string note = default)
            {
                if (reason == UserStatusReasons.EmailChanged)
                    user.IsVerified = false;

                // In case of status blocked, disabled, ect., the status must remain unchanged.
                if (user.Status.Value != UserStatuses.Active)
                    return;

                user.Status.Value = UserStatuses.Restricted;
                user.Status.Reason = reason;
                user.Status.Note = note;
            }

            public void Block(UserStatusReasons reason = UserStatusReasons.None, string note = default)
            {
                user.Status.Value = UserStatuses.Blocked;
                user.Status.Reason = reason;
                user.Status.Note = note;
            }

            public void Disable(UserStatusReasons reason = UserStatusReasons.None, string note = default)
            {
                user.Status.Value = UserStatuses.Disabled;
                user.Status.Reason = reason;
                user.Status.Note = note;
            }
        }
    }
}