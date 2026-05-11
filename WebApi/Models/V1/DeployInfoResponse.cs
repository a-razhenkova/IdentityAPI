namespace WebApi.V1
{
    public class DeployInfoResponse
    {
        /// <summary>
        /// The application version.
        /// </summary>
        /// <example>1.0.0.0</example>
        public required string Version { get; set; }

        /// <summary>
        /// The environment in which the application is running.
        /// </summary>
        /// <example>Development</example>
        public required string Environment { get; set; }

        /// <summary>
        /// The name of the machine where the application is deployed.
        /// </summary>
        /// <example>TEST-123</example>
        public required string MachineName { get; set; }

        /// <summary>
        /// Timestamp of the machine when the information was captured.
        /// </summary>
        public required DateTime MachineTimestamp { get; set; }
    }
}