using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared
{
    public static class Utils
    {
        public static bool IsEqual(this object model1, object model2)
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string json1 = JsonSerializer.Serialize(model1, serializerOptions);
            string json2 = JsonSerializer.Serialize(model2, serializerOptions);

            return json1 == json2;
        }

        public static void Validate(this object model)
        {
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true))
            {
                var errorMsg = new StringBuilder();
                validationResults.ForEach(error => errorMsg.AppendLine(error.ErrorMessage));

                throw new ArgumentException(errorMsg.ToString());
            }
        }

        public static TData DeepCopy<TData>(this TData value)
        {
            var jsonValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            return JsonSerializer.Deserialize<TData>(jsonValue)
                ?? throw new InvalidOperationException();
        }

        public static TAttribute GetRequiredCustomAttribute<TAttribute>(this object value)
            where TAttribute : Attribute
        {
            return value.GetType().GetRequiredCustomAttribute<TAttribute>();
        }

        public static TAttribute GetRequiredCustomAttribute<TAttribute>(this MemberInfo value)
            where TAttribute : Attribute
        {
            TAttribute? attribute = (TAttribute?)value.GetCustomAttribute(typeof(TAttribute), true);

            if (attribute == null)
                throw new ArgumentException($"Attribute of type '{typeof(TAttribute).Name}' not found for value '{value.Name}'.");

            return attribute;
        }
    }
}