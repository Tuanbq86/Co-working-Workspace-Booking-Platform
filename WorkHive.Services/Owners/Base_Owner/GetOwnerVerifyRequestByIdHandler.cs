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
    public record GetOwnerVerifyRequestByIdQuery(int Id) : IQuery<GetOwnerVerifyRequestResult?>;

    public class GetOwnerVerifyRequestByIdValidator : AbstractValidator<GetOwnerVerifyRequestByIdQuery>
    {
        public GetOwnerVerifyRequestByIdValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id phải lớn hơn 0.");
        }
    }

    public class GetOwnerVerifyRequestByIdHandler(IWorkSpaceManageUnitOfWork unit)
        : IQueryHandler<GetOwnerVerifyRequestByIdQuery, GetOwnerVerifyRequestResult?>
    {
        public async Task<GetOwnerVerifyRequestResult?> Handle(GetOwnerVerifyRequestByIdQuery query, CancellationToken cancellationToken)
        {
            var request = await unit.OwnerVerifyRequest.GetByIdAsync(query.Id);
            if (request is null) return null;

            return new GetOwnerVerifyRequestResult(
                request.Id,
                request.OwnerId,
                request.UserId,
                request.Message,
                request.Status,
                request.GoogleMapUrl,
                request.LicenseName,
                request.LicenseNumber,
                request.LicenseAddress,
                request.CharterCapital,
                request.LicenseFile,
                request.OwnerName,
                request.Facebook,
                request.Instagram,
                request.Tiktok,
                request.CreatedAt,
                request.UpdatedAt,
                request.RegistrationDate
            );
        }

    }
}
