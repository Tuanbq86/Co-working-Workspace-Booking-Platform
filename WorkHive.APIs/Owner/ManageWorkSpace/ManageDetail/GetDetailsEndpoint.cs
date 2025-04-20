using Carter;
using MediatR;
using WorkHive.Data.Models;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Detail;

namespace WorkHive.APIs.Owner.ManageWorkSpace.ManageDetail
{
    public record GetDetailsResponse(List<Detail> Details);

    public class GetDetailsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/details/", async (ISender sender) =>
            {
                var query = new GetAllDetailQuery();
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetDetailsResponse>());
                }
                var response = new GetDetailsResponse(result);
                return Results.Ok(response);
            })
            .WithName("GetDetails")
            .Produces<GetDetailsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Detail")
            .WithSummary("Get all details")
            .WithDescription("Retrieve all details.");
        }
    }
}
