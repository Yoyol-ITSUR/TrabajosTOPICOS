package com.scar.residentapp.data.model

import com.google.gson.annotations.SerializedName

/**
 * Clase de datos que representa la respuesta JSON esperada de la Web API
 * después de un intento de inicio de sesión.
 * Las propiedades están mapeadas a los nombres JSON exactos devueltos por la API.
 */
data class LoginResponseDto(
    @SerializedName("Success")
    val success: Boolean,
    @SerializedName("Message")
    val message: String?, // Se permite que sea nulo en caso de que la API no lo envíe
    @SerializedName("Token")
    val token: String?, // El token puede ser nulo si el login falla
    @SerializedName("ResidentId")
    val residentId: Int? // El ID del residente puede ser nulo si el login falla o no hay residente asociado
)
