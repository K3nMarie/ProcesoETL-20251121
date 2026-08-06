using E_ETL_electiva1.Data.context;
using E_ETL_electiva1.Data.Repositories;
using E_ETL_electiva1.Entities.interfaces;
using E_ETL_electiva1.Process;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// --- Fuente CSV (extracción) -------------------------------------------------
var csvRelativePath = builder.Configuration["Etl:CsvPath"] ?? "Csv/surveys_part1.csv";
var csvFullPath = Path.IsPathRooted(csvRelativePath)
    ? csvRelativePath
    : Path.Combine(AppContext.BaseDirectory, csvRelativePath);

builder.Services.AddSingleton<ICsvRepository>(_ => new CsvRepo(csvFullPath));

// --- Base analítica destino (carga) ------------------------------------------
var analiticaConnectionString = builder.Configuration.GetConnectionString("BdAnalitica")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'BdAnalitica' en appsettings.json.");

builder.Services.AddDbContext<AnaliticalDbElectiva1Context>(options =>
    options.UseSqlServer(analiticaConnectionString));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
