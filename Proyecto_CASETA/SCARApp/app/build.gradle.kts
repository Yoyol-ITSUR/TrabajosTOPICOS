
plugins {
    id("com.android.application")       // Plugin de aplicación de Android.
    id("org.jetbrains.kotlin.android")  // Plugin de Kotlin Android.
}

// Configuración de Android para el módulo.
android {

    namespace = "com.scar.residentapp"
    // Define la versión del SDK de compilación.
    compileSdk = 34

    // Configuración por defecto para la aplicación.
    defaultConfig {
        applicationId = "com.scar.residentapp" // ID único de tu aplicación.
        minSdk = 24 // Versión mínima de Android compatible (Android 7.0 Nougat).
        targetSdk = 34 // Versión de Android para la que se optimiza la aplicación.
        versionCode = 1 // Código de versión incremental para lanzamientos.
        versionName = "1.0" // Nombre de la versión visible para el usuario.

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner" // Runner para pruebas instrumentadas.
        vectorDrawables {
            useSupportLibrary = true // Habilita el soporte para Vector Drawables en versiones antiguas de Android.
        }
    }

    // Configuración para los tipos de compilación (debug, release).
    buildTypes {
        release {
            isMinifyEnabled = false // Deshabilita la ofuscación y optimización para depuración.
            proguardFiles(getDefaultProguardFile("proguard-android-optimize.txt"), "proguard-rules.pro") // Archivos ProGuard para optimización.
        }
    }

    // Configuración para las opciones de compilación de Kotlin.
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_1_8 // Compatibilidad con Java 8.
        targetCompatibility = JavaVersion.VERSION_1_8 // Compatibilidad con Java 8.
    }
    kotlinOptions {
        jvmTarget = "1.8" // Versión de JVM para Kotlin.
    }

    // Habilita Jetpack Compose.
    buildFeatures {
        compose = true // Habilita las características de Compose.
    }

    // Configuración del compilador de Compose.
    composeOptions {
        kotlinCompilerExtensionVersion = "1.5.1" // Versión de la extensión del compilador de Compose. Debe coincidir con tu versión de Kotlin.
    }

    // Empaquetado de recursos.
    packaging {
        resources {
            excludes += "/META-INF/{AL2.0,LGPL2.1}" // Excluye archivos problemáticos para evitar conflictos.
        }
    }
}

// Dependencias del proyecto.
dependencies {
    // Dependencias estándar de Android Jetpack.
    implementation("androidx.core:core-ktx:1.12.0") // Extensión KTX para Kotlin (funciones de conveniencia).
    implementation("androidx.lifecycle:lifecycle-runtime-ktx:2.6.2") // Soporte de ciclo de vida para Kotlin.
    implementation("androidx.activity:activity-compose:1.8.1") // Integración de Compose con Activity.

    // Dependencias de Jetpack Compose.
    implementation(platform("androidx.compose:compose-bom:2023.08.00")) // BOM para gestionar versiones de Compose.
    implementation("androidx.compose.ui:ui") // Componentes básicos de UI.
    implementation("androidx.compose.ui:ui-graphics") // Gráficos de UI.
    implementation("androidx.compose.ui:ui-tooling-preview") // Herramientas de vista previa en Android Studio.
    implementation("androidx.compose.material3:material3") // Componentes de Material Design 3.

    // Dependencias para pruebas.
    testImplementation("junit:junit:4.13.2") // JUnit para pruebas unitarias.
    androidTestImplementation("androidx.test.ext:junit:1.1.5") // JUnit para pruebas instrumentadas.
    androidTestImplementation("androidx.test.espresso:espresso-core:3.5.1") // Espresso para pruebas de UI.
    androidTestImplementation(platform("androidx.compose:compose-bom:2023.08.00")) // BOM para pruebas de Compose.
    androidTestImplementation("androidx.compose.ui:ui-test-junit4") // Reglas de JUnit para pruebas de Compose.
    debugImplementation("androidx.compose.ui:ui-tooling") // Herramientas de depuración para Compose.
    debugImplementation("androidx.compose.ui:ui-test-manifest") // Manifiesto de prueba para Compose.

    // Dependencias para la comunicación con la Web API (Retrofit y OkHttp).
    implementation("com.squareup.retrofit2:retrofit:2.9.0") // Cliente HTTP para peticiones REST.
    implementation("com.squareup.retrofit2:converter-gson:2.9.0") // Convertidor de JSON a objetos Kotlin (Gson).
    implementation("com.squareup.okhttp3:okhttp:4.11.0") // Cliente HTTP subyacente.
    implementation("com.squareup.okhttp3:logging-interceptor:4.11.0") // Interceptor para loggear peticiones HTTP (útil para depuración).

    // Dependencias para Coroutines (manejo de asincronía).
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-android:1.7.1") // Coroutines para Android.

    // Dependencia para la generación de códigos QR (ZXing Android Embedded).
    // Esta es una librería popular para generar y escanear QR.
    implementation("com.journeyapps:zxing-android-embedded:4.3.0")
    // La librería ZXing core subyacente.
    implementation("com.google.zxing:core:3.5.2")

    // Dependencia para guardar preferencias de usuario (URL de API, ID de residente).
    implementation("androidx.preference:preference-ktx:1.2.1") // SharedPreferences con extensiones KTX.
}
