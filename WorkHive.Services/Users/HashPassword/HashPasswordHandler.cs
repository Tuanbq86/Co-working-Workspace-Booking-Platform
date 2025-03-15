using WorkHive.BuildingBlocks.CQRS;

namespace WorkHive.Services.Users.HashPassword;

public record HashPasswordCommand(string Password)
    : ICommand<HashPasswordResult>;
public record HashPasswordResult(string HashPassword);

public class HashPasswordHandler
    : ICommandHandler<HashPasswordCommand, HashPasswordResult>
{
    public async Task<HashPasswordResult> Handle(HashPasswordCommand command, 
        CancellationToken cancellationToken)
    {
        return new HashPasswordResult(BCrypt.Net.BCrypt.EnhancedHashPassword(command.Password));
    }
}
