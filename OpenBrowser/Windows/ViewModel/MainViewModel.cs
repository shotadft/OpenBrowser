using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using OpenBrowser.Data;

namespace OpenBrowser.Windows.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler? OnRequestOpenWindow = null;

        public static readonly PageHistory history = new();
        public static int tabIndex = 0;

        public ICommand? UndoButtonClickCommand { get; }
        public ICommand? ForwardButtonClickCommand { get; }
        public ICommand? UpdateButtonClickCommand { get; }
        public ICommand? HomeButtonClickCommand { get; }
        public ICommand? SettingButtonClickCommand { get; }

        public ICommand? TabChangedEventCommand { get; }

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
            history.CurrentUrl[SelectedTabIndex] = new Uri("https://www.google.com");
            OnRequestOpenWindow?.Invoke(this, EventArgs.Empty);
        }

        private string _AppName = App.AppName ?? string.Empty;
        public string AppName
        {
            get => _AppName;
            private set
            {
                _AppName = value;
                OnPropertyChanged(nameof(AppName));
            }
        }

        private int _selectedTabIndex = 0;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    tabIndex = _selectedTabIndex;
                    OnPropertyChanged(nameof(SelectedTabIndex));
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
