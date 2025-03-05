using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Owner.ManageWorkSpace.WorkSpace;
using WorkHive.Services.UploadImages;

namespace WorkHive.APIs.Image;

public record UploadImageRequest(IFormFile Image);
public record UploadImageResponse(int Status, string Message, List<string> Data);

public class UploadImageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/images/upload", async(HttpContext context, ISender sender) =>
        {
            var form = await context.Request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();
            if (file == null)
                return Results.BadRequest("Image is required");

            var result = await sender.Send(new UploadImageCommand(file));
            var response = result.Adapt<UploadImageResponse>();
            return Results.Ok(response);
        })
        .Accepts<UploadImageRequest>("multipart/form-data")
        .WithName("UploadImage")
        .Produces<CreateWorkspaceResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Upload Image")
        .WithDescription("Upload Image");
    }
}
