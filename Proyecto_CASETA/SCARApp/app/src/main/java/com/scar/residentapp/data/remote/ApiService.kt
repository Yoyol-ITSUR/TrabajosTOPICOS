package com.scar.residentapp.data.remote

import com.scar.residentapp.data.model.LoginRequestDto
import com.scar.residentapp.data.model.LoginResponseDto
import com.scar.residentapp.data.model.GuestRegistrationRequest
import com.scar.residentapp.data.model.GuestRegistrationResponse
import retrofit2.Response // Importa la clase Response de Retrofit
import retrofit2.http.Body
import retrofit2.http.POST

/**
 * Interfaz de servicio para la comunicación con la Web API.
 * Retrofit utilizará esta interfaz para generar el código necesario para realizar
 * las llamadas HTTP a los endpoints definidos.
 */
interface ApiService {

    /**
     * Define el endpoint para el inicio de sesión de residentes.
     *
     * @param loginRequest El cuerpo de la solicitud POST, que contiene el nombre de usuario y la contraseña.
     * Se enviará como JSON.
     * @return Un objeto `Response<LoginResponseDto>` que encapsula la respuesta del servidor.
     * `Response` permite verificar el código de estado HTTP y si la solicitud fue exitosa.
     * `LoginResponseDto` es el modelo de datos que esperamos recibir del servidor.
     */
    @POST("api/Auth/login")
    suspend fun loginResident(@Body loginRequest: LoginRequestDto): Response<LoginResponseDto>

    /**
     * Define el endpoint para el registro de nuevos invitados y la generación de su token de acceso.
     *
     * @param guestRegistrationRequest El cuerpo de la solicitud POST, que contiene los datos del nuevo invitado.
     * Se enviará como JSON.
     * @return Un objeto `Response<GuestRegistrationResponse>` que encapsula la respuesta del servidor.
     * Se espera que la respuesta incluya el éxito de la operación, un mensaje y el token generado.
     */
    @POST("api/Invitados")
    suspend fun registerGuest(@Body guestRegistrationRequest: GuestRegistrationRequest): Response<GuestRegistrationResponse>
}
