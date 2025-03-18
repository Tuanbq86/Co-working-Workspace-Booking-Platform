//using Carter;
//using Mapster;
//using MediatR;
//using WorkHive.Services.Manage_Feedback.User_Feedback;

//namespace WorkHive.APIs.Manage_Feedback.User
//{

//    public record CreateFeedbackRequest(string Description, int UserId, int OwnerId, List<ImageFeedbackDTO> Images);

//    public record CreateFeedbackResponse(string Notification);

//    public class CreateFeedbackEndpoint : ICarterModule
//    {
//        public void AddRoutes(IEndpointRouteBuilder app)
//        {
//            app.MapPost("/feedbacks", async (CreateFeedbackRequest request, ISender sender) =>
//            {
//                var query = request.Adapt<CreateFeedbackCommand>();
//                var result = await sender.Send(query);
//                var response = result.Adapt<CreateFeedbackResponse>();
//                return Results.Created($"/feedbacks", response);
//            })
//            .WithName("CreateFeedback")
//            .Produces<CreateFeedbackResponse>(StatusCodes.Status201Created)
//            .ProducesProblem(StatusCodes.Status400BadRequest)
//            .WithTags("Feedback")
//            .WithSummary("Create a new feedback")
//            .WithDescription("Creates a new feedback with the provided details.");
//        }
//    }
    
//}
