using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.Services.Users.GetUser;

public record GetAllCustomerQuery() : IQuery<GetAllCustomerResult>;
public record GetAllCustomerResult(List<UserDTO> Customers);

public class GetAllCustomerHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetAllCustomerQuery, GetAllCustomerResult>
{
    public async Task<GetAllCustomerResult> Handle(GetAllCustomerQuery query, 
        CancellationToken cancellationToken)
    {
        var users = await userUnit.User.GetAllAsync();

        var customers = users.Where(x => x.RoleId.Equals(4)).ToList();

        List<UserDTO> customerResult = new List<UserDTO>();

        foreach(var item in customers)
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
                RoleName = role.RoleName,
                IsBan = item.IsBan
            };

            customerResult.Add(UserDto);
        }

        return new GetAllCustomerResult(customerResult);
    }
}
