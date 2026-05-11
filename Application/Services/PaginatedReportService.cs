using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application
{
    public class PaginatedReportService : IPaginatedReport
    {
        protected readonly AppSettings _appSettings;

        public PaginatedReportService(IOptionsSnapshot<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<PaginatedReportDto<TDataDto>> Prepare<TData, TDataDto>(IQueryable<TData> dataQuery, PaginatedQuery query,
            Func<IEnumerable<TData>, IEnumerable<TDataDto>> mapperCallback, CancellationToken cancellationToken = default)
        {
            int itemsPerPage = query.ItemsPerPage <= 0 || query.ItemsPerPage > _appSettings.PaginatedReport.DefaultMaxAllowedItemsPerPage
                ? _appSettings.PaginatedReport.DefaultItemsPerPage
                : query.ItemsPerPage;

            int itemsCount = await dataQuery.CountAsync();
            int pagesCount = Convert.ToInt32(Math.Ceiling((double)itemsCount / itemsPerPage));

            int pageNumber = query.RequestedPageNumber;
            if (query.ItemsPerPage <= 0)
            {
                pageNumber = 1;
            }
            else if (query.ItemsPerPage > pagesCount)
            {
                pageNumber = pagesCount;
            }

            IEnumerable<TData> data = await dataQuery
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(cancellationToken);

            return new PaginatedReportDto<TDataDto>()
            {
                RequestedPageNumber = pageNumber,
                PagesCount = pagesCount,
                ItemsPerPage = itemsPerPage,
                Data = mapperCallback(data)
            };
        }
    }
}