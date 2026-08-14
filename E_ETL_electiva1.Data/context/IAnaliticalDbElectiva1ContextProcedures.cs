// Adaptado a partir del original (IAnaliticalDbElectiva1ContextProcedures) para exponer los
// stored procedures reales del Project: Project/SistemaAnalisisOpinionesClientes/DBAnalitica.
#nullable enable
using System.Threading;
using System.Threading.Tasks;
using E_ETL_electiva1.Data.context;

namespace E_ETL_electiva1.Data.context
{
    public partial interface IAnaliticalDbElectiva1ContextProcedures
    {
        Task<int> sp_upsert_categoriaAsync(int? idCategoria, string nombreCategoria, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default);

        Task<int> sp_upsert_productoAsync(int? idProducto, string nombre, int idCategoria, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default);

        Task<int> sp_upsert_canalAsync(string nombreCanal, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default);

        Task<int> sp_upsert_clienteAsync(string? idCliente, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default);
        Task<int> sp_upsert_opinionAsync(int? idOpinion, int idProducto, string? idCliente, int? idCanal, System.DateOnly fechaOpinion, byte puntajeSatisfaccion, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default);

        Task<int> sp_insert_opinion_sin_canalAsync(int idProducto, string? idCliente, System.DateOnly fechaOpinion, byte puntajeSatisfaccion, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default);
    }
}
