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

        public async Task<PaginatedReportDto<TDataDto>> Prepare<TData, TDataDto>(IQueryable<TData> query, PaginatedQuery settings,
            Func<IEnumerable<TData>, IEnumerable<TDataDto>> mapperCallback, CancellationToken cancellationToken = default)
        {
            int itemsPerPage = settings.ItemsPerPage <= 0 || settings.ItemsPerPage > _appSettings.PaginatedReport.DefaultMaxAllowedItemsPerPage
                ? _appSettings.PaginatedReport.DefaultItemsPerPage
                : settings.ItemsPerPage;

            int itemsCount = await query.CountAsync();
            int pagesCount = Convert.ToInt32(Math.Ceiling((double)itemsCount / itemsPerPage));

            int pageNumber = settings.RequestedPageNumber;
            if (settings.ItemsPerPage <= 0)
            {
                pageNumber = 1;
            }
            else if (settings.ItemsPerPage > pagesCount)
            {
                pageNumber = pagesCount;
            }

            IEnumerable<TData> data = await query
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