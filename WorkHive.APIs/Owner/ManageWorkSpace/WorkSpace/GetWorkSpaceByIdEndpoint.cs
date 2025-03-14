using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Workspace;
using WorkHive.Services.Owners.ManageWorkSpace.CRUD_Base_Workspace;

namespace WorkHive.APIs.Owner.ManageWorkSpace.WorkSpace
{
    public record GetWorkSpaceByIdResponse(GetWorkSpaceByIdResult GetWorkSpaceByIdResult);

    public class GetWorkSpaceByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/workspaces/{id}", async (int id, ISender sender) =>
            {
                var query = new GetWorkSpaceByIdQuery(id);
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetWorkSpaceByIdResponse>()); 
                }
                var response = new GetWorkSpaceByIdResponse(result);

                return Results.Ok(response);
            })
            .WithName("GetWorkSpaceById")
            .Produces<GetWorkSpaceByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Workspace")
            .WithSummary("Get Workspace by ID")
            .WithDescription("Retrieve a workspace using its ID.");
        }
    }
}

