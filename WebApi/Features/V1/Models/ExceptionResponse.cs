using System.Text.Json.Serialization;

namespace WebApi.V1
{
    public class ExceptionResponse
    {
        /// <summary>
        /// Message.
        /// </summary>
        /// <example>Internal Server Error</example>
        [JsonPropertyName("title")]
        public required string Title { get; set; }

        /// <summary>
        /// Exception message.
        /// </summary>
        /// <example>Invalid column name.</example>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Exception details.
        /// </summary>
        /// <example>null</example>
        [JsonPropertyName("details")]
        public string? Details { get; set; }
    }
}