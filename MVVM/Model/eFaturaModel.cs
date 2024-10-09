using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bilsoft.MVVM.Model
{
    public class eFaturaModel
    {
        private readonly string _csvPath;

        public eFaturaModel(string csvPath)
        {
            _csvPath = csvPath;
        }

        // Veriyi yükleyip filtreleyen metod
        public List<double[]> GetFilteredData(int year, int month)
        {
            if (!File.Exists(_csvPath))
                return new List<double[]>();

            var lines = File.ReadAllLines(_csvPath);
            var filteredData = lines
                .Skip(1) // Başlık satırını atla
                .Select(line => line.Split(','))
                .Where(data => int.Parse(data[0]) == year && int.Parse(data[1]) == month)
                .Select(data => new double[]
                {
                    double.Parse(data[2]), // E-Arşiv
                    double.Parse(data[3]), // E-Fatura
                    double.Parse(data[4]), // E-İrsaliye
                    double.Parse(data[5])  // Normal
                })
                .ToList();

            return filteredData;
        }
    }
}
