using System.Reflection;

namespace Infrastructure
{
    public static class InfrastructureAssembly
    {
        public static Assembly GetExecutingAssembly()
            => Assembly.GetExecutingAssembly();

        public static string GetName()
            => Assembly.GetExecutingAssembly()?.GetName().Name ?? "unknown";
    }
}