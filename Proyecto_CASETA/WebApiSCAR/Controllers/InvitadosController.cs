using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSCAR.Models;


/*
 * Es el Api Controller para los invitados.
 * Aquí se maneja los cruds para las peticiones http
 * Y OBVIAMENTE tenemos que respetar y aplicar lo asincrono.
 * NOTA: La función principal es gestionar los datos de los invitados
 * y, crucialmente, generar un token QR único para cada nuevo invitado al momento de su creación.
*/

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
            // Incluir el Residente asociado para poder mostrar su nombre completo en el cliente
            return await _context.Invitados.Include(i => i.Residente).ToListAsync();
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
            // Incluir el Residente asociado
            var invitado = await _context.Invitados.Include(i => i.Residente).FirstOrDefaultAsync(i => i.Id == id);

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

            // Si el Token no se envía en la actualización, mantener el existente.
            // Si se envía un token vacío o nulo, se mantiene el existente.
            var existingInvitado = await _context.Invitados.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if (existingInvitado == null)
            {
                return NotFound($"Invitado con ID {id} no encontrado.");
            }

            // Si el token en la petición es nulo o vacío, usar el token existente.
            // De lo contrario, usar el token proporcionado.
            if (string.IsNullOrWhiteSpace(invitado.Token))
            {
                invitado.Token = existingInvitado.Token;
            }

            _context.Entry(invitado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvitadoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Invitados
        /// <summary>
        /// Crea un nuevo invitado en el sistema y genera un token QR único para él.
        /// </summary>
        /// <param name="invitado">Objeto Invitado con los datos del nuevo invitado (sin Token).</param>
        /// <returns>El invitado creado con su ID y el Token QR generado.</returns>
        [HttpPost]
        public async Task<ActionResult<Invitado>> PostInvitado(Invitado invitado)
        {
            // Generar un token QR único para el nuevo invitado
            invitado.Token = Guid.NewGuid().ToString(); // Genera un GUID como token

            // Revisamos si el token ya existe
            if (await _context.Invitados.AnyAsync(i => i.Token == invitado.Token))
            {
                // Si por alguna razón el GUID generado ya existe, generar uno nuevo o manejar el error.
                invitado.Token = Guid.NewGuid().ToString();
            }

            _context.Invitados.Add(invitado);
            await _context.SaveChangesAsync();

            // Devolver el invitado creado, que ahora incluye el Token generado.
            return CreatedAtAction("GetInvitado", new { id = invitado.Id }, invitado);
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
                return NotFound();
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
