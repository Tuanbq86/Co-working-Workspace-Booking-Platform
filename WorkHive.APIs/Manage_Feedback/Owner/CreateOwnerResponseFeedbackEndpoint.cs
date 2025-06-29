﻿using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response;

namespace WorkHive.APIs.Manage_Feedback.Owner
{
    /// <summary>
    /// Command to create a new owner response feedback.
    /// </summary>
    /// <param name="Title"></param>
    /// <param name="Description"></param>
    /// <param name="OwnerId"></param>
    /// <param name="FeedbackId"></param>
    /// <param name="Images"></param>
    public record CreateOwnerResponseFeedbackRequest(string Title, string Description, int OwnerId, int FeedbackId, List<ImageResponseFeedbackDTO>? Images = null);

    public record CreateOwnerResponseFeedbackResponse(string Notification);
    public class CreateOwnerResponseFeedbackEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/owner-response-feedbacks", async (CreateOwnerResponseFeedbackRequest request, ISender sender) =>
            {
                var query = request.Adapt<CreateOwnerResponseFeedbackCommand>();
                var result = await sender.Send(query);
                var response = result.Adapt<CreateOwnerResponseFeedbackResponse>();
                return Results.Created($"/owner-response-feedbacks", response);
            })
            .WithName("CreateOwnerResponseFeedback")
            .Produces<CreateOwnerResponseFeedbackResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Owner Response Feedback")
            .WithSummary("Create a new owner response feedback")
            .WithDescription("Creates a new owner response feedback with the provided details.");
        }
    }
}
