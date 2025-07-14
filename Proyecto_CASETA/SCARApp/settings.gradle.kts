// Define cómo Gradle gestiona los plugins para todo el proyecto.
// Aquí se especifican los repositorios donde Gradle debe buscar los plugins.
pluginManagement {
    repositories {
        google()        // Repositorio de Google para plugins de Android.
        mavenCentral()  // Repositorio central de Maven para otros plugins.
        gradlePluginPortal() // Repositorio oficial de plugins de Gradle.
    }
}

// Define la gestión de dependencias para todo el proyecto.
// Esto incluye los repositorios donde Gradle buscará las dependencias de las librerías.
dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.FAIL_ON_PROJECT_REPOS) // Configura Gradle para fallar si un módulo intenta añadir repositorios a nivel de proyecto.
    repositories {
        google()        // Repositorio de Google para Android Jetpack, etc.
        mavenCentral()  // Repositorio central de Maven para la mayoría de las bibliotecas.
    }
}

// Define el nombre raíz de tu proyecto Gradle.
rootProject.name = "SCARApp"

// Incluye los módulos de tu proyecto.
// ":app" se refiere al módulo de la aplicación principal.
include(":app")
