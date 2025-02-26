using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;
using WorkHive.Services.Users.RegisterUser;

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

public class LoginUserHandler(IUserUnitOfWork userUnit, ITokenRepository tokenRepo)
    : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var IsExist = userUnit.User.FindUserByEmailOrPhone(command.Auth, command.Password);

        if (IsExist == false)
            throw new UserNotFoundException("User", command.Auth);

        var user = userUnit.User.GetAll().Where(u => u.Phone.Equals(command.Auth) 
        || u.Email.Equals(command.Auth)).FirstOrDefault();

        string token = tokenRepo.GenerateJwtToken(user!);

        return new LoginUserResult(token, "Login successfully");
    }
}
