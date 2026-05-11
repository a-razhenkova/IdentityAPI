namespace WebApi.V1
{
    public class UpdateClientRightRequest
    {
        /// <summary>
        /// Flag indicating whether the client has the right to notify a party.
        /// </summary>
        public required bool CanNotify { get; set; }
    }
}