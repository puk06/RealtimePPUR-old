using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Octokit;

namespace RealtimePPUR
{
    public partial class RealtimePPUR : Form
    {
        private System.Windows.Forms.Label CurrentPP;
        private System.Windows.Forms.Label SR;
        private System.Windows.Forms.Label SSPP;
        private System.Windows.Forms.Label GOOD;
        private System.Windows.Forms.Label OK;
        private System.Windows.Forms.Label MISS;
        private System.Windows.Forms.Label AVGOFFSET;
        private System.Windows.Forms.Label UR;
        private System.Windows.Forms.Label AVGOFFSETHELP;
        private Point mousePoint;
        private int status;
        private int mode;
        private bool isosumode;
        private string displayFormat;
        private bool nowPlaying;
        private int bad;
        private int katu;
        private int geki;
        private int currentGamemode;
        private int x, y;
        private static Process _ppurProcess;
        private static Process _gosumemoryProcess;
        private static bool gosumemoryLaunched = false;
        Dictionary<string, string> configDictionary = new Dictionary<string, string>();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        //フォントをローカルファイルから読み込みます。
        public PrivateFontCollection FontCollection;
        public RealtimePPUR()
        {
            launchSoftwares();
            string filePath = "Config.txt";
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                // 各行を名前と値に分割
                string[] parts = line.Split('=');

                if (parts.Length == 2)
                {
                    string name = parts[0].Trim();
                    string value = parts[1].Trim();
                    configDictionary[name] = value;
                }
            }
            FontCollection = new PrivateFontCollection();
            FontCollection.AddFontFile("./src/Fonts/MPLUSRounded1c-ExtraBold.ttf");
            FontCollection.AddFontFile("./src/Fonts/Nexa Light.otf");
            InitializeComponent();
            float fontsize;
            var result = float.TryParse(configDictionary["FONTSIZE"], out fontsize);
            if (!result)
            {
                MessageBox.Show("Config.txtのFONTSIZEの値が不正です。初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                inGameValue.Font = new Font(FontCollection.Families[0], 19F);
            }
            else
            {
                inGameValue.Font = new Font(FontCollection.Families[0], fontsize);
            }
            if (configDictionary["SR"] == "true")
                sRToolStripMenuItem.Checked = true;
            if (configDictionary["SSPP"] == "true")
                sSPPToolStripMenuItem.Checked = true;
            if (configDictionary["CURRENTPP"] == "true")
                currentPPToolStripMenuItem.Checked = true;
            if (configDictionary["HITS"] == "true")
                hitsToolStripMenuItem.Checked = true;
            if (configDictionary["UR"] == "true")
                uRToolStripMenuItem.Checked = true;
            if (configDictionary["AVGOFFSET"] == "true")
                avgOffsetToolStripMenuItem.Checked = true;
            if (configDictionary["OFFSETHELP"] == "true")
                offsetHelpToolStripMenuItem.Checked = true;
            githubUpdateChecker();
            updateLoop();
        }

        private void realtimePPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientSize = new Size(316, 65);
            BackgroundImage = Properties.Resources.PP;
            if (mode == 2)
            {
                foreach (Control control in Controls)
                {
                    if (control.Name == "inGameValue") continue;
                    control.Location = new Point(control.Location.X, control.Location.Y + 65);
                }
            }
            mode = 1;
        }

