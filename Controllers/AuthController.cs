using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LAB06_UlisesSoto.DTOs;
using LAB06_UlisesSoto.Services.Interfaces;

namespace LAB06_UlisesSoto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// LOGIN: Genera token JWT. Usuarios disponibles:
        /// admin/admin123, user1/user123, profesor1/teacher123
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tokenResponse = await _authService.AuthenticateAsync(loginDto);

                if (tokenResponse == null)
                {
                    return Unauthorized(new ErrorResponseDto
                    {
                        Message = "Credenciales inválidas. Usuarios disponibles: admin/admin123, user1/user123, profesor1/teacher123"
                    });
                }

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// ENDPOINT PROTEGIDO: Solo Administradores
        /// </summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("admin-only")]
        public ActionResult<ProtectedDataDto> GetAdminData()
        {
            try
            {
                var protectedData = _authService.GetProtectedData(User.Claims);
                protectedData.Message = "¡Datos de administrador! Solo admins pueden ver esto.";
                return Ok(protectedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en endpoint de admin");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// ENDPOINT PROTEGIDO: Profesores y Administradores
        /// </summary>
        [Authorize(Policy = "TeacherOrAdmin")]
        [HttpGet("teacher-admin")]
        public ActionResult<ProtectedDataDto> GetTeacherAdminData()
        {
            try
            {
                var protectedData = _authService.GetProtectedData(User.Claims);
                protectedData.Message = "¡Datos para profesores y administradores!";
                return Ok(protectedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en endpoint teacher-admin");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// ENDPOINT PROTEGIDO: Todos los usuarios autenticados
        /// </summary>
        [Authorize(Policy = "AllUsers")]
        [HttpGet("all-users")]
        public ActionResult<ProtectedDataDto> GetAllUsersData()
        {
            try
            {
                var protectedData = _authService.GetProtectedData(User.Claims);
                protectedData.Message = "¡Datos para todos los usuarios autenticados!";
                return Ok(protectedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en endpoint all-users");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// ENDPOINT PROTEGIDO: Para compatibilidad con implementación anterior
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("protected")]
        public ActionResult<ProtectedDataDto> GetProtectedData()
        {
            try
            {
                var protectedData = _authService.GetProtectedData(User.Claims);
                return Ok(protectedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en endpoint protegido");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// UTILIDAD: Generar hash de contraseña (solo para desarrollo)
        /// </summary>
        [HttpPost("generate-hash")]
        public ActionResult<object> GenerateHash([FromBody] string password)
        {
            try
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(11));
                return Ok(new { password, hash });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando hash");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Error interno del servidor"
                });
            }
        }
    }
}