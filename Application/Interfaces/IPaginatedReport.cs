namespace Application
{
    public interface IPaginatedReport
    {
        Task<PaginatedReport<TDataDto>> Prepare<TData, TDataDto>(IQueryable<TData> query, PaginationParams pageParams, CancellationToken cancellationToken);
    }
}