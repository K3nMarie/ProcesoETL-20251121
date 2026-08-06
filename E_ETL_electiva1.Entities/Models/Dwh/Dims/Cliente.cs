// Adaptado al esquema de AnaliticaOpinionesClientes (Project/SistemaAnalisisOpinionesClientes/DBAnalitica).
// En el modelo analítico el cliente solo conserva el código homologado (VARCHAR(6)); los
// datos personales (Nombre, Email) permanecen en el sistema transaccional SistemaOpinionCliente.
#nullable enable
using System.Collections.Generic;
using E_ETL_electiva1.Entities.Models.Dwh.Facts;

namespace E_ETL_electiva1.Entities.Models.Dwh.Dims;

public partial class Cliente
{
    public string IdCliente { get; set; } = null!;

    public virtual ICollection<Opinione> Opiniones { get; set; } = new List<Opinione>();
}
