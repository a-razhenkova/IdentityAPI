using System.Reflection;

namespace Infrastructure
{
    public static class InfrastructureAssembly
    {
        public static Assembly GetExecutingAssembly()
            => Assembly.GetExecutingAssembly();
    }
}