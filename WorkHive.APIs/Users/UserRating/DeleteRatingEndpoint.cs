using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Users.UserRating;

namespace WorkHive.APIs.Users.UserRating;

public record DeleteRatingRequest(int UserId, int RatingId);
public record DeleteRatingResponse(string Notification);

public class DeleteRatingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/users/deleterating", async ([FromBody]DeleteRatingRequest request, [FromServices]ISender sender) =>
        {
            var command = request.Adapt<DeleteRatingCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<DeleteRatingResponse>();

            return Results.Ok(response);
        })
        .WithName("Delete rating")
        .Produces<DeleteRatingResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Delete rating")
        .RequireAuthorization("Customer")
        .WithTags("Rating")
        .WithDescription("Delete rating");
    }
}
//Customer
