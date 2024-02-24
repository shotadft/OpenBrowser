using OpenBrowser.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenBrowser
{
	internal partial class Program
	{
		public const string app_name = "OpenBrowser";
		public const string version = "1.0.3";
		public static Icon icon = Resources.icon;

		[STAThread]
		public static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}