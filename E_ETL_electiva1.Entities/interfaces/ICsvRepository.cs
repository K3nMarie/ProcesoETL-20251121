using E_ETL_electiva1.Entities.Models.csv;
using E_ETL_electiva1.Entities.Models.Dwh.Dims;
using System;
using System.Collections.Generic;
using System.Text;
namespace E_ETL_electiva1.Entities.interfaces
{
    public interface ICsvRepository
    {
        IEnumerable<surveys_part1> GetAll();
    }
}
