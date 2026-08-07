using E_ETL_electiva1.Data.context;
using E_ETL_electiva1.Entities.interfaces;
using E_ETL_electiva1.Entities.interfaces.Iservices;
using Microsoft.Extensions.Logging;

namespace E_ETL_electiva1.Process.services
{
    /// <summary>
    /// Fuente: API REST del sistema transaccional de origen (proyecto E_ETL_electiva1.api),
    /// consumida vía apiRepo/IApiConsRepository. Solo comentarios en redes sociales (upload_Redes)
    /// llegan aquí sin equivalente en las otras dos fuentes.
    /// </summary>
    internal class apiService(
        IApiConsRepository apiConsRepository,
        AnaliticalDbElectiva1Context analitica,
        ILogger<apiService> logger) : IApiService
    {
        private readonly DimensionResolver _resolver = new(analitica);

        public async Task<bool> upload_Clientes()
        {
            var clientes = await apiConsRepository.GetClientes();

            foreach (var cliente in clientes)
            {
                if (string.IsNullOrWhiteSpace(cliente.IdCliente)) continue;
                await _resolver.UpsertClienteAsync(DimensionResolver.HomologarIdCliente(cliente.IdCliente));
            }

            return true;
        }

        public async Task<bool> upload_Productos()
        {
            var productos = await apiConsRepository.GetProductos();
            var cargados = 0;

            foreach (var producto in productos)
            {
                if (string.IsNullOrWhiteSpace(producto.Nombre)) continue;

                // El endpoint de Productos no serializa el nombre de la categoría (solo el
                // IdCategoria del sistema de origen, que no es comparable con el del DWH),
                // así que se agrupan bajo una categoría de reserva propia de esta fuente.
                try
                {
                    await _resolver.UpsertProductoAsync(producto.Nombre, "Sin categoría (API)");
                    cargados++;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "API: no se pudo cargar el producto '{Nombre}'.", producto.Nombre);
                }
            }

            logger.LogInformation("API: productos cargados={cargados}.", cargados);
            return true;
        }

        public async Task<bool> upload_Fuentes()
        {
            await _resolver.UpsertCanalAsync(apiConsRepository.GetFuentes());
            return true;
        }

        public async Task<bool> upload_Redes()
        {
            // En el esquema del DWH, Redes_Sociales no existe como dimensión aparte: quedó
            // unificada dentro de Canales_Origen junto con "Fuentes".
            var redes = await apiConsRepository.GetRedesSociales();

            foreach (var red in redes)
            {
                if (string.IsNullOrWhiteSpace(red.NombreRedSocial)) continue;
                await _resolver.UpsertCanalAsync(red.NombreRedSocial);
            }

            return true;
        }
    }
}
