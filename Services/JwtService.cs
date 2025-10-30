
using System.IdentityModel.Tokens.Jwt;

namespace UserManagementOtpVerfiyApp.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(string mobileNumber, out DateTime expireAt)
        {
            var Key = _config["Jwt:Key"] ?? "dfbdsfnbvldskbnlfbvnndfjkbndsfvfdvdfvdfk242345242";
            var issuer = _config["Jwt:Issuer"] ?? "UserManagementOtpVerfiyApp";
            var minutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var m) ? m : 60 * 24 * 7;

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Key));
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            expireAt = DateTime.UtcNow.AddMinutes(minutes);
            var tokenDescriptor = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: new[]
                {
                    new System.Security.Claims.Claim("mobileNumber", mobileNumber)
                },
                expires: expireAt,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
