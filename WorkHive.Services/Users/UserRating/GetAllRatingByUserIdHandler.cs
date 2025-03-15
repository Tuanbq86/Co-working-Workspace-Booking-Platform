using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.DTOService;

namespace WorkHive.Services.Users.UserRating;

public record GetAllRatingByUserIdQuery(int UserId) : IQuery<GetAllRatingByUserIdResult>;
public record GetAllRatingByUserIdResult(List<RatingByUserIdDTO> RatingByUserIdDTOs);

public class GetAllRatingByUserIdHandler(IUserRatingUnitOfWork userRating)
    : IQueryHandler<GetAllRatingByUserIdQuery, GetAllRatingByUserIdResult>
{
    public async Task<GetAllRatingByUserIdResult> Handle(GetAllRatingByUserIdQuery query, 
        CancellationToken cancellationToken)
    {
        List<RatingByUserIdDTO> result = new List<RatingByUserIdDTO>();

        var ratings = await userRating.rating.GetAllRatingByUserId(query.UserId);

        foreach(var item in ratings)
        {
            RatingByUserIdDTO ratingDTO = new RatingByUserIdDTO();

            ratingDTO.Rate = item.Rate;
            ratingDTO.Created_At = item.CreatedAt;
            ratingDTO.Comment = item.Comment;
            ratingDTO.Images = new List<RatingImageDTO>();

            var workspaceRating = item.WorkspaceRatings
                .Where(x => x.RatingId.Equals(item.Id)).FirstOrDefault();

            //For workspace and owner
            var workspace = userRating.workspace.GetById(workspaceRating!.WorkspaceId);
            var owner = userRating.owner.GetById(workspace.OwnerId);

            ratingDTO.Workspace_Name = workspace.Name;
            ratingDTO.Owner_Name = owner.LicenseName;

            var workspaceRatingImages = userRating.workspaceRatingImage.GetAll()
                .Where(x => x.WorkspaceRatingId.Equals(workspaceRating.Id)).ToList();
            
            //For image
            foreach(var wrimage in workspaceRatingImages)
            {
                var image = userRating.image.GetById(wrimage.ImageId);
                ratingDTO.Images.Add(new RatingImageDTO(image.ImgUrl));
            }

            result.Add(ratingDTO);
        }

        return new GetAllRatingByUserIdResult(result);
    }
}
