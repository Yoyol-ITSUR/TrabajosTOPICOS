using System;
using System.Collections.Generic;

/*
 * Define los Data Transfer Objects (DTOs) que representan las estructuras de datos 
 * que se intercambian entre la aplicación de escritorio y la Web API. 
 * Incluye DTOs para User, Residente, Invitado y BitacoraRegistro, 
 * asegurando que los datos se envíen y reciban en un formato compatible.
*/

namespace App_Caseta.Servicios
{
    // DTO para la entidad User
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Solo para envío (creación/actualización), no se espera en GET
    }

    // DTO para la entidad Residente
    public class ResidenteDto
    {
        public int UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Propiedad calculada para mostrar en ComboBox y DataGridView
        public string NombreCompleto => $"{Nombre} {Apellido}";
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
        public string Token { get; set; } // El cliente lo recibe, la API lo genera en POST
        public int ResidenteId { get; set; }
        public ResidenteDto Residente { get; set; } // Propiedad de navegación para detalles del residente
    }

    // DTO para la entidad BitacoraRegistro
    public class BitacoraRegistroDto
    {
        public int Id { get; set; }
        public DateTime FechaHoraEntrada { get; set; }
        public DateTime? FechaHoraSalida { get; set; }
        public int? ResidenteId { get; set; }
        public ResidenteDto Residente { get; set; } // Propiedad de navegación
        public int? InvitadoId { get; set; }
        public InvitadoDto Invitado { get; set; } // Propiedad de navegación
    }

    // DTO para la solicitud de escaneo de QR (lo que el cliente envía a la API)
    public class ScanRequestDto
    {
        public string QrToken { get; set; }
        public int GuardiaId { get; set; }
    }

    // DTO para la respuesta del escaneo de QR (lo que el cliente espera de la API)
    // Este DTO debe coincidir con el que la API devuelve.
    public class ScanResponseDto
    {
        public string Message { get; set; }
        public string PersonName { get; set; } // Nombre completo de la persona
        public string PersonType { get; set; } // "Residente" o "Invitado"
        public string AccessType { get; set; } // "Entrada" o "Salida"
        public bool IsSuccess { get; set; } // Para indicar si la operación fue exitosa
    }

}
