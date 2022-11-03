using AutoMapper;
using BasicBilling.Service.Modules.Bills.Commands;
using FluentValidation;
using MediatR;

namespace BasicBilling.Service.Modules.Bills.Validations
{
    internal class ValidateDelete : AbstractValidator<DeleteCommand>
    {
        public ValidateDelete()
        {
            RuleFor(x => x.BillsId).GreaterThan(0);
        }
    }
}
