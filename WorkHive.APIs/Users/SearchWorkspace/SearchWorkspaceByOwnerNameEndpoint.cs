//using Carter;
//using MediatR;
//using WorkHive.Services.Users.SearchWorkspace;

//namespace WorkHive.APIs.Users.SearchWorkspace;

//public record SearchWorkspaceByOwnerNameResponse(List<WorkspaceSearchByOwnerNameDTO> WorkspaceSearchByOwnerNameDTOs);

//public class SearchWorkspaceByOwnerNameEndpoint : ICarterModule
//{
//    public void AddRoutes(IEndpointRouteBuilder app)
//    {
//        app.MapGet("/users/searchbyusernameforuser{OwnerName}", async (string OwnerName, ISender sender) =>
//        {
//            var result = await sender.Send(new SearchWorkspaceByOwnerNameQuery(OwnerName));

//            var response = new SearchWorkspaceByOwnerNameResponse(result.WorkspaceSearchByOwnerNameDTOs);

//            return Results.Ok(response);
//        })
//        .WithName("Search by owner name")
//        .Produces<SearchWorkspaceByOwnerNameResponse>(StatusCodes.Status200OK)
//        .ProducesProblem(StatusCodes.Status400BadRequest)
//        .WithSummary("Search by owner name")
//        .WithTags("Search")
//        .WithDescription("Search by owner name");
//    }
//}
