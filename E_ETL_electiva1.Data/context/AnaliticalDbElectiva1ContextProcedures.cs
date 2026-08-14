// Adaptado a partir del original (AnaliticalDbElectiva1ContextProcedures) para invocar los
// stored procedures reales del Project: Project/SistemaAnalisisOpinionesClientes/DBAnalitica.
#nullable enable
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace E_ETL_electiva1.Data.context
{
    public partial class AnaliticalDbElectiva1Context
    {
        private IAnaliticalDbElectiva1ContextProcedures? _procedures;

        public virtual IAnaliticalDbElectiva1ContextProcedures Procedures
        {
            get => _procedures ??= new AnaliticalDbElectiva1ContextProcedures(this);
            set => _procedures = value;
        }

        public IAnaliticalDbElectiva1ContextProcedures GetProcedures() => Procedures;
    }

    public partial class AnaliticalDbElectiva1ContextProcedures : IAnaliticalDbElectiva1ContextProcedures
    {
        private readonly AnaliticalDbElectiva1Context _context;

        public AnaliticalDbElectiva1ContextProcedures(AnaliticalDbElectiva1Context context)
        {
            _context = context;
        }

        public virtual async Task<int> sp_upsert_categoriaAsync(int? idCategoria, string nombreCategoria, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default)
        {
            var parameterReturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
            };

            var sqlParameters = new[]
            {
                new SqlParameter { ParameterName = "IdCategoria", Value = (object?)idCategoria ?? DBNull.Value, SqlDbType = SqlDbType.Int },
                new SqlParameter { ParameterName = "NombreCategoria", Size = 100, Value = nombreCategoria, SqlDbType = SqlDbType.VarChar },
                parameterReturnValue,
            };

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC @returnValue = [dbo].[sp_upsert_categoria] @IdCategoria = @IdCategoria, @NombreCategoria = @NombreCategoria",
                sqlParameters, cancellationToken ?? CancellationToken.None);

            returnValue?.SetValue(parameterReturnValue.Value);
            return result;
        }

        public virtual async Task<int> sp_upsert_productoAsync(int? idProducto, string nombre, int idCategoria, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default)
        {
            var parameterReturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
            };

            var sqlParameters = new[]
            {
                new SqlParameter { ParameterName = "IdProducto", Value = (object?)idProducto ?? DBNull.Value, SqlDbType = SqlDbType.Int },
                new SqlParameter { ParameterName = "Nombre", Size = 150, Value = nombre, SqlDbType = SqlDbType.VarChar },
                new SqlParameter { ParameterName = "IdCategoria", Value = idCategoria, SqlDbType = SqlDbType.Int },
                parameterReturnValue,
            };

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC @returnValue = [dbo].[sp_upsert_producto] @IdProducto = @IdProducto, @Nombre = @Nombre, @IdCategoria = @IdCategoria",
                sqlParameters, cancellationToken ?? CancellationToken.None);

            returnValue?.SetValue(parameterReturnValue.Value);
            return result;
        }

        public virtual async Task<int> sp_upsert_canalAsync(string nombreCanal, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default)
        {
            var parameterReturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
            };

            var sqlParameters = new[]
            {
                new SqlParameter { ParameterName = "NombreCanal", Size = 100, Value = nombreCanal, SqlDbType = SqlDbType.VarChar },
                parameterReturnValue,
            };

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC @returnValue = [dbo].[sp_upsert_canal] @NombreCanal = @NombreCanal",
                sqlParameters, cancellationToken ?? CancellationToken.None);

            returnValue?.SetValue(parameterReturnValue.Value);
            return result;
        }

        public virtual async Task<int> sp_upsert_clienteAsync(string? idCliente, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default)
        {
            var parameterReturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
            };

            var sqlParameters = new[]
            {
                new SqlParameter { ParameterName = "IdCliente", Size = 100, Value = (object?)idCliente ?? DBNull.Value, SqlDbType = SqlDbType.VarChar },
                parameterReturnValue,
            };

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC @returnValue = [dbo].[sp_upsert_cliente] @IdCliente = @IdCliente",
                sqlParameters, cancellationToken ?? CancellationToken.None);

            returnValue?.SetValue(parameterReturnValue.Value);
            return result;
        }

        public virtual async Task<int> sp_upsert_opinionAsync(int? idOpinion, int idProducto, string? idCliente, int? idCanal, DateOnly fechaOpinion, byte puntajeSatisfaccion, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default)
        {
            var parameterReturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
            };

            var sqlParameters = new[]
            {
                new SqlParameter { ParameterName = "IdOpinion", Value = (object?)idOpinion ?? DBNull.Value, SqlDbType = SqlDbType.Int },
                new SqlParameter { ParameterName = "IdProducto", Value = idProducto, SqlDbType = SqlDbType.Int },
                new SqlParameter { ParameterName = "IdCliente", Size = 6, Value = (object?)idCliente ?? DBNull.Value, SqlDbType = SqlDbType.VarChar },
                new SqlParameter { ParameterName = "IdCanal", Value = (object?)idCanal ?? DBNull.Value, SqlDbType = SqlDbType.Int },
                new SqlParameter { ParameterName = "FechaOpinion", Value = fechaOpinion.ToDateTime(TimeOnly.MinValue), SqlDbType = SqlDbType.Date },
                new SqlParameter { ParameterName = "PuntajeSatisfaccion", Value = puntajeSatisfaccion, SqlDbType = SqlDbType.TinyInt },
                parameterReturnValue,
            };

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC @returnValue = [dbo].[sp_upsert_opinion] @IdOpinion = @IdOpinion, @IdProducto = @IdProducto, @IdCliente = @IdCliente, @IdCanal = @IdCanal, @FechaOpinion = @FechaOpinion, @PuntajeSatisfaccion = @PuntajeSatisfaccion",
                sqlParameters, cancellationToken ?? CancellationToken.None);

            returnValue?.SetValue(parameterReturnValue.Value);
            return result;
        }

        public virtual async Task<int> sp_insert_opinion_sin_canalAsync(int idProducto, string? idCliente, DateOnly fechaOpinion, byte puntajeSatisfaccion, OutputParameter<int>? returnValue = null, CancellationToken? cancellationToken = default)
        {
            var parameterReturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
            };

            var sqlParameters = new[]
            {
                new SqlParameter { ParameterName = "IdProducto", Value = idProducto, SqlDbType = SqlDbType.Int },
                new SqlParameter { ParameterName = "IdCliente", Size = 6, Value = (object?)idCliente ?? DBNull.Value, SqlDbType = SqlDbType.VarChar },
                new SqlParameter { ParameterName = "FechaOpinion", Value = fechaOpinion.ToDateTime(TimeOnly.MinValue), SqlDbType = SqlDbType.Date },
                new SqlParameter { ParameterName = "PuntajeSatisfaccion", Value = puntajeSatisfaccion, SqlDbType = SqlDbType.TinyInt },
                parameterReturnValue,
            };

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC @returnValue = [dbo].[sp_insert_opinion_sin_canal] @IdProducto = @IdProducto, @IdCliente = @IdCliente, @FechaOpinion = @FechaOpinion, @PuntajeSatisfaccion = @PuntajeSatisfaccion",
                sqlParameters, cancellationToken ?? CancellationToken.None);

            returnValue?.SetValue(parameterReturnValue.Value);
            return result;
        }
    }
}
