using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.DTOService;

namespace WorkHive.Services.Managers;

public record GetAllStaffQuery() : IQuery<GetAllStaffResult>;
public record GetAllStaffResult(List<UserDTOForManager> Staffs);

public class GetAllStaffHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetAllStaffQuery, GetAllStaffResult>
{
    public async Task<GetAllStaffResult> Handle(GetAllStaffQuery query, 
        CancellationToken cancellationToken)
    {
        var staffs = userUnit.User.GetAll().Where(x => x.RoleId.Equals(3)).ToList();

        List<UserDTOForManager> result = new List<UserDTOForManager>();

        foreach(var item in staffs)
        {
            var UserDto = new UserDTOForManager
            {
                Id = item.Id,
                Name = item.Name,
                Phone = item.Phone,
                Email = item.Email,
                Status = item.Status,
                Avatar = item.Avatar,
                Location = item.Location,
                DateOfBirth = item.DateOfBirth,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Sex = item.Sex,
                RoleId = item.RoleId
            };

            result.Add(UserDto);
        }

        return new GetAllStaffResult(result);
    }
}
