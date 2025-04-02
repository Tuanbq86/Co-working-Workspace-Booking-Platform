using System;
using System.IdentityModel.Tokens.Jwt;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;


namespace WorkHive.Services.Owners.LoginOwner.Decode_JWT
{

    public record DecodeJwtOwnerCommand(string Token) : ICommand<DecodeJwtOwnerResult>;
    public record DecodeJwtOwnerResult(Dictionary<string, string> Claims, string Avatar);


    class DecodeJwtTokenOwnerHandler(ITokenRepository tokenRepo, IWorkSpaceManageUnitOfWork ownerUnit)
    : ICommandHandler<DecodeJwtOwnerCommand, DecodeJwtOwnerResult>
    {
        public async Task<DecodeJwtOwnerResult> Handle(DecodeJwtOwnerCommand command,
            CancellationToken cancellationToken)
        {
            var claims = tokenRepo.DecodeJwtToken(command.Token);

            var ownerId = claims.FirstOrDefault(c => c.Key == JwtRegisteredClaimNames.Sub).Value;

            var owner = ownerUnit.WorkspaceOwner.GetById(Convert.ToInt32(ownerId));

            return new DecodeJwtOwnerResult(claims, owner.Avatar);
        }
    }
}
