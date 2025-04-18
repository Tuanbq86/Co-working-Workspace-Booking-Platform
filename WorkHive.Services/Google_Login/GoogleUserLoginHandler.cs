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
using System.Threading.Tasks;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Google_Login
{
    public record GoogleUserLoginCommand(string IdToken) : IRequest<GoogleUserLoginResult>;

    public record GoogleUserLoginResult(string Token, User User);

    public class GoogleUserLoginHandler(IUserUnitOfWork unit, IConfiguration config)
        : IRequestHandler<GoogleUserLoginCommand, GoogleUserLoginResult>
    {
        public async Task<GoogleUserLoginResult> Handle(GoogleUserLoginCommand request, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={request.IdToken}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Token không hợp lệ từ Google.");

            var googleInfo = await response.Content.ReadFromJsonAsync<GoogleTokenInfo>(cancellationToken: cancellationToken);
            if (googleInfo == null || string.IsNullOrWhiteSpace(googleInfo.Email))
                throw new Exception("Không thể xác thực token Google.");

            var user = await unit.User.FindByEmailAsync(googleInfo.Email);
            if (user == null)
            {
                user = new User
                {
                    Email = googleInfo.Email,
                    Name = googleInfo.Name,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    Avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(googleInfo.Name)}",
                    Phone = "",
                    RoleId = 2, // Hoặc ID tương ứng với role người dùng
                    IsBan = 0,
                };

                await unit.User.CreateAsync(user);
                await unit.SaveAsync();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new GoogleUserLoginResult(
                new JwtSecurityTokenHandler().WriteToken(jwt),
                user
            );
        }
    }

    public class GoogleTokenInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
