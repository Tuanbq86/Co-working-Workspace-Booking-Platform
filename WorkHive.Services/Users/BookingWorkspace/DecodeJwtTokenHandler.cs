using System.IdentityModel.Tokens.Jwt;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.BookingWorkspace;

public record DecodeJwtCommand(string Token) : ICommand<DecodeJwtResult>;
public record DecodeJwtResult(Dictionary<string, string> Claims, string AvatarUrl);

public class DecodeJwtTokenHandler(ITokenRepository tokenRepo, IUserUnitOfWork userUnit)
    : ICommandHandler<DecodeJwtCommand, DecodeJwtResult>
{
    public async Task<DecodeJwtResult> Handle(DecodeJwtCommand command, 
        CancellationToken cancellationToken)
    {
        var claims = tokenRepo.DecodeJwtToken(command.Token);

        var userId = claims.FirstOrDefault(c => c.Key == JwtRegisteredClaimNames.Sub).Value;

        var user = userUnit.User.GetById(Convert.ToInt32(userId));

        return new DecodeJwtResult(claims, user.Avatar);
    }
}
