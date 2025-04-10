using System.Threading.Tasks;
using System.Windows;
using OpenBrowser.Security.Filer;
using OpenBrowser.Security.Filer.Cache;
using OpenBrowser.Windows.Dialog;

namespace OpenBrowser.Windows
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            undoButton.Click -= UndoButton_Click;
            forwardButton.Click -= ForwardButton_Click;
            updateButton.Click -= UpdateButton_Click;
            homeButton.Click -= HomeButton_Click;
            settingButton.Click -= SettingButton_Click;
        }

        private void UndoButton_Click(object? sender, RoutedEventArgs e)
        {

        }

        private void ForwardButton_Click(object? sender, RoutedEventArgs e)
        {

        }

        private void UpdateButton_Click(object? sender, RoutedEventArgs e)
        {

        }

        private void HomeButton_Click(object? sender, RoutedEventArgs e)
        {

        }

        private async void SettingButton_Click(object? sender, RoutedEventArgs e)
        {
            LoginDialog loginDialog = new() { Owner = this };
            if (loginDialog.ShowDialog() == true)
            {
                string username = loginDialog.Username;
                string password = loginDialog.Password;

                BasicCertificateCache cacheFileManager = new();
                await cacheFileManager.SaveCertCacheAsync("https://example.com", username, password);
                MessageBox.Show($"Username: {username}{Environment.NewLine}Password: {password}", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
