using E_ETL_electiva1.Data.context;
using E_ETL_electiva1.Entities.interfaces;
using E_ETL_electiva1.Entities.Models.csv;

namespace E_ETL_electiva1.Process
{
    /// <summary>
    /// Proceso ETL de "Opiniones de Clientes" adaptado al esquema del Project:
    ///   - Extract : archivo CSV de encuestas (E_ETL_electiva1.Data/Csv/surveys_part1.csv).
    ///   - Transform: homologa IdCliente a VARCHAR(6) y resuelve/crea el Canal de Origen
    ///                (antes "Fuente") a partir del texto libre del CSV.
    ///   - Load    : inserta en la base analítica AnaliticaOpinionesClientes usando los
    ///                stored procedures sp_upsert_canal, sp_upsert_cliente y sp_upsert_opinion
    ///                definidos en Project/SistemaAnalisisOpinionesClientes/DBAnalitica.
    ///
    /// Nota: los Productos (Nombre + Categoría) se asumen precargados por el proceso de datos
    /// maestros del sistema transaccional; este ETL solo referencia su IdProducto. Si un
    /// producto del CSV aún no existe en la base analítica, la fila se registra como error y
    /// se continúa con el resto (no detiene la corrida completa).
    /// </summary>
    public class Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory,
        ICsvRepository csvRepository,
        IHostApplicationLifetime lifetime) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await RunEtlAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Proceso ETL cancelado.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "El proceso ETL finalizó con un error no controlado.");
            }
            finally
            {
                // Es un proceso batch de una sola pasada: al terminar, se detiene el host.
                lifetime.StopApplication();
            }
        }

        private async Task RunEtlAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Iniciando ETL de Opiniones de Clientes {time}", DateTimeOffset.Now);

            var encuestas = csvRepository.GetAll().ToList();
            logger.LogInformation("Se leyeron {count} registros del CSV de encuestas.", encuestas.Count);

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AnaliticalDbElectiva1Context>();
            var procedures = context.GetProcedures();

            // Cachés en memoria para no re-consultar el mismo canal/cliente varias veces en la corrida.
            var canalCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var clientesResueltos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var cargadas = 0;
            var conError = 0;

            foreach (var encuesta in encuestas)
            {
                stoppingToken.ThrowIfCancellationRequested();

                try
                {
                    var idCanal = await ResolverCanalAsync(procedures, canalCache, encuesta.Fuente, stoppingToken);
                    var idClienteAnalitico = HomologarIdCliente(encuesta.IdCliente);

                    await AsegurarClienteAsync(procedures, clientesResueltos, idClienteAnalitico, stoppingToken);

                    await procedures.sp_upsert_opinionAsync(
                        idOpinion: null,
                        idProducto: encuesta.IdProducto,
                        idCliente: idClienteAnalitico,
                        idCanal: idCanal,
                        fechaOpinion: encuesta.Fecha,
                        puntajeSatisfaccion: (byte)encuesta.PuntajeSatisfaccion,
                        cancellationToken: stoppingToken);

                    cargadas++;
                }
                catch (Exception ex)
                {
                    conError++;
                    logger.LogWarning(ex,
                        "No se pudo cargar la opinión IdOpinion={IdOpinion} (IdProducto={IdProducto}). Se omite y se continúa.",
                        encuesta.IdOpinion, encuesta.IdProducto);
                }
            }

            logger.LogInformation(
                "ETL finalizado. Registros cargados: {cargadas}. Registros con error: {conError}.",
                cargadas, conError);
        }

        /// <summary>
        /// La base analítica no distingue "Fuente" de "Red Social": ambas quedaron unificadas en
        /// la dimensión Canales_Origen. Se upsertea el nombre tal como viene en el CSV.
        /// </summary>
        private static async Task<int> ResolverCanalAsync(
            IAnaliticalDbElectiva1ContextProcedures procedures,
            Dictionary<string, int> cache,
            string nombreCanal,
            CancellationToken ct)
        {
            if (cache.TryGetValue(nombreCanal, out var idCanalCacheado))
                return idCanalCacheado;

            var salida = new E_ETL_electiva1.Data.context.OutputParameter<int>();
            await procedures.sp_upsert_canalAsync(nombreCanal, salida, ct);

            var idCanal = salida.Value;
            cache[nombreCanal] = idCanal;
            return idCanal;
        }

        private static async Task AsegurarClienteAsync(
            IAnaliticalDbElectiva1ContextProcedures procedures,
            HashSet<string> clientesResueltos,
            string idCliente,
            CancellationToken ct)
        {
            if (clientesResueltos.Contains(idCliente))
                return;

            await procedures.sp_upsert_clienteAsync(idCliente, cancellationToken: ct);
            clientesResueltos.Add(idCliente);
        }

        /// <summary>
        /// Clientes.IdCliente en la base analítica es VARCHAR(6); el CSV trae un identificador
        /// numérico entero que puede tener más de 6 dígitos, por lo que se homologa tomando los
        /// últimos 6 dígitos (o rellenando con ceros a la izquierda si tiene menos).
        /// </summary>
        private static string HomologarIdCliente(int idCliente)
        {
            var texto = idCliente.ToString();
            return texto.Length > 6 ? texto[^6..] : texto.PadLeft(6, '0');
        }
    }
}
