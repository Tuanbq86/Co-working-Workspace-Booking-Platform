using Carter;
using MediatR;
using WorkHive.Services.Owners.Base_Owner;

namespace WorkHive.APIs.Owner.Manage_Owner
{
    public class GetOwnerVerifyRequestByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-verify-requests/{id:int}", async (int id, ISender sender) =>
            {
                var query = new GetOwnerVerifyRequestByIdQuery(id);
                var result = await sender.Send(query);

                if (result is null)
                {
                    return Results.NotFound($"Không tìm thấy request với ID: {id}");
                }

                return Results.Ok(result);
            })
            .WithName("GetOwnerVerifyRequestById")
            .Produces<GetOwnerVerifyRequestResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Owner")
            .WithSummary("Get owner verify request by ID")
            .WithDescription("Retrieve a specific owner verification request by its ID.");
        }
    }
}
