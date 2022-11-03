using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BasicBilling.API.Filter
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ValidationException)
            {
                context.ModelState.Clear();
                var validationException = context.Exception as ValidationException;
                foreach (var validationExceptionError in validationException?.Errors ?? new List<ValidationFailure>())
                    context.ModelState.AddModelError(validationExceptionError.PropertyName, validationExceptionError.ErrorMessage);
            }

            if (!context.ModelState.IsValid)
            {
                context.Exception = null;
                context.Result = new BadRequestObjectResult(context.ModelState);
            }

            base.OnActionExecuted(context);
        }
    }
}
