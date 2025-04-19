using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenBrowser.Windows.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _DialogDescription = $"{MainViewModel.history.CurrentUrl[MainViewModel.tabIndex]} にアクセスするにはログインしてください";
        public string DialogDescription
        {
            get => _DialogDescription;
            private set
            {
                _DialogDescription = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
