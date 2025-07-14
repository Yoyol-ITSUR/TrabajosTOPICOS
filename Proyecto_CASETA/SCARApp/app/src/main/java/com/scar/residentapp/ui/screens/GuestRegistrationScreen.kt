package com.scar.residentapp.ui.screens

import android.content.Context
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.scar.residentapp.data.model.GuestRegistrationRequest
import com.scar.residentapp.data.model.GuestRegistrationResponse
import com.scar.residentapp.data.remote.ApiClient
import kotlinx.coroutines.launch
import android.util.Log // Importar para usar Log para depuración
import com.google.gson.Gson // Importar Gson para logging del cuerpo de la respuesta (si se usa en logs)
import com.scar.residentapp.ui.theme.SCARAppTheme
import java.text.SimpleDateFormat // Para formatear la fecha
import java.util.Date // Para obtener la fecha actual
import java.util.Locale // Para especificar el locale en SimpleDateFormat

/**
 * Pantalla de Registro de Invitados.
 * Permite al residente registrar nuevos invitados, ingresando sus datos personales
 * y la fecha de visita. Una vez registrado, la aplicación generará y mostrará
 * un token de acceso único para el invitado.
 *
 * @param onLogout Callback que se invoca cuando el usuario decide cerrar sesión,
 * manejado por la Activity principal para redirigir al usuario a la pantalla de login.
 * @param onTokenGenerated Callback que se invoca cuando un token ha sido generado exitosamente
 * por la Web API. Recibe el token como un String y puede usarse para acciones adicionales.
 */
