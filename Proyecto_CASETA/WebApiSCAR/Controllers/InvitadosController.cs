using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSCAR.Models;
using System.Diagnostics; // Necesario para Debug.WriteLine (logs de depuración)

namespace WebApiSCAR.Controllers
{
    [Route("api/[controller]")] // Define la ruta base para este controlador
    [ApiController] // Indica que esta clase es un controlador de API con comportamientos específicos de API
    public class InvitadosController : ControllerBase
    {
        private readonly SCARContext _context; // Contexto de la base de datos para interactuar con EF Core

        /// <summary>
        /// Constructor del controlador. Inyecta el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de SCARContext proporcionada por la inyección de dependencias.</param>
        public InvitadosController(SCARContext context)
        {
            _context = context;
        }

        // GET: api/Invitados
        /// <summary>
        /// Obtiene una lista de todos los invitados registrados en el sistema.
        /// Incluye la información del residente asociado para cada invitado.
        /// </summary>
        /// <returns>Una acción de resultado que contiene una colección de objetos Invitado.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invitado>>> GetInvitados()
        {
            // Retorna todos los invitados, incluyendo su Residente asociado para una consulta completa.
            return await _context.Invitados.Include(i => i.Residente).ToListAsync();
        }

        // GET: api/Invitados/5
        /// <summary>
        /// Obtiene un invitado específico por su ID.
        /// </summary>
        /// <param name="id">El ID del invitado a buscar.</param>
        /// <returns>Una acción de resultado que contiene el objeto Invitado si se encuentra,
        /// o un resultado NotFound si el invitado no existe.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Invitado>> GetInvitado(int id)
        {
            // Busca el invitado por ID, incluyendo su Residente asociado.
            var invitado = await _context.Invitados.Include(i => i.Residente).FirstOrDefaultAsync(i => i.Id == id);

            if (invitado == null)
            {
                return NotFound(); // Retorna 404 si el invitado no se encuentra
            }

            return invitado; // Retorna el invitado encontrado
        }

        // PUT: api/Invitados/5
        /// <summary>
        /// Actualiza un invitado existente en el sistema.
        /// </summary>
        /// <param name="id">El ID del invitado a actualizar.</param>
        /// <param name="invitado">El objeto Invitado con los datos actualizados.</param>
        /// <returns>Un resultado NoContent si la actualización fue exitosa,
        /// BadRequest si el ID en la ruta no coincide con el ID del objeto,
        /// o NotFound si el invitado no existe en la base de datos.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvitado(int id, Invitado invitado)
        {
            if (id != invitado.Id)
            {
                return BadRequest(); // Retorna 400 si los IDs no coinciden
            }

            _context.Entry(invitado).State = EntityState.Modified; // Marca la entidad como modificada

            try
            {
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            }
            catch (DbUpdateConcurrencyException) // Manejo de errores de concurrencia
            {
                if (!InvitadoExists(id))
                {
                    return NotFound(); // Retorna 404 si el invitado fue eliminado por otro proceso
                }
                else
                {
                    throw; // Relanza la excepción si es otro tipo de error de concurrencia
                }
            }
            return NoContent(); // Retorna 204 si la actualización fue exitosa
        }

