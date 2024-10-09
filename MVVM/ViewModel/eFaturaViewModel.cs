using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System.Collections.ObjectModel;
using Bilsoft.MVVM.Model;
using System.IO;
using Bilsoft.core;
using System.Linq;
using System;
using System.Windows.Input;
using System.Data; // DataTable için gerekli
using System.Collections.Generic;


namespace Bilsoft.MVVM.ViewModel
{
    public class eFaturaViewModel : ObservableObject
    {
        private readonly eFaturaModel _model;
        public ObservableCollection<int> AvailableYears { get; set; }
        public ObservableCollection<string> AvailableMonths { get; set; }
        public ObservableCollection<string> Labels { get; set; } // Senaryo İsimleri
        public ObservableCollection<string> SenaryoLabels { get; set; }
        public ObservableCollection<ISeries> Series { get; set; }
        public ICommand RefreshCommand { get; set; } // Butonun bağlı olduğu komut
        public DataTable DataTable { get; set; } // DataGrid için gerekli özellik
        public ObservableCollection<Axis> XAxes { get; set; }
        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear)); // UI'ye değişiklik olduğunu bildirir
                UpdateChart(); // Değişiklikten sonra grafiği güncelle
            }
        }

        private string _selectedMonth;
        public string SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                _selectedMonth = value;
                OnPropertyChanged(nameof(SelectedMonth)); // UI'ye değişiklik olduğunu bildirir
                UpdateChart(); // Değişiklikten sonra grafiği güncelle
            }
        }

        public eFaturaViewModel()
        {

            var csvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Analizler", "aylik_senaryolar.csv");
                _model = new eFaturaModel(csvPath);
        

            AvailableYears = new ObservableCollection<int> { 2021, 2022, 2023, 2024 };
            AvailableMonths = new ObservableCollection<string> { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };
            
            SelectedYear = 2022; // Varsayılan
            SelectedMonth = "Ocak"; // Varsayılan

            XAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Labels = new List<string> { "E-Arşiv", "E-Fatura", "E-İrsaliye", "Normal" } // Sabit etiketler
                }
            };
            
            // Buton için komut
            RefreshCommand = new RelayCommand(o => UpdateTableAndChart());

        }

        private void UpdateTableAndChart()
        {
            LoadDataTable();
            UpdateChart();
        }
        private void LoadDataTable()
        {

            DataTable = new DataTable();
            var csvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Analizler", "aylik_senaryolar.csv");

            using (var reader = new StreamReader(csvPath))
            {
                bool firstLine = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (firstLine)
                    {
                        foreach (var column in values)
                        {
                            DataTable.Columns.Add(column); // Başlıkları ekle
                        }
                        firstLine = false;
                    }
                    else
                    {
                        DataTable.Rows.Add(values); // Verileri ekle
                    }
                }
            }

            OnPropertyChanged(nameof(DataTable));
        }

        private void UpdateChart()
        {

            var filteredData = _model.GetFilteredData(SelectedYear, GetMonthNumber(SelectedMonth));

            // İlk veri setini alıyoruz
            var values = filteredData.FirstOrDefault() ?? new double[] { 0, 0, 0, 0 };

            Series = new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Values = values
                }
            };

           
            OnPropertyChanged(nameof(Series));
        }

        private int GetMonthNumber(string monthName)
        {
            return monthName switch
            {
                "Ocak" => 1,
                "Şubat" => 2,
                "Mart" => 3,
                "Nisan" => 4,
                "Mayıs" => 5,
                "Haziran" => 6,
                "Temmuz" => 7,
                "Ağustos" => 8,
                "Eylül" => 9,
                "Ekim" => 10,
                "Kasım" => 11,
                "Aralık" => 12,
                _ => 1
            };
        }
    }
}
