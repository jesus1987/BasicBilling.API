using BasicBilling.Service.Models;
using BasicBilling.Service.Modules.Bills.Commands;
using BasicBilling.Service.Modules.Bills.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BasicBilling.API.Controllers
{
    [Route("billing/")]
    public class BillsController : Base
    {
        private readonly IMediator mediator;
        public BillsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("bills/{id}")]
        public async Task<ActionResult<Bills>> Get(int id)
        {
            return await mediator.Send(new GetByIdQuery { BillsId = id});
        }

        [Route("search")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Bills>>> GetList(string category)
        {
            return Ok(await mediator.Send(new SearchQuery { Category = category}));
        }

        [Route("bills")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Bills value)
        {
            await mediator.Send(new CreateCommand {
                Category = value.Category,
                ClientId = value.ClientId,
                Period = value.Period,
                Status = value.Status,
                Description = value.Description,
                Id = value.Id 
            });
            return Ok();
        }

        [HttpPut("pay")]
        public async Task<ActionResult> Put([FromBody] Bills value)
        {
            await mediator.Send(new PayCommand
            {
                Category = value.Category,
                ClientId = value.ClientId,
                Period = value.Period,
                Status = value.Status,
                Description = value.Description,
                Id = value.Id
            });
            return Ok();
        }

        [HttpDelete("bills/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await mediator.Send(new DeleteCommand { BillsId = id});
            return Ok();
        }
    }
}
