using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Services.Users.BookingWorkspace;

public record DecodeJwtCommand(string Token) : ICommand<DecodeJwtResult>;
public record DecodeJwtResult(Dictionary<string, string> Claims);

public class DecodeJwtTokenHandler(ITokenRepository tokenRepo)
    : ICommandHandler<DecodeJwtCommand, DecodeJwtResult>
{
    public async Task<DecodeJwtResult> Handle(DecodeJwtCommand command, 
        CancellationToken cancellationToken)
    {
        var claims = tokenRepo.DecodeJwtToken(command.Token);

        return new DecodeJwtResult(claims);
    }
}
