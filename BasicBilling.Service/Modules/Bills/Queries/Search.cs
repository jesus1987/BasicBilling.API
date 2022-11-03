using BasicBilling.Service.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBilling.Service.Modules.Bills.Queries
{
    public class SearchQuery : IRequest<IEnumerable<Models.Bills>>
    {
        public int? ClientId { get; set; }
        public int? Status { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
    }

    internal class HandleSearch : IRequestHandler<SearchQuery, IEnumerable<Models.Bills>>
    {
        private readonly IRepository<Models.Bills> repository;
        public HandleSearch(IRepository<Models.Bills> repository)
        {
            this.repository = repository;
        }
        public async Task<IEnumerable<Models.Bills>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            var query = repository.Query;
            if (request.ClientId.HasValue)
            {
                query = query.Where(x => x.ClientId == request.ClientId);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status);
            }
            if (!string.IsNullOrEmpty(request.Category))
            {
                query = query.Where(x => x.Category == request.Category);
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}
