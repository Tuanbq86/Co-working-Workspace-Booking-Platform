//using Carter;
//using MediatR;
//using WorkHive.Services.Manage_Feedback.User_Feedback;

//namespace WorkHive.APIs.Manage_Feedback.User
//{
//    public record GetFeedbackByIdResponse(int Id, string Description, string Status, int UserId, int OwnerId, DateTime? CreatedAt, List<string> ImageUrls);

//    public class GetFeedbackByIdEndpoint : ICarterModule
//    {
//        public void AddRoutes(IEndpointRouteBuilder app)
//        {
//            app.MapGet("/feedbacks/{id}", async (int id, ISender sender) =>
//            {
//                var query = new GetFeedbackByIdQuery(id);
//                var result = await sender.Send(query);

//                return result == null
//                    ? Results.NotFound("Feedback not found")
//                    : Results.Ok(new GetFeedbackByIdResponse(
//                        result.Id,
//                        result.Description,
//                        result.Status,
//                        result.UserId,
//                        result.OwnerId,
//                        result.CreatedAt,
//                        result.ImageUrls));
//            })
//            .WithName("GetFeedbackById")
//            .Produces<GetFeedbackByIdResponse>(StatusCodes.Status200OK)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .WithTags("Feedback")
//            .WithSummary("Get feedback by ID")
//            .WithDescription("Retrieves feedback using its unique identifier.");
//        }
//    }
//}
