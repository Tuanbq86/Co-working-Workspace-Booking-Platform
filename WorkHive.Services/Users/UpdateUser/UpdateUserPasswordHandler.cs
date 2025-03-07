using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Users.UpdateUser;

public record UpdateUserPasswordCommand
    (int UserId, string OldPassword, string NewPassword, string ConfirmPassword) 
    : ICommand<UpdateUserPasswordResult>;
public record UpdateUserPasswordResult(string Notification);

public class UpdateUserPasswordHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<UpdateUserPasswordCommand, UpdateUserPasswordResult>
{
    public async Task<UpdateUserPasswordResult> Handle(UpdateUserPasswordCommand command, 
        CancellationToken cancellationToken)
    {
        //Get userId and roleId for Booking in session containing token in a session working
        //var token = httpContext.HttpContext!.Session.GetString("token")!.ToString();
        //var listInfo = tokenRepo.DecodeJwtToken(token);

        //var userId = listInfo[JwtRegisteredClaimNames.Sub];

        var user = userUnit.User.GetById(command.UserId);

        //Check user password with old password in command
        if (BCrypt.Net.BCrypt.EnhancedVerify(command.OldPassword, user.Password))
            throw new UserBadRequestException("Error old password");

        //Check new password and confirm password
        if (command.NewPassword.ToLower().Trim()
            .Equals(command.ConfirmPassword.ToLower().Trim()))
            throw new UserBadRequestException("New password and confirm password are different");

        user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(command.ConfirmPassword, 13);

        userUnit.User.Update(user);

        await userUnit.SaveAsync();

        return new UpdateUserPasswordResult("Update successfully");
    }
}
