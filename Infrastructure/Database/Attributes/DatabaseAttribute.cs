namespace Infrastructure
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseAttribute : Attribute
    {
        public DatabaseAttribute(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        public string ConnectionStringName { get; init; }

        public string DefaultSchemaName { get; init; } = "dbo";

        public string MigrationsHistoryTableName { get; init; } = "applied_migration";

        public string ScriptsHistoryTableName { get; init; } = "applied_script";

        public int CommandTimeoutInSeconds { get; set; } = 120;
    }
}