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
using WorkHive.Services.Users.LoginUser;

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

    public class LoginOwnerHandler(IWorkspaceOwnerUnitOfWork ownerUnit, ITokenRepository tokenRepo,
        IHttpContextAccessor httpContext)
        : ICommandHandler<LoginOwnerCommand, LoginOwnerResult>
    {
        public async Task<LoginOwnerResult> Handle(LoginOwnerCommand command, CancellationToken cancellationToken)
        {
            // Check user standing by email/phone and password
            var IsExist = ownerUnit.WorkspaceOwner.FindWorkspaceOwnerByEmailOrPhone(command.Auth, command.Password);

            if (IsExist == false)
                throw new UserNotFoundException("User", command.Auth);

            // Get user to use generate JWT token

            var userList = await ownerUnit.WorkspaceOwner.GetAllAsync();

            var user = userList.FirstOrDefault(u => u.Phone.ToLower().Trim().Equals(command.Auth.ToLower().Trim()) ||
                       u.Email.ToLower().Trim().Equals(command.Auth.ToLower().Trim()));

            if (user is null)
                throw new UserNotFoundException("User", command.Auth);

            string token = tokenRepo.GenerateJwtToken(user!);

            //Save token into session to use in a working session
            httpContext.HttpContext!.Session.SetString("token", token);

            return new LoginOwnerResult(token, "Login successfully");
        }
    }
}