        private void offsetHelperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundImage = Properties.Resources.UR;
            ClientSize = new Size(316, 65);
            if (mode == 0)
            {
                foreach (Control control in Controls)
                {
                    if (control.Name == "inGameValue") continue;
                    control.Location = new Point(control.Location.X, control.Location.Y - 65);
                }
            }
            else if (mode == 1)
            {
                foreach (Control control in Controls)
                {
                    if (control.Name == "inGameValue") continue;
                    control.Location = new Point(control.Location.X, control.Location.Y - 65);
                }
            }
            mode = 2;
        }

        private void realtimePPURToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundImage = Properties.Resources.PPUR;
            ClientSize = new Size(316, 130);
            if (mode == 2)
            {
                foreach (Control control in Controls)
                {
                    if (control.Name == "inGameValue") continue;
                    control.Location = new Point(control.Location.X, control.Location.Y + 65);
                }
            }
            mode = 0;
        }

        private void updateLoop()
        {
            Timer timer = new Timer();
            timer.Interval = 1;
            timer.Tick += async (sender, e) =>
            {
                using (HttpClient client = new HttpClient())
                {
                    string dataurl = "http://127.0.0.1:3000/";
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(dataurl);
                        string json = await response.Content.ReadAsStringAsync();
                        JObject data = JsonConvert.DeserializeObject<JObject>(json);

                        double sr = (double)data["PP"]["SR"];
                        double sspp = (double)data["PP"]["SSPP"];
                        double currentPP = (double)data["PP"]["CurrentPP"];
                        double Good = (double)data["PP"]["good"];
                        double Ok = (double)data["PP"]["ok"];
                        double Miss = (double)data["PP"]["miss"];
                        double AvgOffset = -(double)data["Hiterror"]["AvgOffset"];
                        double ur = (double)data["Hiterror"]["UR"];
                        double AvgOffsethelp = -(AvgOffset);
                        bad = (int)data["PP"]["bad"];
                        katu = (int)data["PP"]["katu"];
                        geki = (int)data["PP"]["geki"];
                        currentGamemode = (int)data["PP"]["mode"];
                        status = (int)data["PP"]["status"];

                        AVGOFFSET.Text = AvgOffset + "ms";
                        UR.Text = ur.ToString("F0");
                        AVGOFFSETHELP.Text = AvgOffsethelp.ToString("F0");

                        CurrentPP.Text = currentPP.ToString("F0");
                        int textWidth = TextRenderer.MeasureText(CurrentPP.Text, CurrentPP.Font).Width;
                        CurrentPP.Width = textWidth;
                        CurrentPP.Left = ClientSize.Width - CurrentPP.Width - 35;

                        SR.Text = sr.ToString();
                        SSPP.Text = sspp.ToString("F0");

                        GOOD.Text = Good.ToString();
                        int textWidthGood = TextRenderer.MeasureText(GOOD.Text, GOOD.Font).Width;
                        GOOD.Width = textWidthGood;
                        GOOD.Left = ((ClientSize.Width - GOOD.Width) / 2) - 120;


                        OK.Text = Ok.ToString();
                        int textWidthOK = TextRenderer.MeasureText(OK.Text, OK.Font).Width;
                        OK.Width = textWidthOK;
                        OK.Left = ((ClientSize.Width - OK.Width) / 2) - 61;

                        MISS.Text = Miss.ToString();
                        int textWidthMiss = TextRenderer.MeasureText(MISS.Text, MISS.Font).Width;
                        MISS.Width = textWidthMiss;
                        MISS.Left = ((ClientSize.Width - MISS.Width) / 2) - 3;

                        displayFormat = "";
                        if (sRToolStripMenuItem.Checked) displayFormat += "SR: " + sr + "\n";
                        if (sSPPToolStripMenuItem.Checked) displayFormat += "SSPP: " + sspp.ToString("F0") + "pp\n";
                        if (currentPPToolStripMenuItem.Checked) displayFormat += "PP: " + currentPP.ToString("F0") + "pp\n";
                        if (hitsToolStripMenuItem.Checked)
                        {
                            if (currentGamemode == 0 || currentGamemode == 2)
                            {
                                displayFormat += $"Hits: {Good}/{Ok}/{bad}/{Miss}" + "\n";
                            }
                            else if (currentGamemode == 1)
                            {
                                displayFormat += $"Hits: {Good}/{Ok}/{Miss}" + "\n";
                            }
                            else if (currentGamemode == 2)
                            {
                                displayFormat += $"Hits: {geki}/{Good}/{katu}/{Ok}/{bad}/{Miss}" + "\n";
                            }
                            else
                            {
                                displayFormat += $"Hits: {geki}/{Good}/{katu}/{Miss}" + "\n";
                            }
                        }
                        if (uRToolStripMenuItem.Checked) displayFormat += "UR: " + ur.ToString("F0") + "\n";
                        if (avgOffsetToolStripMenuItem.Checked) displayFormat += "AvgOffset: " + AvgOffset + "\n";
                        if (offsetHelpToolStripMenuItem.Checked) displayFormat += "OffsetHelp: " + AvgOffsethelp.ToString("F0") + "\n";
                    }
                    catch
                    {
                        SR.Text = "0";
                        SSPP.Text = "0";
                        CurrentPP.Text = "0";
                        GOOD.Text = "0";
                        OK.Text = "0";
                        MISS.Text = "0";
                        AVGOFFSET.Text = "0ms";
                        UR.Text = "0";
                        AVGOFFSETHELP.Text = "0";
                    }
                }
            };
            timer.Start();
        }

        private void RealtimePPUR_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                mousePoint = new Point(e.X, e.Y);
            }
        }

        private void RealtimePPUR_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Left += e.X - mousePoint.X;
                Top += e.Y - mousePoint.Y;
            }
        }

        async void githubUpdateChecker()
        {
            string softwareReleasesLatest = "https://github.com/puk06/RealtimePPUR/releases/latest";
            StreamReader currentVersion = new StreamReader("./version.txt");
            string currentVersionString = currentVersion.ReadToEnd();
            currentVersion.Close();
            var owner = "puk06";
            var repo = "RealtimePPUR";

            var githubClient = new GitHubClient(new ProductHeaderValue("RealtimePPUR"));

            try
            {
                var latestRelease = await githubClient.Repository.Release.GetLatest(owner, repo);

                if (latestRelease.Name != currentVersionString)
                {
                    DialogResult result = MessageBox.Show($"最新バージョンがあります！\n\n現在: {currentVersionString} \n更新後: {latestRelease.TagName}\n\nダウンロードページを開きますか？", "アップデートのお知らせ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        Process.Start(softwareReleasesLatest);
                    }
                }
            }
            catch
            {
                MessageBox.Show("アップデートチェック中にエラーが発生しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void osuModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int left = 0;
            int top = 0;
            var leftresult = int.TryParse(configDictionary["LEFT"], out left);
            var topresult = int.TryParse(configDictionary["TOP"], out top);
            if (!leftresult || !topresult)
            {
                MessageBox.Show("Config.txtのLEFTまたはTOPの値が不正です。数値以外入力しないでください。", "エラー", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                osuModeToolStripMenuItem.Checked = false;
                isosumode = false;
                return;
            }
            isosumode = isosumode ? false : true;
            if (isosumode)
            {
                osuModeToolStripMenuItem.Checked = true;
            }
            else
            {
                osuModeToolStripMenuItem.Checked = false;
            }

            Timer timer = new Timer();
            timer.Interval = 1;
            timer.Tick += (senderTimer, eTimer) =>
            {
                if (isosumode)
                {
                    if (status != 2)
                    {
                        if (mode == 0)
                        {
                            BackgroundImage = Properties.Resources.PPUR;
                            ClientSize = new Size(316, 130);
                        }
                        else if (mode == 1)
                        {
                            BackgroundImage = Properties.Resources.PP;
                            ClientSize = new Size(316, 65);
                        }
                        else if (mode == 2)
                        {
                            BackgroundImage = Properties.Resources.UR;
                            ClientSize = new Size(316, 65);
                        }

                        if (nowPlaying)
                        {
                            Location = new Point(x, y);
                            nowPlaying = false;
                        }

                        inGameValue.Visible = false;
                        SR.Visible = true;
                        SSPP.Visible = true;
                        CurrentPP.Visible = true;
                        GOOD.Visible = true;
                        OK.Visible = true;
                        MISS.Visible = true;
                        AVGOFFSET.Visible = true;
                        UR.Visible = true;
                        AVGOFFSETHELP.Visible = true;
                        return;
                    }

                    // 現在のformの位置を取得
                    string processNameToFind = "osu!";
                    Process[] processes = Process.GetProcessesByName(processNameToFind);

                    if (processes.Length > 0)
                    {
                        Process osuProcess = processes[0];
                        IntPtr osuMainWindowHandle = osuProcess.MainWindowHandle;
                        if (GetForegroundWindow() != osuMainWindowHandle)
                        {
                            if (mode == 0)
                            {
                                BackgroundImage = Properties.Resources.PPUR;
                                ClientSize = new Size(316, 130);
                            }
                            else if (mode == 1)
                            {
                                BackgroundImage = Properties.Resources.PP;
                                ClientSize = new Size(316, 65);
                            }
                            else if (mode == 2)
                            {
                                BackgroundImage = Properties.Resources.UR;
                                ClientSize = new Size(316, 65);
                            }

                            if (nowPlaying)
                            {
                                Location = new Point(x, y);
                                nowPlaying = false;
                            }

                            inGameValue.Visible = false;
                            SR.Visible = true;
                            SSPP.Visible = true;
                            CurrentPP.Visible = true;
                            GOOD.Visible = true;
                            OK.Visible = true;
                            MISS.Visible = true;
                            AVGOFFSET.Visible = true;
                            UR.Visible = true;
                            AVGOFFSETHELP.Visible = true;
                            return;
                        }

                        if (osuMainWindowHandle != IntPtr.Zero)
                        {
                            RECT rect;
                            if (GetWindowRect(osuMainWindowHandle, out rect))
                            {
                                if (!nowPlaying)
                                {
                                    x = Location.X;
                                    y = Location.Y;
                                    nowPlaying = true;
                                }

                                BackgroundImage = null;
                                inGameValue.Visible = true;
                                AVGOFFSETHELP.Visible = false;
                                SR.Visible = false;
                                SSPP.Visible = false;
                                CurrentPP.Visible = false;
                                GOOD.Visible = false;
                                OK.Visible = false;
                                MISS.Visible = false;
                                AVGOFFSET.Visible = false;
                                UR.Visible = false;
                                Size = new Size(inGameValue.Width, inGameValue.Height);
                                inGameValue.Text = displayFormat;
                                Location = new Point(rect.Left + left + 2, rect.Top + top);
                            }
                        }
                    }
                }
                else
                {
                    if (mode == 0)
                    {
                        BackgroundImage = Properties.Resources.PPUR;
                        ClientSize = new Size(316, 130);
                    }
                    else if (mode == 1)
                    {
                        BackgroundImage = Properties.Resources.PP;
                        ClientSize = new Size(316, 65);
                    }
                    else if (mode == 2)
                    {
                        BackgroundImage = Properties.Resources.UR;
                        ClientSize = new Size(316, 65);
                    }

                    if (nowPlaying)
                    {
                        Location = new Point(x, y);
                        nowPlaying = false;
                    }

                    inGameValue.Visible = false;
                    SR.Visible = true;
                    SSPP.Visible = true;
                    CurrentPP.Visible = true;
                    GOOD.Visible = true;
                    OK.Visible = true;
                    MISS.Visible = true;
                    AVGOFFSET.Visible = true;
                    UR.Visible = true;
                    AVGOFFSETHELP.Visible = true;
                }
            };
            timer.Start();
        }

        private void launchSoftwares()
        {
            // Launch gosumemory
            if (Process.GetProcessesByName("gosumemory").Length == 0)
            {
                try
                {
                    if (!File.Exists("./src/gosumemory/gosumemory.exe"))
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
                    gosumemoryLaunched = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Gosumemoryの起動に失敗しました。\nエラー内容: {e}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }

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
            catch (Exception e)
            {
                MessageBox.Show($"PPUR.jsの起動に失敗しました。\nエラー内容: {e}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void RealtimePPUR_Closed(object sender, EventArgs e)
        {
            try
            {
                if (gosumemoryLaunched)
                {
                    _gosumemoryProcess.Kill();
                }

                _ppurProcess.Kill();
            }
            catch
            {
                // ignored
            }
        }

        private void sRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sRToolStripMenuItem.Checked = sRToolStripMenuItem.Checked ? false : true;
        }

        private void currentPPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPPToolStripMenuItem.Checked = sSPPToolStripMenuItem.Checked ? false : true;
        }

        private void sSPPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sSPPToolStripMenuItem.Checked = sSPPToolStripMenuItem.Checked ? false : true;
        }

        private void hitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hitsToolStripMenuItem.Checked = hitsToolStripMenuItem.Checked ? false : true;
        }

        private void uRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uRToolStripMenuItem.Checked = uRToolStripMenuItem.Checked ? false : true;
        }

        private void offsetHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            offsetHelpToolStripMenuItem.Checked = offsetHelpToolStripMenuItem.Checked ? false : true;
        }

        private void avgOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            avgOffsetToolStripMenuItem.Checked = avgOffsetToolStripMenuItem.Checked ? false : true;
        }
    }
}
