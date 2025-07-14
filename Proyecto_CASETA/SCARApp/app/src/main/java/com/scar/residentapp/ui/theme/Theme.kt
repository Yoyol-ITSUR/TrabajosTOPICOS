// app/src/main/java/com/scar/residentapp/ui/theme/SCARAppTheme.kt
package com.scar.residentapp.ui.theme

import android.app.Activity
import android.os.Build
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.dynamicDarkColorScheme
import androidx.compose.material3.dynamicLightColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.runtime.SideEffect
import androidx.compose.ui.graphics.toArgb
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.platform.LocalView
import androidx.core.view.WindowCompat

// Define tus esquemas de color para el tema oscuro
private val DarkColorScheme = darkColorScheme(
    primary = Purple80,
    secondary = PurpleGrey80,
    tertiary = Pink80,
    background = DarkBackground, // Color de fondo oscuro personalizado
    onBackground = DarkOnBackground, // Color de texto en fondo oscuro
    surface = DarkSurface, // Color de superficie oscuro
    onSurface = DarkOnSurface, // Color de texto en superficie oscura
    error = DarkError, // Color de error
    onError = DarkOnError, // Color de texto en error
    primaryContainer = DarkPrimaryContainer, // Contenedor primario
    onPrimaryContainer = DarkOnPrimaryContainer, // Texto en contenedor primario
    secondaryContainer = DarkSecondaryContainer, // Contenedor secundario
    onSecondaryContainer = DarkOnSecondaryContainer // Texto en contenedor secundario
)

// Define tus esquemas de color para el tema claro
private val LightColorScheme = lightColorScheme(
    primary = Purple40,
    secondary = PurpleGrey40,
    tertiary = Pink40,
    background = LightBackground, // Color de fondo claro personalizado
    onBackground = LightOnBackground, // Color de texto en fondo claro
    surface = LightSurface, // Color de superficie claro
    onSurface = LightOnSurface, // Color de texto en superficie clara
    error = LightError, // Color de error
    onError = LightOnError, // Color de texto en error
    primaryContainer = LightPrimaryContainer, // Contenedor primario
    onPrimaryContainer = LightOnPrimaryContainer, // Texto en contenedor primario
    secondaryContainer = LightSecondaryContainer, // Contenedor secundario
    onSecondaryContainer = LightOnSecondaryContainer // Texto en contenedor secundario
)

@Composable
fun SCARAppTheme(
    darkTheme: Boolean = isSystemInDarkTheme(),
    // Dynamic color is available on Android 12+
    dynamicColor: Boolean = true,
    content: @Composable () -> Unit
) {
    val colorScheme = when {
        dynamicColor && Build.VERSION.SDK_INT >= Build.VERSION_CODES.S -> {
            val context = LocalContext.current
            if (darkTheme) dynamicDarkColorScheme(context) else dynamicLightColorScheme(context)
        }
        darkTheme -> DarkColorScheme
        else -> LightColorScheme
    }
    val view = LocalView.current
    if (!view.isInEditMode) {
        SideEffect {
            val window = (view.context as Activity).window
            window.statusBarColor = colorScheme.primary.toArgb()
            WindowCompat.getInsetsController(window, view).isAppearanceLightStatusBars = darkTheme
        }
    }

    MaterialTheme(
        colorScheme = colorScheme,
        typography = Typography, // Asegúrate de que 'Typography' esté definido en Typography.kt
        content = content
    )
}