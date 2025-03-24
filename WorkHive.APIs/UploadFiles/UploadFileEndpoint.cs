using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Owner.ManageWorkSpace.WorkSpace;
using WorkHive.Services.UploadFiles;

namespace WorkHive.APIs.UploadFiles;

public record UploadFileRequest(List<IFormFile> Files);
public record UploadFileResponse(int Status, string Message, List<string> Data);

public class UploadFileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/upload", async (HttpContext context, ISender sender) =>
        {
            //Đọc multipart/form-data từ request 
            var form = await context.Request.ReadFormAsync();
            //Lấy danh sách file từ form
            var files = form.Files.ToList();
            //Kiểm tra
            if (files == null || files.Count == 0)
                return Results.BadRequest("At least one file is required");
            //Gửi Handler xử lý
            var result = await sender.Send(new UploadFileCommand(files));
            var response = result.Adapt<UploadFileResponse>();

            return Results.Ok(response);
        })
        .Accepts<UploadFileRequest>("multipart/form-data")
        .WithName("Upload PDF File")
        .Produces<UploadFileResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Upload File")
        .WithTags("Upload on cloudinary")
        .WithDescription("Upload PDF File");
    }
}