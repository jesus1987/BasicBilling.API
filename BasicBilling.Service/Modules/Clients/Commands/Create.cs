using BasicBilling.Service.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BasicBilling.Service.Modules.Clients.Commands
{
    public class CreateCommand : Models.Client, IRequest<Models.Client>
    {
    }
    internal class HandleCreate : IRequestHandler<CreateCommand, Models.Client>
    {
        private readonly IRepository<Models.Client> repository;
        public HandleCreate(IRepository<Models.Client> repository)
        {
            this.repository = repository;
        }

        public async Task<Models.Client> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            return await repository.Create(new Models.Client { Id = request.Id, Name = request.Name});
        }
    }
}
