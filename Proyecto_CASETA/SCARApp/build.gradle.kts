// Define los plugins que se aplicarán a los módulos del proyecto.
plugins {
    // Plugin de aplicación de Android, necesario para construir aplicaciones Android.
    id("com.android.application") version "8.2.0" apply false

    // Plugin de librería de Android, si tuvieras módulos de librerías.
    id("com.android.library") version "8.2.0" apply false

    // Plugin de Kotlin Android, fundamental para el desarrollo con Kotlin.
    id("org.jetbrains.kotlin.android") version "1.9.0" apply false
}

// Configuración para la limpieza del proyecto.
tasks.register("clean", Delete::class) {
    delete(rootProject.buildDir) // Elimina el directorio de construcción de todo el proyecto.
}
