using System.Reflection.Metadata.Ecma335;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.Services.Users.GetUser;

public record GetUserByIdQuery(int Id) : IQuery<GetUserByIdResult>;
public record GetUserByIdResult(UserDTO User);

public class GetUserByIdHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetUserByIdQuery, GetUserByIdResult>
{
    public async Task<GetUserByIdResult> Handle(GetUserByIdQuery query, 
        CancellationToken cancellationToken)
    {
        var user = userUnit.User.GetAll().FirstOrDefault(u => u.Id.Equals(query.Id));

        if (user is null)
            return new GetUserByIdResult(new UserDTO());
            

        var userDTO = new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Email = user.Email.Trim(),
            Password = user.Password,
            Status = user.Status,
            Avatar = user.Avatar,
            Location = user.Location,
            DateOfBirth = user.DateOfBirth,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Sex = user.Sex,
            RoleId = user.RoleId
        };

        return new GetUserByIdResult(userDTO);
    }
}
