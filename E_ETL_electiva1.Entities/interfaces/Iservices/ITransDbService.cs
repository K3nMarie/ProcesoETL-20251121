namespace E_ETL_electiva1.Entities.interfaces.Iservices
{
    public interface ITransDbService
    {
        public Task<bool> upload_Clientes();
        public Task<bool> upload_Productos();
        public Task<bool> upload_Fuentes();
    }
}
