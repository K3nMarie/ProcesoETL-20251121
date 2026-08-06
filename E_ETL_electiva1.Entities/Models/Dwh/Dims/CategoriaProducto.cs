// Adaptado al esquema de AnaliticaOpinionesClientes (Project/SistemaAnalisisOpinionesClientes/DBAnalitica).
#nullable enable
using System.Collections.Generic;

namespace E_ETL_electiva1.Entities.Models.Dwh.Dims;

public partial class CategoriaProducto
{
    public int IdCategoria { get; set; }

    public string NombreCategoria { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
