using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSCAR.Models;

namespace WebApiSCAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitadosController : ControllerBase
    {
        private readonly SCARContext _context;

        public InvitadosController(SCARContext context)
        {
            _context = context;
        }

        // GET: api/Invitados
        /// <summary>
        /// Obtiene una lista de todos los invitados registrados en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos Invitado.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invitado>>> GetInvitados()
        {
            // Incluir la información del Residente que invitó, si es necesario.
            // .Include(i => i.Residente)
            return await _context.Invitados.ToListAsync();
        }

        // GET: api/Invitados/5
        /// <summary>
        /// Obtiene un invitado específico por su ID.
        /// </summary>
        /// <param name="id">El ID del invitado a buscar.</param>
        /// <returns>El objeto Invitado correspondiente al ID o NotFound si no existe.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Invitado>> GetInvitado(int id)
        {
            var invitado = await _context.Invitados.FindAsync(id);

            if (invitado == null)
            {
                return NotFound();
            }

            return invitado;
        }

        // PUT: api/Invitados/5
        /// <summary>
        /// Actualiza la información de un invitado existente.
        /// </summary>
        /// <param name="id">El ID del invitado a actualizar.</param>
        /// <param name="invitado">Objeto Invitado con la información actualizada.</param>
        /// <returns>NoContent si la actualización fue exitosa, BadRequest o NotFound en caso contrario.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvitado(int id, Invitado invitado)
        {
            if (id != invitado.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del invitado proporcionado.");
            }

            // No se debe permitir cambiar el Token desde un PUT directo, ya que se genera automáticamente.
            // Si el Token se cambia manualmente aquí, podría generar problemas de seguridad o duplicados.
            // Se puede cargar el existente y actualizar otras propiedades.
            var existingInvitado = await _context.Invitados.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if (existingInvitado == null)
            {
                return NotFound($"Invitado con ID {id} no encontrado.");
            }

            // Mantener el token existente si no se desea cambiarlo.
            // Si se desea un nuevo token, se debería usar un método específico o generar uno nuevo aquí.
            // Para este ejemplo, asumimos que el Token se mantiene igual después de la creación.
            invitado.Token = existingInvitado.Token;

            _context.Entry(invitado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvitadoExists(id))
                {
                    return NotFound($"Invitado con ID {id} no encontrado después de intentar actualizar.");
                }
                else
                {
                    throw; // Re-lanzar la excepción si es otro tipo de error de concurrencia.
                }
            }

            return NoContent();
        }

        // POST: api/Invitados
        /// <summary>
        /// Crea un nuevo invitado y genera un token QR único para su acceso.
        /// </summary>
        /// <param name="invitado">Objeto Invitado con los datos del nuevo invitado.</param>
        /// <returns>El invitado creado con su ID y el token QR generado.</returns>
        [HttpPost]
        public async Task<ActionResult<Invitado>> PostInvitado(Invitado invitado)
        {
            // Generar un token único para el invitado.
            // GUID (Globally Unique Identifier) es ideal para esto, asegurando alta probabilidad de unicidad.
            invitado.Token = Guid.NewGuid().ToString();

            _context.Invitados.Add(invitado);
            await _context.SaveChangesAsync();

            // Devolver el invitado creado, incluyendo el token generado.
            return CreatedAtAction(nameof(GetInvitado), new { id = invitado.Id }, invitado);
        }

        // DELETE: api/Invitados/5
        /// <summary>
        /// Elimina un invitado del sistema por su ID.
        /// </summary>
        /// <param name="id">El ID del invitado a eliminar.</param>
        /// <returns>NoContent si la eliminación fue exitosa, NotFound si el invitado no existe.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvitado(int id)
        {
            var invitado = await _context.Invitados.FindAsync(id);
            if (invitado == null)
            {
                return NotFound($"Invitado con ID {id} no encontrado para eliminar.");
            }

            _context.Invitados.Remove(invitado);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica si un invitado con el ID especificado existe en la base de datos.
        /// </summary>
        /// <param name="id">El ID del invitado a verificar.</param>
        /// <returns>True si el invitado existe, False en caso contrario.</returns>
        private bool InvitadoExists(int id)
        {
            return _context.Invitados.Any(e => e.Id == id);
        }
    }
}
