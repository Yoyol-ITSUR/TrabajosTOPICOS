package com.scar.residentapp.data.model

import com.google.gson.annotations.SerializedName

/**
 * Clase de datos que representa la respuesta JSON esperada de la Web API
 * después de registrar un invitado.
 * Las propiedades están mapeadas a los nombres JSON exactos devueltos por la API.
 *
 * @property id El ID único del invitado registrado. Puede ser nulo si la operación falla.
 * @property token El token de acceso único generado para el invitado. Puede ser nulo si la operación falla.
 * @property success Un valor booleano que indica si la operación fue exitosa.
 * @property message Un mensaje descriptivo del resultado de la operación. Puede ser nulo si la API no lo proporciona.
 */
data class GuestRegistrationResponse(
    @SerializedName("id") // Mapea a la propiedad 'Id' en el JSON de la API
    val id: Int?, // Se permite que sea nulo para manejar casos de error donde el ID no se genera
    @SerializedName("token") // Mapea a la propiedad 'Token' en el JSON de la API
    val token: String?, // Se permite que sea nulo en caso de error o si no se genera un token
    @SerializedName("success") // Mapea a la propiedad 'Success' en el JSON de la API
    val success: Boolean, // Indica el estado general de la operación
    @SerializedName("message") // Mapea a la propiedad 'Message' en el JSON de la API
    val message: String? // Se permite que sea nulo en caso de que la API no lo envíe
)
