using System.Reflection;

namespace Infrastructure
{
    public static class ApplicationAssembly
    {
        public static string GetName()
            => Assembly.GetExecutingAssembly()?.GetName().Name ?? "unknown";
    }
}