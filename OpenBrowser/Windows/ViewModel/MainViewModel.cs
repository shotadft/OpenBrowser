using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using OpenBrowser.Data;
using OpenBrowser.Net;

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
        public ICommand? AddressBarEnterKeyCommand { get; }

        public MainViewModel()
        {
            UndoButtonClickCommand = new RelayCommand(UndoButtonClick);
            ForwardButtonClickCommand = new RelayCommand(ForwardButtonClick);
            UpdateButtonClickCommand = new RelayCommand(UpdateButtonClick);
            HomeButtonClickCommand = new RelayCommand(HomeButtonClick);
            SettingButtonClickCommand = new RelayCommand(SettingButtonClick);
            AddressBarEnterKeyCommand = new RelayCommand(PlessADBEnterKey);
        }

        private string _AppName = App.AppName ?? string.Empty;
        public string AppName
        {
            get => _AppName;
            private set
            {
                if (_AppName != value)
                {
                    _AppName = value;
                    OnPropertyChanged(nameof(AppName));
                }
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

        private string _addressBarText = string.Empty;
        public string AddressBarText
        {
            get => _addressBarText;
            set
            {
                if (_addressBarText != value)
                {
                    _addressBarText = value;
                    OnPropertyChanged(nameof(AddressBarText));
                }
            }
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

        private async void PlessADBEnterKey()
        {
            await NavigateTo();
        }

        private async Task NavigateTo()
        {
            string url = AddressBarText;
            if (string.IsNullOrWhiteSpace(url)) return;

            Uri? uri = NetHandler.ConvertURIString(url);

            var (str, finalUri) = await NetHandler.GetStringAsync(uri);
            AddressBarText = (history.CurrentUrl[tabIndex] = uri)?.ToString() ?? AddressBarText;
            history.Navigate(tabIndex, history.GetCurrent(tabIndex));

            await File.WriteAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "index.html"), str);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
