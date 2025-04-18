using System.Windows;

namespace OpenBrowser
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            tabControl.Dispose();
        }
    }
}