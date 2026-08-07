using Microsoft.EntityFrameworkCore;
using E_ETL_electiva1.Entities.interfaces;
using E_ETL_electiva1.api.context;

namespace E_ETL_electiva1.Data.Repositories
{
    // Lector genérico sobre el sistema transaccional de origen (E_ETL_electiva1.api /
    // opiniones_de_clientesDBContext). Público para poder registrarse por tipo genérico
    // en el contenedor de dependencias del proceso ETL.
    public class TransDbRepo<T> : IDbReaderRepository<T> where T : class
    {
        protected readonly opiniones_de_clientesDBContext _context;
        protected readonly DbSet<T> _dbSet;

        public TransDbRepo(opiniones_de_clientesDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.AsNoTracking().ToListAsync();

        public async Task<T?> GetByIdAsync(object id) =>
            await _dbSet.FindAsync(id);
    }
}
