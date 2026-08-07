using E_ETL_electiva1.api.context;
using E_ETL_electiva1.Data.context;
using E_ETL_electiva1.Data.Repositories;
using E_ETL_electiva1.Entities.interfaces;
using E_ETL_electiva1.Entities.interfaces.Iservices;
using E_ETL_electiva1.Process;
using E_ETL_electiva1.Process.services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// --- Fuente CSV (encuestas internas) -----------------------------------------
var csvRelativePath = builder.Configuration["Etl:CsvPath"] ?? "Csv/surveys_part1.csv";
var csvFullPath = Path.IsPathRooted(csvRelativePath)
    ? csvRelativePath
    : Path.Combine(AppContext.BaseDirectory, csvRelativePath);

builder.Services.AddSingleton<ICsvRepository>(_ => new CsvRepo(csvFullPath));

// --- Fuente base de datos transaccional (origen) ------------------------------
var connStringTrans = builder.Configuration.GetConnectionString("BdTransaccional")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'BdTransaccional' en appsettings.json.");

builder.Services.AddDbContext<opiniones_de_clientesDBContext>(options =>
    options.UseSqlServer(connStringTrans));

builder.Services.AddScoped(typeof(IDbReaderRepository<>), typeof(TransDbRepo<>));

// --- Fuente API REST (misma base de origen, expuesta como servicio) ----------
var apiBaseUrl = builder.Configuration["Etl:ApiBaseUrl"] ?? "https://localhost:7163/";

builder.Services.AddHttpClient<IApiConsRepository, apiRepo>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// --- Base analítica destino (carga) -------------------------------------------
var connStringAnalitica = builder.Configuration.GetConnectionString("BdAnalitica")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'BdAnalitica' en appsettings.json.");

builder.Services.AddDbContext<AnaliticalDbElectiva1Context>(options =>
    options.UseSqlServer(connStringAnalitica));

// --- Servicios de carga por fuente --------------------------------------------
builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddScoped<ITransDbService, DbTransService>();
builder.Services.AddScoped<IApiService, apiService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

try
{
    Console.WriteLine("=== Proceso ETL: Opiniones de Clientes ===");
    Console.WriteLine($"CSV: {csvFullPath}");
    Console.WriteLine($"BD transaccional: {connStringTrans}");
    Console.WriteLine($"BD analítica:     {connStringAnalitica}");
    Console.WriteLine("-------------------------------------------");

    await host.RunAsync();

    Console.WriteLine("-------------------------------------------");
    Console.WriteLine("Proceso finalizado. Revisa los mensajes anteriores para ver qué se cargó.");
}
catch (Exception ex)
{
    Console.WriteLine("-------------------------------------------");
    Console.WriteLine("El proceso terminó con un error antes de completar la carga:");
    Console.WriteLine(ex);
}
finally
{
    // Evita que la ventana se cierre sola al terminar (p. ej. al ejecutar el .exe con doble clic).
    if (Environment.UserInteractive)
    {
        Console.WriteLine();
        Console.WriteLine("Presiona una tecla para cerrar...");
        Console.ReadKey();
    }
}
