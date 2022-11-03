using BasicBilling.Service.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBilling.Service.Modules.Bills.Commands
{
    public class DeleteCommand : IRequest
    {
        public int BillsId { get; set; }
    }

    internal class HandleDelete : IRequestHandler<DeleteCommand>
    {
        private readonly IRepository<Models.Bills> repository;
        public HandleDelete(IRepository<Models.Bills> repository)
        {
            this.repository = repository;
        }

        public async Task<Unit> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            var bills = await repository.Query.SingleOrDefaultAsync(at => at.Id == request.BillsId);
            if(bills != null)
            {
                await repository.Delete(bills);
            }
            
            return Unit.Value;
        }
    }
}
