package com.scar.residentapp.ui.screens

import android.content.Context // Necesario para acceder a SharedPreferences
import androidx.compose.foundation.layout.* // Para organizar los elementos de la UI (Column, Spacer)
import androidx.compose.foundation.shape.RoundedCornerShape // Para aplicar bordes redondeados a los componentes
import androidx.compose.foundation.text.KeyboardOptions // Para configurar el tipo de teclado
import androidx.compose.material3.* // Componentes de Material Design 3 (Text, Button, OutlinedTextField, Surface, CircularProgressIndicator)
import androidx.compose.runtime.* // Para gestionar el estado de los componentes Compose (remember, mutableStateOf, LaunchedEffect, rememberCoroutineScope)
import androidx.compose.ui.Alignment // Para alinear elementos dentro de los layouts
import androidx.compose.ui.Modifier // Para modificar el comportamiento y apariencia de los composables
import androidx.compose.ui.platform.LocalContext // Para obtener el contexto de la aplicación en un composable
import androidx.compose.ui.text.input.KeyboardType // Tipos de teclado para KeyboardOptions
import androidx.compose.ui.text.input.PasswordVisualTransformation // Para ocultar la entrada de texto de la contraseña
import androidx.compose.ui.tooling.preview.Preview // Para la vista previa en Android Studio
import androidx.compose.ui.unit.dp // Unidades de densidad de píxeles para el espaciado
import com.scar.residentapp.data.model.LoginRequestDto // DTO para la solicitud de login a la API
import com.scar.residentapp.data.model.LoginResponseDto // DTO para la respuesta de login de la API
import com.scar.residentapp.data.remote.ApiClient // Cliente para realizar llamadas a la API
import com.scar.residentapp.ui.theme.SCARAppTheme // Tema de la aplicación para consistencia visual
import kotlinx.coroutines.launch // Para lanzar corrutinas (operaciones asíncronas)
import android.util.Log // Para logging de depuración en Logcat
import com.google.gson.Gson // Para serializar/deserializar objetos JSON en logs

/**
 * Pantalla de Autenticación (Login) para residentes.
 * Esta pantalla permite a los usuarios iniciar sesión en la aplicación
 * proporcionando sus credenciales (nombre de usuario y contraseña).
 * Se comunica con la Web API para autenticar al usuario y, en caso de éxito,
 * guarda el ID del residente y el token de autenticación para su uso posterior.
 *
 * @param onLoginSuccess Callback que se invoca cuando el proceso de autenticación es exitoso.
 * Su propósito es notificar a la `MainActivity` para que navegue a la pantalla principal de la aplicación.
 * @param onNavigateToUrlConfig Callback que se invoca cuando el usuario decide volver a la
 * pantalla de configuración de la URL de la API. Esto es útil para cambiar la URL del backend
 * si es necesario (ej. durante el desarrollo con ngrok).
 */
