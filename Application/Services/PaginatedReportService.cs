using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application
{
    public class PaginatedReportService : IPaginatedReport
    {
        protected readonly AppSettings _appSettings;
        protected readonly IMapper _mapper;

        public PaginatedReportService(IOptionsSnapshot<AppSettings> appSettings, IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        public async Task<PaginatedReport<TDataDto>> Prepare<TData, TDataDto>(IQueryable<TData> query, PaginationParams pageParams, CancellationToken cancellationToken)
        {
            int itemsPerPage = pageParams.ItemsPerPage <= 0 || pageParams.ItemsPerPage > _appSettings.PaginatedReport.DefaultMaxAllowedItemsPerPage
                ? _appSettings.PaginatedReport.DefaultItemsPerPage
                : pageParams.ItemsPerPage;

            int itemsCount = await query.CountAsync();
            int pagesCount = Convert.ToInt32(Math.Ceiling((double)itemsCount / itemsPerPage));

            int pageNumber = pageParams.RequestedPageNumber;
            if (pageParams.ItemsPerPage <= 0)
            {
                pageNumber = 1;
            }
            else if (pageParams.ItemsPerPage > pagesCount)
            {
                pageNumber = pagesCount;
            }

            IEnumerable<TData> data = await query
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(cancellationToken);

            return new PaginatedReport<TDataDto>()
            {
                RequestedPageNumber = pageNumber,
                PagesCount = pagesCount,
                ItemsPerPage = itemsPerPage,
                Data = _mapper.Map<IEnumerable<TDataDto>>(data)
            };
        }
    }
}