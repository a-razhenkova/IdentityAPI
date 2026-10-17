namespace Application
{
    public interface IPaginatedReport
    {
        Task<PaginatedReportDto<TDataDto>> Prepare<TData, TDataDto>(IQueryable<TData> dataQuery, PaginatedQuery query,
            Func<IEnumerable<TData>, IEnumerable<TDataDto>> mapperCallback, CancellationToken cancellationToken = default);
    }
}