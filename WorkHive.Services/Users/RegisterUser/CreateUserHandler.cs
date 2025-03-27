using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant.Wallet;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Users.RegisterUser;

public record CreateUserCommand(string Name, string Email,
    string Phone, string Password, string Sex, int RoleId) : ICommand<CreateUserResult>;

public record CreateUserResult(string Token, string Notification);

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
            .MinimumLength(5).WithMessage("Length of name have to have minimum being 5")
            .MaximumLength(50).WithMessage("Length of name have to have maximum being 50");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("A valid email format is required");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required")
            .Length(10).WithMessage("The number of characterics is exact 10 characterics");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");

        RuleFor(x => x.Sex).NotEmpty().WithMessage("Sex is required");

        RuleFor(x => x.RoleId).NotEmpty().WithMessage("RoleId is required")
            .Must(RoleId => RoleId >= 2 && RoleId <= 4).WithMessage("RoleId must be between 2 and 4 and not equal 1");
    }
}


public class CreateUserHandler(IUserUnitOfWork userUnit, ITokenRepository tokenRepo,
    IHttpContextAccessor httpContext) : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUserCommand command, 
        CancellationToken cancellationToken)
    {
        //Checking exist used email and phone number for registering

        var existEmailOrPhoneUser = userUnit.User.GetAll().
            Where(x => x.Email.ToLower().Equals(command.Email.ToLower()) ||
            x.Phone.ToLower().Equals(command.Phone.ToLower())).FirstOrDefault();

        if (existEmailOrPhoneUser is not null)
            throw new BadRequestEmailOrPhoneException("Email or Phone has been used");

        //Create new user for registering

        var tempUser = userUnit.User.RegisterUser(command.Name, command.Email,
            command.Phone, command.Password, command.Sex);

        var newUser = new User
        {
            Name = tempUser.Name,
            Email = tempUser.Email,
            Phone = tempUser.Phone,
            Sex = tempUser.Sex,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = "Active",
            //Using Bcrypt to hash password using SHA-512 algorithm
            //Work factor time so long when increment for safety(13)
            Password = BCrypt.Net.BCrypt.EnhancedHashPassword(tempUser.Password, 13),
            RoleId = command.RoleId,
            IsBan = 0
        };

        await userUnit.User.CreateAsync(newUser);

        var wallet = new Wallet
        {
            Balance = 0,
            Status = WalletStatus.Active.ToString()
        };
        await userUnit.Wallet.CreateAsync(wallet);

        var customerWallet = new CustomerWallet
        {
            Status = WalletStatus.Active.ToString(),
            WalletId = wallet.Id,
            UserId = newUser.Id
        };
        userUnit.CustomerWallet.Create(customerWallet);

        await userUnit.SaveAsync();

        //Tạo thông báo
        var userNotifi = new UserNotification
        {
            UserId = newUser.Id,
            IsRead = 0,
            CreatedAt = DateTime.Now,
            Description = "Tạo người dùng thành công",
            Status = "Active"
        };
        await userUnit.UserNotification.CreateAsync(userNotifi);

        var token = tokenRepo.GenerateJwtToken(newUser);

        httpContext.HttpContext!.Session.SetString("token", token);

        return new CreateUserResult(token, "Create successfully");
    }
}
