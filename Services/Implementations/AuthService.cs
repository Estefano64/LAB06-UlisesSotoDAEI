using LAB06_UlisesSoto.DTOs;
using LAB06_UlisesSoto.Models;
using LAB06_UlisesSoto.Services.Interfaces;
// using LAB06_UlisesSoto.Data; // Removed, using scaffolded context
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LAB06_UlisesSoto.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public AuthService(
            IConfiguration configuration,
            ILogger<AuthService> logger,
            ApplicationDbContext context,
            IPasswordService passwordService)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<TokenResponseDto?> AuthenticateAsync(LoginDto loginDto)
        {
            try
            {
                // Buscar usuario en la base de datos
                var user = await GetUserByUsernameAsync(loginDto.User);

                if (user == null || user.IsActive != true)
                {
                    _logger.LogWarning("Usuario no encontrado o inactivo: {User}", loginDto.User);
                    return null;
                }

                // Verificar contraseña
                if (!ValidateUserCredentials(user, loginDto.Password))
                {
                    _logger.LogWarning("Contraseña incorrecta para: {User}", loginDto.User);
                    return null;
                }

                // Generar token JWT
                var token = GenerateJwtToken(user.Username, user.Role, user.IdUser.ToString());
                var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpiryInMinutes", 30);
                var expirationTime = DateTime.Now.AddMinutes(expirationMinutes);

                _logger.LogInformation("Token generado exitosamente para: {User}", user.Username);

                return new TokenResponseDto
                {
                    Token = token,
                    Expires = expirationTime,
                    User = user.Username,
                    Role = user.Role,
                    Message = "Autenticación exitosa"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en autenticación para usuario: {User}", loginDto.User);
                throw;
            }
        }

        public ProtectedDataDto GetProtectedData(IEnumerable<Claim> userClaims)
        {
            var userName = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Unknown";
            var role = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Unknown";

            return new ProtectedDataDto
            {
                Message = "¡Datos protegidos! Solo usuarios autenticados pueden ver esto.",
                User = userName,
                Role = role,
                Timestamp = DateTime.Now,
                Data = new
                {
                    SecretInfo = "Información confidencial",
                    AdminLevel = "Nivel máximo",
                    Permissions = new[] { "READ", "WRITE", "DELETE", "ADMIN" }
                }
            };
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public bool ValidateUserCredentials(User user, string password)
        {
            return _passwordService.VerifyPassword(password, user.PasswordHash);
        }

        private string GenerateJwtToken(string user, string role, string userId)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"] ?? "TuClaveSecretaSuperSegura123456789012345678901234567890";
            var issuer = jwtSettings["Issuer"] ?? "LAB06-Universidad";
            var audience = jwtSettings["Audience"] ?? "LAB06-Students";
            var expiryInMinutes = _configuration.GetValue<int>("Jwt:ExpiryInMinutes", 30);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}