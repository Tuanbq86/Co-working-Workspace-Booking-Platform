using Carter;
using FluentValidation;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;

namespace WorkHive.APIs.Owners.ManageWorkSpace.Amenity
{
    public record GetAmenitiesByWorkSpaceIdCommand(int WorkSpaceId) : ICommand<List<AmenityDTO>>;

    public record AmenityDTO(int Id, string Name, decimal? Price, int? Quantity, string ImgUrl, string Description, string Category, string Status);

    public class GetAmenitiesByWorkSpaceIdValidator : AbstractValidator<GetAmenitiesByWorkSpaceIdCommand>
    {
        public GetAmenitiesByWorkSpaceIdValidator()
        {
            RuleFor(x => x.WorkSpaceId)
                .GreaterThan(0).WithMessage("WorkSpace ID must be greater than 0");
        }
    }

    public record GetAmenitiesByWorkSpaceIdResponse(List<AmenityDTO> Amenities);

    public class GetAmenitiesByWorkSpaceIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/amenities/WorkSpace/{WorkSpaceId}", async (int WorkSpaceId, ISender sender) =>
            {
                var command = new GetAmenitiesByWorkSpaceIdCommand(WorkSpaceId);
                var result = await sender.Send(command);
                var response = new GetAmenitiesByWorkSpaceIdResponse(result);

                return Results.Ok(response);
            })
            .WithName("GetAmenitiesByWorkSpaceId")
            .Produces<GetAmenitiesByWorkSpaceIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Amenities by WorkSpace ID")
            .WithDescription("Retrieve all amenities belonging to a specific WorkSpace.");
        }
    }
}
