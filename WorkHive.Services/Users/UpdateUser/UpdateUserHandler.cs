using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Users.UpdateUser;

public record UpdateUserCommand(int UserId, string Name, string Email, string Location, string Phone, 
    DateOnly? DateOfBirth, string Sex, string Avatar) : ICommand<UpdateUserResult>;
public record UpdateUserResult(string Notification);

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.Name)
            .MinimumLength(5).WithMessage("Length of name have to have minimum being 5")
            .MaximumLength(50).WithMessage("Length of name have to have maximum being 50");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email format is required");

        RuleFor(x => x.Phone)
            .Length(10).WithMessage("The number of characterics is exact 10 characterics");

        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob == null || (dob >= new DateOnly(1955, 1, 1) && dob <= new DateOnly(2013, 1, 1)))
            .WithMessage("Date of birth must be between 01/01/1955 and 01/01/2013");
    }
}

public class UpdateUserHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<UpdateUserCommand, UpdateUserResult>
{
    public async Task<UpdateUserResult> Handle(UpdateUserCommand command, 
        CancellationToken cancellationToken)
    {
        var user = userUnit.User.GetById(command.UserId);

        //Check null user
        if (user is null)
            return new UpdateUserResult("Không tìm thấy người dùng để cập nhật");

        //Check email and phone of user who get in database with others in database
        bool isDuplicate = userUnit.User.GetAll()
                .Where(u => !string.IsNullOrEmpty(u.Phone))
                .Any(u => u.Id != user.Id &&
              (u.Email.ToLower().Trim() == command.Email.ToLower().Trim() || u.Phone.ToLower().Trim() == command.Phone.ToLower().Trim()));

        if (isDuplicate)
        {
            return new UpdateUserResult("Email và số điện thoại đã được sử dụng");
        }

        user.Name = command.Name;
        user.Email = command.Email.Trim();
        user.Location = command.Location;
        user.Phone = command.Phone;
        user.DateOfBirth = command.DateOfBirth;
        user.Sex = command.Sex;
        user.Avatar = command.Avatar;

        userUnit.User.Update(user);
        await userUnit.SaveAsync();

        return new UpdateUserResult("Cập nhật thông tin thành công");
    }
}