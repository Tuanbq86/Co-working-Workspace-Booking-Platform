using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using WorkHive.BuildingBlocks.CQRS;
using FluentValidation;
using WorkHive.BuildingBlocks.Exceptions;

namespace WorkHive.Services.UploadFiles;

public record UploadImageCommand(List<IFormFile> Images) : ICommand<UploadImageResult>;
public record UploadImageResult(int Status, string Message, List<string> Data);

public class UploadImageValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageValidator()
    {
        RuleFor(x => x.Images)
            .NotEmpty().WithMessage("At least one image is required");

        RuleForEach(x => x.Images)
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
        if (request.Images == null || !request.Images.Any())
            throw new BadRequestException("At least one image is required");

        var imageUrls = new List<string>();

        foreach (var image in request.Images)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, image.OpenReadStream()),
                Folder = "IMAGES",
                Transformation = new Transformation().Width(500).Height(500).Crop("fill")
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Failed to upload image: {image.FileName}");

            imageUrls.Add(uploadResult.SecureUrl.ToString());
        }

        return new UploadImageResult(200, "Upload success!", imageUrls);

    }
}