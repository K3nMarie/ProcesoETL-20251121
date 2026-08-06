using E_ETL_electiva1.Entities.Models.Dwh.Dims;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_ETL_electiva1.Entities.Models.csv
{
    public class surveys_part1
    {
        public int IdOpinion { get; set; }
        public int IdCliente { get; set; }
        public int IdProducto { get; set; }
        public DateOnly Fecha { get; set; }
        public string Comentario { get; set; }
        public string Clasificacion { get; set; }
        public int PuntajeSatisfaccion { get; set; }
        public string Fuente { get; set; } 

    }
}
