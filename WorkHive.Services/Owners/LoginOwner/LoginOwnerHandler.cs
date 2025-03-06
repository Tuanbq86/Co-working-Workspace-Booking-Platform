using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;
using WorkHive.Services.Owners.LoginOwner;

namespace WorkHive.Services.Owners.LoginOwner
{

    public record LoginOwnerCommand(string Auth, string Password) : ICommand<LoginOwnerResult>;
    public record LoginOwnerResult(string Token, string Notification);
    public class LoginOwnerCommandValidator : AbstractValidator<LoginOwnerCommand>
    {
        public LoginOwnerCommandValidator()
        {
            RuleFor(x => x.Auth).NotEmpty().WithMessage("Email or Phone is required");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    public class LoginOwnerHandler(IWorkspaceOwnerUnitOfWork OwnerUnit, ITokenRepository tokenRepo,
        IHttpContextAccessor httpContext)
        : ICommandHandler<LoginOwnerCommand, LoginOwnerResult>
    {
        public async Task<LoginOwnerResult> Handle(LoginOwnerCommand command, CancellationToken cancellationToken)
        {
            // Check Owner standing by email/phone and password
            var IsExist = OwnerUnit.WorkspaceOwner.FindOwnerByEmailOrPhone(command.Auth, command.Password);

            if (IsExist == false)
                throw new OwnerNotFoundException("WorkSpaceOwner", command.Auth);

            // Get Owner to use generate JWT token
            var Owner = OwnerUnit.WorkspaceOwner.GetAll().Where(u => u.Phone.Equals(command.Auth)
            || u.Email.Equals(command.Auth)).FirstOrDefault();

            string token = tokenRepo.GenerateJwtToken(Owner!);

            //Save token into session to use in a working session
            httpContext.HttpContext!.Session.SetString("token", token);

            return new LoginOwnerResult(token, "Login successfully");
        }
    }
}
