using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Manage_Feedback.User_Feedback;

namespace WorkHive.APIs.Manage_Feedback.User
{

    public record CreateFeedbackRequest(string Title, string Description, int UserId, int BookingId, List<ImageFeedbackDTO> Images);

    public record CreateFeedbackResponse(string Notification);

    public class CreateFeedbackEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/feedbacks", async (CreateFeedbackRequest request, ISender sender) =>
            {
                var query = request.Adapt<CreateFeedbackCommand>();
                var result = await sender.Send(query);
                if (result.Notification.Contains("already has feedback"))
                {
                    return Results.BadRequest(result);
                }
                var response = result.Adapt<CreateFeedbackResponse>();
                return Results.Created($"/feedbacks", response);
            })
            .WithName("CreateFeedback")
            .Produces<CreateFeedbackResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Feedback")
            .WithSummary("Create a new feedback")
            .WithDescription("Creates a new feedback with the provided details, ensuring a booking has only one feedback.");
        }
    }
}