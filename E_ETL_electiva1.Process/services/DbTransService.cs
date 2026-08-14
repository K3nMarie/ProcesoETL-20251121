using E_ETL_electiva1.api.Models;
using E_ETL_electiva1.Data.context;
using E_ETL_electiva1.Entities.interfaces;
using E_ETL_electiva1.Entities.interfaces.Iservices;
using Microsoft.Extensions.Logging;

namespace E_ETL_electiva1.Process.services
{
    /// <summary>
    /// Fuente: base de datos transaccional del sistema de origen (opiniones_de_clientesDB,
    /// leída directamente vía EF Core a través de IDbReaderRepository&lt;T&gt;). A diferencia
    /// del CSV, aquí sí llegan Nombre y Categoría de cada producto, por lo que esta fuente
    /// (junto con la API) es la que realmente puebla el catálogo de Productos del DWH.
    /// </summary>
    internal class DbTransService(
        IDbReaderRepository<Clientes> clientesRepo,
        IDbReaderRepository<Productos> productosRepo,
        AnaliticalDbElectiva1Context analitica,
        ILogger<DbTransService> logger) : ITransDbService
    {
        private readonly DimensionResolver _resolver = new(analitica);

        public async Task<bool> upload_Clientes()
        {
            var clientes = await clientesRepo.GetAllAsync();

            foreach (var cliente in clientes)
            {
                if (cliente.IdCliente <= 0) continue;
                await _resolver.UpsertClienteAsync(DimensionResolver.HomologarIdCliente(cliente.IdCliente.ToString()));
            }

            return true;
        }

        public async Task<bool> upload_Productos()
        {
            var productos = await productosRepo.GetAllAsync();
            var cargados = 0;

            foreach (var producto in productos)
            {
                if (string.IsNullOrWhiteSpace(producto.Nombre)) continue;

                // Nota: IDbReaderRepository<T> es genérico y no hace Include() de navegaciones,
                // así que IdCategoriaNavigation normalmente llega null aquí; se usa un valor de
                // reserva para no bloquear la carga. Si se necesita la categoría real, conviene
                // sustituir este repositorio genérico por uno específico con .Include(p => p.IdCategoriaNavigation).
                var nombreCategoria = producto.IdCategoriaNavigation?.NombreCategoria ?? "Sin categoría";

                try
                {
                    await _resolver.UpsertProductoAsync(producto.Nombre, nombreCategoria);
                    cargados++;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "BD transaccional: no se pudo cargar el producto '{Nombre}'.", producto.Nombre);
                }
            }

            logger.LogInformation("BD transaccional: productos cargados={cargados}.", cargados);
            return true;
        }

        public async Task<bool> upload_Fuentes()
        {
            // En el sistema de origen, el "canal" declarado por cada dato viene dado por
            // Tipos_Fuente.NombreTipo (a través de Fuente_Datos). Como el repositorio genérico
            // solo expone una entidad por llamada, se homologa aquí con un valor representativo
            // fijo para no depender de un cuarto repositorio genérico solo para esto.
            await _resolver.UpsertCanalAsync("Sistema Transaccional");
            return true;
        }
    }
}
