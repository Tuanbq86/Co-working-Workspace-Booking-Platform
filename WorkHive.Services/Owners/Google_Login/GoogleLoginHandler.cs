//using MediatR;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Net.Http.Json;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using WorkHive.Data.Models;
//using WorkHive.Repositories.IRepositories;

//namespace WorkHive.Services.Owners.Google_Login
//{
//    public class GoogleLoginCommand : IRequest<GoogleLoginResult>
//    {
//        public string IdToken { get; set; }
//    }
//    public class GoogleLoginResult
//    {
//        public string Token { get; set; }
//        public WorkspaceOwner Owner { get; set; }
//    }

//    public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, GoogleLoginResult>
//    {
//        private readonly IWorkspaceOwnerRepository _repository;
//        private readonly IConfiguration _config;

//        public GoogleLoginHandler(IWorkspaceOwnerRepository repository, IConfiguration config)
//        {
//            _repository = repository;
//            _config = config;
//        }

//        public async Task<GoogleLoginResult> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
//        {
//            using var httpClient = new HttpClient();
//            var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={request.IdToken}");

//            if (!response.IsSuccessStatusCode)
//                throw new Exception("Token không hợp lệ từ Google.");

//            var googleInfo = await response.Content.ReadFromJsonAsync<GoogleTokenInfo>();
//            if (googleInfo == null || string.IsNullOrWhiteSpace(googleInfo.Email))
//                throw new Exception("Không thể xác thực token Google.");

//            // Tìm hoặc tạo mới
//            var owner = await _repository.FindWorkspaceOwnerByEmail(googleInfo.Email);
//            if (owner == null)
//            {
//                owner = new WorkspaceOwner
//                {
//                    Email = googleInfo.Email,
//                    IdentityName = googleInfo.Name,
//                    Status = "Pending",
//                    CreatedAt = DateTime.UtcNow,
//                    Avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(googleInfo.Name)}",
//                    PhoneStatus = "Unverified"
//                };

//                await _repository.CreateAsync(owner);
//            }

//            // JWT
//            var claims = new List<Claim>
//        {
//            new Claim(ClaimTypes.Email, owner.Email),
//            new Claim(ClaimTypes.Name, owner.IdentityName ?? ""),
//            new Claim("OwnerId", owner.Id.ToString()),
//            new Claim(ClaimTypes.Role, "WorkspaceOwner")
//        };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var jwt = new JwtSecurityToken(
//                issuer: _config["Jwt:Issuer"],
//                audience: _config["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(1),
//                signingCredentials: creds);

//            return new GoogleLoginResult
//            {
//                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
//                Owner = owner
//            };
//        }
//    }

//}
