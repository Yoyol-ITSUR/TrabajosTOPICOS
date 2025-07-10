using System;
using System.Collections.Generic;

namespace App_Caseta.Servicios
{
    // DTO para la entidad User (usuario del sistema, ej. guardia)
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Solo para enviar (registro/actualización), no se espera recibir el hash.
    }

    // DTO para la entidad Residente
    public class ResidenteDto
    {
        public int UserId { get; set; } // Clave primaria y foránea a User
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Propiedad de navegación para incluir el User asociado si la API lo devuelve.
        // Asegúrate de que tu API incluya esta relación si quieres usarla.
        public UserDto User { get; set; }
    }

    // DTO para la entidad Invitado
    public class InvitadoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaVisita { get; set; }
        public string Token { get; set; } // El token generado para el QR
        public int ResidenteId { get; set; } // Clave foránea al residente que invita

        // Propiedad de navegación para incluir el Residente asociado si la API lo devuelve.
        public ResidenteDto Residente { get; set; }
    }

    // DTO para la entidad BitacoraRegistro
    public class BitacoraRegistroDto
    {
        public int Id { get; set; }
        public DateTime FechaHoraEntrada { get; set; }
        public DateTime? FechaHoraSalida { get; set; } // Nullable, para salidas pendientes

        // Las siguientes propiedades son para mostrar la información del residente o invitado
        // en la bitácora, si la API las incluye en la respuesta.
        public int? ResidenteId { get; set; }
        public ResidenteDto Residente { get; set; }

        public int? InvitadoId { get; set; }
        public InvitadoDto Invitado { get; set; }
    }

    // DTO para la solicitud de escaneo de QR (enviado a la API)
    public class ScanRequestDto
    {
        public string QrToken { get; set; }
        public int? GuardiaId { get; set; } // Opcional, para identificar al guardia que escanea
    }
}
