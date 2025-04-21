using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.CRUD_Base_Workspace;

namespace WorkHive.APIs.Owner.ManageWorkSpace
{
    public record UpdateWorkspaceRequest(
        string Name,
        string Description,
        int Capacity,
        string Category,
        string Status,
        int CleanTime,
        int Area,
        TimeOnly? OpenTime,
        TimeOnly? CloseTime,
        int? Is24h,
        string Code,
        List<PriceDTO> Prices,
        List<ImageDTO> Images,
        List<FacilityDTO> Facilities,
        List<PolicyDTO> Policies,
        List<DetailDTO> Details
    );

    public record UpdateWorkspaceResponse(string Notification);

    public class UpdateWorkspaceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/workspaces/{id}", async (int id, UpdateWorkspaceRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateWorkSpaceCommand>() with { Id = id };
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateWorkspaceResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateWorkspace")
            .Produces<UpdateWorkspaceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Workspace")
            .WithSummary("Update an existing workspace")
            .WithDescription("Updates a workspace by its ID with the provided details.");
        }
    }
}
