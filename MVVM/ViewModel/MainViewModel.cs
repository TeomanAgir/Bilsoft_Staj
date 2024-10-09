using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bilsoft.core;
using System.Windows.Input;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Reflection;


namespace Bilsoft.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand AnaSayfaViewCommand { get; set; }
        public RelayCommand FaturaTuruViewCommand { get; set; }
        public RelayCommand eFaturaViewCommand { get; set; }
        public RelayCommand GelirGiderViewCommand { get; set; }

        public AnaSayfaViewModel AnaVM { get; set; }
        public FaturaTuruViewModel FaturaTuruVM { get; set; }
        public eFaturaViewModel eFaturaVM { get; set; }
        public GelirGiderViewModel GelirGiderVM { get; set; }
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value; OnPropertyChanged();
            }
        }

        public ICommand DosyaSecCommand { get; }

        private string _selectedFilePath;
        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public MainViewModel()
        {
            AnaVM = new AnaSayfaViewModel();
            FaturaTuruVM = new FaturaTuruViewModel();
            eFaturaVM = new eFaturaViewModel();
            GelirGiderVM = new GelirGiderViewModel();
            CurrentView = AnaVM;

            AnaSayfaViewCommand = new RelayCommand(o =>
            {
                CurrentView = AnaVM;
            });
            FaturaTuruViewCommand = new RelayCommand(o =>
            {
                CurrentView = FaturaTuruVM;
            });
            eFaturaViewCommand = new RelayCommand(o =>
            {
                CurrentView = eFaturaVM;
            });
            GelirGiderViewCommand = new RelayCommand(o =>
            {
                CurrentView = GelirGiderVM;
            });
            // ICommand nesnesini oluşturuyoruz ve Execute metodunu atıyoruz
            DosyaSecCommand = new RelayCommand(ExecuteDosyaSecCommand);
            AnalizEtCommand = new RelayCommand(ExecuteAnalizEtCommand, CanExecuteAnalizEtCommand);  
        }
    

        public ICommand AnalizEtCommand { get; }
        private void ExecuteAnalizEtCommand(object parameter)
        {
            StatusMessage = "Python scripti çalıştırılıyor...";
            Debug.WriteLine("Analiz Et komutu çalıştırıldı.");
            MessageBox.Show("Analiz Et komutu çalıştırıldı."); 

            RunPythonExe(SelectedFilePath);
            StatusMessage = "Python scripti tamamlandı!";
        }


        private bool CanExecuteAnalizEtCommand(object parameter)
        {
            // Analiz Et komutunun çalışıp çalışmayacağını belirler.
            return !string.IsNullOrEmpty(SelectedFilePath);
        }

        private void ExecuteDosyaSecCommand(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName;

                // Seçilen dosya yolunu MessageBox ile gösterin
                
                System.Diagnostics.Debug.WriteLine($"Seçilen dosya yolu: {SelectedFilePath}");
            }
        }
        private void RunPythonExe(string selectedFilePath)
        {
            //string exePath = @"C:\Users\Teoman\source\repos\TeomanAgir\Bilsoft_Staj\scripts\scriptexe\dist\son.exe";
            string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string projectDirectory = Directory.GetParent(appDirectory).Parent.Parent.FullName;
            string exePath = Path.Combine(projectDirectory, "scripts", "scriptexe", "dist", "son.exe");
            
            Debug.WriteLine("Exe yolu: " + exePath);  // Bu satır exePath'in doğru olup olmadığını kontrol etmek için

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{SelectedFilePath}\"", // Eğer argüman gerekiyorsa, burada verilebilir
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Debug.WriteLine(result);
                }

                using (StreamReader error = process.StandardError)
                {
                    string errorResult = error.ReadToEnd();
                    if (!string.IsNullOrEmpty(errorResult))
                    {
                        Debug.WriteLine("Hata: " + errorResult);
                        
                    }
                }
            }
        }
    }
}
