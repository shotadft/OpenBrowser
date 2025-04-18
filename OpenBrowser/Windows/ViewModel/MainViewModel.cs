using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace OpenBrowser.Windows.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand? ButtonClickCommand { get; }

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

        private string _message = "";
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }

        private void OnButtonClick()
        {
            Message = "ボタンがクリックされました！";
        }

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
