﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSCAR.Models;

/*
 * Es el Api Controller para los residentes.
 * Aquí se maneja los cruds para las peticiones http
 * Y OBVIAMENTE tenemos que respetar y aplicar lo asincrono
 * NOTA: Implementa la lógica para gestionar los datos de los residentes, SIEMPRE
 * se crean usuarios nuevos para cada residente creado. Pero se pueden crear 
 * usuarios sin residentes. Desde usuarios nada más.
*/

// Los comentarios en los metodos son los ejemplos de la url
// para las peticiones.
namespace WebApiSCAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentesController : ControllerBase
    {
        private readonly SCARContext _context;

        public ResidentesController(SCARContext context)
        {
            _context = context;
        }

        // A estos ejemplos me refiero
        // GET: api/Residentes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Residente>>> GetResidentes()
        {
            return await _context.Residentes.ToListAsync();
        }

        // GET: api/Residentes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Residente>> GetResidente(int id)
        {
            var residente = await _context.Residentes.FindAsync(id);

            if (residente == null)
            {
                return NotFound();
            }

            return residente;
        }

        // PUT: api/Residentes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResidente(int id, Residente residente)
        {
            if (id != residente.UserId)
            {
                return BadRequest();
            }

            _context.Entry(residente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResidenteExists(id))
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

        // POST: api/Residentes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Residente>> PostResidente(Residente residente)
        {
            _context.Residentes.Add(residente);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ResidenteExists(residente.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetResidente", new { id = residente.UserId }, residente);
        }

        // DELETE: api/Residentes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResidente(int id)
        {
            var residente = await _context.Residentes.FindAsync(id);
            if (residente == null)
            {
                return NotFound();
            }

            _context.Residentes.Remove(residente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResidenteExists(int id)
        {
            return _context.Residentes.Any(e => e.UserId == id);
        }
    }
}
