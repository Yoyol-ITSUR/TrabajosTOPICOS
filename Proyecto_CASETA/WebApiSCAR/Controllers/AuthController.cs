using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSCAR.Models; // Importa los modelos de la base de datos (User, Residente)
using BCrypt.Net; // Para la verificación de contraseñas hasheadas
using System.Diagnostics; // Necesario para Debug.WriteLine (logs de depuración)
using Newtonsoft.Json; // Importa Newtonsoft.Json!

namespace WebApiSCAR.Controllers
{
    [Route("api/[controller]")] // Define la ruta base para este controlador (ej. /api/Auth)
    [ApiController] // Indica que esta clase es un controlador de API con comportamientos específicos de API
    public class AuthController : ControllerBase
    {
        private readonly SCARContext _context; // Contexto de la base de datos para interactuar con EF Core

        /// <summary>
        /// Constructor del controlador de autenticación. Inyecta el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de SCARContext proporcionada por la inyección de dependencias.</param>
        public AuthController(SCARContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Maneja las solicitudes de inicio de sesión de los usuarios.
        /// Autentica al usuario basándose en el nombre de usuario y la contraseña,
        /// y devuelve un token de autenticación junto con el ID del residente asociado.
        /// </summary>
        /// <param name="request">Objeto LoginRequestDto que contiene el nombre de usuario y la contraseña.</param>
        /// <returns>
        /// Un objeto LoginResponseDto indicando el éxito o fracaso del login,
        /// un mensaje, un token de autenticación (si es exitoso) y el ID del residente.
        /// </returns>
        [HttpPost("login")] // Define el método HTTP POST y la ruta específica para el login (ej. /api/Auth/login)
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request) // Tipo de retorno IActionResult para control explícito de ContentResult
        {
            // Busca al usuario por su nombre de usuario en la base de datos.
            Debug.WriteLine($"[AuthController DEBUG] Intento de login para usuario: {request.Username}");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            // Registra los valores clave para depuración de credenciales.
            Debug.WriteLine($"[AuthController DEBUG] Usuario encontrado: {(user != null ? user.Username : "NULO")}");
            Debug.WriteLine($"[AuthController DEBUG] Contraseña recibida (PLANA): {request.Password}");
            Debug.WriteLine($"[AuthController DEBUG] Contraseña hasheada en DB: {(user != null ? user.Password : "NULO")}");

            // Verifica si el usuario existe y si la contraseña proporcionada coincide con la contraseña hasheada.
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                Debug.WriteLine($"[AuthController DEBUG] Login fallido para usuario: {request.Username}. Credenciales inválidas.");
                // Prepara el DTO de respuesta para credenciales inválidas.
                var errorDto = new LoginResponseDto
                {
                    Success = false,
                    Message = "Credenciales inválidas. Por favor, verifica tu usuario y contraseña."
                };
                // Serializa explícitamente el DTO de error a JSON y lo devuelve como 401 Unauthorized.
                string jsonErrorResponse = JsonConvert.SerializeObject(errorDto, Formatting.Indented);
                Debug.WriteLine($"[AuthController DEBUG] JSON de error enviado (401 Unauthorized): {jsonErrorResponse}");
                return Unauthorized(jsonErrorResponse);
            }

            // Si la autenticación del usuario es exitosa, busca el residente asociado a este usuario.
            // Se asume que la relación entre User y Residente es uno a uno, donde Residente.UserId
            // es una clave foránea que referencia a User.Id.
            var residente = await _context.Residentes.FirstOrDefaultAsync(r => r.UserId == user.Id);

            // Verifica si se encontró un residente asociado al usuario.
            if (residente == null)
            {
                Debug.WriteLine($"[AuthController DEBUG] Login exitoso para usuario {request.Username}, pero no se encontró residente asociado.");
                // Prepara el DTO de respuesta para usuario sin residente asociado.
                var errorDto = new LoginResponseDto
                {
                    Success = false,
                    Message = "Usuario autenticado, pero no asociado a un residente válido en el sistema."
                };
                // Serializa explícitamente el DTO de error a JSON y lo devuelve como 401 Unauthorized.
                string jsonErrorResponse = JsonConvert.SerializeObject(errorDto, Formatting.Indented);
                Debug.WriteLine($"[AuthController DEBUG] JSON de error enviado (401 Unauthorized - sin residente): {jsonErrorResponse}");
                return Unauthorized(jsonErrorResponse);
            }

            // Genera un token de autenticación único (usando un GUID como ejemplo simple).
            // En una aplicación real, esto podría ser un JWT (JSON Web Token).
            var authToken = Guid.NewGuid().ToString();
            Debug.WriteLine($"[AuthController DEBUG] Token generado para usuario {request.Username}: {authToken}");

            // Prepara el DTO de respuesta exitosa con todos los datos.
            var responseDto = new LoginResponseDto
            {
                Success = true,
                Message = "Inicio de sesión exitoso.",
                Token = authToken,
                ResidentId = residente.UserId // ID del residente asociado
            };

            // Serializa explícitamente el DTO de respuesta exitosa a JSON.
            string jsonResponse = JsonConvert.SerializeObject(responseDto, Formatting.Indented);
            Debug.WriteLine($"[AuthController DEBUG] JSON de éxito enviado (200 OK): {jsonResponse}"); // Log del JSON final

            // Retorna 200 OK con el DTO de respuesta serializado explícitamente como ContentResult.
            // Esto asegura que la cadena JSON generada por Newtonsoft.Json sea el cuerpo de la respuesta.
            return Content(jsonResponse, "application/json");
        }
    }
}
