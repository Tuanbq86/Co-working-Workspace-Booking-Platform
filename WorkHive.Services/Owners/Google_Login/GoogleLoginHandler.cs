using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.Google_Login
{
    public record GoogleLoginCommand(string IdToken) : IRequest<GoogleLoginResult>;

    public record GoogleLoginResult(string Token, WorkspaceOwner Owner);

    public class GoogleLoginHandler(IWorkSpaceManageUnitOfWork unit, IConfiguration config)
        : IRequestHandler<GoogleLoginCommand, GoogleLoginResult>
    {
        public async Task<GoogleLoginResult> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={request.IdToken}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Token không hợp lệ từ Google.");

            var googleInfo = await response.Content.ReadFromJsonAsync<GoogleTokenInfo>(cancellationToken: cancellationToken);
            if (googleInfo == null || string.IsNullOrWhiteSpace(googleInfo.Email))
                throw new Exception("Không thể xác thực token Google.");

            var owner = await unit.WorkspaceOwner.FindByEmailAsync(googleInfo.Email);
            if (owner == null)
            {
                owner = new WorkspaceOwner
                {
                    Email = googleInfo.Email,
                    OwnerName = googleInfo.Name,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    Avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(googleInfo.Name)}",
                    PhoneStatus = "Unverified"
                };

                await unit.WorkspaceOwner.CreateAsync(owner);
                await unit.SaveAsync(); // Đừng quên commit transaction nếu cần
            }

            // JWT Token creation
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, owner.Email),
                new Claim(ClaimTypes.Name, owner.OwnerName ?? ""),
                new Claim("OwnerId", owner.Id.ToString()),
                new Claim(ClaimTypes.Role, "WorkspaceOwner")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new GoogleLoginResult(
                new JwtSecurityTokenHandler().WriteToken(jwt),
                owner
            );
        }
    }
}
