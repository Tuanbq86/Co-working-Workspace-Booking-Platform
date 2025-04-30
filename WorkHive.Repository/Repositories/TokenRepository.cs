using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public sealed class TokenRepository : ITokenRepository
{
    private readonly IConfiguration _configuration;
    private readonly WorkHiveContext _context;
    public TokenRepository(IConfiguration configuration, WorkHiveContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public Dictionary<string, string> DecodeJwtToken(string token)
    {
        Dictionary<string, string> claims = new Dictionary<string, string>();

        var handler = new JwtSecurityTokenHandler(); 
        var jwtToken = handler.ReadJwtToken(token); 


        foreach (var claim in jwtToken.Claims)
        {
            claims.Add(claim.Type, claim.Value);
        }

        return claims;
    }

    public string GenerateJwtToken(User user)
    {
        //Lấy role để phân quyền
        var roleForAuthorize = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email.TrimEnd()),
        new Claim(JwtRegisteredClaimNames.Name, user.Name),
        new Claim("Phone", user.Phone.ToString()),
        new Claim("Sex", user.Sex.ToString()),
        new Claim("Status", user.Status.ToString()),
        //Phân quyền bằng roleId
        new Claim("RoleId", user.RoleId.ToString())
    };

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Issuer"]!,
            claims,
            expires: DateTime.Now.AddDays(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateJwtToken(WorkspaceOwner Owner)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, Owner.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, Owner.Email),
        new Claim("Phone", Owner.Phone),
        new Claim("RoleId", "5")
        //new Claim("Avatar", Owner.Avatar),
        //new Claim("Sex", Owner.Sex.ToString()),
        //new Claim("Status", Owner.Status.ToString()),
        };

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Issuer"]!,
            claims,
            expires: DateTime.Now.AddDays(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
