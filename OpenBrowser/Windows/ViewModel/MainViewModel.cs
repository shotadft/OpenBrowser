using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace OpenBrowser.Windows.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand? UndoButtonClickCommand { get; }
        public ICommand? ForwardButtonClickCommand { get; }
        public ICommand? UpdateButtonClickCommand { get; }
        public ICommand? HomeButtonClickCommand { get; }
        public ICommand? SettingButtonClickCommand { get; }

        public MainViewModel()
        {
            UndoButtonClickCommand = new RelayCommand(UndoButtonClick);
            ForwardButtonClickCommand = new RelayCommand(ForwardButtonClick);
            UpdateButtonClickCommand = new RelayCommand(UpdateButtonClick);
            HomeButtonClickCommand = new RelayCommand(HomeButtonClick);
            SettingButtonClickCommand = new RelayCommand(SettingButtonClick);
        }

        private void UndoButtonClick()
        {
            MessageBox.Show("Undo button clicked.");
        }

        private void ForwardButtonClick()
        {
            MessageBox.Show("Forward button clicked.");
        }

        private void UpdateButtonClick()
        {
            MessageBox.Show("Update button clicked.");
        }

        private void HomeButtonClick()
        {
            MessageBox.Show("Home button clicked.");
        }

        private void SettingButtonClick()
        {
            MessageBox.Show("Setting button clicked.");
        }


        private string _AppName = App.AppName ?? string.Empty;
        public string AppName
        {
            get => _AppName;
            private set
            {
                _AppName = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
