using System.Diagnostics.CodeAnalysis;

namespace WebApi.V1
{
    public class SimpleResponse<TValue>
    {
        [SetsRequiredMembers]
        public SimpleResponse(TValue value)
        {
            Value = value;
        }

        /// <summary>
        /// Value.
        /// </summary>
        /// <example>value</example>
        public required TValue Value { get; set; }
    }
}