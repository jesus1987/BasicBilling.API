using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BasicBilling.API.Filter
{
    public class ExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            object responseObj;
            switch (context.Exception)
            {
                default:
                    responseObj = new { context.Exception.Message };
                    break;
            };

            var result = JsonConvert.SerializeObject(responseObj);
            context.Result = new ContentResult
            {
                Content = result,
                StatusCode = 500,
                ContentType = "application/json",
            };

            context.ExceptionHandled = true;
        }
    }
}
