using System.ComponentModel.DataAnnotations;

namespace LAB06_UlisesSoto.DTOs
{
    /// <summary>
    /// DTO para el login del usuario
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        public string User { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para la respuesta del token JWT
    /// </summary>
    public class TokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public string User { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para los datos protegidos
    /// </summary>
    public class ProtectedDataDto
    {
        public string Message { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public object Data { get; set; } = new();
    }

    /// <summary>
    /// DTO para respuestas de error
    /// </summary>
    public class ErrorResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}