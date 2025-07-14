package com.scar.residentapp.data.model

import com.google.gson.annotations.SerializedName

/**
 * Clase de datos que representa el cuerpo de la solicitud de inicio de sesión (Login Request).
 * Contiene las credenciales que se enviarán a la Web API para autenticar a un residente.
 *
 * @property username El nombre de usuario del residente. Este campo debe coincidir con el nombre
 * esperado por la Web API para el campo de usuario.
 * @property password La contraseña del residente. Este campo debe coincidir con el nombre
 * esperado por la Web API para el campo de contraseña.
 */
data class LoginRequestDto(
    @SerializedName("username")
    val username: String,
    @SerializedName("password")
    val password: String
)
