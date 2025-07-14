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
    /// <summary>
    /// Modelo que representa un usuario del sistema.
    /// Contiene credenciales de autenticación.
    /// </summary>
    public class User
    {
        [Key] // Define Id como la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indica que el valor es auto-generado por la base de datos
        public int Id { get; set; }

        [Required] // Campo requerido
        public string Username { get; set; }

        [Required] // Campo requerido
        public string Password { get; set; }

        // Propiedad de navegación para la relación uno a uno con Residente (opcional)
        public Residente? Residente { get; set; }
    }

    /// <summary>
    /// Modelo que representa a un residente.
    /// Contiene información personal y de contacto del residente.
    /// </summary>
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

        // Clave foránea que también es la clave primaria y establece la relación uno a uno con User.
        [Key]
        [ForeignKey("User")] // Define UserId como clave foránea que referencia a la tabla User
        public int UserId { get; set; }
        public User? User { get; set; } // Propiedad de navegación a la entidad User
    }

    /// <summary>
    /// Modelo que representa a un invitado.
    /// Contiene datos del invitado y el token de acceso generado.
    /// </summary>
    public class Invitado
    {
        [Key] // Define Id como la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indica que el valor es auto-generado por la base de datos
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaVisita { get; set; }

        // El Token es opcional en la base de datos, pero siempre se genera en la API.
        public string? Token { get; set; }

        // Clave foránea para la relación con Residente (el residente que invita).
        [ForeignKey("Residente")] // Define ResidenteId como clave foránea que referencia a la tabla Residente
        public int ResidenteId { get; set; }
        public Residente? Residente { get; set; } // Propiedad de navegación a la entidad Residente
    }

    /// <summary>
    /// Modelo que registra los eventos de entrada y salida de personas (residentes o invitados).
    /// </summary>
    public class BitacoraRegistro
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaHoraEntrada { get; set; }
        public DateTime? FechaHoraSalida { get; set; } // Fecha de salida puede ser nula si la persona aún está dentro

        // Claves foráneas opcionales para asociar a un Residente o un Invitado.
        public int? ResidenteId { get; set; }
        public Residente? Residente { get; set; } // Propiedad de navegación a la entidad Residente

        public int? InvitadoId { get; set; }
        public Invitado? Invitado { get; set; } // Propiedad de navegación a la entidad Invitado
    }

    /// <summary>
    /// DTO para la solicitud de escaneo de QR.
    /// Representa los datos que el cliente envía a la API para validar un token QR.
    /// </summary>
    public class ScanRequestDto
    {
        public string QrToken { get; set; }
        public int GuardiaId { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta del escaneo de QR.
    /// Representa los datos que la API devuelve al cliente después de un escaneo de QR.
    /// </summary>
    public class ScanResponseDto
    {
        public string Message { get; set; }
        public string PersonName { get; set; } // Nombre completo de la persona que accedió
        public string PersonType { get; set; } // Tipo de persona ("Residente" o "Invitado")
        public string AccessType { get; set; } // Tipo de acceso ("Entrada" o "Salida")
        public bool IsSuccess { get; set; } // Indica si el escaneo y registro fueron exitosos
    }

    /// <summary>
    /// DTO (Data Transfer Object) utilizado para estructurar la respuesta JSON
    /// al cliente después de registrar o consultar un invitado.
    /// Contiene los datos esenciales que la aplicación móvil necesita,
    /// asegurando la consistencia en la comunicación API-cliente.
    /// </summary>
    public class InvitadoResponseDto
    {
        /// <summary>
        /// ID único del invitado registrado en la base de datos.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Token de acceso único generado para el invitado.
        /// Este token se utiliza para el registro de entrada/salida.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Indica si la operación de registro del invitado fue exitosa.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo sobre el resultado de la operación (éxito o error).
        /// </summary>
        public string Message { get; set; }
    }

    // <summary>
    /// DTO (Data Transfer Object) utilizado para estructurar la respuesta JSON
    /// al cliente después de un intento de inicio de sesión.
    /// Contiene el estado del login, un mensaje descriptivo, un token de autenticación
    /// y el ID del residente asociado.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Indica si la operación de inicio de sesión fue exitosa.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo sobre el resultado del inicio de sesión (éxito o error).
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Token de autenticación generado si el inicio de sesión fue exitoso.
        /// Este token se puede usar para futuras solicitudes autenticadas.
        /// Es nulo si el login falla.
        /// </summary>
        public string? Token { get; set; } // Propiedad nullable

        /// <summary>
        /// ID del residente asociado al usuario que inició sesión.
        /// Es nulo si el login falla o el usuario no está asociado a un residente.
        /// </summary>
        public int? ResidentId { get; set; } // Propiedad nullable
    }

    /// <summary>
    /// DTO (Data Transfer Object) utilizado para estructurar la solicitud JSON
    /// de inicio de sesión recibida del cliente.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Nombre de usuario para la autenticación.
        /// Campo requerido.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña para la autenticación.
        /// Campo requerido.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
