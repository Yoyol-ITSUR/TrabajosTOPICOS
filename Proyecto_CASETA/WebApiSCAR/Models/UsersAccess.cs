using System; // Necesario para DateTime
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*
Este archivo define las clases para el modelo de datos.
Concuerda con la mayoria de tablas de la base de datos.
También incluye los Data Transfer Objects (DTOs) ScanRequestDto y ScanResponseDto
que se utilizan para la comunicación estructurada entre la API y la aplicación cliente.
*/

namespace WebApiSCAR.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        public Residente? Residente { get; set; }
    }

    public class Residente
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string Telefono { get; set; }
        [Required]
        public string Direccion { get; set; }

        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Relación con User
        [Key, ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }

    public class Invitado
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaVisita { get; set; }

        public string? Token { get; set; }

        // Relación con Residente
        [ForeignKey("Residente")]
        public int ResidenteId { get; set; }
        public Residente? Residente { get; set; }
    }

    public class BitacoraRegistro
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaHoraEntrada { get; set; }
        public DateTime? FechaHoraSalida { get; set; }

        // Puede ser residente o invitado
        public int? ResidenteId { get; set; }
        public Residente? Residente { get; set; }

        public int? InvitadoId { get; set; }
        public Invitado? Invitado { get; set; }
    }

    // DTO para la solicitud de escaneo de QR (lo que el cliente envía a la API)
    public class ScanRequestDto
    {
        public string QrToken { get; set; }
        public int GuardiaId { get; set; }
    }

    // DTO para la respuesta del escaneo de QR (lo que el cliente espera de la API)
    // Este DTO debe de coincidir con el que la API devuelve.
    public class ScanResponseDto
    {
        public string Message { get; set; }
        public string PersonName { get; set; } // Nombre completo de la persona
        public string PersonType { get; set; } // "Residente" o "Invitado"
        public string AccessType { get; set; } // "Entrada" o "Salida"
        public bool IsSuccess { get; set; } // Para indicar si la operación fue exitosa
    }
}
