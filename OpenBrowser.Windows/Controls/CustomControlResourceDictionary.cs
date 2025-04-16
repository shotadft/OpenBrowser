using System.Windows;
using System.Windows.Controls;

namespace OpenBrowser.Windows.Controls
{
    public partial class CustomControlResourceDictionary : ResourceDictionary
    {
        private async void AddTabButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement element && element.TemplatedParent is CustomTabControl tabControl) await tabControl.AddTabAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"タブの追加中にエラーが発生しました:{Environment.NewLine} {ex.Message}", "Tab Addition Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CloseTabButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement element && element.TemplatedParent is TabItem tabItem && tabItem.Parent is CustomTabControl tabControl)
                    await tabControl.CloseTabAsync(tabItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"タブの削除中にエラーが発生しました:{Environment.NewLine} {ex.Message}", "Tab Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
