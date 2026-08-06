// Adaptado al esquema de AnaliticaOpinionesClientes (Project/SistemaAnalisisOpinionesClientes/DBAnalitica).
// Reemplaza a la antigua dimensión "Fuente"/"RedesSociale" del ETL original.
#nullable enable
using System.Collections.Generic;
using E_ETL_electiva1.Entities.Models.Dwh.Facts;

namespace E_ETL_electiva1.Entities.Models.Dwh.Dims;

public partial class Fuente
{
    public int IdCanal { get; set; }

    public string NombreCanal { get; set; } = null!;

    public virtual ICollection<Opinione> Opiniones { get; set; } = new List<Opinione>();
}
