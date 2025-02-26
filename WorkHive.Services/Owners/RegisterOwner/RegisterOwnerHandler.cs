using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Owner.RegisterOwner;

public record RegisterOwnerCommand(string Email, string Phone, 
    string Password) : ICommand<RegisterOwnerResult>;

public record RegisterOwnerResult(string Notification);

public class RegisterOwnerValidatior : AbstractValidator<RegisterOwnerCommand>
{
    public RegisterOwnerValidatior()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email format is required");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required")
            .Length(10).WithMessage("The number of characterics is exact 10 characterics");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}
public class RegisterOwnerHandler(IWorkspaceOwnerUnitOfWork ownerUnit)
    : ICommandHandler<RegisterOwnerCommand, RegisterOwnerResult>
{
    public async Task<RegisterOwnerResult> Handle(RegisterOwnerCommand command, CancellationToken cancellationToken)
    {
        //Checking exist used email and phone number for registering

        var existEmailOrPhoneOwner = ownerUnit.WorkspaceOwner.GetAll().
            Where(x => x.Email.ToLower().Equals(command.Email.ToLower()) ||
            x.Phone.ToLower().Equals(command.Phone.ToLower())).FirstOrDefault();

        if (existEmailOrPhoneOwner is not null)
            throw new BadRequestEmailOrPhoneException("Email or Phone has been used");

        //Create new Owner for registering

        var tempOwner = ownerUnit.WorkspaceOwner.RegisterOwnerByPhoneAndEmail(command.Email,
            command.Phone, command.Password);

        var newOwner = new WorkspaceOwner
        {
            Email = tempOwner.Email,
            Phone = tempOwner.Phone,
            //Using Bcrypt to hash password using SHA-512 algorithm
            //Work factor time so long when increment for safety(13)
            Password = BCrypt.Net.BCrypt.EnhancedHashPassword(tempOwner.Password, 13),
        };

        ownerUnit.WorkspaceOwner.Create(newOwner);

        await ownerUnit.SaveAsync();

        return new RegisterOwnerResult("Register successfully");
    }
}
