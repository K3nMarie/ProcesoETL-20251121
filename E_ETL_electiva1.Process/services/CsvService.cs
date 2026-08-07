using E_ETL_electiva1.Data.context;
using E_ETL_electiva1.Entities.interfaces;
using E_ETL_electiva1.Entities.interfaces.Iservices;
using E_ETL_electiva1.Entities.Models.csv;
using Microsoft.Extensions.Logging;

namespace E_ETL_electiva1.Process.services
{
    /// <summary>
    /// Fuente: archivo CSV de encuestas internas de satisfacción (surveys_part1.csv).
    /// Solo sube las dimensiones que el CSV puede sustentar por sí mismo (Clientes, Canales)
    /// y, a diferencia de las otras dos fuentes, también carga el hecho Opiniones_Clientes.
    /// </summary>
    internal class CsvService(ICsvRepository csvRepository, AnaliticalDbElectiva1Context analitica, ILogger<CsvService> logger) : ICsvService
    {
        private readonly DimensionResolver _resolver = new(analitica);
        private readonly IAnaliticalDbElectiva1ContextProcedures _procedures = analitica.GetProcedures();

        public async Task<bool> upload_Clientes()
        {
            var idsClientes = csvRepository.GetAll()
                .Select(e => e.IdCliente.ToString())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct();

            foreach (var idCliente in idsClientes)
                await _resolver.UpsertClienteAsync(DimensionResolver.HomologarIdCliente(idCliente));

            return true;
        }

        public Task<bool> upload_Productos()
        {
            // El CSV solo trae el IdProducto de origen, sin Nombre ni Categoría, por lo que
            // no puede originar una fila válida en Productos (Nombre/IdCategoria son NOT NULL
            // y el id es IDENTITY en el DWH). Se asume que TransDbService/apiService ya
            // cargaron el catálogo de productos antes de que corra esta fuente.
            logger.LogInformation("CSV: se omite la carga de Productos (el archivo no trae Nombre/Categoría).");
            return Task.FromResult(true);
        }

        public async Task<bool> upload_Fuentes()
        {
            var fuentes = csvRepository.GetAll()
                .Select(e => e.Fuente)
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .Distinct();

            foreach (var fuente in fuentes)
                await _resolver.UpsertCanalAsync(fuente);

            return true;
        }

        public async Task<bool> upload_Opiniones()
        {
            var canalCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var cargadas = 0;
            var conError = 0;

            foreach (surveys_part1 encuesta in csvRepository.GetAll())
            {
                try
                {
                    var idCliente = DimensionResolver.HomologarIdCliente(encuesta.IdCliente.ToString());
                    await _resolver.UpsertClienteAsync(idCliente);

                    if (!canalCache.TryGetValue(encuesta.Fuente, out var idCanal))
                    {
                        idCanal = await _resolver.UpsertCanalAsync(encuesta.Fuente);
                        canalCache[encuesta.Fuente] = idCanal;
                    }

                    await _procedures.sp_upsert_opinionAsync(
                        idOpinion: null,
                        idProducto: encuesta.IdProducto,
                        idCliente: idCliente,
                        idCanal: idCanal,
                        fechaOpinion: encuesta.Fecha,
                        puntajeSatisfaccion: (byte)encuesta.PuntajeSatisfaccion);

                    cargadas++;
                }
                catch (Exception ex)
                {
                    conError++;
                    logger.LogWarning(ex,
                        "CSV: no se pudo cargar la opinión IdOpinion={IdOpinion} (IdProducto={IdProducto}). Se omite.",
                        encuesta.IdOpinion, encuesta.IdProducto);
                }
            }

            logger.LogInformation("CSV: opiniones cargadas={cargadas}, con error={conError}.", cargadas, conError);
            return true;
        }
    }
}
