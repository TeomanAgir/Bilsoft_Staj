using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Bilsoft.MVVM.Model
{
    public class GelirGiderModel
    {
        private readonly string _csvPath;

        public GelirGiderModel(string csvPath)
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
            .Skip(1)
            .Select(line => line.Split(','))
            .Where(data => int.Parse(data[0]) == year && int.Parse(data[1]) == month)
            .Select(data =>
            {
                try
                {
                    return new double[]
                    {
                        
                        double.Parse(data[5], CultureInfo.InvariantCulture), // İşlem Yapılan Gün Sayısı
                        double.Parse(data[6], CultureInfo.InvariantCulture), // Gelir sütunu
                        double.Parse(data[8], CultureInfo.InvariantCulture)  // Gider sütunu
                
                    };
                }
                catch (IndexOutOfRangeException)
                {
                    // Hata oluştuğunda null döndür
                    return null;
                }
            })
            .Where(data => data != null)
            .ToList();

            return filteredData;
        }
       
    }
}
