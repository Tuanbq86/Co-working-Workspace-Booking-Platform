using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.Services.Users.GetUser;

public record GetAllUserQuery() : IQuery<GetAllUserResult>;
public record GetAllUserResult(List<UserDTO> Users);

public class GetAllUserHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetAllUserQuery, GetAllUserResult>
{
    public async Task<GetAllUserResult> Handle(GetAllUserQuery query, 
        CancellationToken cancellationToken)
    {
        var users = await userUnit.User.GetAllAsync();

        var others = users.Where(x => !x.RoleId.Equals(4)).ToList();

        List<UserDTO> otherResult = new List<UserDTO>();

        foreach (var item in others)
        {
            var role = userUnit.Role.GetById(item.RoleId);
            var UserDto = new UserDTO
            {
                Id = item.Id,
                Name = item.Name,
                Phone = item.Phone,
                Email = item.Email.Trim(),
                Status = item.Status,
                Avatar = item.Avatar,
                Location = item.Location,
                DateOfBirth = item.DateOfBirth,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Sex = item.Sex,
                RoleName = role.RoleName
            };

            otherResult.Add(UserDto);
        }

        return new GetAllUserResult(otherResult);
    }
}
