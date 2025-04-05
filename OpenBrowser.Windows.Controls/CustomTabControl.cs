using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace OpenBrowser.Windows.Controls
{
    [ToolboxBitmap(typeof(TabControl))]
    public class CustomTabControl : TabControl
    {
        static CustomTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTabControl), new FrameworkPropertyMetadata(typeof(CustomTabControl)));
        }

        public override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!DesignerProperties.GetIsInDesignMode(this) && this.Items.Count == 0) await NewTabAsync();

            var addButton = GetTemplateChild("AddTabButton") as Button;
            if (addButton != null) addButton.Click += new(AddButton_Click);
        }

        private async void AddButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this)) await NewTabAsync();
        }

        private async Task NewTabAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                TabItem newTab = new()
                {
                    Header = "New Tab",
                    Content = new Grid { Background = new SolidColorBrush(Colors.LightGray) }
                };
                this.Items.Add(newTab);
                this.SelectedItem = newTab;
            });
        }

        public ICommand CloseTabCommand => new RelayCommand(CloseTab);

        private void CloseTab(object? parameter)
        {
            if (parameter != null && parameter is TabItem tabItem)
            {
                this.Items.Remove(tabItem);
                if (this.Items.Count == 0)
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }

    public class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
    {
        private readonly Action<object?> _execute = execute;
        private readonly Func<object?, bool>? _canExecute = canExecute;

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}