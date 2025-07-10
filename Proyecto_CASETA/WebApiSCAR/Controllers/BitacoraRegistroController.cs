using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSCAR.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        // DTO para la solicitud de escaneo de QR
        /// <summary>
        /// Objeto de transferencia de datos (DTO) para la solicitud de escaneo de un código QR.
        /// Contiene el token leído del QR y opcionalmente el ID del guardia que realiza el registro.
        /// </summary>
        public class ScanRequestDto
        {
            /// <summary>
            /// El token de texto leído del código QR.
            /// </summary>
            public string QrToken { get; set; }

            /// <summary>
            /// El ID del guardia que realiza el registro. Es opcional y puede ser nulo.
            /// </summary>
            public int? GuardiaId { get; set; } // Asumiendo que los guardias podrían tener un ID de usuario o ser un ID específico.
        }

        // POST: api/BitacoraRegistro/Scan
        /// <summary>
        /// Procesa el escaneo de un código QR para registrar la entrada o salida de un invitado.
        /// La lógica determina si es una entrada (si el invitado no está dentro) o una salida (si ya está dentro).
        /// </summary>
        /// <param name="request">Objeto ScanRequestDto que contiene el token QR y el ID del guardia.</param>
        /// <returns>Un mensaje de éxito o error indicando el resultado del registro.</returns>
        [HttpPost("Scan")]
        public async Task<IActionResult> ScanQr([FromBody] ScanRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.QrToken))
            {
                return BadRequest("El token QR no puede estar vacío.");
            }

            // 1. Buscar el Invitado asociado al token QR.
            var invitado = await _context.Invitados
                                         .FirstOrDefaultAsync(i => i.Token == request.QrToken);

            if (invitado == null)
            {
                return NotFound("Token QR no válido o invitado no encontrado.");
            }

            // 2. Validar la fecha de visita del invitado.
            // Si FechaVisita es la fecha esperada de la visita y quieres restringir el acceso a ese día.
            // Por simplicidad, asumimos que FechaVisita es solo una fecha de referencia de registro del invitado.
            // Si se necesitara una validación estricta por fecha, se añadiría aquí:
            /*
            if (invitado.FechaVisita.Date != DateTime.Today.Date)
            {
                return BadRequest($"Acceso denegado: Este QR es para una visita en la fecha {invitado.FechaVisita.ToShortDateString()}.");
            }
            */

            // 3. Verificar si el invitado está actualmente "dentro" del fraccionamiento.
            // Se busca el registro más reciente para este invitado que no tiene FechaHoraSalida.
            var registroActivo = await _context.BitacorasRegistro
                                               .Where(br => br.InvitadoId == invitado.Id && br.FechaHoraSalida == null)
                                               .OrderByDescending(br => br.FechaHoraEntrada)
                                               .FirstOrDefaultAsync();

            if (registroActivo != null)
            {
                // El invitado está "dentro" -> Registrar salida.
                registroActivo.FechaHoraSalida = DateTime.Now;
                _context.Entry(registroActivo).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok($"Salida registrada para el invitado: {invitado.Nombre} {invitado.Apellido}.");
            }
            else
            {
                // El invitado no está "dentro" -> Registrar entrada.
                var nuevoRegistro = new BitacoraRegistro
                {
                    FechaHoraEntrada = DateTime.Now,
                    InvitadoId = invitado.Id,
                    ResidenteId = null, // Es un invitado, no un residente
                    // GuardiaId = request.GuardiaId // Si BitacoraRegistro tuviera un campo GuardiaId
                };
                _context.BitacorasRegistro.Add(nuevoRegistro);
                await _context.SaveChangesAsync();
                return Ok($"Entrada registrada para el invitado: {invitado.Nombre} {invitado.Apellido}.");
            }
        }

        // GET: api/BitacoraRegistro
        /// <summary>
        /// Obtiene todos los registros de bitácora de acceso (entradas y salidas).
        /// Incluye la información del residente o invitado asociado.
        /// </summary>
        /// <returns>Una lista de registros de bitácora.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BitacoraRegistro>>> GetBitacoraRegistros()
        {
            // Incluir Residentes e Invitados para tener la información completa en la respuesta.
            return await _context.BitacorasRegistro
                                 .Include(br => br.Residente)
                                     .ThenInclude(r => r.User) // Incluir el User asociado al Residente
                                 .Include(br => br.Invitado)
                                 .ToListAsync();
        }

        // GET: api/BitacoraRegistro/ByDate?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
        /// <summary>
        /// Obtiene registros de bitácora filtrados por un rango de fechas de entrada.
        /// </summary>
        /// <param name="startDate">Fecha de inicio del rango (formato YYYY-MM-DD).</param>
        /// <param name="endDate">Fecha de fin del rango (formato YYYY-MM-DD).</param>
        /// <returns>Una lista de registros de bitácora dentro del rango de fechas.</returns>
        [HttpGet("ByDate")]
        public async Task<ActionResult<IEnumerable<BitacoraRegistro>>> GetBitacoraRegistrosByDate(DateTime startDate, DateTime endDate)
        {
            // Asegurarse de que el rango de fechas incluya el día completo.
            // endDate.AddDays(1) para incluir hasta el final del día.
            return await _context.BitacorasRegistro
                                 .Where(br => br.FechaHoraEntrada.Date >= startDate.Date && br.FechaHoraEntrada.Date <= endDate.Date)
                                 .Include(br => br.Residente)
                                     .ThenInclude(r => r.User)
                                 .Include(br => br.Invitado)
                                 .ToListAsync();
        }

        // GET: api/BitacoraRegistro/ByResidente/5
        /// <summary>
        /// Obtiene registros de bitácora asociados a un residente específico.
        /// </summary>
        /// <param name="residenteId">El ID del residente.</param>
        /// <returns>Una lista de registros de bitácora para el residente especificado.</returns>
        [HttpGet("ByResidente/{residenteId}")]
        public async Task<ActionResult<IEnumerable<BitacoraRegistro>>> GetBitacoraRegistrosByResidente(int residenteId)
        {
            var residenteExists = await _context.Residentes.AnyAsync(r => r.UserId == residenteId);
            if (!residenteExists)
            {
                return NotFound($"Residente con ID {residenteId} no encontrado.");
            }

            return await _context.BitacorasRegistro
                                 .Where(br => br.ResidenteId == residenteId)
                                 .Include(br => br.Residente)
                                     .ThenInclude(r => r.User)
                                 .Include(br => br.Invitado) // Puede haber registros de invitados también si la lógica lo permite
                                 .ToListAsync();
        }

        // GET: api/BitacoraRegistro/ByInvitado/5
        /// <summary>
        /// Obtiene registros de bitácora asociados a un invitado específico.
        /// </summary>
        /// <param name="invitadoId">El ID del invitado.</param>
        /// <returns>Una lista de registros de bitácora para el invitado especificado.</returns>
        [HttpGet("ByInvitado/{invitadoId}")]
        public async Task<ActionResult<IEnumerable<BitacoraRegistro>>> GetBitacoraRegistrosByInvitado(int invitadoId)
        {
            var invitadoExists = await _context.Invitados.AnyAsync(i => i.Id == invitadoId);
            if (!invitadoExists)
            {
                return NotFound($"Invitado con ID {invitadoId} no encontrado.");
            }

            return await _context.BitacorasRegistro
                                 .Where(br => br.InvitadoId == invitadoId)
                                 .Include(br => br.Invitado)
                                 .Include(br => br.Residente) // Puede haber registros de residentes también si la lógica lo permite
                                     .ThenInclude(r => r.User)
                                 .ToListAsync();
        }

        // GET: api/BitacoraRegistro/CurrentStatus
        /// <summary>
        /// Obtiene una lista de todos los invitados y residentes que se encuentran actualmente dentro del fraccionamiento.
        /// </summary>
        /// <returns>Una lista de registros de bitácora con FechaHoraSalida nula.</returns>
        [HttpGet("CurrentStatus")]
        public async Task<ActionResult<IEnumerable<BitacoraRegistro>>> GetCurrentStatus()
        {
            return await _context.BitacorasRegistro
                                 .Where(br => br.FechaHoraSalida == null) // Registros donde la salida aún no ha sido registrada
                                 .Include(br => br.Residente)
                                     .ThenInclude(r => r.User)
                                 .Include(br => br.Invitado)
                                 .ToListAsync();
        }
    }
}
