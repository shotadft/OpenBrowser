using System.Diagnostics;

namespace OpenBrowser
{
    internal static class Program
    {
        public const string app_name = "OpenBrowser";
        public const string version = "1.0.4";
        public static Icon icon = new Icon(@"icon/icon.ico");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}