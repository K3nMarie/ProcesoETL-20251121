// Adaptado al esquema de AnaliticaOpinionesClientes (Project/SistemaAnalisisOpinionesClientes/DBAnalitica).
#nullable enable
using System;
using E_ETL_electiva1.Entities.Models.Dwh.Dims;

namespace E_ETL_electiva1.Entities.Models.Dwh.Facts;

public partial class Opinione
{
    public int IdOpinion { get; set; }

    public int IdProducto { get; set; }

    public string? IdCliente { get; set; }

    public int? IdCanal { get; set; }

    public DateOnly FechaOpinion { get; set; }

    public string? Clasificacion { get; set; }

    public byte? PuntajeSatisfaccion { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Fuente? IdCanalNavigation { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
