using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.DTOService;

namespace WorkHive.Services.Users.UserRating;

public record GetAllRatingByWorkspaceIdQuery(int WorkspaceId)
    : IQuery<GetAllRatingByWorkspaceIdResult>;
public record GetAllRatingByWorkspaceIdResult(List<RatingByWorkspaceIdDTO> RatingByWorkspaceIdDTOs);

public class GetAllRatingByWorkspaceIdHandler(IUserRatingUnitOfWork userRating)
    : IQueryHandler<GetAllRatingByWorkspaceIdQuery, GetAllRatingByWorkspaceIdResult>
{
    public async Task<GetAllRatingByWorkspaceIdResult> Handle(GetAllRatingByWorkspaceIdQuery query, 
        CancellationToken cancellationToken)
    {
        List<RatingByWorkspaceIdDTO> result = new List<RatingByWorkspaceIdDTO>();

        var workspaceRatings = userRating.workspaceRating.GetAll()
            .Where(x => x.WorkspaceId.Equals(query.WorkspaceId)).ToList();

        foreach(var item in workspaceRatings)
        {
            var rating = userRating.rating.GetById(item.RatingId);
            var user = userRating.user.GetById(rating.UserId);
            var workspace = userRating.workspace.GetById(item.WorkspaceId);
            var owner = userRating.owner.GetById(workspace.OwnerId);

            var workspaceRatingImages = userRating.workspaceRatingImage.GetAll()
                .Where(x => x.WorkspaceRatingId.Equals(item.Id)).ToList();

            List<RatingByWorkspaceIdImageDTO> images = new List<RatingByWorkspaceIdImageDTO>();

            foreach (var imageItem in workspaceRatingImages)
            {
                var image = userRating.image.GetById(imageItem.ImageId);
                images.Add(new RatingByWorkspaceIdImageDTO(image.ImgUrl));
            }

            result.Add(new RatingByWorkspaceIdDTO
            {
                Rate = rating.Rate,
                Comment = rating.Comment,
                Created_At = rating.CreatedAt,
                Owner_Name = owner.LicenseName,
                Workspace_Name = workspace.Name,
                Images = images,
                User_Avatar = user.Avatar,
                User_Name = user.Name,
                RatingId = rating.Id,
                UserId = user.Id
            });
        }

        return new GetAllRatingByWorkspaceIdResult(result);
    }
}
