namespace Application
{
    public abstract record PaginatedQuery
    {
        public required int RequestedPageNumber { get; set; } = 0;

        public required int ItemsPerPage { get; set; } = 0;
    }
}