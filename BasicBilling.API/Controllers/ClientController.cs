using BasicBilling.Service.Models;
using BasicBilling.Service.Modules.Clients.Commands;
using BasicBilling.Service.Modules.Clients.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasicBilling.API.Controllers
{
    [Route("client")]
    public class ClientController : Base
    {
        private readonly IMediator mediator;
        public ClientController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Client value)
        {
            await mediator.Send(new CreateCommand
            {
                Name = value.Name
            });
            return Ok();
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Client>>> GetList()
        {
            return Ok(await mediator.Send(new SearchQuery()));
        }
    }
}
