using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace RealtimePPUR
{
    internal static class Program
    {
        public static Process _ppurProcess;
        private static Process _gosumemoryProcess;

        [STAThread]
        private static void Main()
        {
            if (Process.GetProcessesByName("RealtimePPUR").Length > 1)
            {
                MessageBox.Show("RealtimePPURは既に起動しています。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists("./src/Fonts/MPLUSRounded1c-ExtraBold.ttf"))
            {
                MessageBox.Show("MPLUSRounded1c-ExtraBoldフォントファイルが存在しません。ソフトをもう一度ダウンロードしてください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists("./src/Fonts/Nexa Light.otf"))
            {
                MessageBox.Show("Nexa Lightフォントファイルが存在しません。ソフトをもう一度ダウンロードしてください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Process.GetProcessesByName("gosumemory").Length == 0)
            {
                try
                {
                    if (!File.Exists("./src/gosumemory/gosumemory.exe"))
                    {
                        DialogResult result = MessageBox.Show("Gosumemoryがフォルダ内から見つかりませんでした。\nGithubからダウンロードしますか？", "エラー", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (result != DialogResult.Yes)
                        {
                            return;
                        }

                        MessageBox.Show("ダウンロードページをwebブラウザで開きます。\nインストール方法: ダウンロードしたフォルダを開き、\"src/gosumemory/gosumemory.exe\"となるように配置する。config.iniもgosumemory.exeと同じフォルダに入れてください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Process.Start("https://github.com/l3lackShark/gosumemory/releases/");
                        return;
                    }

                    _gosumemoryProcess = new Process();
                    _gosumemoryProcess.StartInfo.FileName = "./src/gosumemory/gosumemory.exe";
                    _gosumemoryProcess.StartInfo.CreateNoWindow = true;
                    _gosumemoryProcess.StartInfo.UseShellExecute = false;
                    _gosumemoryProcess.Start();
                }
                catch (Exception error)
                {
                    MessageBox.Show($"gosumemoryの起動に失敗しました。\nエラー内容: {error}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                if (!File.Exists("./src/nodejs/node.exe"))
                {
                    MessageBox.Show("Node.exeがフォルダ内から見つかりませんでした。このソフトをGithubからもう一度ダウンロードしてください。", "エラー", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    return;
                }

                if (!File.Exists("./src/PPUR.js"))
                {
                    MessageBox.Show("PPUR.jsがフォルダ内から見つかりませんでした。このソフトをGithubからもう一度ダウンロードしてください。", "エラー", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    return;
                }

                _ppurProcess = new Process();
                _ppurProcess.StartInfo.FileName = "./src/nodejs/node.exe";
                _ppurProcess.StartInfo.Arguments = "./src/PPUR.js";
                _ppurProcess.StartInfo.CreateNoWindow = true;
                _ppurProcess.StartInfo.UseShellExecute = false;
                _ppurProcess.Start();
            }
            catch (Exception error)
            {
                MessageBox.Show($"PPUR.jsの起動に失敗しました。\nエラー内容: {error}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                
                CultureInfo.CurrentCulture = new CultureInfo("en-us");
                CultureInfo.CurrentUICulture = new CultureInfo("en-us");
                Application.ApplicationExit += Application_ApplicationExit;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new RealtimePpur());    
            }
            catch
            {
                try
                {
                    if (_gosumemoryProcess != null && !_gosumemoryProcess.HasExited)
                    {
                        _gosumemoryProcess.Kill();
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show($"gosumemoryの終了に失敗しました。\nエラー内容: {error}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    if (_ppurProcess != null && !_ppurProcess.HasExited)
                    {
                        _ppurProcess.Kill();
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show($"PPUR.jsの終了に失敗しました。\nエラー内容: {error}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                MessageBox.Show($"ソフトの起動に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                if (_gosumemoryProcess != null && !_gosumemoryProcess.HasExited)
                {
                    _gosumemoryProcess.Kill();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show($"gosumemoryの終了に失敗しました。\nエラー内容: {error}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                if (_ppurProcess != null && !_ppurProcess.HasExited)
                {
                    _ppurProcess.Kill();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show($"PPUR.jsの終了に失敗しました。\nエラー内容: {error}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
