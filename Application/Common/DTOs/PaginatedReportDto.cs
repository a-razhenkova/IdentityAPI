namespace Application
{
    public class PaginatedReportDto<TData>
    {
        public required int RequestedPageNumber { get; set; }

        public required int PagesCount { get; set; }

        public required int ItemsPerPage { get; set; }

        public required IEnumerable<TData> Data { get; set; }
    }
}