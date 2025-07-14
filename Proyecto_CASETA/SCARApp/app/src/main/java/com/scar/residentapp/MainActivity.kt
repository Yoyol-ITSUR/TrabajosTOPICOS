package com.scar.residentapp

import android.content.Context
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.tooling.preview.Preview
import com.scar.residentapp.ui.screens.LoginScreen
import com.scar.residentapp.ui.screens.UrlConfigurationScreen
import com.scar.residentapp.ui.theme.SCARAppTheme
import androidx.compose.foundation.layout.wrapContentSize
import androidx.compose.ui.Alignment
import com.scar.residentapp.ui.screens.GuestRegistrationScreen

/**
 * Define las posibles rutas de navegación (pantallas) dentro de la aplicación.
 * Esto ayuda a manejar el estado de la UI de forma clara y segura.
 */
sealed class Screen {
    object UrlConfig : Screen() // Pantalla para configurar la URL de la API
    object Login : Screen()     // Pantalla de inicio de sesión
    object Home : Screen()      // Pantalla principal (registro de invitados)
}

class MainActivity : ComponentActivity() {
    // Constantes para SharedPreferences, deben coincidir con las de UrlConfigurationScreen
    private val PREFS_NAME = "ResidentAppPrefs"
    private val API_URL_KEY = "apiUrl"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            SCARAppTheme {
                // Un contenedor de superficie que usa el color 'background' del tema
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background
                ) {
                    // Manejador de la navegación de la aplicación
                    ResidentAppNavigation()
                }
            }
        }
    }

    /**
     * Composable que gestiona la navegación entre las diferentes pantallas de la aplicación.
     * Utiliza un estado para determinar qué pantalla se debe mostrar.
     */
    @Composable
    fun ResidentAppNavigation() {
        // Obtenemos el contexto para acceder a SharedPreferences.
        val context = LocalContext.current

        // Estado mutable que representa la pantalla actual que se debe mostrar.
        // Se inicializa en UrlConfig y se actualiza en LaunchedEffect.
        var currentScreen by remember { mutableStateOf<Screen>(Screen.UrlConfig) }

        // Efecto lanzado que se ejecuta una vez cuando el composable entra en la composición.
        // Se encarga de determinar la pantalla inicial al cargar la aplicación.
        LaunchedEffect(Unit) {
            val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
            val savedUrl = prefs.getString(API_URL_KEY, "")

            // Si ya hay una URL guardada, navegamos directamente a la pantalla de Login.
            // De lo contrario, nos quedamos en la pantalla de configuración de URL.
            currentScreen = if (!savedUrl.isNullOrBlank()) {
                Screen.Login
            } else {
                Screen.UrlConfig
            }
        }

        // Usamos una sentencia 'when' para renderizar la pantalla correcta
        // basándonos en el valor de 'currentScreen'.
        when (currentScreen) {
            is Screen.UrlConfig -> {
                // Cuando la URL se guarda, navegamos a la pantalla de Login.
                UrlConfigurationScreen(onUrlSaved = { url ->
                    // Aquí, la URL ya está guardada en SharedPreferences por UrlConfigurationScreen.
                    // Solo necesitamos cambiar la pantalla.
                    currentScreen = Screen.Login
                })
            }
            is Screen.Login -> {
                // Cuando el login es exitoso, navegamos a la pantalla Home (registro de invitados).
                LoginScreen(
                    onLoginSuccess = {
                        currentScreen = Screen.Home // Navega a la pantalla principal (GuestRegistrationScreen)
                    },
                    // Permite volver a la pantalla de configuración de URL desde el login.
                    onNavigateToUrlConfig = {
                        currentScreen = Screen.UrlConfig
                    }
                )
            }
            is Screen.Home -> {
                // Aquí se renderiza la pantalla de registro de invitados.
                GuestRegistrationScreen(
                    onLogout = {
                        // Al cerrar sesión, limpiamos la URL guardada (opcional, pero buena práctica)
                        val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
                        with(prefs.edit()) {
                            remove(API_URL_KEY) // Elimina la URL guardada
                            apply()
                        }
                        currentScreen = Screen.Login // Vuelve a la pantalla de login
                    },
                    onTokenGenerated = { token ->
                        // Aquí puedes manejar el token generado si MainActivity necesita saberlo.
                        // Por ahora, GuestRegistrationScreen ya lo muestra, así que solo imprimimos.
                        println("Token generado recibido en MainActivity: $token")
                    }
                )
            }
        }
    }
}

/**
 * Vista previa de la aplicación completa.
 */
@Preview(showBackground = true)
@Composable
fun DefaultPreview() {
    SCARAppTheme {
        // En la vista previa, podemos mostrar una pantalla específica o un estado inicial.
        // Aquí mostramos la pantalla de configuración de URL por defecto.
        UrlConfigurationScreen(onUrlSaved = {})
    }
}
