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
using System.Globalization;
using System.Runtime.InteropServices;
using Octokit;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Drawing.Drawing2D;

namespace RealtimePPUR
{
    public sealed partial class RealtimePpur : Form
    {
        private System.Windows.Forms.Label _currentPp, _sr, _sspp, _good, _ok, _miss, _avgoffset, _ur, _avgoffsethelp;
        private Point _mousePoint;
        private string _displayFormat;
        private int _status;
        private int _mode;
        private int _x;
        private int _y;
        private int _prevCalculationSpeed;
        private readonly int _calculationSpeedDetectedValue;
        private bool _isosumode;
        private bool _nowPlaying;
        private readonly bool _speedReduction;
        private int _currentBackgroundImage = 1;
        private readonly string _ingameoverlayPriority;
        private readonly Dictionary<string, string> _configDictionary = new Dictionary<string, string>();
        private readonly HttpClient _client = new HttpClient();
        private readonly PrivateFontCollection _fontCollection;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect rect);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left, Top, Right, Bottom;
        }

        public RealtimePpur()
        {
            _fontCollection = new PrivateFontCollection();
            _fontCollection.AddFontFile("./src/Fonts/MPLUSRounded1c-ExtraBold.ttf");
            _fontCollection.AddFontFile("./src/Fonts/Nexa Light.otf");
            InitializeComponent();

            if (!File.Exists("Config.txt"))
            {
                MessageBox.Show("Config.txtがフォルダ内に存在しないため、すべての項目がOffとして設定されます。アップデートチェックのみ行われます。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GithubUpdateChecker();
                sRToolStripMenuItem.Checked = false;
                sSPPToolStripMenuItem.Checked = false;
                currentPPToolStripMenuItem.Checked = false;
                currentACCToolStripMenuItem.Checked = false;
                hitsToolStripMenuItem.Checked = false;
                uRToolStripMenuItem.Checked = false;
                offsetHelpToolStripMenuItem.Checked = false;
                avgOffsetToolStripMenuItem.Checked = false;
                progressToolStripMenuItem.Checked = false;
                ifFCPPToolStripMenuItem.Checked = false;
                ifFCHitsToolStripMenuItem.Checked = false;
                expectedManiaScoreToolStripMenuItem.Checked = false;
                healthPercentageToolStripMenuItem.Checked = false;
                _speedReduction = false;
                _calculationSpeedDetectedValue = 100;
                _ingameoverlayPriority = "1/2/3/4/5/6/7/8/9/10/11/12";
                inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
            }
            else
            {
                string[] lines = File.ReadAllLines("Config.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');

                    if (parts.Length != 2) continue;
                    string name = parts[0].Trim();
                    string value = parts[1].Trim();
                    _configDictionary[name] = value;
                }

                if (_configDictionary.TryGetValue("UPDATECHECK", out string test11) && test11 == "true")
                {
                    GithubUpdateChecker();
                }

                var defaultmodeTest = _configDictionary.TryGetValue("DEFAULTMODE", out string defaultmodestring);
                if (defaultmodeTest)
                {
                    var defaultModeResult = int.TryParse(defaultmodestring, out int defaultmode);
                    if (!defaultModeResult || !(defaultmode == 0 || defaultmode == 1 || defaultmode == 2))
                    {
                        MessageBox.Show("Config.txtのDEFAULTMODEの値が不正であったため、初期値の0が適用されます。0、1、2のどれかを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        switch (defaultmode)
                        {
                            case 1:
                                ClientSize = new Size(316, 65);
                                BackgroundImage = Properties.Resources.PP;
                                _currentBackgroundImage = 2;
                                RoundCorners();
                                _mode = 1;
                                break;

                            case 2:
                                ClientSize = new Size(316, 65);
                                BackgroundImage = Properties.Resources.UR;
                                _currentBackgroundImage = 3;
                                RoundCorners();
                                foreach (Control control in Controls)
                                {
                                    if (control.Name == "inGameValue") continue;
                                    control.Location = new Point(control.Location.X, control.Location.Y - 65);
                                }
                                _mode = 2;
                                break;
                        }
                    }
                }

                sRToolStripMenuItem.Checked = _configDictionary.TryGetValue("SR", out string test) && test == "true";
                sSPPToolStripMenuItem.Checked = _configDictionary.TryGetValue("SSPP", out string test2) && test2 == "true";
                currentPPToolStripMenuItem.Checked = _configDictionary.TryGetValue("CURRENTPP", out string test3) && test3 == "true";
                currentACCToolStripMenuItem.Checked = _configDictionary.TryGetValue("CURRENTACC", out string test4) && test4 == "true";
                hitsToolStripMenuItem.Checked = _configDictionary.TryGetValue("HITS", out string test5) && test5 == "true";
                uRToolStripMenuItem.Checked = _configDictionary.TryGetValue("UR", out string test6) && test6 == "true";
                offsetHelpToolStripMenuItem.Checked = _configDictionary.TryGetValue("OFFSETHELP", out string test7) && test7 == "true";
                avgOffsetToolStripMenuItem.Checked = _configDictionary.TryGetValue("AVGOFFSET", out string test8) && test8 == "true";
                progressToolStripMenuItem.Checked = _configDictionary.TryGetValue("PROGRESS", out string test9) && test9 == "true";
                ifFCPPToolStripMenuItem.Checked = _configDictionary.TryGetValue("IFFCPP", out string test13) && test13 == "true";
                ifFCHitsToolStripMenuItem.Checked = _configDictionary.TryGetValue("IFFCHITS", out string test14) && test14 == "true";
                expectedManiaScoreToolStripMenuItem.Checked = _configDictionary.TryGetValue("EXPECTEDMANIASCORE", out string test15) && test15 == "true";
                healthPercentageToolStripMenuItem.Checked = _configDictionary.TryGetValue("HEALTHPERCENTAGE", out string test17) && test17 == "true";
                _speedReduction = _configDictionary.TryGetValue("SPEEDREDUCTION", out string test10) && test10 == "true";
                _ingameoverlayPriority = _configDictionary.TryGetValue("INGAMEOVERLAYPRIORITY", out string test16) ? test16 : "1/2/3/4/5/6/7/8/9/10/11/12";

                if (_configDictionary.TryGetValue("USECUSTOMFONT", out string test12) && test12 == "true")
                {
                    if (File.Exists("Font"))
                    {
                        var fontDictionary = new Dictionary<string, string>();
                        string[] fontInfo = File.ReadAllLines("Font");
                        foreach (string line in fontInfo)
                        {
                            string[] parts = line.Split('=');
                            if (parts.Length != 2) continue;
                            string name = parts[0].Trim();
                            string value = parts[1].Trim();
                            fontDictionary[name] = value;
                        }

                        var fontName = fontDictionary.TryGetValue("FONTNAME", out string fontNameValue);
                        var fontSize = fontDictionary.TryGetValue("FONTSIZE", out string fontSizeValue);
                        var fontStyle = fontDictionary.TryGetValue("FONTSTYLE", out string fontStyleValue);

                        if (fontDictionary.Count == 3 && fontName && fontNameValue != "" && fontSize && fontSizeValue != "" && fontStyle && fontStyleValue != "")
                        {
                            try
                            {
                                inGameValue.Font = new Font(fontNameValue, float.Parse(fontSizeValue),
                                    (FontStyle)Enum.Parse(typeof(FontStyle), fontStyleValue));
                            }
                            catch
                            {
                                MessageBox.Show("Fontファイルのフォント情報が不正であったため、デフォルトのフォントが適用されます。一度Fontファイルを削除してみることをお勧めします。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                var fontsizeResult = _configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
                                if (!fontsizeResult)
                                {
                                    MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                                }
                                else
                                {
                                    var result = float.TryParse(fontsizeValue, out float fontsize);
                                    if (!result)
                                    {
                                        MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                                    }
                                    else
                                    {
                                        inGameValue.Font = new Font(_fontCollection.Families[0], fontsize);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Fontファイルのフォント情報が不正であったため、デフォルトのフォントが適用されます。一度Fontファイルを削除してみることをお勧めします。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            var fontsizeResult = _configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
                            if (!fontsizeResult)
                            {
                                MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                            }
                            else
                            {
                                var result = float.TryParse(fontsizeValue, out float fontsize);
                                if (!result)
                                {
                                    MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                                }
                                else
                                {
                                    inGameValue.Font = new Font(_fontCollection.Families[0], fontsize);
                                }
                            }
                        }
                    }
                    else
                    {
                        var fontsizeResult = _configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
                        if (!fontsizeResult)
                        {
                            MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                        }
                        else
                        {
                            var result = float.TryParse(fontsizeValue, out float fontsize);
                            if (!result)
                            {
                                MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                            }
                            else
                            {
                                inGameValue.Font = new Font(_fontCollection.Families[0], fontsize);
                            }
                        }
                    }
                }
                else
                {
                    var fontsizeResult = _configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
                    if (!fontsizeResult)
                    {
                        MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                    }
                    else
                    {
                        var result = float.TryParse(fontsizeValue, out float fontsize);
                        if (!result)
                        {
                            MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                        }
                        else
                        {
                            inGameValue.Font = new Font(_fontCollection.Families[0], fontsize);
                        }
                    }
                }

                var speedReductionValueResult = _configDictionary.TryGetValue("SPEEDREDUCTIONVALUE", out string speedReductionValue);
                if (!speedReductionValueResult && _speedReduction)
                {
                    MessageBox.Show("Config.txtにSPEEDREDUCTIONVALUEの値が存在しないため、初期値の100が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _calculationSpeedDetectedValue = 100;
                }
                else if (_speedReduction)
                {
                    var tryResult = int.TryParse(speedReductionValue, out _calculationSpeedDetectedValue);
                    if (!tryResult)
                    {
                        MessageBox.Show("Config.txtのSPEEDREDUCTIONVALUEの値が不正であったため、初期値の100が設定されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _calculationSpeedDetectedValue = 100;
                    }
                }
            }
            UpdateLoop();
        }

        private void realtimePPURToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_mode == 0) return;
            ClientSize = new Size(316, 130);
            BackgroundImage = Properties.Resources.PPUR;
            _currentBackgroundImage = 1;
            RoundCorners();
            if (_mode == 2)
            {
                foreach (Control control in Controls)
                {
                    if (control.Name == "inGameValue") continue;
                    control.Location = new Point(control.Location.X, control.Location.Y + 65);
                }
            }
            _mode = 0;
        }

        private void realtimePPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_mode == 1) return;
            ClientSize = new Size(316, 65);
            BackgroundImage = Properties.Resources.PP;
            _currentBackgroundImage = 2;
            RoundCorners();
            if (_mode == 2)
            {
                foreach (Control control in Controls)
                {
                    if (control.Name == "inGameValue") continue;
                    control.Location = new Point(control.Location.X, control.Location.Y + 65);
                }
            }
            _mode = 1;
        }

        private void offsetHelperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_mode == 2) return;
            ClientSize = new Size(316, 65);
            BackgroundImage = Properties.Resources.UR;
            _currentBackgroundImage = 3;
            RoundCorners();
            if (_mode == 0 || _mode == 1)
            {
                foreach (Control control in Controls)
                {
                    if (control.Name == "inGameValue") continue;
                    control.Location = new Point(control.Location.X, control.Location.Y - 65);
                }
            }
            _mode = 2;
        }

        private void changeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog font = new FontDialog();
            try
            {
                if (font.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                inGameValue.Font = font.Font;
                DialogResult fontfDialogResult = MessageBox.Show("このフォントを保存しますか？", "情報", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);
                if (fontfDialogResult == DialogResult.No)
                {
                    return;
                }

                try
                {
                    if (File.Exists("Font"))
                    {
                        const string filePath = "Font";
                        StreamWriter sw = new StreamWriter(filePath, false);
                        string fontInfo =
                            $"※絶対にこのファイルを自分で編集しないでください！\n※フォント名などを編集してしまうとフォントが見つからず、Windows標準のフォントが割り当てられてしまいます。\n※もし編集してしまった場合はこのファイルを削除することをお勧めします。\nFONTNAME={font.Font.Name}\nFONTSIZE={font.Font.Size}\nFONTSTYLE={font.Font.Style}";
                        sw.WriteLine(fontInfo);
                        sw.Close();
                        MessageBox.Show(
                            "フォントの保存に成功しました。Config.txtのUSECUSTOMFONTをtrueにすることで起動時から保存されたフォントを使用できます。右クリック→Load Fontからでも読み込むことが可能です！",
                            "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        FileStream fs = File.Create("Font");
                        string fontInfo =
                            $"※絶対にこのファイルを自分で編集しないでください！\n※フォント名などを編集してしまうとフォントが見つからず、Windows標準のフォントが割り当てられてしまいます。\n※もし編集してしまった場合はこのファイルを削除することをお勧めします。\nFONTNAME={font.Font.Name}\nFONTSIZE={font.Font.Size}\nFONTSTYLE={font.Font.Style}";
                        byte[] fontInfoByte = System.Text.Encoding.UTF8.GetBytes(fontInfo);
                        fs.Write(fontInfoByte, 0, fontInfoByte.Length);
                        fs.Close();
                        MessageBox.Show(
                            "フォントの保存に成功しました。Config.txtのUSECUSTOMFONTをtrueにすることで起動時から保存されたフォントを使用できます。右クリック→Load Fontからでも読み込むことが可能です！",
                            "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch
                {
                    MessageBox.Show("フォントの保存に失敗しました。もしFontファイルが作成されていたら削除することをお勧めします。", "エラー", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("フォントの変更に失敗しました。対応していないフォントです。", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ReSharper disable once FunctionRecursiveOnAllPaths
        private async void UpdateLoop()
        {
            try
            {
                if (Process.GetProcessesByName("gosumemory").Length == 0)
                {
                    MessageBox.Show("Gosumemoryがクラッシュした、もしくはGosumemoryを起動していたソフトが閉じられた可能性があります。ソフトを再起動してください。",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }

                if (Program._ppurProcess.HasExited)
                {
                    MessageBox.Show("PPUR.jsがクラッシュした可能性があります。\n\n対処法: このソフトを閉じた後にタスクマネージャーを開き、Node.js JavaScript Runtime、もしくはRealtimePPURを全て閉じ、ソフトを再起動してください。\n\nこのエラーはほとんどの場合、既に裏でPPUR.jsが起動しているときに起きます。", "エラー", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Close();
                }

                HttpResponseMessage response = await _client.GetAsync("http://127.0.0.1:3000/");
                string json = await response.Content.ReadAsStringAsync();
                JObject data = JsonConvert.DeserializeObject<JObject>(json);

                double sr = (double)data["PP"]["SR"];
                double fullSr = (double)data["PP"]["fullSR"];
                double sspp = (double)data["PP"]["SSPP"];
                double currentPp = (double)data["PP"]["CurrentPP"];
                double ifFcpp = (double)data["PP"]["ifFCPP"];
                int good = (int)data["PP"]["good"];
                int ok = (int)data["PP"]["ok"];
                int miss = (int)data["PP"]["miss"];
                int sliderbreaks = (int)data["PP"]["sliderBreaks"];
                double avgOffset = -(double)data["Hiterror"]["AvgOffset"];
                double ur = (double)data["Hiterror"]["UR"];
                double avgOffsethelp = (double)data["Hiterror"]["AvgOffset"];
                int bad = (int)data["PP"]["bad"];
                int katu = (int)data["PP"]["katu"];
                int geki = (int)data["PP"]["geki"];
                int currentGamemode = (int)data["PP"]["mode"];
                int currentCalculationSpeed = (int)data["calculatingTime"];
                int ifFcGood = (int)data["PP"]["ifFCHits300"];
                int ifFcOk = (int)data["PP"]["ifFCHits100"];
                int ifFcBad = (int)data["PP"]["ifFCHits50"];
                int ifFcMiss = (int)data["PP"]["ifFCHitsMiss"];
                double healthPercentage = (double)data["PP"]["healthBar"];
                _status = (int)data["PP"]["status"];

                if (_prevCalculationSpeed == 0)
                {
                    _prevCalculationSpeed = currentCalculationSpeed;
                }
                else if (currentCalculationSpeed - _prevCalculationSpeed > _calculationSpeedDetectedValue &&
                         _speedReduction)
                {
                    new ToastContentBuilder()
                        .AddText("Calculation Speed Reduction Detected!")
                        .AddText("Calculation speed is slower than usual! \nCurrent Calculation speed: " +
                                 currentCalculationSpeed + "ms")
                        .Show();
                }
                _prevCalculationSpeed = currentCalculationSpeed;

                _avgoffset.Text = avgOffset.ToString(CultureInfo.CurrentCulture = new CultureInfo("en-us")) + "ms";
                _ur.Text = ur.ToString("F0");
                _avgoffsethelp.Text = avgOffsethelp.ToString("F0");
                _sr.Text = sr.ToString(CultureInfo.CurrentCulture = new CultureInfo("en-us"));
                _sspp.Text = sspp.ToString("F0");
                _currentPp.Text = currentPp.ToString("F0");
                _currentPp.Width = TextRenderer.MeasureText(_currentPp.Text, _currentPp.Font).Width;
                _currentPp.Left = ClientSize.Width - _currentPp.Width - 35;

                switch (currentGamemode)
                {
                    case 0:
                        _good.Text = good.ToString();
                        _good.Width = TextRenderer.MeasureText(_good.Text, _good.Font).Width;
                        _good.Left = (ClientSize.Width - _good.Width) / 2 - 120;

                        _ok.Text = (ok + bad).ToString();
                        _ok.Width = TextRenderer.MeasureText(_ok.Text, _ok.Font).Width;
                        _ok.Left = (ClientSize.Width - _ok.Width) / 2 - 61;

                        _miss.Text = miss.ToString();
                        _miss.Width = TextRenderer.MeasureText(_miss.Text, _miss.Font).Width;
                        _miss.Left = (ClientSize.Width - _miss.Width) / 2 - 3;
                        break;

                    case 1:
                        _good.Text = good.ToString();
                        _good.Width = TextRenderer.MeasureText(_good.Text, _good.Font).Width;
                        _good.Left = (ClientSize.Width - _good.Width) / 2 - 120;

                        _ok.Text = ok.ToString();
                        _ok.Width = TextRenderer.MeasureText(_ok.Text, _ok.Font).Width;
                        _ok.Left = (ClientSize.Width - _ok.Width) / 2 - 61;

                        _miss.Text = miss.ToString();
                        _miss.Width = TextRenderer.MeasureText(_miss.Text, _miss.Font).Width;
                        _miss.Left = (ClientSize.Width - _miss.Width) / 2 - 3;
                        break;

                    case 2:
                        _good.Text = good.ToString();
                        _good.Width = TextRenderer.MeasureText(_good.Text, _good.Font).Width;
                        _good.Left = (ClientSize.Width - _good.Width) / 2 - 120;

                        _ok.Text = (ok + bad).ToString();
                        _ok.Width = TextRenderer.MeasureText(_ok.Text, _ok.Font).Width;
                        _ok.Left = (ClientSize.Width - _ok.Width) / 2 - 61;

                        _miss.Text = miss.ToString();
                        _miss.Width = TextRenderer.MeasureText(_miss.Text, _miss.Font).Width;
                        _miss.Left = (ClientSize.Width - _miss.Width) / 2 - 3;
                        break;

                    case 3:
                        _good.Text = (good + geki).ToString();
                        _good.Width = TextRenderer.MeasureText(_good.Text, _good.Font).Width;
                        _good.Left = (ClientSize.Width - _good.Width) / 2 - 120;

                        _ok.Text = (katu + ok + bad).ToString();
                        _ok.Width = TextRenderer.MeasureText(_ok.Text, _ok.Font).Width;
                        _ok.Left = (ClientSize.Width - _ok.Width) / 2 - 61;

                        _miss.Text = miss.ToString();
                        _miss.Width = TextRenderer.MeasureText(_miss.Text, _miss.Font).Width;
                        _miss.Left = (ClientSize.Width - _miss.Width) / 2 - 3;
                        break;
                }

                _displayFormat = "";
                var ingameoverlayPriorityArray = _ingameoverlayPriority.Replace(" ", "").Split('/');
                foreach (var priorityValue in ingameoverlayPriorityArray)
                {
                    var priorityValueResult = int.TryParse(priorityValue, out int priorityValueInt);
                    if (!priorityValueResult) continue;
                    switch (priorityValueInt)
                    {
                        case 1:
                            if (sRToolStripMenuItem.Checked)
                            {
                                _displayFormat += "SR: " +
                                                 sr.ToString(CultureInfo.CurrentCulture = new CultureInfo("en-us")) +
                                                 " / " + fullSr.ToString(CultureInfo.CurrentCulture = new CultureInfo("en-us")) + "\n";
                            }

                            break;

                        case 2:
                            if (sSPPToolStripMenuItem.Checked)
                            {
                                _displayFormat += "SSPP: " + sspp.ToString("F0") + "pp\n";
                            }

                            break;

                        case 3:
                            if (currentPPToolStripMenuItem.Checked)
                            {
                                if (ifFCPPToolStripMenuItem.Checked && currentGamemode != 3)
                                {
                                    _displayFormat += "PP: " + currentPp.ToString("F0") + " / " + ifFcpp.ToString("F0") +
                                                     "pp\n";
                                }
                                else
                                {
                                    _displayFormat += "PP: " + currentPp.ToString("F0") + "pp\n";
                                }
                            }

                            break;

                        case 4:
                            if (currentACCToolStripMenuItem.Checked)
                            {
                                _displayFormat += "ACC: " + data["PP"]["CurrentACC"] + "%\n";
                            }

                            break;

                        case 5:
                            if (hitsToolStripMenuItem.Checked)
                            {
                                switch (currentGamemode)
                                {
                                    case 0:
                                        _displayFormat += $"Hits: {good}/{ok}/{bad}/{miss} ({sliderbreaks})\n";
                                        break;

                                    case 1:
                                        _displayFormat += $"Hits: {good}/{ok}/{miss}\n";
                                        break;

                                    case 2:
                                        _displayFormat += $"Hits: {good}/{ok}/{bad}/{miss}\n";
                                        break;

                                    case 3:
                                        _displayFormat += $"Hits: {geki}/{good}/{katu}/{ok}/{bad}/{miss}\n";
                                        break;
                                }
                            }

                            break;

                        case 6:
                            if (ifFCHitsToolStripMenuItem.Checked)
                            {
                                switch (currentGamemode)
                                {
                                    case 0:
                                        _displayFormat += $"ifFCHits: {ifFcGood}/{ifFcOk}/{ifFcBad}/{ifFcMiss}\n";
                                        break;

                                    case 1:
                                        _displayFormat += $"ifFCHits: {ifFcGood}/{ifFcOk}/{ifFcMiss}\n";
                                        break;

                                    case 2:
                                        _displayFormat += $"ifFCHits: {ifFcGood}/{ifFcOk}/{ifFcBad}/{ifFcMiss}\n";
                                        break;
                                }
                            }

                            break;

                        case 7:
                            if (uRToolStripMenuItem.Checked)
                            {
                                _displayFormat += "UR: " + ur.ToString("F0") + "\n";
                            }

                            break;

                        case 8:
                            if (offsetHelpToolStripMenuItem.Checked)
                            {
                                _displayFormat += "OffsetHelp: " + avgOffsethelp.ToString("F0") + "\n";
                            }

                            break;

                        case 9:
                            if (expectedManiaScoreToolStripMenuItem.Checked && currentGamemode == 3)
                            {
                                _displayFormat += "ManiaScore: " + data["PP"]["expectedManiaScore"] + "\n";
                            }

                            break;

                        case 10:
                            if (avgOffsetToolStripMenuItem.Checked)
                            {
                                _displayFormat += "AvgOffset: " +
                                                 avgOffset.ToString(CultureInfo.CurrentCulture = new CultureInfo("en-us")) + "\n";
                            }

                            break;

                        case 11:
                            if (progressToolStripMenuItem.Checked)
                            {
                                _displayFormat += "Progress: " + data["PP"]["progress"] + "%\n";
                            }

                            break;

                        case 12:
                            if (healthPercentageToolStripMenuItem.Checked)
                            {
                                _displayFormat += "HP: " + healthPercentage + "%\n";
                            }

                            break;
                    }
                }

                inGameValue.Text = _displayFormat;

                response = null;
                json = null;
                data = null;
            }
            catch
            {
                inGameValue.Text = "Error";
                _sr.Text = "0";
                _sspp.Text = "0";
                _currentPp.Text = "0";
                _good.Text = "0";
                _ok.Text = "0";
                _miss.Text = "0";
                _avgoffset.Text = "0ms";
                _ur.Text = "0";
                _avgoffsethelp.Text = "0";
            }
            finally
            {
                UpdateLoop();
            }
        }

        private void loadFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("Font"))
            {
                var fontDictionaryLoad = new Dictionary<string, string>();
                string[] fontInfo = File.ReadAllLines("Font");
                foreach (string line in fontInfo)
                {
                    string[] parts = line.Split('=');
                    if (parts.Length != 2) continue;
                    string name = parts[0].Trim();
                    string value = parts[1].Trim();
                    fontDictionaryLoad[name] = value;
                }

                var fontName = fontDictionaryLoad.TryGetValue("FONTNAME", out string fontNameValue);
                var fontSize = fontDictionaryLoad.TryGetValue("FONTSIZE", out string fontSizeValue);
                var fontStyle = fontDictionaryLoad.TryGetValue("FONTSTYLE", out string fontStyleValue);

                if (fontDictionaryLoad.Count == 3 && fontName && fontNameValue != "" && fontSize && fontSizeValue != "" && fontStyle && fontStyleValue != "")
                {
                    try
                    {
                        inGameValue.Font = new Font(fontNameValue, float.Parse(fontSizeValue), (FontStyle)Enum.Parse(typeof(FontStyle), fontStyleValue));
                        MessageBox.Show($"フォントの読み込みに成功しました。\n\nフォント名: {fontNameValue}\nサイズ: {fontSizeValue}\nスタイル: {fontStyleValue}", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Fontファイルのフォント情報が不正、もしくは非対応であったため読み込まれませんでした。一度Fontファイルを削除してみることをお勧めします。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Fontファイルのフォント情報が不正であったため、読み込まれませんでした。一度Fontファイルを削除してみることをお勧めします。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Fontファイルが存在しません。一度Change Fontでフォントを保存してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void resetFontToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var fontsizeResult = _configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
            if (!fontsizeResult)
            {
                MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                MessageBox.Show("フォントのリセットが完了しました！", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var result = float.TryParse(fontsizeValue, out float fontsize);
                if (!result)
                {
                    MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    inGameValue.Font = new Font(_fontCollection.Families[0], 19F);
                    MessageBox.Show("フォントのリセットが完了しました！", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    inGameValue.Font = new Font(_fontCollection.Families[0], fontsize);
                    MessageBox.Show("フォントのリセットが完了しました！", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void RealtimePPUR_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left) _mousePoint = new Point(e.X, e.Y);
        }

        private void RealtimePPUR_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
            Left += e.X - _mousePoint.X;
            Top += e.Y - _mousePoint.Y;
        }

        private static async void GithubUpdateChecker()
        {
            try
            {
                const string softwareReleasesLatest = "https://github.com/puk06/RealtimePPUR/releases/latest";
                if (!File.Exists("version"))
                {
                    MessageBox.Show("versionファイルが存在しないのでアップデートチェックは無視されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                StreamReader currentVersion = new StreamReader("version");
                string currentVersionString = await currentVersion.ReadToEndAsync();
                currentVersion.Close();
                var githubClient = new GitHubClient(new ProductHeaderValue("RealtimePPUR"));
                var latestRelease = await githubClient.Repository.Release.GetLatest("puk06", "RealtimePPUR");
                if (latestRelease.Name == currentVersionString) return;
                DialogResult result = MessageBox.Show($"最新バージョンがあります！\n\n現在: {currentVersionString} \n更新後: {latestRelease.Name}\n\nダウンロードページを開きますか？", "アップデートのお知らせ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes) Process.Start(softwareReleasesLatest);
            }
            catch
            {
                MessageBox.Show("アップデートチェック中にエラーが発生しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void osuModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lefttest = _configDictionary.TryGetValue("LEFT", out string leftvalue);
            var toptest = _configDictionary.TryGetValue("TOP", out string topvalue);
            if (!lefttest || !toptest)
            {
                MessageBox.Show("Config.txtにLEFTまたはTOPの値が存在しなかったため、osu! Modeの起動に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var leftResult = int.TryParse(leftvalue, out int left);
            var topResult = int.TryParse(topvalue, out int top);
            if ((!leftResult || !topResult) && !_isosumode)
            {
                MessageBox.Show("Config.txtのLEFT、またはTOPの値が不正であったため、osu! Modeの起動に失敗しました。LEFT、TOPには数値以外入力しないでください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _isosumode = !_isosumode;
            osuModeToolStripMenuItem.Checked = _isosumode;

            Timer timer = new Timer
            {
                Interval = 1
            };

            if (_isosumode)
            {
                timer.Start();
                timer.Tick += (o, args) =>
                {
                    if (_isosumode)
                    {
                        var processes = Process.GetProcessesByName("osu!");
                        if (processes.Length > 0)
                        {
                            Process osuProcess = processes[0];
                            IntPtr osuMainWindowHandle = osuProcess.MainWindowHandle;
                            if (GetWindowRect(osuMainWindowHandle, out Rect rect) && _status == 2 && GetForegroundWindow() == osuMainWindowHandle && osuMainWindowHandle != IntPtr.Zero)
                            {
                                if (!_nowPlaying)
                                {
                                    _x = Location.X;
                                    _y = Location.Y;
                                    _nowPlaying = true;
                                }

                                BackgroundImage = null;
                                _currentBackgroundImage = 0;
                                inGameValue.Visible = true;
                                _avgoffsethelp.Visible = false;
                                _sr.Visible = false;
                                _sspp.Visible = false;
                                _currentPp.Visible = false;
                                _good.Visible = false;
                                _ok.Visible = false;
                                _miss.Visible = false;
                                _avgoffset.Visible = false;
                                _ur.Visible = false;
                                Region = null;
                                Size = new Size(inGameValue.Width, inGameValue.Height);
                                Location = new Point(rect.Left + left + 2, rect.Top + top);
                            }
                            else
                            {
                                switch (_mode)
                                {
                                    case 0:
                                        if (_currentBackgroundImage != 1)
                                        {
                                            ClientSize = new Size(316, 130);
                                            RoundCorners();
                                            BackgroundImage = Properties.Resources.PPUR;
                                            _currentBackgroundImage = 1;
                                        }
                                        break;

                                    case 1:
                                        if (_currentBackgroundImage != 2)
                                        {
                                            ClientSize = new Size(316, 65);
                                            RoundCorners();
                                            BackgroundImage = Properties.Resources.PP;
                                            _currentBackgroundImage = 2;
                                        }
                                        break;

                                    case 2:
                                        if (_currentBackgroundImage != 3)
                                        {
                                            ClientSize = new Size(316, 65);
                                            RoundCorners();
                                            BackgroundImage = Properties.Resources.UR;
                                            _currentBackgroundImage = 3;
                                        }
                                        break;
                                }

                                if (_nowPlaying)
                                {
                                    Location = new Point(_x, _y);
                                    _nowPlaying = false;
                                }

                                inGameValue.Visible = false;
                                _sr.Visible = true;
                                _sspp.Visible = true;
                                _currentPp.Visible = true;
                                _good.Visible = true;
                                _ok.Visible = true;
                                _miss.Visible = true;
                                _avgoffset.Visible = true;
                                _ur.Visible = true;
                                _avgoffsethelp.Visible = true;
                            }
                        }
                        else
                        {
                            switch (_mode)
                            {
                                case 0:
                                    if (_currentBackgroundImage != 1)
                                    {
                                        ClientSize = new Size(316, 130);
                                        RoundCorners();
                                        BackgroundImage = Properties.Resources.PPUR;
                                        _currentBackgroundImage = 1;
                                    }
                                    break;

                                case 1:
                                    if (_currentBackgroundImage != 2)
                                    {
                                        ClientSize = new Size(316, 65);
                                        RoundCorners();
                                        BackgroundImage = Properties.Resources.PP;
                                        _currentBackgroundImage = 2;
                                    }
                                    break;

                                case 2:
                                    if (_currentBackgroundImage != 3)
                                    {
                                        ClientSize = new Size(316, 65);
                                        RoundCorners();
                                        BackgroundImage = Properties.Resources.UR;
                                        _currentBackgroundImage = 3;
                                    }
                                    break;
                            }

                            if (_nowPlaying)
                            {
                                Location = new Point(_x, _y);
                                _nowPlaying = false;
                            }

                            inGameValue.Visible = false;
                            _sr.Visible = true;
                            _sspp.Visible = true;
                            _currentPp.Visible = true;
                            _good.Visible = true;
                            _ok.Visible = true;
                            _miss.Visible = true;
                            _avgoffset.Visible = true;
                            _ur.Visible = true;
                            _avgoffsethelp.Visible = true;
                        }
                    }
                    else
                    {
                        timer.Stop();
                    }
                };
            }
            else
            {
                switch (_mode)
                {
                    case 0:
                        if (_currentBackgroundImage != 1)
                        {
                            ClientSize = new Size(316, 130);
                            RoundCorners();
                            BackgroundImage = Properties.Resources.PPUR;
                            _currentBackgroundImage = 1;
                        }
                        break;

                    case 1:
                        if (_currentBackgroundImage != 2)
                        {
                            ClientSize = new Size(316, 65);
                            RoundCorners();
                            BackgroundImage = Properties.Resources.PP;
                            _currentBackgroundImage = 2;
                        }
                        break;

                    case 2:
                        if (_currentBackgroundImage != 3)
                        {
                            ClientSize = new Size(316, 65);
                            RoundCorners();
                            BackgroundImage = Properties.Resources.UR;
                            _currentBackgroundImage = 3;
                        }
                        break;
                }

                if (_nowPlaying)
                {
                    Location = new Point(_x, _y);
                    _nowPlaying = false;
                }

                inGameValue.Visible = false;
                _sr.Visible = true;
                _sspp.Visible = true;
                _currentPp.Visible = true;
                _good.Visible = true;
                _ok.Visible = true;
                _miss.Visible = true;
                _avgoffset.Visible = true;
                _ur.Visible = true;
                _avgoffsethelp.Visible = true;
            }
        }

        private void RoundCorners()
        {
            const int radius = 11;
            const int diameter = radius * 2;
            GraphicsPath gp = new GraphicsPath();
            gp.AddPie(0, 0, diameter, diameter, 180, 90);
            gp.AddPie(Width - diameter, 0, diameter, diameter, 270, 90);
            gp.AddPie(0, Height - diameter, diameter, diameter, 90, 90);
            gp.AddPie(Width - diameter, Height - diameter, diameter, diameter, 0, 90);
            gp.AddRectangle(new Rectangle(radius, 0, Width - diameter, Height));
            gp.AddRectangle(new Rectangle(0, radius, radius, Height - diameter));
            gp.AddRectangle(new Rectangle(Width - radius, radius, radius, Height - diameter));
            Region = new Region(gp);
        }

        private void RealtimePPUR_Closed(object sender, EventArgs e) => System.Windows.Forms.Application.Exit();

        private void changePriorityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form priorityForm = new ChangePriorityForm();
            priorityForm.Show();
        }

        private void sRToolStripMenuItem_Click(object sender, EventArgs e) => sRToolStripMenuItem.Checked = !sRToolStripMenuItem.Checked;

        private void ifFCPPToolStripMenuItem_Click(object sender, EventArgs e) => ifFCPPToolStripMenuItem.Checked = !ifFCPPToolStripMenuItem.Checked;

        private void ifFCHitsToolStripMenuItem_Click(object sender, EventArgs e) => ifFCHitsToolStripMenuItem.Checked = !ifFCHitsToolStripMenuItem.Checked;

        private void expectedManiaScoreToolStripMenuItem_Click(object sender, EventArgs e) => expectedManiaScoreToolStripMenuItem.Checked = !expectedManiaScoreToolStripMenuItem.Checked;

        private void currentPPToolStripMenuItem_Click(object sender, EventArgs e) => currentPPToolStripMenuItem.Checked = !currentPPToolStripMenuItem.Checked;

        private void sSPPToolStripMenuItem_Click(object sender, EventArgs e) => sSPPToolStripMenuItem.Checked = !sSPPToolStripMenuItem.Checked;

        private void hitsToolStripMenuItem_Click(object sender, EventArgs e) => hitsToolStripMenuItem.Checked = !hitsToolStripMenuItem.Checked;

        private void uRToolStripMenuItem_Click(object sender, EventArgs e) => uRToolStripMenuItem.Checked = !uRToolStripMenuItem.Checked;

        private void offsetHelpToolStripMenuItem_Click(object sender, EventArgs e) => offsetHelpToolStripMenuItem.Checked = !offsetHelpToolStripMenuItem.Checked;

        private void currentACCToolStripMenuItem_Click(object sender, EventArgs e) => currentACCToolStripMenuItem.Checked = !currentACCToolStripMenuItem.Checked;

        private void progressToolStripMenuItem_Click(object sender, EventArgs e) => progressToolStripMenuItem.Checked = !progressToolStripMenuItem.Checked;

        private void avgOffsetToolStripMenuItem_Click(object sender, EventArgs e) => avgOffsetToolStripMenuItem.Checked = !avgOffsetToolStripMenuItem.Checked;

        private void healthPercentageToolStripMenuItem_Click(object sender, EventArgs e) => healthPercentageToolStripMenuItem.Checked = !healthPercentageToolStripMenuItem.Checked;
    }
}