        // POST: api/Invitados
        /// <summary>
        /// Crea un nuevo invitado en el sistema y genera un Token QR único para él.
        /// </summary>
        /// <param name="invitado">El objeto Invitado con los datos del nuevo invitado recibido del cliente.</param>
        /// <returns>Un objeto InvitadoResponseDto con los datos del invitado creado, su token y el estado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult<InvitadoResponseDto>> PostInvitado(Invitado invitado)
        {
            // Genera un Token QR único si no se ha proporcionado uno (debería ser nulo al crear un nuevo invitado).
            if (string.IsNullOrEmpty(invitado.Token))
            {
                invitado.Token = Guid.NewGuid().ToString();
            }

            // --- Inicio de Depuración para ResidenteId ---
            // Registra el ID del residente recibido en la solicitud para depuración.
            Debug.WriteLine($"[InvitadosController DEBUG] ResidenteId recibido en la solicitud: {invitado.ResidenteId}");

            // Verifica si el ResidenteId proporcionado existe en la base de datos.
            // Esto es crucial para la validación de la relación de clave foránea.
            var residenteExiste = await _context.Residentes.AnyAsync(r => r.UserId == invitado.ResidenteId);
            Debug.WriteLine($"[InvitadosController DEBUG] ¿Residente con UserId {invitado.ResidenteId} existe en DB?: {residenteExiste}");
            // --- Fin de Depuración ---

            // Valida el ResidenteId: debe ser mayor que 0 y existir en la tabla Residentes.
            if (invitado.ResidenteId <= 0 || !residenteExiste)
            {
                // Si la validación falla, devuelve un BadRequest con un DTO de error.
                var errorDto = new InvitadoResponseDto
                {
                    Id = 0, // ID 0 para indicar que no se creó ningún invitado válido
                    Token = null, // Token nulo en caso de error de validación
                    Success = false, // Indica que la operación no fue exitosa
                    Message = "El ID del residente que invita no es válido o no existe. Por favor, verifica el ID del residente e intenta de nuevo."
                };
                return BadRequest(errorDto); // Devuelve 400 Bad Request con el DTO de error
            }

            // Añade el nuevo invitado al contexto de la base de datos.
            _context.Invitados.Add(invitado);
            // Guarda los cambios en la base de datos.
            // Después de esta llamada, el 'invitado.Id' debería ser actualizado con el valor auto-generado por la DB.
            await _context.SaveChangesAsync();

            // --- Inicio de Depuración ---
            // Registra el ID del invitado y el token después de guardar en la DB para verificar su actualización.
            Debug.WriteLine($"[InvitadosController DEBUG] Invitado.Id después de SaveChangesAsync: {invitado.Id}");
            Debug.WriteLine($"[InvitadosController DEBUG] Invitado.Token después de SaveChangesAsync: {invitado.Token}");
            // --- Fin de Depuración ---

            // Recarga el objeto Invitado desde la base de datos para garantizar que todos los campos,
            // especialmente el ID auto-generado y el Token estén completamente actualizados
            // en el objeto en memoria antes de construir la respuesta.
            var invitadoActualizado = await _context.Invitados.FindAsync(invitado.Id);
            if (invitadoActualizado == null)
            {
                // Este caso es poco probable si SaveChangesAsync fue exitoso, pero es una buena medida de seguridad.
                var notFoundDto = new InvitadoResponseDto
                {
                    Success = false,
                    Message = "Error interno: Invitado no encontrado después de guardar. Contacte a soporte técnico."
                };
                return NotFound(notFoundDto); // Devuelve 404 Not Found con el DTO de error
            }

            // Mapea el objeto 'invitadoActualizado' (con los datos más recientes de la DB)
            // a un InvitadoResponseDto para la respuesta al cliente.
            var responseDto = new InvitadoResponseDto
            {
                Id = invitadoActualizado.Id, // Asegura que el ID sea el generado por la DB
                Token = invitadoActualizado.Token, // Asegura que el Token sea el final (generado o actualizado)
                Success = true, // Indica que la operación fue exitosa
                Message = "Invitado registrado y token generado con éxito."
            };

            // Retorna 200 OK con el DTO de respuesta.
            // ASP.NET Core se encargará de serializar automáticamente 'responseDto' a JSON
            // utilizando la configuración de serialización definida en Program.cs.
            return Ok(responseDto);
        }

        // DELETE: api/Invitados/5
        /// <summary>
        /// Elimina un invitado del sistema por su ID.
        /// </summary>
        /// <param name="id">El ID del invitado a eliminar.</param>
        /// <returns>Un resultado NoContent si la eliminación fue exitosa,
        /// o NotFound si el invitado no existe.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvitado(int id)
        {
            var invitado = await _context.Invitados.FindAsync(id);
            if (invitado == null)
            {
                return NotFound(); // Retorna 404 si el invitado no se encuentra
            }

            _context.Invitados.Remove(invitado); // Marca la entidad para eliminación
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

            return NoContent(); // Retorna 204 si la eliminación fue exitosa
        }

        /// <summary>
        /// Método auxiliar para verificar si un invitado con un ID específico existe.
        /// </summary>
        /// <param name="id">El ID del invitado a verificar.</param>
        /// <returns>True si el invitado existe, False en caso contrario.</returns>
        private bool InvitadoExists(int id)
        {
            return _context.Invitados.Any(e => e.Id == id);
        }
    }
}
