using System.Reflection;
using System.Windows;

namespace OpenBrowser
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static readonly string? AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string? AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    }
}
