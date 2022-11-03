using BasicBilling.Service.Models;
using BasicBilling.Service.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBilling.Service.Modules.Clients.Queries
{
    public class SearchQuery : IRequest<IEnumerable<Client>>
    {
    }

    internal class HandleSearch : IRequestHandler<SearchQuery, IEnumerable<Client>>
    {
        private readonly IRepository<Models.Client> repository;
        public HandleSearch(IRepository<Models.Client> repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Client>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            var query = repository.Query;
            return await query.ToListAsync(cancellationToken);
        }
    }
}
