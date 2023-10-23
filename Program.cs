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
            if (Process.GetProcessesByName("RealtimePPUR").Length > 1)
            {
                MessageBox.Show("RealtimePPURは既に起動しています。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CultureInfo.CurrentCulture = new CultureInfo("en-us");
            CultureInfo.CurrentUICulture = new CultureInfo("en-us");
            Application.ApplicationExit += Application_ApplicationExit;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RealtimePPUR());
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                RealtimePPUR._gosumemoryProcess.Kill();
            }
            catch
            {
                // ignored
            }

            try
            {
                RealtimePPUR._ppurProcess.Kill();
            }
            catch
            {
                // ignored
            }
        }
    }
}
