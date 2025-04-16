using System.Reflection;
using System.Windows;

namespace OpenBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string? AppName { get; } = Assembly.GetExecutingAssembly().GetName().Name;
        public static string? AppVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    }
}
