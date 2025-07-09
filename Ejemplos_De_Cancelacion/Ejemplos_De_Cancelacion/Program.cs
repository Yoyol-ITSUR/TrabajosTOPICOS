using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ejemplos_De_Cancelacion
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Demo: Cancelación básica ===");
            // Muestra cómo una tarea puede monitorear un token de cancelación y detenerse elegantemente.
            DemoBasicCancellation();
            await Task.Delay(1000); 

            Console.WriteLine("\n=== Demo: Cancelación de objetos ===");
            // Demuestra cómo registrar callbacks en un token de cancelación para ejecutar lógica personalizada
            // cuando se solicita la cancelación, afectando a múltiples objetos.
            DemoObjectCancellation();
            await Task.Delay(1000);

            Console.WriteLine("\n=== Demo: Escucha por polling ===");
            // Ilustra cómo una operación de larga duración puede verificar periódicamente
            // si se ha solicitado una cancelación.
            DemoPolling();
            await Task.Delay(2000);

            Console.WriteLine("\n=== Demo: Callback de cancelación ===");
            // Muestra cómo usar un callback para cancelar una operación externa (como una petición HTTP)
            // cuando el token de cancelación es activado.
            await DemoRegisterCallback();
            await Task.Delay(1000);

            Console.WriteLine("\n=== Demo: Escucha con WaitHandle ===");
            // Demuestra cómo un hilo puede esperar por la señal de un evento o por una solicitud de cancelación,
            // utilizando WaitHandle.
            DemoWaitHandle();
            await Task.Delay(2000);

            Console.WriteLine("\n=== Demo: Tokens enlazados ===");
            // Presenta cómo combinar múltiples tokens de cancelación en uno solo,
            // de modo que la cancelación de cualquiera de los tokens originales
            // cancele el token enlazado.
            DemoLinkedTokens();

            Console.WriteLine("\nTodas las demostraciones han finalizado. Presiona cualquier tecla para salir.");
            Console.ReadKey();
        }

        // Demuestra el mecanismo básico de cancelación.
        // Inicia una tarea en un ThreadPool que monitorea un CancellationToken.
        // Luego, solicita la cancelación y observa cómo la tarea responde.
        static void DemoBasicCancellation()
        {
            // CancellationTokenSource es el objeto que gestiona y envía la señal de cancelación.
            var cancellationSource = new CancellationTokenSource();
            // CancellationToken es el objeto que la tarea monitorea para detectar la cancelación.
            var cancellationToken = cancellationSource.Token;

            Console.WriteLine("Iniciando tarea de trabajo en segundo plano...");
            // Encolamos un método para que se ejecute en un hilo del ThreadPool,
            // pasándole el token de cancelación.
            ThreadPool.QueueUserWorkItem(DoSomeWork, cancellationToken);

            // Simulamos un retraso antes de solicitar la cancelación.
            Thread.Sleep(500);
            // Solicitamos la cancelación. Esto activa el token.
            cancellationSource.Cancel();
            Console.WriteLine("Solicitud de cancelación enviada a la tarea.");
            Thread.Sleep(2500); // Damos tiempo a la tarea para que detecte y responda a la cancelación.

            // Es importante liberar los recursos del CancellationTokenSource.
            cancellationSource.Dispose();
            Console.WriteLine("Demostración de cancelación básica finalizada.");
        }

        // Simula una operación de trabajo que se ejecuta en un hilo separado.
        // Periódicamente, verifica si se ha solicitado una cancelación a través del token.
        static void DoSomeWork(object state)
        {
            // Se asegura de que el objeto de estado sea un CancellationToken.
            if (state is CancellationToken cancellationRequestToken)
            {
                Console.WriteLine("  - Tarea de trabajo iniciada. Realizando cálculos intensivos...");
                // Bucle que simula una operación de larga duración.
                for (int iterationCount = 0; iterationCount < 100000; iterationCount++)
                {
                    // Verifica si se ha solicitado la cancelación.
                    if (cancellationRequestToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"  - Iteración {iterationCount + 1}: ¡Solicitud de cancelación detectada! Deteniendo la operación.");
                        break; // Sale del bucle para detener la operación.
                    }
                    // Simula trabajo intensivo que consume CPU.
                    Thread.SpinWait(500000);
                }
                if (!cancellationRequestToken.IsCancellationRequested)
                {
                    Console.WriteLine("  - Tarea de trabajo completada sin interrupción.");
                }
            }
        }

        // Clase de ejemplo que representa un objeto que puede ser cancelado.
        // Contiene un identificador y un método 'Cancel' que se invoca al cancelar.
        class CancelableObject
        {
            public string ObjectId { get; } // Propiedad para almacenar el identificador del objeto.

            // Constructor para inicializar una nueva instancia de CancelableObject.
            public CancelableObject(string id) => ObjectId = id;

            // Método que se ejecuta cuando se solicita la cancelación de este objeto.
            // Simula la limpieza o el manejo de la cancelación para este objeto específico.
            public void Cancel() => Console.WriteLine($"  > Objeto {ObjectId}: Método Cancel() invocado.");
        }

        // Demuestra cómo registrar múltiples callbacks en un único CancellationToken.
        // Cuando el token se cancela, todos los callbacks registrados se ejecutan.
        static void DemoObjectCancellation()
        {
            var cancellationSource = new CancellationTokenSource();
            var cancellationToken = cancellationSource.Token;

            Console.WriteLine("Registrando múltiples callbacks en el token de cancelación...");

            // Creamos varias instancias de objetos cancelables.
            var objectInstance1 = new CancelableObject("Instancia A");
            var objectInstance2 = new CancelableObject("Instancia B");
            var objectInstance3 = new CancelableObject("Instancia C");

            // Registramos un método de cada objeto como callback.
            // Cuando el 'cancellationToken' se cancele, estos métodos serán invocados.
            cancellationToken.Register(() => objectInstance1.Cancel());
            cancellationToken.Register(() => objectInstance2.Cancel());
            cancellationToken.Register(() => objectInstance3.Cancel());

            Console.WriteLine("Callbacks registrados. Solicitando cancelación...");
            // Solicitamos la cancelación, lo que dispara todos los callbacks registrados.
            cancellationSource.Cancel();
            Console.WriteLine("Solicitud de cancelación enviada. Verifique la salida de los callbacks.");

            // Es buena práctica disponer del CancellationTokenSource cuando ya no se necesita.
            cancellationSource.Dispose();
            Console.WriteLine("Demostración de cancelación de objetos finalizada.");
        }

        // Estructura simple para representar dimensiones de una cuadrícula.
        // Utilizada en la demostración de polling.
        struct GridDimensions { public int Columns, Rows; public GridDimensions(int c, int r) { Columns = c; Rows = r; } }

        // Demuestra la técnica de "polling" (sondeo) para la cancelación.
        // Una tarea de fondo inicia una operación de bucle anidado y, después de un tiempo,
        // solicita la cancelación, la cual es verificada periódicamente por el bucle.
        static void DemoPolling()
        {
            var gridDimensions = new GridDimensions(4, 4); // Definimos una cuadrícula de 4x4 para la demostración.
            var cancellationSource = new CancellationTokenSource();
            var cancellationToken = cancellationSource.Token;

            Console.WriteLine("Iniciando tarea de cancelación automática en 1 segundo...");
            // Iniciamos una tarea que cancelará el token después de 1 segundo.
            Task.Run(async () => {
                await Task.Delay(1000); // Espera 1 segundo.
                cancellationSource.Cancel(); // Solicita la cancelación.
                Console.WriteLine("\n  >> Tarea de cancelación automática ha solicitado la cancelación.");
            });

            Console.WriteLine("Iniciando operación de bucles anidados con sondeo de cancelación...");
            // Llamamos al método que contiene los bucles anidados, pasándole el token.
            NestedLoops(gridDimensions, cancellationToken);

            cancellationSource.Dispose();
            Console.WriteLine("Demostración de sondeo de cancelación finalizada.");
        }

        // Realiza una operación de bucles anidados que simula trabajo.
        // Utiliza la técnica de "polling" (sondeo) para verificar si se ha solicitado una cancelación.
        static void NestedLoops(GridDimensions gridRectangle, CancellationToken cancellationToken)
        {
            // El bucle exterior verifica el token de cancelación en cada iteración.
            for (int colIndex = 0; colIndex < gridRectangle.Columns && !cancellationToken.IsCancellationRequested; colIndex++)
            {
                for (int rowIndex = 0; rowIndex < gridRectangle.Rows; rowIndex++)
                {
                    // Simula una pequeña cantidad de trabajo en cada iteración interna.
                    Thread.SpinWait(50000);
                    Console.Write($"{colIndex},{rowIndex} "); // Imprime las coordenadas actuales.
                }
                Console.WriteLine(); // Nueva línea después de cada columna.
            }
            // Después de los bucles, verifica si la operación fue cancelada o completada.
            if (cancellationToken.IsCancellationRequested)
                Console.WriteLine("  >> Operación de bucles anidados fue cancelada.");
            else
                Console.WriteLine("  >> Operación de bucles anidados completada sin cancelación.");
        }

        // Demuestra cómo registrar un callback para cancelar una operación de HttpClient.
        // El callback se ejecuta cuando el CancellationTokenSource se cancela,
        // permitiendo cancelar peticiones HTTP pendientes.
        static async Task DemoRegisterCallback()
        {
            var cancellationSource = new CancellationTokenSource();
            var httpClient = new HttpClient(); // Cliente HTTP para realizar peticiones.

            Console.WriteLine("Registrando callback para cancelar peticiones HTTP...");
            // Registramos un callback que se ejecutará cuando 'cancellationSource' se cancele.
            // Este callback cancelará cualquier petición pendiente del httpClient.
            cancellationSource.Token.Register(() =>
            {
                httpClient.CancelPendingRequests();
                Console.WriteLine("  >> Callback: Petición HTTP cancelada via CancellationToken.");
            });

            Console.WriteLine("Iniciando petición HTTP asíncrona a un sitio web de ejemplo...");
            // Intentamos obtener una cadena de una URL.
            // Usamos un sitio que probablemente no exista o tarde en responder para simular una operación larga.
            var httpRequestTask = httpClient.GetStringAsync("http://www.ejemplo-sitio-largo-tiempo.com/data");

            // Solicitamos la cancelación del CancellationTokenSource después de 100 milisegundos.
            // Esto activará el callback registrado.
            cancellationSource.CancelAfter(100);

            try
            {
                // Esperamos a que la petición HTTP finalice.
                string responseContent = await httpRequestTask;
                Console.WriteLine($"  - Petición HTTP completada. Longitud de respuesta: {responseContent.Length}");
            }
            catch (TaskCanceledException)
            {
                // Capturamos la excepción que se lanza cuando la tarea es cancelada.
                Console.WriteLine("  >> TaskCanceledException capturada: La petición HTTP fue cancelada.");
            }
            catch (Exception ex)
            {
                // Capturamos otras excepciones (ej. si el sitio no existe o hay problemas de red).
                Console.WriteLine($"  >> Error inesperado al realizar la petición HTTP: {ex.Message}");
            }
            finally
            {
                // Es importante disponer del HttpClient y el CancellationTokenSource.
                httpClient.Dispose();
                cancellationSource.Dispose();
                Console.WriteLine("Demostración de callback de cancelación finalizada.");
            }
        }

        // Demuestra cómo usar WaitHandle para esperar por una señal de evento
        // o por una solicitud de cancelación de un token.
        static void DemoWaitHandle()
        {
            // ManualResetEventSlim es un evento ligero que puede ser señalado.
            var manualResetEvent = new ManualResetEventSlim(false); // Inicialmente no señalado.
            // CancellationTokenSource que se cancelará después de un tiempo específico.
            var cancellationSource = new CancellationTokenSource(1000); // Se cancela después de 1 segundo.

            Console.WriteLine("Esperando que un evento sea señalado o que la cancelación ocurra...");

            // WaitHandle.WaitAny espera por cualquiera de los WaitHandles proporcionados.
            // Retorna el índice del handle que fue señalado o que causó la terminación.
            int signaledIndex = WaitHandle.WaitAny(
                new WaitHandle[] { manualResetEvent.WaitHandle, cancellationSource.Token.WaitHandle },
                TimeSpan.FromSeconds(2) // Tiempo máximo de espera.
            );

            // Evaluamos cuál de los handles fue señalado o si el tiempo de espera se agotó.
            switch (signaledIndex)
            {
                case 0:
                    Console.WriteLine("  - ManualResetEvent fue señalado: La operación continuaría por evento externo.");
                    break;
                case 1:
                    Console.WriteLine("  - CancellationToken fue cancelado: La operación fue detenida por solicitud de cancelación.");
                    break;
                default:
                    Console.WriteLine("  - Tiempo de espera agotado: Ningún evento fue señalado ni se solicitó cancelación a tiempo.");
                    break;
            }

            // Liberamos los recursos.
            manualResetEvent.Dispose();
            cancellationSource.Dispose();
            Console.WriteLine("Demostración de escucha con WaitHandle finalizada.");
        }

        // Demuestra cómo crear un CancellationTokenSource enlazado a múltiples tokens.
        // Si cualquiera de los tokens fuente se cancela, el token enlazado también se cancela.
        // Esto es útil para combinar múltiples condiciones de cancelación (ej. timeout interno y cancelación externa).
        static void DemoLinkedTokens()
        {
            // CancellationTokenSource para un timeout interno (ej. la operación no debe durar más de 500ms).
            var internalCancellationSource = new CancellationTokenSource(500);
            // CancellationTokenSource para una cancelación externa (ej. el usuario presiona un botón de cancelar).
            var externalCancellationSource = new CancellationTokenSource(1000); // Se cancela después de 1 segundo.

            Console.WriteLine("Creando un token enlazado a un timeout interno y una cancelación externa...");
            // Creamos un CancellationTokenSource que se cancelará si 'internalCts.Token'
            // o 'externalCts.Token' se cancelan.
            var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
                                                internalCancellationSource.Token, externalCancellationSource.Token);

            try
            {
                Console.WriteLine("Iniciando trabajo interno que monitorea el token enlazado...");
                // Pasamos el token enlazado a la operación que queremos cancelar.
                DoWorkInternal(linkedCancellationSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Capturamos la excepción de cancelación.
                // Luego, verificamos qué token fue el que realmente causó la cancelación.
                if (internalCancellationSource.IsCancellationRequested)
                    Console.WriteLine("  >> Operación finalizada: Cancelada por timeout interno.");
                else if (externalCancellationSource.IsCancellationRequested)
                    Console.WriteLine("  >> Operación finalizada: Cancelación solicitada externamente.");
                else
                    Console.WriteLine("  >> Operación finalizada: Cancelación por causa desconocida en el token enlazado.");
            }
            finally
            {
                // Liberamos los recursos de todos los CancellationTokenSources.
                internalCancellationSource.Dispose();
                externalCancellationSource.Dispose();
                linkedCancellationSource.Dispose();
                Console.WriteLine("Demostración de tokens enlazados finalizada.");
            }
        }

        // Simula una operación de trabajo que lanza una OperationCanceledException
        // si se solicita la cancelación a través del token.
        static void DoWorkInternal(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 5; i++)
            {
                // Lanza una OperationCanceledException si se ha solicitado la cancelación.
                // Esta es una forma común de propagar la cancelación.
                cancellationToken.ThrowIfCancellationRequested();
                Console.WriteLine($"  - Trabajando en la iteración {i + 1}...");
                Thread.Sleep(300); // Simula trabajo.
            }
            Console.WriteLine("  >> Trabajo completado sin cancelación.");
        }
    }
}
