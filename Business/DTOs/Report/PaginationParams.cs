namespace Business
{
    public class PaginationParams
    {
        /// <summary>
        /// The page number to retrieve from the paginated results.
        /// </summary>
        public int RequestedPageNumber { get; set; } = 0;

        /// <summary>
        /// The number of items to include per page in the paginated results.
        /// </summary>
        public int ItemsPerPage { get; set; } = 0;
    }
}