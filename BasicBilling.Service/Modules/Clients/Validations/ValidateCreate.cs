using AutoMapper;
using BasicBilling.Service.Modules.Clients.Commands;
using FluentValidation;
using MediatR;

namespace BasicBilling.Service.Modules.Clients.Validations
{
    internal class ValidateCreate : AbstractValidator<CreateCommand>
    {
        public ValidateCreate()
        {
            RuleFor(x => x.Name).NotNull();
        }
    }
}
