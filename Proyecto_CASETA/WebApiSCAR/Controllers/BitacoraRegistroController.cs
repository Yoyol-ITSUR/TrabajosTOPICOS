using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSCAR.Models;

/*
 * Es el controlador API que gestiona los registros de entrada y salida de invitados.
 * Contiene el endpoint Scan que valida un token QR y registra el acceso (entrada o salida), 
 * devolviendo una respuesta estructurada al cliente. También filtra y consulta el historial de accesos.
*/

namespace WebApiSCAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BitacoraRegistroController : ControllerBase
    {
        private readonly SCARContext _context;

        public BitacoraRegistroController(SCARContext context)
        {
            _context = context;
        }

        // GET: api/BitacoraRegistro
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BitacoraRegistro>>> GetBitacorasRegistro()
        {
            return await _context.BitacorasRegistro
                                 .Include(b => b.Residente)
                                 .Include(b => b.Invitado)
                                 .ToListAsync();
        }

        // GET: api/BitacoraRegistro/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BitacoraRegistro>> GetBitacoraRegistro(int id)
        {
            var bitacoraRegistro = await _context.BitacorasRegistro
                                                 .Include(b => b.Residente)
                                                 .Include(b => b.Invitado)
                                                 .FirstOrDefaultAsync(b => b.Id == id);

            if (bitacoraRegistro == null)
            {
                return NotFound();
            }

            return bitacoraRegistro;
        }

        // GET: api/BitacoraRegistro/ByDate?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
        [HttpGet("ByDate")]
        public async Task<ActionResult<IEnumerable<BitacoraRegistro>>> GetBitacorasByDate(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.Date.AddDays(1).AddTicks(-1);

            return await _context.BitacorasRegistro
                                 .Include(b => b.Residente)
                                 .Include(b => b.Invitado)
                                 .Where(b => b.FechaHoraEntrada >= startDate && b.FechaHoraEntrada <= endDate)
                                 .ToListAsync();
        }

        // GET: api/BitacoraRegistro/CurrentStatus
        [HttpGet("CurrentStatus")]
        public async Task<ActionResult<IEnumerable<BitacoraRegistro>>> GetCurrentStatus()
        {
            return await _context.BitacorasRegistro
                                 .Include(b => b.Residente)
                                 .Include(b => b.Invitado)
                                 .Where(b => b.FechaHoraSalida == null)
                                 .ToListAsync();
        }

        // POST: api/BitacoraRegistro/Scan
        [HttpPost("Scan")]
        public async Task<ActionResult<ScanResponseDto>> Scan([FromBody] ScanRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.QrToken))
            {
                return BadRequest(new ScanResponseDto { Message = "El token QR no puede estar vacío.", IsSuccess = false });
            }

            // Buscar el token en la tabla Invitados
            var invitado = await _context.Invitados.Include(i => i.Residente).FirstOrDefaultAsync(i => i.Token == request.QrToken);

            if (invitado != null)
            {
                // Encontró un invitado con el token
                var registroExistente = await _context.BitacorasRegistro
                                                      .Where(b => b.InvitadoId == invitado.Id && b.FechaHoraSalida == null)
                                                      .FirstOrDefaultAsync();

                if (registroExistente == null)
                {
                    // Registrar una nueva entrada
                    var nuevoRegistro = new BitacoraRegistro
                    {
                        FechaHoraEntrada = DateTime.Now,
                        FechaHoraSalida = null,
                        InvitadoId = invitado.Id,
                        ResidenteId = null
                    };
                    _context.BitacorasRegistro.Add(nuevoRegistro);
                    await _context.SaveChangesAsync();

                    return Ok(new ScanResponseDto
                    {
                        Message = $"Acceso concedido para Invitado: {invitado.Nombre} {invitado.Apellido}. Registro de entrada exitoso.",
                        PersonName = $"{invitado.Nombre} {invitado.Apellido}",
                        PersonType = "Invitado",
                        AccessType = "Entrada",
                        IsSuccess = true
                    });
                }
                else
                {
                    // Registrar la salida
                    registroExistente.FechaHoraSalida = DateTime.Now;
                    _context.Entry(registroExistente).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return Ok(new ScanResponseDto
                    {
                        Message = $"Salida registrada para Invitado: {invitado.Nombre} {invitado.Apellido}. Registro de salida exitoso.",
                        PersonName = $"{invitado.Nombre} {invitado.Apellido}",
                        PersonType = "Invitado",
                        AccessType = "Salida",
                        IsSuccess = true
                    });
                }
            }
            else
            {
                // Si el token no se encontró en Invitados, aquí podrías añadir lógica para Residentes
                // si ellos también usaran tokens QR para este sistema de acceso.
                // Por ahora, solo maneja invitados.

                return BadRequest(new ScanResponseDto { Message = "Token no válido o no encontrado para ningún invitado.", IsSuccess = false });
            }
        }

        // POST: api/BitacoraRegistro
        [HttpPost]
        public async Task<ActionResult<BitacoraRegistro>> PostBitacoraRegistro(BitacoraRegistro bitacoraRegistro)
        {
            _context.BitacorasRegistro.Add(bitacoraRegistro);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBitacoraRegistro", new { id = bitacoraRegistro.Id }, bitacoraRegistro);
        }

        // PUT: api/BitacoraRegistro/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBitacoraRegistro(int id, BitacoraRegistro bitacoraRegistro)
        {
            if (id != bitacoraRegistro.Id)
            {
                return BadRequest();
            }

            _context.Entry(bitacoraRegistro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BitacoraRegistroExists(id))
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

        // DELETE: api/BitacoraRegistro/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBitacoraRegistro(int id)
        {
            var bitacoraRegistro = await _context.BitacorasRegistro.FindAsync(id);
            if (bitacoraRegistro == null)
            {
                return NotFound();
            }

            _context.BitacorasRegistro.Remove(bitacoraRegistro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BitacoraRegistroExists(int id)
        {
            return _context.BitacorasRegistro.Any(e => e.Id == id);
        }
    }
}
