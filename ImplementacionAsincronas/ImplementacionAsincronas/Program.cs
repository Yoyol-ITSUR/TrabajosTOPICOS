using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text.Json;
using System.Threading.Tasks;

public class Program { 

    public static async Task Main()
    {
        // El primer ejemplo que nos muestran en la practica
        string direccionArchivo = "ejemplo.txt";
        string content = await ReadFileAsync(direccionArchivo);
        Console.WriteLine(content);

        await Task.Delay(2000);
        // Pasamos al segundo ejemplo sobre las async task
        string dirPath = @"C:\TempDir";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        string nombreArchivo = "accout.json";
        string nombreDir = Path.Combine(dirPath, nombreArchivo);

        Account cuenta = new Account { Name = "Elize Harmsen", Balance = 1000.00m };

        // Ahora guardamos la cuenta con el metodo asicrono
        SaveAccountDataAsync(nombreDir, cuenta);

        // Y Cargamos para probar el metodo asincrono
        Account cuentaCargada = await LoadAccountDataAsync(nombreDir);
        Console.WriteLine($"Nombre: {cuentaCargada.Name}, Balance: {cuentaCargada.Balance}");
        await Task.Delay(2000);

        //Pasamos al tercer ejemplo
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // URL de un ejemplo
                string url = "https://petstore.swagger.io/v2/pet/findByStatus?status=available";
                HttpResponseMessage respuesta = await client.GetAsync(url);
                respuesta.EnsureSuccessStatusCode();
                string responseBody = await respuesta.Content.ReadAsStringAsync();
                Console.WriteLine("\nRespuesta del tercer ejemplo: \n"+ responseBody);

                // Deserializamos la respuesta json, e imprimimos los detalles
                var pets = JsonSerializer.Deserialize<List<Pet>>(responseBody);
                foreach (var pet in pets)
                {
                    Console.WriteLine($"Pet ID: {pet.id}, Nombre: {pet.name}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error de peticion: {e.Message}");
            }
        }
        await Task.Delay(2000);


        // Cuarto ejemplo de la practica
        Console.WriteLine("\nEntramos al cuarto ejemplo");
        var results = new ConcurrentBag<int>();
        Parallel.For(0, 100, i =>
        {
            Task.Delay(100).Wait();
            results.Add(i);
        });
        Console.WriteLine($"Items procesados en paralelo: {results.Count}");

        var urls = new List<string>
        {
            "https://example.com",
            "https://example.org",
            "https://example.net"
        };

        var tasks = new List<Task<string>>();
        foreach (var url in urls)
        {
            tasks.Add(FetchDataAsync(url));
        }

        //esperamos a que las tareas se terminen
        var results1 = await Task.WhenAll(tasks);

        foreach (var result in results1) {
            Console.WriteLine(result);
        }
        await Task.Delay(2000);

        try
        {
            TraverseTreeParallelForEach(@"C:\Program Files", (f) =>
            {
                try
                {
                    // solamente leeremos, nada mas
                    byte[] data = File.ReadAllBytes(f);
                }
                catch (FileNotFoundException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (SecurityException) { }

                Console.WriteLine(f);
            });
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(@"El directorio 'C:\Program Files' no existe.");
        }
        await Task.Delay(2000);

        // Vamos por el 5to ejemplo
        Console.WriteLine("5to ejemplo:");
        Console.WriteLine("Primera parte de manejar 3: ");
        HandleThree();

        Console.WriteLine("Segundo ejemplo de manera 4: ");
        HandleFour();
        await Task.Delay(2000);



    }

    public static async Task<string> ReadFileAsync(string filePath) {
        using (StreamReader reader = new StreamReader(filePath))
        {
            string content = await reader.ReadToEndAsync();
            return content;
        }
    }

    public static async Task SaveAccountDataAsync(string filePath, Account account)
    {
        string jsonString = JsonSerializer.Serialize(account);
        await File.WriteAllTextAsync(filePath, jsonString);
    }

    public static async Task<Account> LoadAccountDataAsync(string filePath)
    {
        string jsonString = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<Account>(jsonString);
    }

    public static async Task<string> FetchDataAsync(string url)
    {
        using (var client = new HttpClient())
        {
            return await client.GetStringAsync(url);
        }
    }

    public static void TraverseTreeParallelForEach(string root, Action<string> action) {
        int filecount = 0;
        var sw = Stopwatch.StartNew();

        int procCount = Environment.ProcessorCount;

        Stack<string> dirs = new Stack<string>();

        if (!Directory.Exists(root))
        {
            throw new ArgumentException(
                "La ruta madre no existe", nameof(root));
        }
        dirs.Push(root);

        while ( dirs.Count > 0)
        {
            string currentDir = dirs.Pop();
            string[] subDirs = { };
            string[] files = { };

            try {
                subDirs = Directory.GetDirectories(currentDir);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                continue;
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            try
            {
                files = Directory.GetFiles(currentDir);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                continue;
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                continue;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            // Ahora se hara una ejecución en paralelo
            try
            {
                if (files.Length < procCount)
                {
                    foreach (var file in files)
                    {
                        action(file);
                        filecount++;
                    }
                }
                else
                {
                    Parallel.ForEach(files, () => 0, (file, loopState, localcount) => {
                        action(file);
                        return (int)++localcount;
                    }, (c) =>
                    {
                        Interlocked.Add(ref filecount, c);
                    });
                }
            }
            catch (AggregateException ae)
            {
                ae.Handle((ex) =>
                {
                    if (ex is UnauthorizedAccessException)
                    {
                        Console.WriteLine(ex);
                        return true;
                    }
                    return false;
                });
            }

            foreach (string str in subDirs)
            {
                dirs.Push(str);
            }
        }

        Console.WriteLine($"{filecount} archivos procesados en {sw.ElapsedMilliseconds} milisegundos");
    }

    public static void HandleThree()
    {
        var task = Task.Run(() => throw new CustomException("Esta exepcion fue esperada"));

        try
        {
            task.Wait();
        }
        catch (AggregateException ae)
        {
            foreach (var ex in ae.InnerExceptions)
            {
                if (ex is CustomException)
                {
                    Console.WriteLine(ex.Message);
                }
                else
                {
                    throw ex;
                }
            }
        }
    }

    public static void HandleFour()
    {
        var task = Task.Run(() => throw new CustomException("Esta exepcion fue esperada"));

        try
        {
            task.Wait();
        }
        catch (AggregateException ae)
        {

            ae.Handle(ex =>
            {
                if (ex is CustomException)
                {
                    Console.WriteLine(ex.Message);
                    return true;
                }
                return false;
            });
        }
    }
}

// Clase para el segundo ejemplo
public class Account
{
    public string Name { get; set; }
    public decimal Balance { get; set; }
}


// Clases para el tercer ejemplo
public class Pet
{
    public long id { get; set; }
    public string name { get; set; }
    public Category category { get; set; }
    public List<string> photoUrls { get; set; }
    public List<Tag> tags { get; set; }
    public string status { get; set; }
}
public class Category
{
    public long id { get; set; }
    public string name { get; set; }
}
public class Tag
{
    public long id { get; set; }
    public string name { get; set; }
}

// Clases para el ejemplo 5
public class CustomException : Exception
{
    public CustomException(string message) : base(message) { }
}