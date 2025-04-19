using System.Windows;
using OpenBrowser.Windows.Dialog;

namespace OpenBrowser
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            viewModel.OnRequestOpenWindow += ViewModel_OnRequestOpenWindow;
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            tabControl.Dispose();
        }

        private void ViewModel_OnRequestOpenWindow(object? sender, EventArgs e)
        {
            LoginDialog loginDialog = new() { Owner = this };
            if (loginDialog.ShowDialog() == true)
            {
                // Do something after the dialog is closed
                MessageBox.Show("Login successful!");
            }
            else
            {
                // Handle the case when the dialog is closed without success
                MessageBox.Show("Login failed or canceled.");
            }
        }
    }
}