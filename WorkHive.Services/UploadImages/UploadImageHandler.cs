using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using WorkHive.BuildingBlocks.CQRS;
using FluentValidation;
using WorkHive.BuildingBlocks.Exceptions;

namespace WorkHive.Services.UploadImages;

public record UploadImageCommand(IFormFile Image) : ICommand<UploadImageResult>;
public record UploadImageResult(int Status, string Message, List<string> Data);

public class UploadImageValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageValidator()
    {
        RuleFor(x => x.Image)
            .NotNull().WithMessage("Image is required")
            .Must(x => x.ContentType.Contains("image"))
            .WithMessage("Only image files are allowed");
    }
}

public class UploadImageHandler(Cloudinary cloudinary)
    : ICommandHandler<UploadImageCommand, UploadImageResult>
{
    public async Task<UploadImageResult> Handle(UploadImageCommand request, 
        CancellationToken cancellationToken)
    {
        if (request.Image is null || request.Image.Length == 0)
            throw new BadRequestException("Image is required");

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(request.Image.FileName, request.Image.OpenReadStream()),
            Folder = "IMAGES",
            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
        };

        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception("Failed to upload image");

        var imageUrl = uploadResult.SecureUrl.ToString();

        return new UploadImageResult(200, "Upload success !", 
            new List<string> { imageUrl });

    }
}

