using System.Text.Json;
using E_ETL_electiva1.api.Models;
using E_ETL_electiva1.Entities.interfaces;

namespace E_ETL_electiva1.Data.Repositories
{
    // Consume la API REST del sistema transaccional de origen (proyecto E_ETL_electiva1.api),
    // que expone el mismo modelo genérico (Clientes/Productos/Redes_Sociales) usado por TransDbRepo.
    public class apiRepo : IApiConsRepository
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public apiRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Clientes>> GetClientes()
        {
            var respuesta = await _httpClient.GetAsync("api/Clientes");
            respuesta.EnsureSuccessStatusCode();
            var json = await respuesta.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Clientes>>(json, JsonOptions) ?? [];
        }

        public async Task<IEnumerable<Productos>> GetProductos()
        {
            var respuesta = await _httpClient.GetAsync("api/Productos");
            respuesta.EnsureSuccessStatusCode();
            var json = await respuesta.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Productos>>(json, JsonOptions) ?? [];
        }

        public string GetFuentes()
        {
            // El sistema de origen no expone un endpoint de fuentes; se identifica el canal
            // "API REST" de forma fija, igual que hacía la plantilla original.
            return "Social Comments";
        }

        public async Task<IEnumerable<Redes_Sociales>> GetRedesSociales()
        {
            var respuesta = await _httpClient.GetAsync("api/Redes_Sociales");
            respuesta.EnsureSuccessStatusCode();
            var json = await respuesta.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Redes_Sociales>>(json, JsonOptions) ?? [];
        }
    }
}
