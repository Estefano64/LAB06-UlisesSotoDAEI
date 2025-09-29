using LAB06_UlisesSoto.DTOs;
using LAB06_UlisesSoto.Models;
using System.Security.Claims;

namespace LAB06_UlisesSoto.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Autentica un usuario y genera token JWT
        /// </summary>
        /// <param name="loginDto">Credenciales del usuario</param>
        /// <returns>Token de respuesta o null si credenciales inválidas</returns>
        Task<TokenResponseDto?> AuthenticateAsync(LoginDto loginDto);

        /// <summary>
        /// Obtiene datos protegidos para usuario autenticado
        /// </summary>
        /// <param name="userClaims">Claims del usuario</param>
        /// <returns>Datos protegidos</returns>
        ProtectedDataDto GetProtectedData(IEnumerable<Claim> userClaims);

        /// <summary>
        /// Busca un usuario por username
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <returns>Usuario o null si no existe</returns>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Verifica si las credenciales son válidas
        /// </summary>
        /// <param name="user">Usuario de la base de datos</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>True si las credenciales son válidas</returns>
        bool ValidateUserCredentials(User user, string password);
    }
}