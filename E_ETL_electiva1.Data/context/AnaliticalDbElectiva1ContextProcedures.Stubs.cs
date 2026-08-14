using System.Threading;
using System.Threading.Tasks;

namespace E_ETL_electiva1.Data.context
{
    // Partial class to provide simple stub implementations for missing interface methods
    // These stubs return a default OutputParameter result so the project compiles.
    // Replace bodies with real stored-proc calls or generated code as needed.
    public partial class AnaliticalDbElectiva1ContextProcedures
    {
        public virtual Task<OutputParameter<int>?> sp_upsert_clienteAsync(int clienteId, OutputParameter<int>? output = null, CancellationToken cancellationToken = default)
        {
            // Ensure a Task-returning result on all code paths.
            return Task.FromResult(output);
        }

        public virtual Task<OutputParameter<int>?> sp_insert_opinion_sin_canalAsync(int id, string? texto, DateOnly fecha, byte flag, OutputParameter<int>? output = null, CancellationToken cancellationToken = default)
        {
            // Ensure a Task-returning result on all code paths.
            return Task.FromResult(output);
        }
    }
}
