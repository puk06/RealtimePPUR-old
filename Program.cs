using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;

namespace RealtimePPUR
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");
            CultureInfo.CurrentUICulture = new CultureInfo("en-us");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RealtimePPUR());
        }
    }
}
