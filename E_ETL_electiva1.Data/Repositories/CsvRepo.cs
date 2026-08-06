using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using E_ETL_electiva1.Entities.Models.csv;
using E_ETL_electiva1.Entities.interfaces;
namespace E_ETL_electiva1.Data.Repositories
{
    internal class CsvRepo : ICsvRepository
    {

        private readonly string _filePath;

        public CsvRepo(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerable<surveys_part1> GetAll()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"El archivo CSV no fue encontrado en: {_filePath}");
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, config);


            return csv.GetRecords<surveys_part1>().ToList();
        }
    }
}
