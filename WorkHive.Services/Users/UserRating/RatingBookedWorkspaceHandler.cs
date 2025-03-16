using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.UserRating;

public record RatingBookedWorkspaceCommand
    (int BookingId, Byte Rate, string Comment, List<RatingImage> Images) 
    : ICommand<RatingBookedWorkspaceResult>;
public record RatingBookedWorkspaceResult(string Notification, int BookingIsReview);
public record RatingImage(string Url);

public class RatingBookedWorkspaceHandler(IUserRatingUnitOfWork userRating)
    : ICommandHandler<RatingBookedWorkspaceCommand, RatingBookedWorkspaceResult>
{
    public async Task<RatingBookedWorkspaceResult> Handle(RatingBookedWorkspaceCommand command, 
        CancellationToken cancellationToken)
    {
        //Check booking is null
        var booking = userRating.booking.GetById(command.BookingId);

        if (booking is null)
            return new RatingBookedWorkspaceResult("Không tìm thấy booking để đánh giá", 0);

        //Check rated booking
        var existingRating = userRating.rating.GetAll()
            .Where(r => r.BookingId == booking.Id).FirstOrDefault();

        if (existingRating is not null)
            return new RatingBookedWorkspaceResult("Bạn đã đánh giá booking này rồi", booking.IsReview!.Value);

        //Add new rating for booking
        var rating = new Rating
        {
            Rate = command.Rate,
            Comment = command.Comment,
            CreatedAt = DateTime.Now,
            UserId = booking.UserId,
            BookingId = booking.Id
        };
        await userRating.rating.CreateAsync(rating);

        booking.IsReview = 1;
        await userRating.booking.UpdateAsync(booking);

        var workspaceRating = new WorkspaceRating
        {
            RatingId = rating.Id,
            WorkspaceId = booking.WorkspaceId
        };
        await userRating.workspaceRating.CreateAsync(workspaceRating);

        foreach (var item in command.Images)
        {
            var image = new Image
            {
               ImgUrl = item.Url,
               Title = "Rating image",
               CreatedAt = DateTime.Now,
               UpdatedAt = DateTime.Now
            };
            await userRating.image.CreateAsync(image);

            var workspaceRatingImage = new WorkspaceRatingImage
            {
                WorkspaceRatingId = workspaceRating.Id,
                ImageId = image.Id
            };
            await userRating.workspaceRatingImage.CreateAsync(workspaceRatingImage);
        }

        return new RatingBookedWorkspaceResult("Đánh giá thành công", booking.IsReview.Value);
    }
}
