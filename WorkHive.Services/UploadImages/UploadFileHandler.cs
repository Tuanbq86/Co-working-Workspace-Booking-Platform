using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using WorkHive.BuildingBlocks.CQRS;
using FluentValidation;
using WorkHive.BuildingBlocks.Exceptions;

namespace WorkHive.Services.UploadImages;

public record UploadFileCommand(List<IFormFile> Files) : ICommand<UploadFileResult>;
public record UploadFileResult(int Status, string Message, List<string> Data);

public class UploadFileValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileValidator()
    {
        RuleFor(x => x.Files)
            .NotEmpty().WithMessage("At least one file is required");

        RuleForEach(x => x.Files)
            .NotNull().WithMessage("File is required")
            .Must(x => x.ContentType.Contains("image") || x.ContentType.Contains("pdf"))
            .WithMessage("Only image and PDF files are allowed");
    }
}

public class UploadFileHandler(Cloudinary cloudinary)
    : ICommandHandler<UploadFileCommand, UploadFileResult>
{
    public async Task<UploadFileResult> Handle(UploadFileCommand request, 
        CancellationToken cancellationToken)
    {
        if (request.Files == null || !request.Files.Any())
            throw new BadRequestException("At least one file is required");

        //khởi tạo danh sách url để lưu trữ
        var fileUrls = new List<string>();

        //Duyệt và tải tham số lên cho cloudinary
        foreach (var file in request.Files)
        {
            var uploadParams = new ImageUploadParams
            {
                //Đọc nội dung để gửi lên cloudinary
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                //Định nghĩa thư mục chứa tệp trên cloudinary
                Folder = "FILES",
                //Nếu là ảnh xử lý sao cho đúng tỷ lệ
                Transformation = file.ContentType.Contains("image")
                    ? new Transformation().Width(500).Height(500).Crop("fill")
                    : null
            };

            //Tải lên và kiểm tra
            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Failed to upload file: {file.FileName}");

            fileUrls.Add(uploadResult.SecureUrl.ToString());
        }

        return new UploadFileResult(200, "Upload success!", fileUrls);
    }
}

