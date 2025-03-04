//using FluentValidation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WorkHive.BuildingBlocks.CQRS;
//using WorkHive.BuildingBlocks.Exceptions;
//using WorkHive.Data.Models;
//using WorkHive.Repositories.IUnitOfWork;

//namespace WorkHive.Services.Owners.ManageWorkSpace.GetAllById
//{
//    public record GetWorkSpacesByOwnerIdQuery(int id) : IQuery<List<GetWorkSpaceByOwnerIdResult>>;

//    public record GetWorkSpaceByOwnerIdResult(int Id, string Name, string Description, int? Capacity, string Category, string Status, int? CleanTime, int? Area, int OwnerId, List<WorkspacePriceDTO> Prices,
//    List<WorkspaceImageDTO> Images);

//    public record WorkspacePriceDTO(int Id, decimal? Price, string Category);
//    public record WorkspaceImageDTO(int Id, string ImgUrl);




//    public class GetWorkSpacesByOwnerIdValidator : AbstractValidator<GetWorkSpacesByOwnerIdQuery>
//    {
//        //public GetWorkSpacesByOwnerIdValidator()
//        //{
//        //    RuleFor(x => x.id)
//        //        .GreaterThan(0).WithMessage("Owner ID must be greater than 0");
//        //}
//    }
//    public class GetWorkSpacesByOwnerIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
//    : IQueryHandler<GetWorkSpacesByOwnerIdQuery, List<GetWorkSpaceByOwnerIdResult>>
//    {
//        public async Task<List<GetWorkSpaceByOwnerIdResult>> Handle(GetWorkSpacesByOwnerIdQuery Query, CancellationToken cancellationToken)
//        {
//            var workspaces = await workSpaceManageUnit.Workspace.GetAllWorkSpaceByOwnerIdAsync(Query.id);

//            if (workspaces == null || !workspaces.Any())
//            {
//                throw new NotFoundException($"No workspaces found for OwnerId {Query.id}");
//            }

//            return workspaces.Select(ws => new GetWorkSpaceByOwnerIdResult(
//                ws.Id,
//                ws.Name,
//                ws.Description,
//                ws.Capacity,
//                ws.Category,
//                ws.Status,
//                ws.CleanTime,
//                ws.Area,
//                ws.OwnerId,
//                ws.WorkspacePrices.Select(wp => new WorkspacePriceDTO(
//                    wp.Price.Id,
//                    wp.Price.Price1,
//                    wp.Price.Category
//                )).ToList(),
//                ws.WorkspaceImages.Select(wi => new WorkspaceImageDTO(
//                    wi.Image.Id,
//                    wi.Image.ImgUrl
//                )).ToList()
//                )).ToList();
//        }
//    }
//}
