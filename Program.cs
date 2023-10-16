using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;

namespace RealtimePPUR
{
    internal static class Program
    {
        private static Process _ppurProcess;
        private static Process _gosumemoryProcess;

        [STAThread]
        static void Main()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us");
            CultureInfo.CurrentUICulture = new CultureInfo("en-us");

            try
            {
                //Launch PPUR.js
                _ppurProcess = new Process();
                _ppurProcess.StartInfo.FileName = "./src/nodejs/node.exe";
                _ppurProcess.StartInfo.Arguments = "./src/PPUR.js";
                _ppurProcess.StartInfo.CreateNoWindow = true;
                _ppurProcess.StartInfo.UseShellExecute = false;
                _ppurProcess.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("PPUR.jsの起動に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Launch gosumemory
            if (Process.GetProcessesByName("gosumemory").Length == 0)
            {
                try
                {
                    if (!System.IO.File.Exists("./src/gosumemory/gosumemory.exe"))
                    {
                        DialogResult result = MessageBox.Show("Gosumemoryがフォルダ内から見つかりませんでした。\nGithubからダウンロードしますか？", "エラー", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (result == DialogResult.Yes)
                        {
                            MessageBox.Show("ダウンロードページをwebブラウザで開きます。\nインストール方法: ダウンロードしたフォルダを開き、./src/gosumemory/gosumemory.exeとなるように配置する。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start("https://github.com/l3lackShark/gosumemory/releases/");
                        }

                        return;
                    }

                    _gosumemoryProcess = new Process();
                    _gosumemoryProcess.StartInfo.FileName = "./src/gosumemory/gosumemory.exe";
                    _gosumemoryProcess.StartInfo.CreateNoWindow = true;
                    _gosumemoryProcess.StartInfo.UseShellExecute = false;
                    _gosumemoryProcess.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("Gosumemoryの起動に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Application.ApplicationExit += Application_ApplicationExit;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RealtimePPUR());
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            _ppurProcess.Kill();
            _gosumemoryProcess.Kill();
        }
    }
}
