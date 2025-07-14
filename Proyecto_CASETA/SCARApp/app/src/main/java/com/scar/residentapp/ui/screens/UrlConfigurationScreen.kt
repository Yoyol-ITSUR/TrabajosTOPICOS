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
import com.scar.residentapp.ui.theme.SCARAppTheme

/**
 * Pantalla de configuración de la URL de la API.
 * Esta es la primera pantalla que el usuario verá al iniciar la aplicación.
 * Su propósito principal es permitir al usuario ingresar y guardar la URL base
 * de la Web API (por ejemplo, una URL de ngrok) para que la aplicación pueda comunicarse con ella.
 *
 * @param onUrlSaved Callback que se invoca cuando el usuario guarda una URL válida.
 * Recibe la URL ingresada como String, lo que indica que la configuración ha sido exitosa
 * y se puede proceder a la siguiente pantalla (login).
 */
@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun UrlConfigurationScreen(onUrlSaved: (String) -> Unit) {
    // Obtenemos el contexto actual de la aplicación. Esto es necesario para acceder a SharedPreferences,
    // que es donde almacenaremos la URL de forma persistente.
    val context = LocalContext.current

    // Definimos las constantes para el nombre del archivo de preferencias compartidas
    // y la clave bajo la cual se guardará la URL de la API.
    val PREFS_NAME = "ResidentAppPrefs"
    val API_URL_KEY = "apiUrl"

    // Estado mutable para almacenar el texto que el usuario está ingresando en el campo de la URL.
    // 'remember' asegura que este estado se mantenga a través de las recomposiciones de Compose,
    // evitando que el texto se pierda si la UI se redibuja.
    var urlInput by remember { mutableStateOf("") }

    // Estado mutable para controlar si se debe mostrar un mensaje de error de validación
    // debajo del campo de texto de la URL (por ejemplo, si el campo está vacío).
    var showUrlError by remember { mutableStateOf(false) }

    // Un efecto lanzado que se ejecuta una vez cuando el composable entra por primera vez en la composición.
    // Su propósito es cargar cualquier URL previamente guardada desde SharedPreferences
    // y pre-rellenar el campo de texto.
    LaunchedEffect(Unit) {
        val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
        val savedUrl = prefs.getString(API_URL_KEY, "")
        // Si se encuentra una URL guardada y no está vacía, la asignamos al estado urlInput.
        if (!savedUrl.isNullOrBlank()) {
            urlInput = savedUrl
            // Si la URL ya está guardada (por qué se haya usado la app),
            // podríamos considerar navegar automáticamente
            // a la pantalla de login para una experiencia más fluida. Por ahora, solo la pre-rellenamos.
        }
    }

    // Surface es un contenedor de Material Design que proporciona un fondo y elevación.
    // Lo utilizamos para envolver toda la pantalla, aplicando el color de fondo definido en nuestro tema.
    Surface(
        modifier = Modifier.fillMaxSize(), // El modificador fillMaxSize() hace que Surface ocupe todo el espacio disponible en la pantalla.
        color = MaterialTheme.colorScheme.background // Se utiliza el color de fondo especificado en el tema de Material Design.
    ) {
        // Column organiza sus elementos hijos verticalmente.
        // Aquí lo usamos para centrar el título, el campo de texto y el botón en la pantalla.
        Column(
            modifier = Modifier
                .fillMaxSize() // La columna también ocupa todo el tamaño para permitir el centrado.
                .padding(24.dp), // Se añade un padding de 24dp alrededor de todo el contenido para un mejor espaciado.
            horizontalAlignment = Alignment.CenterHorizontally, // Centra los elementos hijos horizontalmente.
            verticalArrangement = Arrangement.Center // Centra los elementos hijos verticalmente, distribuyéndolos equitativamente.
        ) {
            // Texto de título para la pantalla de configuración.
            Text(
                text = "Configurar URL de la API",
                style = MaterialTheme.typography.headlineMedium, // Aplica un estilo de texto de encabezado mediano de Material Design 3.
                color = MaterialTheme.colorScheme.onBackground, // El color del texto se adapta al fondo del tema.
                modifier = Modifier.padding(bottom = 32.dp) // Añade un espacio de 32dp debajo del título.
            )

            // Campo de texto delineado (OutlinedTextField) para que el usuario ingrese la URL.
            // Este componente de Material Design proporciona una interfaz clara y moderna.
            OutlinedTextField(
                value = urlInput, // El valor actual del campo de texto, vinculado al estado 'urlInput'.
                onValueChange = { newValue ->
                    urlInput = newValue // Actualiza el estado 'urlInput' cada vez que el usuario escribe.
                    showUrlError = false // Oculta cualquier mensaje de error si el usuario comienza a corregir la entrada.
                },
                label = { Text("URL de la API (ej. https://abcdef.ngrok.io)") }, // Etiqueta flotante que describe el campo.
                placeholder = { Text("Ingresa la URL de tu Web API") }, // Texto de marcador de posición visible cuando el campo está vacío.
                isError = showUrlError, // Un booleano que indica si el campo debe mostrar un estado de error.
                supportingText = {
                    if (showUrlError) {
                        Text("Por favor, ingresa una URL válida y no vacía.") // Mensaje de error si 'showUrlError' es verdadero.
                    }
                },
                keyboardOptions = KeyboardOptions(
                    keyboardType = KeyboardType.Uri // Sugiere al sistema operativo mostrar un teclado optimizado para la entrada de URLs.
                ),
                singleLine = true, // Restringe la entrada a una sola línea de texto.
                modifier = Modifier
                    .fillMaxWidth() // Hace que el campo ocupe todo el ancho disponible.
                    .padding(bottom = 16.dp), // Añade un espacio de 16dp debajo del campo.
                shape = RoundedCornerShape(8.dp) // Aplica bordes redondeados al campo de texto.
            )

            // Botón para guardar la URL ingresada y continuar.
            Button(
                onClick = {
                    // Lógica de validación simple: verifica que el campo de la URL no esté vacío.
                    if (urlInput.isNotBlank()) {
                        // Si la URL es válida, la guardamos en SharedPreferences para persistencia.
                        val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
                        with(prefs.edit()) {
                            putString(API_URL_KEY, urlInput.trim()) // Guarda la URL eliminando espacios en blanco al inicio/final.
                            apply() // Aplica los cambios de forma asíncrona para no bloquear el hilo principal.
                        }
                        // Llama al callback 'onUrlSaved' para notificar a la Activity principal
                        // que la URL ha sido guardada y se puede proceder.
                        onUrlSaved(urlInput.trim())
                    } else {
                        // Si el campo está vacío, activamos el estado de error para mostrar el mensaje.
                        showUrlError = true
                    }
                },
                modifier = Modifier.fillMaxWidth(), // El botón ocupa todo el ancho disponible.
                shape = RoundedCornerShape(8.dp), // Aplica bordes redondeados al botón.
                contentPadding = PaddingValues(vertical = 12.dp) // Añade padding vertical interno al texto del botón.
            ) {
                Text("Guardar y Continuar") // Texto visible en el botón.
            }
        }
    }
}

/**
 * Función Composable para la vista previa de la pantalla de configuración de URL en Android Studio.
 */
@Preview(showBackground = true) // Muestra un fondo para que la vista previa sea más clara.
@Composable
fun PreviewUrlConfigurationScreen() {
    SCARAppTheme {
        UrlConfigurationScreen(onUrlSaved = {})
    }
}
