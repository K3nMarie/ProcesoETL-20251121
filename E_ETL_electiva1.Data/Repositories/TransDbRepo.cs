using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using E_ETL_electiva1.Entities.interfaces;
using E_ETL_electiva1.api.context;

namespace E_ETL_electiva1.Data.Repositories

{
    internal class TransDbRepo<T> : IDbReaderRepository<T> where T: class
    {
        // Reemplaza con el nombre exacto de tu DbContext generado
        protected readonly opiniones_de_clientesDBContext _context;
        protected readonly DbSet<T> _dbSet;

        public TransDbRepo(opiniones_de_clientesDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(object id) =>
            await _dbSet.FindAsync(id);
    }
}
