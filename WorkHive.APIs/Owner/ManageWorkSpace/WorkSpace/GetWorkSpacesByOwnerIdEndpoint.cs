using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.GetAllById;

namespace WorkHive.APIs.Owner.ManageWorkSpace.WorkSpace
{
    public record GetWorkSpacesByOwnerIdResponse(List<GetWorkSpaceByOwnerIdResult> Workspaces);

    public class GetWorkSpacesByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/workspaces/owner/{Id}", async (int Id, ISender sender) =>
            {
                var query = new GetWorkSpacesByOwnerIdQuery(Id);
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetWorkSpacesByOwnerIdResponse>());
                }
                var response = new GetWorkSpacesByOwnerIdResponse(result);

                return Results.Ok(response);
            })
            .WithName("GetWorkSpacesByOwnerId")
            .Produces<GetWorkSpacesByOwnerIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Workspaces by Owner ID")
            .WithDescription("Retrieve all workspaces belonging to a specific owner.");
        }
    }
}
