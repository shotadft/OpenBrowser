using System.Windows;

namespace OpenBrowser.Windows
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            undoButton.Click -= UndoButton_Click;
            forwardButton.Click -= ForwardButton_Click;
            updateButton.Click -= UpdateButton_Click;
            homeButton.Click -= HomeButton_Click;
            settingButton.Click -= SettingButton_Click;
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
