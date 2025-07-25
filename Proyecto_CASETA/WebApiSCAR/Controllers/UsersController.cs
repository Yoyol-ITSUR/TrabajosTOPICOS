﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSCAR.Models;
using BCrypt.Net; // Necesario para el hashing de contraseñas

/*
 * Es el Api Controller para el usuario.
 * Aquí se maneja los cruds para las peticiones http
 * Y OBVIAMENTE tenemos que respetar y aplicar lo asincrono
*/

namespace WebApiSCAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SCARContext _context;

        public UsersController(SCARContext context)
        {
            _context = context;
        }

        // GET: api/Users
        /// <summary>
        /// Obtiene una lista de todos los usuarios registrados en el sistema.
        /// Las contraseñas se excluyen de la respuesta por seguridad.
        /// </summary>
        /// <returns>Una lista de objetos User (sin contraseñas).</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            // Se proyecta la selección para no exponer la contraseña (hash) en la respuesta de la API.
            return await _context.Users
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    // La propiedad Password no se incluye intencionalmente aquí por seguridad (creo).
                })
                .ToListAsync();
        }

        // GET: api/Users/5
        /// <summary>
        /// Obtiene un usuario específico por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario a buscar.</param>
        /// <returns>El objeto User correspondiente al ID o NotFound si no existe.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Por seguridad, se devuelve un objeto User sin la contraseña.
            return new User { Id = user.Id, Username = user.Username };
        }

        // POST: api/Users
        /// <summary>
        /// Crea un nuevo usuario en el sistema.
        /// La contraseña proporcionada se hashea antes de ser almacenada en la base de datos.
        /// </summary>
        /// <param name="user">Objeto User con los datos del nuevo usuario (Username y Password).</param>
        /// <returns>El usuario creado con su ID.</returns>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // Validar si el nombre de usuario ya existe para evitar duplicados.
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return Conflict("El nombre de usuario ya existe.");
            }

            // Hashear la contraseña antes de guardarla en la base de datos por seguridad.
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Devolver el usuario creado, pero sin la contraseña hasheada por seguridad.
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new User { Id = user.Id, Username = user.Username });
        }

        // PUT: api/Users/5
        /// <summary>
        /// Actualiza la información de un usuario existente.
        /// Requiere que la contraseña sea proporcionada y se hasheará nuevamente.
        /// </summary>
        /// <param name="id">El ID del usuario a actualizar.</param>
        /// <param name="user">Objeto User con la información actualizada.</param>
        /// <returns>NoContent si la actualización fue exitosa, BadRequest o NotFound en caso contrario.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del usuario proporcionado.");
            }

            // Aquí, ModelState.IsValid se ejecutará y validará que la contraseña no esté vacía.
            // Si ModelState.IsValid es false, se devolverá BadRequest automáticamente.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Devuelve los errores de validación, incluyendo el de Password.
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado.");
            }

            // Actualizar Username
            if (existingUser.Username != user.Username)
            {
                if (await _context.Users.AnyAsync(u => u.Username == user.Username && u.Id != id))
                {
                    return Conflict("El nuevo nombre de usuario ya está en uso.");
                }
                existingUser.Username = user.Username;
            }

            // Actualizar Password (siempre se hashea si se envía, ya que ahora es requerido)
            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Entry(existingUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound($"Usuario con ID {id} no encontrado después de intentar actualizar.");
                }
                else
                {
                    throw; // Re-lanzar la excepción si es otro tipo de error de concurrencia.
                }
            }

            return NoContent(); // 204 No Content indica que la operación fue exitosa pero no hay contenido para devolver.
        }

        // DELETE: api/Users/5
        /// <summary>
        /// Elimina un usuario del sistema por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario a eliminar.</param>
        /// <returns>NoContent si la eliminación fue exitosa, NotFound si el usuario no existe.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado para eliminar.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica si un usuario con el ID especificado existe en la base de datos.
        /// </summary>
        /// <param name="id">El ID del usuario a verificar.</param>
        /// <returns>True si el usuario existe, False en caso contrario.</returns>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
