using System.Reflection;
using System.Windows;

namespace OpenBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string appName = Assembly.GetExecutingAssembly().GetName().Name ?? "OpenBrowser";
        public static readonly string appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";
    }
}
