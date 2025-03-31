using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Users.LoginUser;

public record LoginWithPhoneCommand(string Phone) : ICommand<LoginWithPhoneResult>;
public record LoginWithPhoneResult(string UserName);

public class LoginWithPhoneCommandValidator : AbstractValidator<LoginWithPhoneCommand>
{
    public LoginWithPhoneCommandValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required")
            .Length(10).WithMessage("Length of phone must be 10");
    }
}

public class LoginWithPhoneHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<LoginWithPhoneCommand, LoginWithPhoneResult>
{
    public async Task<LoginWithPhoneResult> Handle(LoginWithPhoneCommand command, 
        CancellationToken cancellationToken)
    {
        var user = userUnit.User.FindUserByPhone(command.Phone);

        if (user is null)
            return new LoginWithPhoneResult($"Không tìm thấy người dùng với số điện thoại: {command.Phone}");

        return new LoginWithPhoneResult(user.Name.ToString());
    }
}
