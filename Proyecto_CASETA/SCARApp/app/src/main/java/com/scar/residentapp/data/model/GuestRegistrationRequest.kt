package com.scar.residentapp.data.model

import com.google.gson.annotations.SerializedName

/**
 * Clase de datos que representa el cuerpo de la solicitud para registrar un nuevo invitado.
 * Contiene la información necesaria para crear un registro de invitado en la Web API,
 * incluyendo todos los campos que se requieren en el modelo 'Invitado' del backend.
 *
 * @property name El nombre del invitado. Mapea a la propiedad 'Nombre' en C#.
 * @property surname Los apellidos del invitado. Mapea a la propiedad 'Apellido' en C#.
 * @property email El correo electrónico del invitado. Mapea a la propiedad 'Email' en C#.
 * @property phone El número de teléfono del invitado. Mapea a la propiedad 'Telefono' en C#.
 * @property date La fecha de la visita del invitado. Mapea a la propiedad 'FechaVisita' en C#.
 * @property invitingResidentId El ID del residente que está invitando. Mapea a la propiedad 'ResidenteId' en C#.
 */
data class GuestRegistrationRequest(
    @SerializedName("Nombre") // Coincide con la columna 'Nombre'
    val name: String,
    @SerializedName("Apellido") // Coincide con la columna 'Apellido'
    val surname: String,
    @SerializedName("Email") // Coincide con la columna 'Email'
    val email: String,
    @SerializedName("Telefono") // Coincide con la columna 'Telefono'
    val phone: String,
    @SerializedName("FechaVisita") // Coincide con la columna 'FechaVisita'
    val date: String, // Se envía como String (ej. "YYYY-MM-DD" o formato ISO 8601)
    @SerializedName("ResidenteId") // Coincide con la columna 'ResidenteId'
    val invitingResidentId: Int
)