@OptIn(ExperimentalMaterial3Api::class) // Anotación para usar APIs experimentales de Material3
@Composable
fun GuestRegistrationScreen(
    onLogout: () -> Unit,
    onTokenGenerated: (String) -> Unit
) {
    val context = LocalContext.current // Contexto para acceder a SharedPreferences y servicios del sistema.
    val coroutineScope = rememberCoroutineScope() // Ámbito para lanzar operaciones asíncronas de red.

    // Estados mutables para los campos de entrada del formulario de invitado
    var guestName by remember { mutableStateOf("") }
    var guestSurname by remember { mutableStateOf("") }
    var guestEmail by remember { mutableStateOf("") }
    var guestPhone by remember { mutableStateOf("") }
    var guestDate by remember { mutableStateOf("") } // Representa la fecha de visita (YYYY-MM-DD)

    // Constantes para SharedPreferences, utilizadas para almacenar y recuperar datos persistentes
    val PREFS_NAME = "ResidentAppPrefs"
    val API_URL_KEY = "apiUrl"
    val RESIDENT_ID_KEY = "residentId" // Clave para el ID del residente autenticado

    // Estado para almacenar el ID del residente logueado, leído de SharedPreferences al inicio.
    var invitingResidentId by remember { mutableStateOf<Int?>(null) }

    // Efecto lanzado una vez cuando la pantalla se compone, para cargar el ID del residente.
    LaunchedEffect(Unit) {
        val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
        // Intenta leer el ID del residente. Si no se encuentra, se establece en null.
        invitingResidentId = prefs.getInt(RESIDENT_ID_KEY, -1).takeIf { it != -1 }
        if (invitingResidentId == null) {
            Log.e("GuestRegScreen", "Error: No se encontró el ID del residente logueado en SharedPreferences. El registro de invitados podría fallar.")
        } else {
            Log.d("GuestRegScreen", "ID de Residente cargado de SharedPreferences: $invitingResidentId")
        }
    }

    // Estados para el manejo de mensajes de retroalimentación en la UI
    var showMessage by remember { mutableStateOf(false) } // Controla la visibilidad del mensaje
    var messageText by remember { mutableStateOf("") } // Contiene el texto del mensaje
    var isSuccessMessage by remember { mutableStateOf(false) } // Indica si el mensaje es de éxito o error

    // Estado para almacenar y mostrar el token generado por la API
    var generatedToken by remember { mutableStateOf<String?>(null) }
    // Estado para controlar la visibilidad de la sección completa del token y el botón de limpiar
    var showTokenSection by remember { mutableStateOf(false) }

    // Estado para controlar el indicador de carga durante la operación de red
    var isLoading by remember { mutableStateOf(false) }


    Surface(
        modifier = Modifier.fillMaxSize(),
        color = MaterialTheme.colorScheme.background // Color de fondo de la pantalla
    ) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(24.dp),
            horizontalAlignment = Alignment.CenterHorizontally, // Centra los elementos horizontalmente
            verticalArrangement = Arrangement.SpaceBetween // Distribuye el espacio entre los elementos verticalmente
        ) {
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier.fillMaxWidth()
            ) {
                Text(
                    text = "Registro de Invitados",
                    style = MaterialTheme.typography.headlineMedium,
                    color = MaterialTheme.colorScheme.onBackground,
                    modifier = Modifier.padding(bottom = 24.dp)
                )

                // Campo de texto para el nombre del invitado
                OutlinedTextField(
                    value = guestName,
                    onValueChange = { newValue ->
                        guestName = newValue
                        showMessage = false // Oculta mensajes anteriores al editar
                        showTokenSection = false // Oculta la sección del token al editar
                    },
                    label = { Text("Nombre del Invitado") },
                    placeholder = { Text("Ej. Juan") },
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Text),
                    singleLine = true,
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 16.dp),
                    shape = RoundedCornerShape(8.dp)
                )

                // Campo de texto para los apellidos del invitado
                OutlinedTextField(
                    value = guestSurname,
                    onValueChange = { newValue ->
                        guestSurname = newValue
                        showMessage = false
                        showTokenSection = false
                    },
                    label = { Text("Apellidos del Invitado") },
                    placeholder = { Text("Ej. Pérez García") },
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Text),
                    singleLine = true,
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 16.dp),
                    shape = RoundedCornerShape(8.dp)
                )

                // Campo de texto para el correo electrónico
                OutlinedTextField(
                    value = guestEmail,
                    onValueChange = { newValue ->
                        guestEmail = newValue
                        showMessage = false
                        showTokenSection = false
                    },
                    label = { Text("Correo Electrónico") },
                    placeholder = { Text("ejemplo@dominio.com") },
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                    singleLine = true,
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 16.dp),
                    shape = RoundedCornerShape(8.dp)
                )

                // Campo de texto para el número de teléfono
                OutlinedTextField(
                    value = guestPhone,
                    onValueChange = { newValue ->
                        guestPhone = newValue
                        showMessage = false
                        showTokenSection = false
                    },
                    label = { Text("Teléfono") },
                    placeholder = { Text("Ej. 5512345678") },
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone),
                    singleLine = true,
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 16.dp),
                    shape = RoundedCornerShape(8.dp)
                )

                // Campo de texto para la fecha de visita
                OutlinedTextField(
                    value = guestDate,
                    onValueChange = { newValue ->
                        // Aquí se podría añadir lógica de validación de formato de fecha
                        guestDate = newValue
                        showMessage = false
                        showTokenSection = false
                    },
                    label = { Text("Fecha de Visita (YYYY-MM-DD)") },
                    placeholder = { Text("Ej. 2024-12-31") },
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number), // Se usa Number para facilitar entrada de fecha
                    singleLine = true,
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 24.dp),
                    shape = RoundedCornerShape(8.dp)
                )

                // Botón principal para Generar Token.
                Button(
                    onClick = {
                        // Valida que todos los campos requeridos estén completos y el ID del residente esté disponible
                        if (guestName.isNotBlank() && guestSurname.isNotBlank() &&
                            guestEmail.isNotBlank() && guestPhone.isNotBlank() &&
                            guestDate.isNotBlank() && invitingResidentId != null) {

                            isLoading = true // Activa el indicador de carga
                            showMessage = false // Oculta mensajes anteriores
                            generatedToken = null // Reinicia el token generado
                            showTokenSection = false // Oculta la sección del token mientras se procesa la solicitud

                            // Registra el ID del residente que se enviará a la API para depuración
                            Log.d("GuestRegScreen", "ID de Residente invitador enviado a la API: $invitingResidentId")

                            coroutineScope.launch { // Lanza una corrutina para la operación de red
                                try {
                                    val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
                                    val apiUrl = prefs.getString(API_URL_KEY, "") // Obtiene la URL de la API

                                    if (!apiUrl.isNullOrBlank()) {
                                        ApiClient.setBaseUrl(apiUrl) // Configura la URL base para la llamada API

                                        // Crea el objeto de solicitud de registro de invitado
                                        val request = GuestRegistrationRequest(
                                            name = guestName.trim(),
                                            surname = guestSurname.trim(),
                                            email = guestEmail.trim(),
                                            phone = guestPhone.trim(),
                                            date = guestDate.trim(),
                                            invitingResidentId = invitingResidentId!! // Usa el ID del residente autenticado
                                        )

                                        // Realiza la llamada a la API para registrar al invitado
                                        val response = ApiClient.apiService.registerGuest(request)

                                        Log.d("GuestRegScreen", "Código de respuesta de la API: ${response.code()}")
                                        Log.d("GuestRegScreen", "Respuesta RAW de la API: ${response.raw()}")

                                        if (response.isSuccessful) {
                                            // La respuesta HTTP fue exitosa (código 2xx)
                                            val registrationResponse = response.body()
                                            if (registrationResponse != null) {
                                                Log.d("GuestRegScreen", "Cuerpo de respuesta parseado de la API: ${Gson().toJson(registrationResponse)}")

                                                // Actualiza el estado de la UI con la información de la respuesta
                                                messageText = registrationResponse.message ?: "Operación completada con éxito." // Usa un mensaje por defecto si es nulo
                                                isSuccessMessage = registrationResponse.success
                                                showMessage = true
                                                generatedToken = registrationResponse.token
                                                showTokenSection = true // Muestra la sección del token

                                                // Limpia los campos del formulario después del registro exitoso
                                                guestName = ""
                                                guestSurname = ""
                                                guestEmail = ""
                                                guestPhone = ""
                                                guestDate = ""

                                                // Si la operación fue exitosa según la API, invoca el callback onTokenGenerated
                                                if (registrationResponse.success) {
                                                    registrationResponse.token?.let {
                                                        onTokenGenerated(it)
                                                    }
                                                }
                                            } else {
                                                Log.e("GuestRegScreen", "Error: Cuerpo de respuesta nulo para registro exitoso.")
                                                messageText = "Invitado registrado, pero no se recibió información completa del token."
                                                isSuccessMessage = true
                                                showMessage = true
                                                showTokenSection = false
                                            }
                                        } else {
                                            // La API devolvió un error HTTP (ej. 400 Bad Request, 500 Internal Server Error)
                                            val errorBody = response.errorBody()?.string()
                                            Log.e("GuestRegScreen", "Error en la API: Código ${response.code()}, Cuerpo: $errorBody")
                                            val errorMessage = try {
                                                // Intenta parsear el cuerpo del error si es un JSON con un campo 'message'
                                                Gson().fromJson(errorBody, Map::class.java)["message"] as? String
                                            } catch (e: Exception) {
                                                null
                                            }
                                            messageText = "Error al registrar invitado: ${response.code()}. ${errorMessage ?: errorBody ?: "Inténtalo de nuevo."}"
                                            isSuccessMessage = false
                                            showMessage = true
                                            showTokenSection = false
                                        }
                                    } else {
                                        // La URL de la API no está configurada
                                        messageText = "URL de la API no configurada. Por favor, configura la URL primero."
                                        isSuccessMessage = false
                                        showMessage = true
                                        showTokenSection = false
                                    }
                                } catch (e: Exception) {
                                    // Captura cualquier excepción durante la operación de red (ej. sin conexión)
                                    Log.e("GuestRegScreen", "Excepción durante el registro de invitado: ${e.message}", e)
                                    messageText = "Error de red: ${e.message ?: "Verifica tu conexión y la URL de la API."}"
                                    isSuccessMessage = false
                                    showMessage = true
                                    showTokenSection = false
                                } finally {
                                    isLoading = false // Oculta el indicador de carga al finalizar la operación
                                }
                            }
                        } else {
                            // Campos del formulario incompletos o ID del residente no disponible
                            messageText = if (invitingResidentId == null) {
                                "Error: ID del residente no disponible. Por favor, inicia sesión de nuevo."
                            } else {
                                "Por favor, completa todos los campos requeridos para registrar al invitado."
                            }
                            isSuccessMessage = false
                            showMessage = true
                            generatedToken = null
                            showTokenSection = false
                        }
                    },
                    modifier = Modifier.fillMaxWidth(),
                    shape = RoundedCornerShape(8.dp),
                    contentPadding = PaddingValues(vertical = 12.dp),
                    enabled = !isLoading && invitingResidentId != null // Deshabilita el botón mientras carga o si no hay ID de residente
                ) {
                    if (isLoading) {
                        // Muestra un indicador de progreso si la operación está cargando
                        CircularProgressIndicator(
                            modifier = Modifier.size(24.dp),
                            color = MaterialTheme.colorScheme.onPrimary
                        )
                    } else {
                        Text("Generar Token") // Texto del botón
                    }
                }

                // Muestra el mensaje de éxito/error.
                if (showMessage) {
                    Text(
                        text = messageText, // El texto del mensaje
                        color = if (isSuccessMessage) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.error, // Color según el estado de éxito
                        style = MaterialTheme.typography.bodyMedium,
                        modifier = Modifier.padding(top = 16.dp)
                    )
                }

                // Sección para mostrar el token generado y el botón de copiar.
                // Visible solo si showTokenSection es true y generatedToken no es nulo.
                if (showTokenSection && generatedToken != null) {
                    Column(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        // Tarjeta visual para el token
                        Card(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(top = 16.dp),
                            shape = RoundedCornerShape(12.dp),
                            colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.secondaryContainer)
                        ) {
                            Column(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(16.dp),
                                horizontalAlignment = Alignment.CenterHorizontally
                            ) {
                                Text(
                                    text = "Token Generado:",
                                    style = MaterialTheme.typography.titleMedium,
                                    color = MaterialTheme.colorScheme.onSecondaryContainer
                                )
                                Spacer(modifier = Modifier.height(8.dp))
                                // Campo de texto no editable para mostrar el token
                                OutlinedTextField(
                                    value = generatedToken ?: "", // Muestra el token o una cadena vacía
                                    onValueChange = { /* No editable */ }, // No permite cambios
                                    readOnly = true, // Campo de solo lectura
                                    singleLine = true,
                                    modifier = Modifier.fillMaxWidth(),
                                    textStyle = MaterialTheme.typography.headlineSmall.copy(
                                        color = MaterialTheme.colorScheme.onSecondaryContainer
                                    ),
                                    shape = RoundedCornerShape(8.dp)
                                )
                                Spacer(modifier = Modifier.height(8.dp))
                                // Botón para copiar el token al portapapeles
                                Button(
                                    onClick = {
                                        generatedToken?.let { tokenToCopy ->
                                            val clipboardManager = context.getSystemService(Context.CLIPBOARD_SERVICE) as android.content.ClipboardManager
                                            val clipData = android.content.ClipData.newPlainText("Token QR", tokenToCopy)
                                            clipboardManager.setPrimaryClip(clipData)
                                        }
                                    },
                                    modifier = Modifier.fillMaxWidth(),
                                    shape = RoundedCornerShape(8.dp),
                                    contentPadding = PaddingValues(vertical = 8.dp)
                                ) {
                                    Text("Copiar Token")
                                }
                            }
                        }
                        // Botón para limpiar todos los campos del formulario y ocultar la sección del token
                        Button(
                            onClick = {
                                guestName = ""
                                guestSurname = ""
                                guestEmail = ""
                                guestPhone = ""
                                guestDate = ""
                                generatedToken = null
                                showMessage = false
                                messageText = ""
                                showTokenSection = false // Oculta la sección completa del token
                            },
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(top = 16.dp),
                            shape = RoundedCornerShape(8.dp),
                            contentPadding = PaddingValues(vertical = 12.dp),
                            colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.secondary)
                        ) {
                            Text("Limpiar Campos")
                        }
                    }
                }
            }

            // Botón de Cerrar Sesión, posicionado en la parte inferior de la pantalla.
            TextButton(
                onClick = onLogout,
                modifier = Modifier.padding(top = 24.dp),
                enabled = !isLoading // Deshabilita el botón mientras la operación está en curso
            ) {
                Text("Cerrar Sesión")
            }
        }
    }
}

/**
 * Función Composable para la vista previa de la pantalla de Registro de Invitados en Android Studio.
 */
@Preview(showBackground = true)
@Composable
fun PreviewGuestRegistrationScreen() {
    SCARAppTheme { // Aplica el tema de la aplicación para la vista previa
        GuestRegistrationScreen(onLogout = {}, onTokenGenerated = {})
    }
}
