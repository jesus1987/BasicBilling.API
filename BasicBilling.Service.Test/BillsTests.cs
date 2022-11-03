using BasicBilling.API.Controllers;
using BasicBilling.Service.Models;
using BasicBilling.Service.Modules.Bills.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicBilling.Service.Test
{
    public class BillsTests : BaseTest
    {
        private BillsController billsController;
        private ClientController clientController;
        [SetUp]
        public void SetUp()
        {
            billsController = ServiceProvider.GetService<BillsController>();
            clientController = ServiceProvider.GetService<ClientController>();
        }

        [Test(Description = "Should be able to get bills by Id")]
        public async Task ShouldGetBillsById()
        {
            await clientController.Post(new Client { Name = "Client 1"});
            var clientResult = await clientController.GetList();
            var clients = (IEnumerable<Client>)((Microsoft.AspNetCore.Mvc.OkObjectResult)clientResult.Result).Value;
            var client = clients.FirstOrDefault();
            await billsController.Post(new Bills { ClientId = client.Id, Category = "Test", Period = 1, Status = 1 });
            var billsResult = await billsController.GetList(new SearchQuery());
            var bills = (IEnumerable<Bills>)((Microsoft.AspNetCore.Mvc.OkObjectResult)billsResult.Result).Value;
            bills.Should().NotBeNull();
            bills.First().Id.Should().BeGreaterThan(0);
        }
    }
}
