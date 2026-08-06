// Adaptado al esquema de AnaliticaOpinionesClientes (Project/SistemaAnalisisOpinionesClientes/DBAnalitica).
#nullable enable
using System.Collections.Generic;
using E_ETL_electiva1.Entities.Models.Dwh.Facts;

namespace E_ETL_electiva1.Entities.Models.Dwh.Dims;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdCategoria { get; set; }

    public virtual CategoriaProducto? IdCategoriaNavigation { get; set; }

    public virtual ICollection<Opinione> Opiniones { get; set; } = new List<Opinione>();
}
