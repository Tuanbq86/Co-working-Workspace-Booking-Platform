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
    public record GetOwnerVerifyRequestsByOwnerIdQuery(int OwnerId) : IQuery<List<GetOwnerVerifyRequestsByOwnerIdResult>>;

    public record GetOwnerVerifyRequestsByOwnerIdResult(
        int Id,
        int OwnerId,
        int? UserId,
        string Message,
        string Status,
        string GoogleMapUrl,
        string LicenseName,
        string LicenseNumber,
        string LicenseAddress,
        decimal? CharterCapital,
        string LicenseFile,
        string OwnerName,
        DateOnly? RegistrationDate,
        string Facebook,
        string Instagram,
        string Tiktok,
        DateTime? CreatedAt,
        DateTime? UpdatedAt
    );

    public class GetOwnerVerifyRequestsByOwnerIdValidator : AbstractValidator<GetOwnerVerifyRequestsByOwnerIdQuery>
    {
        public GetOwnerVerifyRequestsByOwnerIdValidator()
        {
            RuleFor(x => x.OwnerId).GreaterThan(0).WithMessage("OwnerId must be greater than 0");
        }
    }

    public class GetOwnerVerifyRequestsByOwnerIdHandler(IWorkSpaceManageUnitOfWork unitOfWork)
        : IQueryHandler<GetOwnerVerifyRequestsByOwnerIdQuery, List<GetOwnerVerifyRequestsByOwnerIdResult>>
    {
        public async Task<List<GetOwnerVerifyRequestsByOwnerIdResult>> Handle(GetOwnerVerifyRequestsByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            var requests = await unitOfWork.OwnerVerifyRequest.GetAllByOwnerIdAsync(query.OwnerId);
            return requests.Select(r => new GetOwnerVerifyRequestsByOwnerIdResult(
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
                r.RegistrationDate,
                r.Facebook,
                r.Instagram,
                r.Tiktok,
                r.CreatedAt,
                r.UpdatedAt
            )).ToList();
        }
    }
}
