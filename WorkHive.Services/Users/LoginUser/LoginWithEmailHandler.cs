using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Users.LoginUser;

public record LoginWithEmailCommand(string Email) : ICommand<LoginWithEmailResult>;
public record LoginWithEmailResult(string UserName);

public class LoginWithEmailCommandValidator : AbstractValidator<LoginWithEmailCommand>
{
    public LoginWithEmailCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email format is required");
    }
}

public class LoginWithEmailHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<LoginWithEmailCommand, LoginWithEmailResult>
{
    public async Task<LoginWithEmailResult> Handle(LoginWithEmailCommand command, CancellationToken cancellationToken)
    {
        var user = userUnit.User.FindUserByEmail(command.Email);

        if (user is null)
            throw new UserNotFoundException("User", command.Email);

        return new LoginWithEmailResult(user.Name.ToString());
    }
}
