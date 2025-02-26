using FluentValidation;
using Microsoft.AspNetCore.Http;
using WorkHive.BuildingBlocks.CQRS;
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
        var user = userUnit.User.GetAll().Where(u => u.Phone.Equals(command.Auth) 
        || u.Email.Equals(command.Auth)).FirstOrDefault();

        string token = tokenRepo.GenerateJwtToken(user!);

        //Save token into session to use in a working session
        httpContext.HttpContext!.Session.SetString("token", token);

        return new LoginUserResult(token, "Login successfully");
    }
}
