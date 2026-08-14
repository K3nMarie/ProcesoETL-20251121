using E_ETL_electiva1.api.Models;

namespace E_ETL_electiva1.Entities.interfaces
{
    // Consume la API REST del sistema transaccional de origen (E_ETL_electiva1.api),
    // que sigue exponiendo el modelo genérico de la plantilla (Clientes/Productos).
    // Nota: Redes_Sociales se retiró porque esa tabla no existe en la BD real
    // (SistemaOpinionCliente); el canal de origen ya se cubre vía GetFuentes()/Canales_Origen.
    public interface IApiConsRepository
    {
        public Task<IEnumerable<Clientes>> GetClientes();
        public Task<IEnumerable<Productos>> GetProductos();
        public string GetFuentes();
    }
}
