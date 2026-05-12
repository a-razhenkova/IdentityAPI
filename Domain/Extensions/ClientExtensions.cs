namespace Domain
{
    public static class ClientExtensions
    {
        extension(Client client)
        {
            public bool IsActivate() => client.Status.Value == ClientStatuses.Active;
            public bool IsBlock() => client.Status.Value == ClientStatuses.Blocked;
            public bool IsDisable() => client.Status.Value == ClientStatuses.Disabled;

            public void Activate()
            {
                client.Status.Value = ClientStatuses.Active;
                client.Status.Reason = ClientStatusReasons.None;
                client.Status.Note = null;
            }

            public void Block(ClientStatusReasons reason = ClientStatusReasons.None, string note = null)
            {
                client.Status.Value = ClientStatuses.Blocked;
                client.Status.Reason = reason;
                client.Status.Note = note;
            }

            public void Disable(ClientStatusReasons reason = ClientStatusReasons.None, string note = null)
            {
                client.Status.Value = ClientStatuses.Disabled;
                client.Status.Reason = reason;
                client.Status.Note = note;
            }

            public void UpdateStatus(ClientStatuses status, ClientStatusReasons reason = ClientStatusReasons.None, string note = null)
            {
                if (client.Status.Value == status && client.Status.Reason == reason && client.Status.Note == note)
                    return; // has nothing to update

                switch (status)
                {
                    case ClientStatuses.Active:
                        client.Activate();
                        break;
                    case ClientStatuses.Disabled:
                        client.Disable(reason, note);
                        break;
                    case ClientStatuses.Blocked:
                        client.Block(reason, note);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}