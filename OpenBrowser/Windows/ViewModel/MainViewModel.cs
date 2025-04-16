using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenBrowser.Windows.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _AppName = App.AppName ?? string.Empty;
        public string AppName
        {
            get => _AppName;
            private set
            {
                _AppName = value;
                RaisePropertyChanged();
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