@OptIn(ExperimentalMaterial3Api::class) // Habilita el uso de componentes experimentales de Material3 como OutlinedTextField
@Composable
fun LoginScreen(
    onLoginSuccess: () -> Unit,
    onNavigateToUrlConfig: () -> Unit
) {
    val context = LocalContext.current // Permite acceder a recursos del sistema Android, como SharedPreferences.
    val coroutineScope = rememberCoroutineScope() // Crea un ámbito para lanzar corrutinas, gestionando su ciclo de vida.

    // --- Estados de la UI ---
    // Estados mutables para almacenar el texto ingresado en los campos de usuario y contraseña.
    var username by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }

    // Estado booleano que controla la visibilidad del mensaje de error en la UI.
    var showLoginError by remember { mutableStateOf(false) }
    // Estado booleano que indica si una operación de red está en curso, para mostrar un indicador de carga.
    var isLoading by remember { mutableStateOf(false) }
    // Estado para almacenar el mensaje de error que se mostrará al usuario, inicializado como cadena vacía.
    var apiErrorMessage by remember { mutableStateOf("") }

    // --- Constantes para SharedPreferences ---
    // Nombre del archivo donde se almacenarán las preferencias de la aplicación.
    val PREFS_NAME = "ResidentAppPrefs"
    // Clave para guardar y recuperar la URL base de la API.
    val API_URL_KEY = "apiUrl"
    // Clave para guardar y recuperar el ID del residente autenticado.
    val RESIDENT_ID_KEY = "residentId"
    // Clave para guardar y recuperar el token de autenticación (aunque no se usa directamente aquí, es buena práctica).
    val AUTH_TOKEN_KEY = "authToken"

    // Contenedor principal de la pantalla, que aplica el color de fondo del tema.
    Surface(
        modifier = Modifier.fillMaxSize(), // Ocupa todo el espacio disponible en la pantalla.
        color = MaterialTheme.colorScheme.background
    ) {
        // Columna que organiza los elementos de la UI verticalmente y los centra en la pantalla.
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(24.dp), // Añade un padding alrededor del contenido.
            horizontalAlignment = Alignment.CenterHorizontally, // Centra los elementos horizontalmente.
            verticalArrangement = Arrangement.Center // Centra los elementos verticalmente.
        ) {
            // Título de la pantalla de inicio de sesión.
            Text(
                text = "Iniciar Sesión",
                style = MaterialTheme.typography.headlineMedium,
                color = MaterialTheme.colorScheme.onBackground,
                modifier = Modifier.padding(bottom = 32.dp)
            )

            // Campo de texto para el nombre de usuario.
            OutlinedTextField(
                value = username,
                onValueChange = { newValue ->
                    username = newValue
                    showLoginError = false // Oculta el mensaje de error si el usuario empieza a escribir.
                },
                label = { Text("Usuario") },
                placeholder = { Text("Ingresa tu usuario") },
                isError = showLoginError, // Muestra un estado de error visual si `showLoginError` es true.
                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Text), // Configura el teclado para entrada de texto.
                singleLine = true, // Limita la entrada a una sola línea.
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 16.dp),
                shape = RoundedCornerShape(8.dp) // Aplica bordes redondeados.
            )

            // Campo de texto para la contraseña.
            OutlinedTextField(
                value = password,
                onValueChange = { newValue ->
                    password = newValue
                    showLoginError = false // Oculta el mensaje de error si el usuario empieza a escribir.
                },
                label = { Text("Contraseña") },
                placeholder = { Text("Ingresa tu contraseña") },
                isError = showLoginError, // Muestra un estado de error visual si `showLoginError` es true.
                visualTransformation = PasswordVisualTransformation(), // Oculta los caracteres de la contraseña.
                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Password), // Configura el teclado para contraseñas.
                singleLine = true, // Limita la entrada a una sola línea.
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 24.dp),
                shape = RoundedCornerShape(8.dp)
            )

            // Muestra el mensaje de error si `showLoginError` es true.
            if (showLoginError) {
                // Log de depuración para ver el estado del mensaje de error justo antes de mostrarlo.
                Log.d("LoginScreen", "Mostrando error en UI: showLoginError=$showLoginError, apiErrorMessage='$apiErrorMessage'")
                Text(
                    text = apiErrorMessage, // El texto del mensaje de error.
                    color = MaterialTheme.colorScheme.error, // Color de texto para indicar error.
                    style = MaterialTheme.typography.bodySmall,
                    modifier = Modifier.padding(bottom = 16.dp)
                )
            }

            // Botón principal para iniciar sesión.
            Button(
                onClick = {
                    // Valida que los campos de usuario y contraseña no estén vacíos.
                    if (username.isNotBlank() && password.isNotBlank()) {
                        // Limpia cualquier mensaje de error anterior y oculta el indicador de carga.
                        apiErrorMessage = ""
                        showLoginError = false
                        isLoading = true // Activa el indicador de carga mientras se procesa la solicitud.

                        // Lanza una corrutina para realizar la operación de red de forma asíncrona.
                        coroutineScope.launch {
                            try {
                                // Obtiene la instancia de SharedPreferences.
                                val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
                                // Recupera la URL de la API guardada previamente.
                                val apiUrl = prefs.getString(API_URL_KEY, "")

                                // Verifica si la URL de la API está configurada.
                                if (!apiUrl.isNullOrBlank()) {
                                    // Configura la URL base en el cliente de la API.
                                    ApiClient.setBaseUrl(apiUrl)

                                    // Crea el objeto de solicitud de login con las credenciales.
                                    val loginRequest = LoginRequestDto(username, password)
                                    // Realiza la llamada a la API.
                                    val response = ApiClient.apiService.loginResident(loginRequest)

                                    // Registra el código de respuesta HTTP y la respuesta RAW completa para depuración.
                                    Log.d("LoginScreen", "API Response Code: ${response.code()}")
                                    Log.d("LoginScreen", "API Raw Response: ${response.raw()}")

                                    // Verifica si la respuesta HTTP fue exitosa (código 2xx).
                                    if (response.isSuccessful) {
                                        // Obtiene el cuerpo de la respuesta, deserializado a LoginResponseDto.
                                        val loginResponse = response.body()
                                        // Verifica que el cuerpo de la respuesta no sea nulo.
                                        if (loginResponse != null) {
                                            // Registra el cuerpo de la respuesta parseado a JSON para depuración.
                                            Log.d("LoginScreen", "API Parsed Response Body: ${Gson().toJson(loginResponse)}")
                                            // Registra los valores clave del objeto deserializado para una verificación rápida.
                                            Log.d("LoginScreen", "VALORES DESPUÉS DE PARSEAR: success=${loginResponse.success}, message='${loginResponse.message}'")

                                            // Verifica la propiedad 'success' dentro del objeto LoginResponseDto.
                                            if (loginResponse.success) {
                                                // Si el login es exitoso, limpia los estados de error.
                                                showLoginError = false
                                                apiErrorMessage = ""

                                                // Guarda el ID del residente en SharedPreferences si está presente.
                                                loginResponse.residentId?.let { residentId ->
                                                    Log.d("LoginScreen", "ID de Residente recibido de la API: $residentId")
                                                    with(prefs.edit()) {
                                                        putInt(RESIDENT_ID_KEY, residentId)
                                                        apply()
                                                    }
                                                }
                                                // Guarda el token de autenticación en SharedPreferences si está presente.
                                                loginResponse.token?.let { token ->
                                                    with(prefs.edit()) {
                                                        putString(AUTH_TOKEN_KEY, token)
                                                        apply()
                                                    }
                                                }
                                                // Registra el éxito del login y navega a la siguiente pantalla.
                                                Log.d("LoginScreen", "Login Exitoso: ${loginResponse.message}, Token: ${loginResponse.token}")
                                                onLoginSuccess()
                                            } else {
                                                // Si la propiedad 'success' es false, muestra el mensaje de error de la API.
                                                apiErrorMessage = loginResponse.message ?: "Credenciales inválidas o error desconocido."
                                                showLoginError = true
                                            }
                                        } else {
                                            // Si el cuerpo de la respuesta es nulo a pesar de un HTTP 200.
                                            Log.e("LoginScreen", "Error: Cuerpo de respuesta nulo para login exitoso (pero HTTP 200).")
                                            apiErrorMessage = "Error de servidor: Respuesta vacía."
                                            showLoginError = true
                                        }
                                    } else {
                                        // Si la respuesta HTTP no fue exitosa (ej. 401, 400, 500).
                                        val errorBody = response.errorBody()?.string()
                                        Log.e("LoginScreen", "Error HTTP en login: Código ${response.code()}, Body: $errorBody")
                                        // Intenta deserializar el cuerpo del error para obtener un mensaje más específico.
                                        val errorResponse = try {
                                            Gson().fromJson(errorBody, LoginResponseDto::class.java)
                                        } catch (e: Exception) {
                                            null
                                        }
                                        // Establece el mensaje de error y activa la visibilidad del error.
                                        apiErrorMessage = errorResponse?.message ?: "Error de conexión o credenciales inválidas. Código: ${response.code()}. Detalles: ${errorBody ?: "No hay detalles."}"
                                        showLoginError = true
                                    }
                                } else {
                                    // Si la URL de la API no está configurada en SharedPreferences.
                                    apiErrorMessage = "URL de la API no configurada. Vuelve a la pantalla anterior."
                                    showLoginError = true
                                }
                            } catch (e: Exception) {
                                // Captura cualquier excepción de red o procesamiento.
                                Log.e("LoginScreen", "Excepción durante el login: ${e.message}", e)
                                apiErrorMessage = "Error de red: ${e.message ?: "Verifica tu conexión y la URL de la API."}"
                                showLoginError = true
                            } finally {
                                isLoading = false // Desactiva el indicador de carga al finalizar la operación.
                            }
                        }
                    } else {
                        // Si los campos de usuario o contraseña están vacíos.
                        apiErrorMessage = "Por favor, ingresa tu usuario y contraseña."
                        showLoginError = true
                    }
                },
                modifier = Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(8.dp),
                contentPadding = PaddingValues(vertical = 12.dp),
                enabled = !isLoading // Deshabilita el botón mientras la operación está en curso.
            ) {
                if (isLoading) {
                    // Muestra un indicador de progreso circular si `isLoading` es true.
                    CircularProgressIndicator(
                        modifier = Modifier.size(24.dp),
                        color = MaterialTheme.colorScheme.onPrimary
                    )
                } else {
                    Text("Iniciar Sesión") // Texto del botón.
                }
            }

            // Botón para navegar a la pantalla de configuración de la URL.
            TextButton(
                onClick = onNavigateToUrlConfig,
                modifier = Modifier.padding(top = 16.dp),
                enabled = !isLoading // Deshabilita el botón mientras la operación está en curso.
            ) {
                Text("Cambiar URL de la API")
            }
        }
    }
}

/**
 * Función Composable para la vista previa de la pantalla de Login en Android Studio.
 */
@Preview(showBackground = true) // Muestra un fondo para que la vista previa sea más clara.
@Composable
fun PreviewLoginScreen() {
    SCARAppTheme { // Aplica el tema de la aplicación para que la vista previa refleje el diseño final.
        // Se llama a la pantalla con callbacks vacíos, ya que en la vista previa no hay lógica de navegación real.
        LoginScreen(onLoginSuccess = {}, onNavigateToUrlConfig = {})
    }
}
