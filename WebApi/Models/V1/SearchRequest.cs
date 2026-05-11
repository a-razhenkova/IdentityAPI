namespace WebApi.V1
{
    public class SearchRequest
    {
        /// <summary>
        /// The page number to retrieve from the paginated results.
        /// </summary>
        public required int RequestedPageNumber { get; set; } = 0;

        /// <summary>
        /// The number of items to include per page in the paginated results.
        /// </summary>
        public required int ItemsPerPage { get; set; } = 0;
    }
}