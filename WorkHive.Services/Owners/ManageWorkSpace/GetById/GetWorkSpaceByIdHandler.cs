using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.BuildingBlocks.Exceptions;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;
using WorkHive.Services.Owners.ManageWorkSpace.GetAllById;
using WorkHive.Services.Users.LoginUser;

namespace WorkHive.Services.Owners.ManageWorkSpace.GetById
{
    public record GetWorkSpaceByIdCommand(int id) : ICommand<GetWorkSpaceByIdResult>;
    public record GetWorkSpaceByIdResult(int Id, string Name, string Description, int? Capacity, string Category, string Status, int? CleanTime, int? Area, int OwnerId, List<WorkspacePriceDTO> Prices,
    List<WorkspaceImageDTO> Images);

    public record WorkspacePriceDTO(int Id, decimal? Price, string Category);
    public record WorkspaceImageDTO(int Id, string ImgUrl);


    public class GetWorkSpaceByIdValidator : AbstractValidator<GetWorkSpaceByIdCommand>
    {
        public GetWorkSpaceByIdValidator()
        {

        }
    }

    public class GetWorkSpaceByIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : ICommandHandler<GetWorkSpaceByIdCommand, GetWorkSpaceByIdResult>
    {
        public async Task<GetWorkSpaceByIdResult> Handle(GetWorkSpaceByIdCommand command, CancellationToken cancellationToken)
        {
            var workspace = await workSpaceManageUnit.Workspace.GetWorkSpaceById(command.id);

            if (workspace == null)
            {
                throw new NotFoundException($"Workspace with ID {command.id} not found!");
            }

            return new GetWorkSpaceByIdResult(
                workspace.Id,
                workspace.Name,
                workspace.Description,
                workspace.Capacity,
                workspace.Category,
                workspace.Status,
                workspace.CleanTime,
                workspace.Area,
                workspace.OwnerId,
                workspace.WorkspacePrices?.Where(wp => wp != null && wp.Price != null)
                    .Select(wp => new WorkspacePriceDTO(
                        wp.Id,
                        wp.Price!.Price1, 
                        wp.Price.Category
                    )).ToList() ?? new List<WorkspacePriceDTO>(),
                workspace.WorkspaceImages?.Where(wi => wi != null && wi.Image != null)
                    .Select(wi => new WorkspaceImageDTO(
                        wi.Image!.Id, 
                        wi.Image.ImgUrl ?? string.Empty 
                    )).ToList() ?? new List<WorkspaceImageDTO>()
            );
        }
    }

}
