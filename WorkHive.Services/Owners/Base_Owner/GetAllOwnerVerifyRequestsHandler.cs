using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.Base_Owner
{
    public record GetAllOwnerVerifyRequestsQuery() : IQuery<List<GetOwnerVerifyRequestResult>>;

    public record GetOwnerVerifyRequestResult(
        int Id,
        int OwnerId,
        int UserId,
        string Message,
        string Status,
        string GoogleMapUrl,
        string LicenseName,
        string LicenseNumber,
        string LicenseAddress,
        decimal? CharterCapital,
        string LicenseFile,
        string OwnerName,
        DateOnly? RegistrationDate
    );

    public class GetAllOwnerVerifyRequestsValidator : AbstractValidator<GetAllOwnerVerifyRequestsQuery>
    {
        public GetAllOwnerVerifyRequestsValidator()
        {
        }
    }

    public class GetAllOwnerVerifyRequestsHandler(IWorkSpaceManageUnitOfWork unit)
        : IQueryHandler<GetAllOwnerVerifyRequestsQuery, List<GetOwnerVerifyRequestResult>>
    {
        public async Task<List<GetOwnerVerifyRequestResult>> Handle(GetAllOwnerVerifyRequestsQuery query, CancellationToken cancellationToken)
        {
            var requests = await unit.OwnerVerifyRequest.GetAllAsync(); // Giả sử bạn có phương thức này

            if (requests == null || !requests.Any())
            {
                return new List<GetOwnerVerifyRequestResult>();
            }

            return requests.Select(r => new GetOwnerVerifyRequestResult(
                r.Id,
                r.OwnerId,
                r.UserId,
                r.Message,
                r.Status,
                r.GoogleMapUrl,
                r.LicenseName,
                r.LicenseNumber,
                r.LicenseAddress,
                r.CharterCapital,
                r.LicenseFile,
                r.OwnerName,
                r.RegistrationDate
            )).ToList();
        }
    }
}
