using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.DTOService;

namespace WorkHive.Services.Managers;
public record GetAllOwnerQuery() : IQuery<GetAllOwnerResult>;
public record GetAllOwnerResult(List<OwnerDTO> Owners);

public class GetAllOwnerForManagerHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetAllOwnerQuery, GetAllOwnerResult>
{
    public async Task<GetAllOwnerResult> Handle(GetAllOwnerQuery query, 
        CancellationToken cancellationToken)
    {
        var owners = await userUnit.Owner.GetAllAsync();

        List<OwnerDTO> result = new List<OwnerDTO>();

        foreach(var item in owners)
        {
            result.Add(new OwnerDTO
            {
                Id = item.Id,
                CharterCapital = item.CharterCapital,
                CreatedAt = item.CreatedAt,
                DateOfBirth = item.DateOfBirth,
                Email = item.Email.Trim(),
                Facebook = item.Facebook,
                GoogleMapUrl = item.GoogleMapUrl,
                IdentityCreatedDate = item.IdentityCreatedDate,
                IdentityExpiredDate = item.IdentityExpiredDate,
                IdentityFile = item.IdentityFile,
                IdentityName = item.IdentityName,
                IdentityNumber = item.IdentityNumber,
                Instagram = item.Instagram,
                LicenseAddress = item.LicenseAddress,
                LicenseFile = item.LicenseFile,
                LicenseName = item.LicenseName,
                LicenseNumber = item.LicenseNumber,
                Nationality = item.Nationality,
                Phone = item.Phone,
                PhoneStatus = item.PhoneStatus,
                PlaceOfOrigin = item.PlaceOfOrigin,
                PlaceOfResidence = item.PlaceOfResidence,
                Sex = item.Sex,
                Status = item.Status,
                Tiktok = item.Tiktok,
                UpdatedAt = item.UpdatedAt  
            });
        }

        return new GetAllOwnerResult(result);
    }
}
