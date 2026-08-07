using E_ETL_electiva1.Data.context;
using Microsoft.EntityFrameworkCore;

namespace E_ETL_electiva1.Process.services
{
    /// <summary>
    /// Punto único para resolver/crear filas de dimensión en AnaliticaOpinionesClientes
    /// (Clientes, Canales_Origen, Categorias_Producto, Productos) a partir de cualquiera
    /// de las tres fuentes (CSV, base transaccional, API REST). Encapsula el patrón
    /// "upsert vía stored procedure + relectura por nombre" que exige el esquema del
    /// Project, ya que los sp_upsert_* no devuelven el id generado por OUTPUT.
    /// </summary>
    internal sealed class DimensionResolver(AnaliticalDbElectiva1Context analitica)
    {
        private readonly IAnaliticalDbElectiva1ContextProcedures _procedures = analitica.GetProcedures();

        public async Task UpsertClienteAsync(string idCliente, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(idCliente)) return;
            await _procedures.sp_upsert_clienteAsync(idCliente, cancellationToken: ct);
        }

        public async Task<int> UpsertCanalAsync(string nombreCanal, CancellationToken ct = default)
        {
            await _procedures.sp_upsert_canalAsync(nombreCanal, cancellationToken: ct);

            return await analitica.Fuentes
                .Where(f => f.NombreCanal == nombreCanal)
                .Select(f => f.IdCanal)
                .SingleAsync(ct);
        }

        public async Task<int> UpsertCategoriaAsync(string nombreCategoria, CancellationToken ct = default)
        {
            await _procedures.sp_upsert_categoriaAsync(null, nombreCategoria, cancellationToken: ct);

            return await analitica.CategoriasProducto
                .Where(c => c.NombreCategoria == nombreCategoria)
                .Select(c => c.IdCategoria)
                .SingleAsync(ct);
        }

        public async Task UpsertProductoAsync(string nombre, string nombreCategoria, CancellationToken ct = default)
        {
            var idCategoria = await UpsertCategoriaAsync(nombreCategoria, ct);
            // Sin un id externo confiable (Productos.IdProducto es IDENTITY en el DWH), el
            // upsert de producto solo puede deduplicar dentro de la misma corrida vía el
            // caché de nombres de cada servicio; sp_upsert_producto siempre inserta cuando
            // no se le pasa @IdProducto.
            await _procedures.sp_upsert_productoAsync(null, nombre, idCategoria, cancellationToken: ct);
        }

        /// <summary>
        /// Clientes.IdCliente en el DWH es VARCHAR(6); se homologa cualquier identificador
        /// de origen (numérico o alfanumérico más largo) tomando los últimos 6 caracteres.
        /// </summary>
        public static string HomologarIdCliente(string idClienteOrigen)
        {
            var texto = (idClienteOrigen ?? string.Empty).Trim();
            if (texto.Length == 0) return texto;
            return texto.Length > 6 ? texto[^6..] : texto.PadLeft(6, '0');
        }
    }
}
