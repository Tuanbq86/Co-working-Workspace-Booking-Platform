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
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
            .MinimumLength(5).WithMessage("Length of name have to have minimum being 5")
            .MaximumLength(50).WithMessage("Length of name have to have maximum being 50");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("A valid email format is required");

        RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required")
            .Length(10).WithMessage("The number of characterics is exact 10 characterics");

        RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("Date of birth is required");

        RuleFor(x => x.Sex).NotEmpty().WithMessage("Sex is required");

        RuleFor(x => x.Avatar).NotEmpty().WithMessage("Avatar is required");
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
            throw new UserNotFoundException("Can not find user to update");

        //Check email and phone of user who get in database with others in database
        bool isDuplicate = userUnit.User.GetAll()
                .Any(u => u.Id != user.Id &&
              (u.Email == command.Email || u.Phone == command.Phone));

        if (isDuplicate)
        {
            throw new UserBadRequestException("Email or password has been used");
        }

        user.Name = command.Name;
        user.Email = command.Email;
        user.Location = command.Location;
        user.Phone = command.Phone;
        user.DateOfBirth = command.DateOfBirth;
        user.Sex = command.Sex;
        user.Avatar = command.Avatar;

        userUnit.User.Update(user);
        await userUnit.SaveAsync();

        return new UpdateUserResult("Update Successfully");
    }
}