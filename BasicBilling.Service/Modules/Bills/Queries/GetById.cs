using BasicBilling.Service.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBilling.Service.Modules.Bills.Queries
{
    public class GetByIdQuery : IRequest<Models.Bills>
    {
        public int BillsId { get; set; }
    }

    internal class HandleGetById : IRequestHandler<GetByIdQuery, Models.Bills>
    {
        private readonly IRepository<Models.Bills> repository;
        public HandleGetById(IRepository<Models.Bills> repository)
        {
            this.repository = repository;
        }

        public async Task<Models.Bills> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var query = repository.Query;
            return await Task.FromResult(query.Single(at => at.Id == request.BillsId));
        }
    }
}
