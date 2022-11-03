using BasicBilling.Service.Models;
using BasicBilling.Service.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBilling.Service.Modules.Bills.Commands
{
    public class CreateCommand : Models.Bills, IRequest
    {
    }

    internal class HandleCreate : IRequestHandler<CreateCommand>
    {
        private readonly IRepository<Models.Bills> repository;
        public HandleCreate(IRepository<Models.Bills> repository)
        {
            this.repository = repository;
        }

        public async Task<Unit> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            await repository.Create(new Models.Bills {
                Id = request.Id,
                Category = request.Category,
                ClientId = 1,//request.ClientId,
                Description = request.Description,
                Period = request.Period,
                Status = (int)BillsStatus.InProgress
            }) ;

            return Unit.Value;
        }
    }
}
