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
            // Check owner standing by email/phone and password
            var IsExist = ownerUnit.WorkspaceOwner.FindWorkspaceOwnerByEmailOrPhone(command.Auth, command.Password);

            if (IsExist == false)
                return new LoginOwnerResult("", "Không tìm thấy owner");

            // Get owner to use generate JWT token

            var ownerList = await ownerUnit.WorkspaceOwner.GetAllAsync();

            var owner = ownerList.FirstOrDefault(u => u.Phone.ToLower().Trim().Equals(command.Auth.ToLower().Trim()) ||
                       u.Email.ToLower().Trim().Equals(command.Auth.ToLower().Trim()));

            if (owner is null)
                return new LoginOwnerResult("", "Sai thông tin đăng nhập");

            if (owner.IsBan!.Value.Equals(1))
                return new LoginOwnerResult("", "Tài khoản bị cấm");

            string token = tokenRepo.GenerateJwtToken(owner!);

            //Save token into session to use in a working session
            httpContext.HttpContext!.Session.SetString("token", token);

            return new LoginOwnerResult(token, "Đăng nhập thành công");
        }
    }
}
