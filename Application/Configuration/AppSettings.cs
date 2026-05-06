using Shared;
using System.ComponentModel.DataAnnotations;

namespace Application
{
    public record AppSettings
    {
        public LogSettings Log { get; init; } = new();

        public SecuritySettings Security { get; init; } = new();

        public DatabaseSettings Database { get; init; } = new();

        [Required]
        public string ClientSubscriptionContractDirectory { get; set; } = string.Empty;

        public PaginatedReportSettings PaginatedReport { get; init; } = new();
    }

    public record DatabaseSettings
    {
        public bool IsDbMigrationAllowed { get; init; } = false;

        public bool IsDbUpAllowed { get; init; } = false;
    }

    public record PaginatedReportSettings
    {
        [Range(minimum: 1, maximum: 1000)]
        public int DefaultItemsPerPage { get; init; } = 20;

        [Range(minimum: 1, maximum: 1000)]
        public int DefaultMaxAllowedItemsPerPage { get; init; } = 1000;
    }
}