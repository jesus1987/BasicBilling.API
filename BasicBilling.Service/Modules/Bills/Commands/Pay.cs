using BasicBilling.Service.Models;
using BasicBilling.Service.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBilling.Service.Modules.Bills.Commands
{
    public class PayCommand : Models.Bills, IRequest
    {
    }

    internal class HandleUpdate : IRequestHandler<PayCommand>
    {
        private readonly IRepository<Models.Bills> repository;
        public HandleUpdate(IRepository<Models.Bills> repository)
        {
            this.repository = repository;
        }

        public async Task<Unit> Handle(PayCommand request, CancellationToken cancellationToken)
        {
            var getBill = await repository.Query.Where(at => at.ClientId == request.ClientId &&
            at.Category == request.Category && at.Period == request.Period).SingleAsync();
            getBill.Status = (int)BillsStatus.Payed;
            await repository.Update(getBill);

            return Unit.Value;
        }
    }
}
