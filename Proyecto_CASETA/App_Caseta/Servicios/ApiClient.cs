using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic; // Para IEnumerable<T>
using System.Net.Http.Json; // Para PostAsJsonAsync, GetFromJsonAsync

/*
 *  Clase de servicio que actúa como cliente HTTP para comunicarse con la Web API (WebApiSCAR). 
 *  Contiene métodos genéricos para realizar peticiones GET, POST, PUT y DELETE
 *  a los diferentes endpoints de la API,
 *  encapsulando la lógica de conexión y serialización/deserialización de datos.
*/

namespace App_Caseta.Servicios
{
    /// <summary>
    /// Cliente API para interactuar con la Web API del Sistema de Control de Acceso Residencial (SCAR).
    /// Proporciona métodos genéricos para realizar peticiones GET, POST, PUT y DELETE.
    /// Añade automáticamente el prefijo "api/" a los endpoints y maneja respuestas 204 No Content.
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;
        public static readonly string url = "https://localhost:7199";

        /// <summary>
        /// Constructor del ApiClient.
        /// </summary>
        /// <param name="baseAddress">La URL base de la Web API 
        public ApiClient(string baseAddress)
        {
            _baseAddress = baseAddress.TrimEnd('/'); // Asegurarse de que no haya una barra al final
            _httpClient = new HttpClient();
            // Configurar el tiempo de espera para las peticiones.
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Envía una solicitud HTTP genérica y deserializa la respuesta JSON.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto esperado en la respuesta.</typeparam>
        /// <param name="method">El método HTTP (GET, POST, PUT, DELETE).</param>
        /// <param name="endpoint">El endpoint específico de la API (ej. "users", "residentes/5").
        /// No necesita incluir "api/", ya que se añade automáticamente.</param>
        /// <param name="data">El objeto de datos a serializar en el cuerpo de la solicitud (para POST/PUT).</param>
        /// <returns>El objeto deserializado de la respuesta. Para 204 No Content, devuelve el valor predeterminado de T.</returns>
        /// <exception cref="HttpRequestException">Se lanza si la respuesta HTTP no es exitosa (código 2xx) y no es 204.</exception>
        private async Task<T> SendRequest<T>(HttpMethod method, string endpoint, object data = null)
        {
            // Construir la URL completa, añadiendo "api/" si el endpoint no lo tiene ya.
            string fullEndpoint = endpoint.StartsWith("api/", StringComparison.OrdinalIgnoreCase) ? endpoint : $"api/{endpoint}";
            string fullUrl = $"{_baseAddress}/{fullEndpoint}";

            System.Diagnostics.Debug.WriteLine($"DEBUG: Solicitando URL: {fullUrl}"); // Para depuración

            var request = new HttpRequestMessage(method, fullUrl);

            if (data != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            // Si la respuesta es 204 No Content, no hay cuerpo para leer.
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return default(T); // Devuelve el valor predeterminado para el tipo T (ej. null para clases)
            }

            // Si la respuesta no es exitosa, intentamos leer el contenido del error para más detalles.
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"API Error Response ({response.StatusCode}): {errorContent}");
                // Incluimos el contenido del error en el mensaje de la excepción para depuración.
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}. Details: {errorContent}", null, response.StatusCode);
            }

            // Para respuestas exitosas con contenido (ej. 200 OK, 201 Created)
            string jsonResponse = await response.Content.ReadAsStringAsync();

            // Si la respuesta es una cadena vacía pero no es 204, puede ser un problema.
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new HttpRequestException($"Received empty response from API for {fullUrl}. Expected JSON content.");
            }

            try
            {
                // Intentamos deserializar el JSON a nuestro tipo T.
                return JsonSerializer.Deserialize<T>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                // Capturamos errores de deserialización JSON y proporcionamos más contexto.
                throw new JsonException($"Failed to deserialize JSON response to {typeof(T).Name}. " +
                                        $"Original error: {ex.Message}. " +
                                        $"Response content: {jsonResponse}", ex);
            }
        }

        /// <summary>
        /// Realiza una petición GET asíncrona a un endpoint de la API.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto esperado en la respuesta.</typeparam>
        /// <param name="endpoint">El endpoint específico de la API (ej. "Users", "Residentes/5").</param>
        /// <returns>El objeto deserializado de la respuesta.</returns>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            return await SendRequest<T>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Realiza una petición POST asíncrona a un endpoint de la API con datos en el cuerpo.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto esperado en la respuesta.</typeparam>
        /// <param name="endpoint">El endpoint específico de la API.</param>
        /// <param name="data">El objeto de datos a enviar en el cuerpo de la solicitud.</param>
        /// <returns>El objeto deserializado de la respuesta (ej. el objeto creado con su ID).</returns>
        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            return await SendRequest<T>(HttpMethod.Post, endpoint, data);
        }

        /// <summary>
        /// Realiza una petición PUT asíncrona a un endpoint de la API con datos en el cuerpo.
        /// Este método no espera un valor de retorno específico (ej. 204 No Content).
        /// </summary>
        /// <param name="endpoint">El endpoint específico de la API.</param>
        public async Task PutAsync(string endpoint, object data)
        {
            // Para PUT, no esperamos un retorno específico, por eso T es 'object'.
            // El default(object) para un tipo de referencia es null, lo cual es correcto para 204.
            await SendRequest<object>(HttpMethod.Put, endpoint, data);
        }

        /// <summary>
        /// Realiza una petición DELETE asíncrona a un endpoint de la API.
        /// Este método no espera un valor de retorno específico (ej. 204 No Content).
        /// </summary>
        /// <param name="endpoint">El endpoint específico de la API.</param>
        public async Task DeleteAsync(string endpoint)
        {
            // Para DELETE, no esperamos un retorno específico.
            await SendRequest<object>(HttpMethod.Delete, endpoint);
        }
    }
}
