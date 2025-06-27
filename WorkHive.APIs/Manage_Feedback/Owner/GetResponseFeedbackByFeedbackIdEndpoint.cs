using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response;

namespace WorkHive.APIs.Manage_Feedback.Owner
{
    public class GetResponseFeedbackByFeedbackIdEndpoint : ICarterModule
    {
        /// <summary>
        /// Registers the endpoint for retrieving owner response feedback by FeedbackId.
        /// </summary>
        /// <param name="app"></param>
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/response-feedbacks/feedback/{feedbackId:int}", async (int feedbackId, ISender sender) =>
            {
                var query = new GetResponseFeedbackByFeedbackIdQuery(feedbackId);
                var result = await sender.Send(query);

                return result == null
                    ? Results.NotFound($"No response feedback found for FeedbackId {feedbackId}")
                    : Results.Ok(result);
            })
            .WithName("GetResponseFeedbackByFeedbackId")
            .Produces<GetResponseFeedbackByFeedbackIdResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner Response Feedback")
            .WithSummary("Get the first owner response feedback by FeedbackId")
            .WithDescription("Retrieves the first owner response feedback associated with a given FeedbackId.");
        }
    }

}
