using E_ETL_electiva1.Entities.interfaces.Iservices;

namespace E_ETL_electiva1.Process
{
    /// <summary>
    /// Orquesta la carga de las tres fuentes hacia AnaliticaOpinionesClientes en un solo ciclo
    /// batch. Orden deliberado: primero las fuentes que pueblan el catálogo de Productos con
    /// Nombre/Categoría reales (BD transaccional y API), y al final el CSV, que además de sus
    /// propias dimensiones (Clientes/Canales) carga el hecho Opiniones_Clientes.
    /// </summary>
    public class Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger, IHostApplicationLifetime lifetime) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbTransService = scope.ServiceProvider.GetRequiredService<ITransDbService>();
                var apiService = scope.ServiceProvider.GetRequiredService<IApiService>();
                var csvService = scope.ServiceProvider.GetRequiredService<ICsvService>();

                logger.LogInformation("Iniciando ciclo de extracción/carga: {time}", DateTimeOffset.Now);

                await EjecutarFuente("Base de datos transaccional", async () =>
                {
                    await dbTransService.upload_Clientes();
                    await dbTransService.upload_Productos();
                    await dbTransService.upload_Fuentes();
                }, logger);

                await EjecutarFuente("API REST", async () =>
                {
                    await apiService.upload_Clientes();
                    await apiService.upload_Productos();
                    await apiService.upload_Fuentes();
                    await apiService.upload_Redes();
                }, logger);

                await EjecutarFuente("CSV", async () =>
                {
                    await csvService.upload_Clientes();
                    await csvService.upload_Fuentes();
                    await csvService.upload_Productos();
                    await csvService.upload_Opiniones();
                }, logger);

                logger.LogInformation("Ciclo finalizado: {time}", DateTimeOffset.Now);
            }
            finally
            {
                // Proceso batch de una sola pasada: al terminar el ciclo, se detiene el host.
                lifetime.StopApplication();
            }
        }

        private static async Task EjecutarFuente(string nombre, Func<Task> accion, ILogger logger)
        {
            try
            {
                await accion();
                logger.LogInformation("Fuente {Fuente} procesada correctamente", nombre);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error procesando la fuente {Fuente}", nombre);
            }
        }
    }
}
