using FluentValidation;
using Microsoft.AspNetCore.Http;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Users.LoginUser;

public record LoginUserCommand(string Auth, string Password) : ICommand<LoginUserResult>;
public record LoginUserResult(string Token, string Notification);

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Auth).NotEmpty().WithMessage("Email or Phone is required");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}

public class LoginUserHandler(IUserUnitOfWork userUnit, ITokenRepository tokenRepo, 
    IHttpContextAccessor httpContext)
    : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        // Check user standing by email/phone and password
        var IsExist = userUnit.User.FindUserByEmailOrPhone(command.Auth, command.Password);

        if (IsExist == false)
            throw new UserNotFoundException("User", command.Auth);

        // Get user to use generate JWT token

        var userList = await userUnit.User.GetAllAsync();

        var user = userList.FirstOrDefault(u => u.Phone.ToLower().Trim().Equals(command.Auth.ToLower().Trim()) ||
                   u.Email.ToLower().Trim().Equals(command.Auth.ToLower().Trim()));

        if (user is null)
            throw new UserNotFoundException("User", command.Auth);

        var userNotifi = new UserNotification
        {
            UserId = user.Id,
            IsRead = 0,
            CreatedAt = DateTime.Now,
            Description = "Đăng nhập thành công",
            Status = "Active"
        };
        await userUnit.UserNotification.CreateAsync(userNotifi);

        string token = tokenRepo.GenerateJwtToken(user!);

        //Save token into session to use in a working session
        httpContext.HttpContext!.Session.SetString("token", token);

        return new LoginUserResult(token, "Login successfully");
    }
}
