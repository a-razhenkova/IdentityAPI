using System.Reflection;

namespace WebApi
{
    public static class WebApiAssembly
    {
        public static string GetName()
            => Assembly.GetExecutingAssembly()?.GetName().Name ?? "unknown";

        public static string GetVersion()
            => Assembly.GetExecutingAssembly()?.GetName().Version?.ToString() ?? "1.0.0.0";
    }
}