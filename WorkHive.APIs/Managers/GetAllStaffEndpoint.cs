using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.DTOService;
using WorkHive.Services.Managers;

namespace WorkHive.APIs.Managers;

public record GetAllStaffResponse(List<UserDTOForManager> Staffs);

public class GetAllStaffEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/managers/getallstaff", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllStaffQuery());

            var response = result.Adapt<GetAllStaffResponse>();

            return Results.Ok(response);
        })
        .WithName("Get all staff")
        .Produces<GetAllStaffResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get all staff")
        .WithTags("Manager")
        .WithDescription("Get all staff");
    }
}
