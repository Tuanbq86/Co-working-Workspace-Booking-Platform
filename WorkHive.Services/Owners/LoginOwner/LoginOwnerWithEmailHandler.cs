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
    public record LoginWithOwnerEmailCommand(string Email) : ICommand<LoginWithOwnerEmailResult>;
    public record LoginWithOwnerEmailResult(string OwnerName);

    public class LoginWithOwnerEmailCommandValidator : AbstractValidator<LoginWithOwnerEmailCommand>
    {
        public LoginWithOwnerEmailCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email format is required");
        }
    }

    public class LoginWithEmailHandler(IWorkspaceOwnerUnitOfWork OwnerUnit)
        : ICommandHandler<LoginWithOwnerEmailCommand, LoginWithOwnerEmailResult>
    {
        public async Task<LoginWithOwnerEmailResult> Handle(LoginWithOwnerEmailCommand command, CancellationToken cancellationToken)
        {
            var Owner = OwnerUnit.WorkspaceOwner.FindWorkspaceOwnerByEmail(command.Email);

            if (Owner is null)
                throw new OwnerNotFoundException("Owner", command.Email);

            return new LoginWithOwnerEmailResult(Owner.Email.ToString());
        }
    }
}
