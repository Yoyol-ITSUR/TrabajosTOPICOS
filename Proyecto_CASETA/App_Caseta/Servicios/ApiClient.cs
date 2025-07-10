using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic; // Para IEnumerable<T>
using System.Net.Http.Json; // Para PostAsJsonAsync, GetFromJsonAsync

namespace App_Caseta.Servicios
{
    /// <summary>
    /// Cliente API para interactuar con la Web API del Sistema de Control de Acceso Residencial (SCAR).
    /// Proporciona métodos genéricos para realizar peticiones GET, POST, PUT y DELETE.
    /// </summary>
    public class ApiClient
    {
        public static readonly string url = "";

        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;

        /// <summary>
        /// Constructor del ApiClient.
        /// </summary>
        /// <param name="baseAddress">La URL base de la Web API.</param>
        public ApiClient(string baseAddress)
        {
            _baseAddress = baseAddress;
            _httpClient = new HttpClient();
            // Configurar el tiempo de espera para las peticiones.
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Envía una solicitud HTTP genérica y deserializa la respuesta JSON.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto esperado en la respuesta.</typeparam>
        /// <param name="method">El método HTTP (GET, POST, PUT, DELETE).</param>
        /// <param name="endpoint">El endpoint específico de la API (ej. "users", "residentes/5").</param>
        /// <param name="data">El objeto de datos a serializar en el cuerpo de la solicitud (para POST/PUT).</param>
        /// <returns>El objeto deserializado de la respuesta.</returns>
        /// <exception cref="HttpRequestException">Se lanza si la respuesta HTTP no es exitosa (código 2xx).</exception>
        private async Task<T> SendRequest<T>(HttpMethod method, string endpoint, object data = null)
        {
            var request = new HttpRequestMessage(method, $"{_baseAddress}/{endpoint}");

            if (data != null)
            {
                // Serializar el objeto de datos a JSON y asignarlo como contenido de la solicitud.
                request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            // Lanza una HttpRequestException si el código de estado HTTP no indica éxito (2xx).
            response.EnsureSuccessStatusCode();

            // Leer el contenido de la respuesta como una cadena JSON.
            var jsonResponse = await response.Content.ReadAsStringAsync();
            // Deserializar la cadena JSON al tipo de objeto esperado.
            // PropertyNameCaseInsensitive = true permite que las propiedades JSON (camelCase)
            // se mapeen a propiedades C# (PascalCase) sin problemas.
            return JsonSerializer.Deserialize<T>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// Realiza una petición GET asíncrona a un endpoint de la API.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto esperado en la respuesta.</typeparam>
        /// <param name="endpoint">El endpoint específico de la API.</param>
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
        /// <param name="data">El objeto de datos a enviar en el cuerpo de la solicitud.</param>
        public async Task PutAsync(string endpoint, object data)
        {
            await SendRequest<object>(HttpMethod.Put, endpoint, data);
        }

        /// <summary>
        /// Realiza una petición DELETE asíncrona a un endpoint de la API.
        /// Este método no espera un valor de retorno específico (ej. 204 No Content).
        /// </summary>
        /// <param name="endpoint">El endpoint específico de la API.</param>
        public async Task DeleteAsync(string endpoint)
        {
            await SendRequest<object>(HttpMethod.Delete, endpoint);
        }
    }
}
