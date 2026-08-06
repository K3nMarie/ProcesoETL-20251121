using System;
using System.Collections.Generic;
using System.Text;

namespace E_ETL_electiva1.Entities.interfaces
{
    public interface IDbReaderRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
    }
}
