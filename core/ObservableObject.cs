using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System;
using System.IO;



namespace Bilsoft.core
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        

       
        

    }
}
