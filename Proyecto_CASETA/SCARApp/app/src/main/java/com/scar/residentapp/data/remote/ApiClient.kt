package com.scar.residentapp.data.remote

import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit

/**
 * Objeto Singleton para configurar y proporcionar la instancia de Retrofit y el ApiService.
 * Este cliente se encargará de la comunicación con la Web API.
 */
object ApiClient {

    // Variable para almacenar la URL base de la API.
    // Se inicializará dinámicamente con la URL guardada por el usuario.
    private var BASE_URL: String = ""

    // Instancia de Retrofit, se inicializa de forma lazy (cuando se accede por primera vez).
    // Será nula hasta que se configure la URL base.
    private var retrofit: Retrofit? = null

    /**
     * Configura la URL base para todas las peticiones de la API.
     * Debe llamarse antes de intentar obtener el ApiService.
     *
     * @param baseUrl La URL base de la Web API (ej. la URL de ngrok).
     */
    fun setBaseUrl(baseUrl: String) {
        // Aseguramos que la URL termine con un '/' para evitar problemas de rutas en Retrofit.
        BASE_URL = if (baseUrl.endsWith("/")) baseUrl else "$baseUrl/"
        // Reiniciamos la instancia de Retrofit para que use la nueva URL base.
        retrofit = null // Forzamos la recreación de Retrofit la próxima vez que se llame a getApiService.
    }

    /**
     * Proporciona la instancia de Retrofit configurada.
     * Si la instancia no existe o la URL base ha cambiado, la crea.
     *
     * @return La instancia de Retrofit.
     * @throws IllegalStateException Si la URL base no ha sido configurada.
     */
    private fun getRetrofit(): Retrofit {
        // Verificamos si la URL base ha sido configurada.
        if (BASE_URL.isEmpty()) {
            throw IllegalStateException("La URL base de la API no ha sido configurada. Llama a setBaseUrl() primero.")
        }

        // Si la instancia de Retrofit no ha sido creada o necesita ser recreada, la construimos.
        if (retrofit == null) {
            // Configuración del interceptor de logging para ver las peticiones y respuestas HTTP en Logcat.
            // Esto es extremadamente útil para la depuración durante el desarrollo.
            val logging = HttpLoggingInterceptor().apply {
                level = HttpLoggingInterceptor.Level.BODY // Registra el cuerpo de la petición y la respuesta.
            }

            // Configuración de OkHttpClient, el cliente HTTP subyacente de Retrofit.
            val httpClient = OkHttpClient.Builder()
                .addInterceptor(logging) // Añadimos el interceptor de logging.
                .connectTimeout(30, TimeUnit.SECONDS) // Tiempo máximo para establecer una conexión.
                .readTimeout(30, TimeUnit.SECONDS)    // Tiempo máximo para leer datos del servidor.
                .writeTimeout(30, TimeUnit.SECONDS)   // Tiempo máximo para escribir datos al servidor.
                .build()

            // Construcción de la instancia de Retrofit.
            retrofit = Retrofit.Builder()
                .baseUrl(BASE_URL) // Establece la URL base.
                .addConverterFactory(GsonConverterFactory.create()) // Añade el convertidor de JSON (Gson).
                .client(httpClient) // Asigna el cliente HTTP configurado.
                .build()
        }
        return retrofit!! // Retorna la instancia de Retrofit (!! indica que no será nula aquí).
    }

    /**
     * Proporciona la instancia del servicio de la API (ApiService).
     *
     * @return La instancia de ApiService, a través de la cual se realizarán las llamadas a la API.
     */
    val apiService: ApiService by lazy {
        // 'lazy' asegura que la instancia de ApiService se cree solo una vez
        // y solo cuando se accede a ella por primera vez.
        getRetrofit().create(ApiService::class.java)
    }
}
