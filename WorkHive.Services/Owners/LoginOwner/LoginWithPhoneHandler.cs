using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Owners.LoginOwner
{
    public record LoginWithWorkspaceOwnerPhoneCommand(string Phone) : ICommand<LoginWithWorkspaceOwnerPhoneResult>;
    public record LoginWithWorkspaceOwnerPhoneResult(string OwnerName);

    public class LoginWithPhoneCommandValidator : AbstractValidator<LoginWithWorkspaceOwnerPhoneCommand>
    {
        public LoginWithPhoneCommandValidator()
        {
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required")
                .Length(10).WithMessage("Length of phone must be 10");
        }
    }

    public class LoginWithPhoneHandler(IWorkspaceOwnerUnitOfWork OwnerUnit)
        : ICommandHandler<LoginWithWorkspaceOwnerPhoneCommand, LoginWithWorkspaceOwnerPhoneResult>
    {
        public async Task<LoginWithWorkspaceOwnerPhoneResult> Handle(LoginWithWorkspaceOwnerPhoneCommand command,
            CancellationToken cancellationToken)
        {
            var Owner = OwnerUnit.WorkspaceOwner.FindOwnerByPhone(command.Phone);

            if (Owner is null)
                throw new OwnerNotFoundException("Owner", command.Phone);

            return new LoginWithWorkspaceOwnerPhoneResult(Owner.Phone.ToString());
        }
    }
}