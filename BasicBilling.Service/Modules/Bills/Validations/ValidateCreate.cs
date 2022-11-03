using AutoMapper;
using BasicBilling.Service.Modules.Bills.Commands;
using FluentValidation;
using MediatR;

namespace BasicBilling.Service.Modules.Bills.Validations
{
    internal class ValidateCreate : AbstractValidator<CreateCommand>
    {
        public ValidateCreate()
        {
            RuleFor(x => x.ClientId).GreaterThan(0);
            RuleFor(x => x.Period).GreaterThan(0);
            RuleFor(x => x.Status).GreaterThan(0);
            RuleFor(x => x.Category).NotNull();
        }
    }
}
