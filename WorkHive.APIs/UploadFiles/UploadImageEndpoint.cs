using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Owner.ManageWorkSpace.WorkSpace;
using WorkHive.Services.UploadFiles;

namespace WorkHive.APIs.UploadFiles;

public record UploadImageRequest(List<IFormFile> Images);
public record UploadImageResponse(int Status, string Message, List<string> Data);

public class UploadImageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/images/upload", async (HttpContext context, ISender sender) =>
        {
            var form = await context.Request.ReadFormAsync();
            var files = form.Files.ToList();

            if (files == null || files.Count == 0)
                return Results.BadRequest("At least one image is required");

            var result = await sender.Send(new UploadImageCommand(files));
            var response = result.Adapt<UploadImageResponse>();

            return Results.Ok(response);
        })
        .Accepts<UploadImageRequest>("multipart/form-data")
        .WithName("UploadImage")
        .Produces<CreateWorkspaceResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Upload Image")
        .WithTags("Upload on cloudinary")
        .WithDescription("Upload Image");
    }
}