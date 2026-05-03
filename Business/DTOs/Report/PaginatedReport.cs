namespace Business
{
    public class PaginatedReport<TData>
    {
        /// <summary>
        /// Requested page number for the report.
        /// </summary>
        public required int RequestedPageNumber { get; set; }

        /// <summary>
        /// Total number of pages available in the report.
        /// </summary>
        public required int PagesCount { get; set; }

        /// <summary>
        /// Number of items per page in the report.
        /// </summary>
        public required int ItemsPerPage { get; set; }

        /// <summary>
        /// Collection of data items for the current page.
        /// </summary>
        public required IEnumerable<TData> Data { get; set; }
    }
}