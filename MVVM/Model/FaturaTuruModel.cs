using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bilsoft.MVVM.Model
{
    public class FaturaTuruModel
    {
        private readonly string _csvPath;

        public FaturaTuruModel(string csvPath)
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
                        double.Parse(data[2]), // ALIŞ FATURASI
                        double.Parse(data[3]), // ALIŞ İRSALİYESİ
                        double.Parse(data[4]), // MÜSTAHSİL
                        double.Parse(data[5]), // SATIŞ FATURASI
                        double.Parse(data[6]), // SATIŞ FATURASI İADESİ
                        double.Parse(data[7]), // SATIŞ İRSALİYESİ
                        double.Parse(data[8]), // İPTAL
                        double.Parse(data[9]) // İPTAL SATIŞ FATURASI
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
            //ALIŞ FATURASI, ALIŞ İRSALİYESİ,MÜSTAHSİL,SATIŞ FATURASI, SATIŞ FATURASI İADESİ, SATIŞ İRSALİYESİ,İPTAL,İPTAL SATIŞ FATURASI
            return filteredData;
        }
    }
}
