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

namespace RealtimePPUR
{
    public partial class RealtimePPUR : Form
    {
        private System.Windows.Forms.Label CurrentPP, SR, SSPP, GOOD, OK, MISS, AVGOFFSET, UR, AVGOFFSETHELP;
        private Point mousePoint;
        private string displayFormat;
        private int status;
        private int mode;
        private int x;
        private int y;
        private int prevCalculationSpeed;
        private readonly int CalculationSpeedDetectedValue;
        private bool isosumode;
        private bool nowPlaying;
        private readonly bool speedReduction;
        private int currentBackgroundImage = 1;
        private string ingameoverlayPriority;
        readonly Dictionary<string, string> configDictionary = new Dictionary<string, string>();
        private readonly HttpClient client = new HttpClient();
        private readonly PrivateFontCollection FontCollection;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        public RealtimePPUR()
        {
            FontCollection = new PrivateFontCollection();
            FontCollection.AddFontFile("./src/Fonts/MPLUSRounded1c-ExtraBold.ttf");
            FontCollection.AddFontFile("./src/Fonts/Nexa Light.otf");
            InitializeComponent();

            if (!File.Exists("Config.txt"))
            {
                MessageBox.Show("Config.txtがフォルダ内に存在しないため、すべての項目がOffとして設定されます。アップデートチェックのみ行われます。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                githubUpdateChecker();
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
                speedReduction = false;
                CalculationSpeedDetectedValue = 100;
                ingameoverlayPriority = "1/2/3/4/5/6/7/8/9/10/11";
                inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                updateLoop();
            }
            else
            {
                string[] lines = File.ReadAllLines("Config.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');

                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    string name = parts[0].Trim();
                    string value = parts[1].Trim();
                    configDictionary[name] = value;
                }

                if (configDictionary.TryGetValue("UPDATECHECK", out string test11) && test11 == "true")
                {
                    githubUpdateChecker();
                }

                var defaultmodeTest = configDictionary.TryGetValue("DEFAULTMODE", out string defaultmodestring);
                if (defaultmodeTest)
                {
                    var defaultModeResult = int.TryParse(defaultmodestring, out int defaultmode);
                    if (!defaultModeResult || !(defaultmode == 0 || defaultmode == 1 || defaultmode == 2))
                    {
                        MessageBox.Show("Config.txtのDEFAULTMODEの値が不正であったため、初期値の0が適用されます。0、1、2の3つのどれかを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        switch (defaultmode)
                        {
                            case 1:
                                ClientSize = new Size(316, 65);
                                BackgroundImage = Properties.Resources.PP;
                                currentBackgroundImage = 2;
                                roundCorners();
                                if (mode == 2)
                                {
                                    foreach (Control control in Controls)
                                    {
                                        if (control.Name == "inGameValue") continue;
                                        control.Location = new Point(control.Location.X, control.Location.Y + 65);
                                    }
                                }
                                mode = 1;
                                break;

                            case 2:
                                ClientSize = new Size(316, 65);
                                BackgroundImage = Properties.Resources.UR;
                                currentBackgroundImage = 3;
                                roundCorners();
                                if (mode == 0 || mode == 1)
                                {
                                    foreach (Control control in Controls)
                                    {
                                        if (control.Name == "inGameValue") continue;
                                        control.Location = new Point(control.Location.X, control.Location.Y - 65);
                                    }
                                }
                                mode = 2;
                                break;
                        }
                    }
                }

                sRToolStripMenuItem.Checked = configDictionary.TryGetValue("SR", out string test) && test == "true";
                sSPPToolStripMenuItem.Checked = configDictionary.TryGetValue("SSPP", out string test2) && test2 == "true";
                currentPPToolStripMenuItem.Checked = configDictionary.TryGetValue("CURRENTPP", out string test3) && test3 == "true";
                currentACCToolStripMenuItem.Checked = configDictionary.TryGetValue("CURRENTACC", out string test4) && test4 == "true";
                hitsToolStripMenuItem.Checked = configDictionary.TryGetValue("HITS", out string test5) && test5 == "true";
                uRToolStripMenuItem.Checked = configDictionary.TryGetValue("UR", out string test6) && test6 == "true";
                offsetHelpToolStripMenuItem.Checked = configDictionary.TryGetValue("OFFSETHELP", out string test7) && test7 == "true";
                avgOffsetToolStripMenuItem.Checked = configDictionary.TryGetValue("AVGOFFSET", out string test8) && test8 == "true";
                progressToolStripMenuItem.Checked = configDictionary.TryGetValue("PROGRESS", out string test9) && test9 == "true";
                ifFCPPToolStripMenuItem.Checked = configDictionary.TryGetValue("IFFCPP", out string test13) && test13 == "true";
                ifFCHitsToolStripMenuItem.Checked = configDictionary.TryGetValue("IFFCHITS", out string test14) && test14 == "true";
                expectedManiaScoreToolStripMenuItem.Checked = configDictionary.TryGetValue("EXPECTEDMANIASCORE", out string test15) && test15 == "true";
                speedReduction = configDictionary.TryGetValue("SPEEDREDUCTION", out string test10) && test10 == "true";
                ingameoverlayPriority = configDictionary.TryGetValue("INGAMEOVERLAYPRIORITY", out string test16) ? test16 : "1/2/3/4/5/6/7/8/9/10/11";

                if (configDictionary.TryGetValue("USECUSTOMFONT", out string test12) && test12 == "true")
                {
                    if (File.Exists("Font"))
                    {
                        var fontDictionary = new Dictionary<string, string>();
                        string[] fontInfo = File.ReadAllLines("Font");
                        foreach (string line in fontInfo)
                        {
                            string[] parts = line.Split('=');

                            if (parts.Length != 2)
                            {
                                continue;
                            }

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
                                var fontsizeResult = configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
                                if (!fontsizeResult)
                                {
                                    MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                                }
                                else
                                {
                                    var result = float.TryParse(fontsizeValue, out float fontsize);
                                    if (!result)
                                    {
                                        MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                                    }
                                    else
                                    {
                                        inGameValue.Font = new Font(FontCollection.Families[0], fontsize);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Fontファイルのフォント情報が不正であったため、デフォルトのフォントが適用されます。一度Fontファイルを削除してみることをお勧めします。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            var fontsizeResult = configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
                            if (!fontsizeResult)
                            {
                                MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                            }
                            else
                            {
                                var result = float.TryParse(fontsizeValue, out float fontsize);
                                if (!result)
                                {
                                    MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                                }
                                else
                                {
                                    inGameValue.Font = new Font(FontCollection.Families[0], fontsize);
                                }
                            }
                        }
                    }
                    else
                    {
                        var fontsizeResult = configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
                        if (!fontsizeResult)
                        {
                            MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                        }
                        else
                        {
                            var result = float.TryParse(fontsizeValue, out float fontsize);
                            if (!result)
                            {
                                MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                            }
                            else
                            {
                                inGameValue.Font = new Font(FontCollection.Families[0], fontsize);
                            }
                        }
                    }
                }
                else
                {
                    var fontsizeResult = configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
                    if (!fontsizeResult)
                    {
                        MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                    }
                    else
                    {
                        var result = float.TryParse(fontsizeValue, out float fontsize);
                        if (!result)
                        {
                            MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                        }
                        else
                        {
                            inGameValue.Font = new Font(FontCollection.Families[0], fontsize);
                        }
                    }
                }

                var speedReductionValueResult = configDictionary.TryGetValue("SPEEDREDUCTIONVALUE", out string speedReductionValue);
                if (!speedReductionValueResult)
                {
                    MessageBox.Show("Config.txtのSPEEDREDUCTIONVALUEの値が存在しないため、初期値の100が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CalculationSpeedDetectedValue = 100;
                }
                else
                {
                    var tryResult = int.TryParse(speedReductionValue, out CalculationSpeedDetectedValue);
                    if (!tryResult)
                    {
                        MessageBox.Show("Config.txtのSPEEDREDUCTIONVALUEの値が不正であったため、初期値の100が設定されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        CalculationSpeedDetectedValue = 100;
                    }
                }
                updateLoop();
            }
        }

        private void realtimePPURToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientSize = new Size(316, 130);
            BackgroundImage = Properties.Resources.PPUR;
            currentBackgroundImage = 1;
            roundCorners();
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

        private void realtimePPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientSize = new Size(316, 65);
            BackgroundImage = Properties.Resources.PP;
            currentBackgroundImage = 2;
            roundCorners();
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
            ClientSize = new Size(316, 65);
            BackgroundImage = Properties.Resources.UR;
            currentBackgroundImage = 3;
            roundCorners();
            if (mode == 0 || mode == 1)
            {
                foreach (Control control in Controls)
                {
                    if (control.Name == "inGameValue") continue;
                    control.Location = new Point(control.Location.X, control.Location.Y - 65);
                }
            }
            mode = 2;
        }

        private void changeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog font = new FontDialog
            {
                Font = inGameValue.Font
            };

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
                            "フォントの保存に成功しました。Config.txtのUSECUSTOMFONTをtrueにすることで起動時から保存されたフォントを使用できます。右クリック→Load Fontでも読み込むことが可能です！",
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
                            "フォントの保存に成功しました。Config.txtのUSECUSTOMFONTをtrueにすることで起動時から保存されたフォントを使用できます。右クリック→Load Fontでも読み込むことが可能です！",
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
                MessageBox.Show("フォントの変更に失敗しました。対応してないフォントです。(TrueTypeフォントのみ対応しています。このフォントはおそらくOpenTypeフォントです。)", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void updateLoop()
        {
            try
            {
                if (Process.GetProcessesByName("gosumemory").Length == 0)
                {
                    MessageBox.Show("gosumemoryがクラッシュした、もしくはgosumemoryを起動していたソフトが閉じられた可能性があります。ソフトを再起動してください。",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }

                if (Program._ppurProcess.HasExited)
                {
                    MessageBox.Show("PPUR.jsがクラッシュした可能性があります。\n\n対処法: このソフトを閉じた後タスクマネージャーを開き、Node.js JavaScript Runtime、もしくはRealtimePPURを全て閉じ、ソフトを再起動してください。\nこのエラーはほとんどの場合、既に裏でPPUR.jsが起動しているときに起きます。", "エラー", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Close();
                }

                HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:3000/");
                string json = await response.Content.ReadAsStringAsync();
                JObject data = JsonConvert.DeserializeObject<JObject>(json);

                double sr = (double)data["PP"]["SR"];
                double sspp = (double)data["PP"]["SSPP"];
                double currentPP = (double)data["PP"]["CurrentPP"];
                double ifFCPP = (double)data["PP"]["ifFCPP"];
                int Good = (int)data["PP"]["good"];
                int Ok = (int)data["PP"]["ok"];
                int Miss = (int)data["PP"]["miss"];
                int sliderbreaks = (int)data["PP"]["sliderBreaks"];
                double AvgOffset = -(double)data["Hiterror"]["AvgOffset"];
                double ur = (double)data["Hiterror"]["UR"];
                double AvgOffsethelp = (double)data["Hiterror"]["AvgOffset"];
                int bad = (int)data["PP"]["bad"];
                int katu = (int)data["PP"]["katu"];
                int geki = (int)data["PP"]["geki"];
                int currentGamemode = (int)data["PP"]["mode"];
                int currentCalculationSpeed = (int)data["calculatingTime"];
                int ifFCGood = (int)data["PP"]["ifFCHits300"];
                int ifFCOk = (int)data["PP"]["ifFCHits100"];
                int ifFCBad = (int)data["PP"]["ifFCHits50"];
                int ifFCMiss = (int)data["PP"]["ifFCHitsMiss"];
                status = (int)data["PP"]["status"];

                if (prevCalculationSpeed == 0)
                {
                    prevCalculationSpeed = currentCalculationSpeed;
                }
                else if (currentCalculationSpeed - prevCalculationSpeed > CalculationSpeedDetectedValue &&
                         speedReduction)
                {
                    new ToastContentBuilder()
                        .AddText("Calculation Speed Reduction Detected!")
                        .AddText("Calculation speed is slower than usual! \nCurrent Calculation speed: " +
                                 currentCalculationSpeed + "ms")
                        .Show();
                }

                prevCalculationSpeed = currentCalculationSpeed;

                AVGOFFSET.Text = AvgOffset.ToString(CultureInfo.CurrentCulture = new CultureInfo("en-us")) + "ms";
                UR.Text = ur.ToString("F0");
                AVGOFFSETHELP.Text = AvgOffsethelp.ToString("F0");
                SR.Text = sr.ToString(CultureInfo.CurrentCulture = new CultureInfo("en-us"));
                SSPP.Text = sspp.ToString("F0");
                CurrentPP.Text = currentPP.ToString("F0");
                CurrentPP.Width = TextRenderer.MeasureText(CurrentPP.Text, CurrentPP.Font).Width;
                CurrentPP.Left = ClientSize.Width - CurrentPP.Width - 35;

                switch (currentGamemode)
                {
                    case 0:
                        GOOD.Text = Good.ToString();
                        GOOD.Width = TextRenderer.MeasureText(GOOD.Text, GOOD.Font).Width;
                        GOOD.Left = ((ClientSize.Width - GOOD.Width) / 2) - 120;

                        OK.Text = (Ok + bad).ToString();
                        OK.Width = TextRenderer.MeasureText(OK.Text, OK.Font).Width;
                        OK.Left = ((ClientSize.Width - OK.Width) / 2) - 61;

                        MISS.Text = Miss.ToString();
                        MISS.Width = TextRenderer.MeasureText(MISS.Text, MISS.Font).Width;
                        MISS.Left = ((ClientSize.Width - MISS.Width) / 2) - 3;
                        break;

                    case 1:
                        GOOD.Text = Good.ToString();
                        GOOD.Width = TextRenderer.MeasureText(GOOD.Text, GOOD.Font).Width;
                        GOOD.Left = ((ClientSize.Width - GOOD.Width) / 2) - 120;

                        OK.Text = Ok.ToString();
                        OK.Width = TextRenderer.MeasureText(OK.Text, OK.Font).Width;
                        OK.Left = ((ClientSize.Width - OK.Width) / 2) - 61;

                        MISS.Text = Miss.ToString();
                        MISS.Width = TextRenderer.MeasureText(MISS.Text, MISS.Font).Width;
                        MISS.Left = ((ClientSize.Width - MISS.Width) / 2) - 3;
                        break;

                    case 2:
                        GOOD.Text = Good.ToString();
                        GOOD.Width = TextRenderer.MeasureText(GOOD.Text, GOOD.Font).Width;
                        GOOD.Left = ((ClientSize.Width - GOOD.Width) / 2) - 120;

                        OK.Text = (Ok + bad).ToString();
                        OK.Width = TextRenderer.MeasureText(OK.Text, OK.Font).Width;
                        OK.Left = ((ClientSize.Width - OK.Width) / 2) - 61;

                        MISS.Text = Miss.ToString();
                        MISS.Width = TextRenderer.MeasureText(MISS.Text, MISS.Font).Width;
                        MISS.Left = ((ClientSize.Width - MISS.Width) / 2) - 3;
                        break;

                    case 3:
                        GOOD.Text = (Good + geki).ToString();
                        GOOD.Width = TextRenderer.MeasureText(GOOD.Text, GOOD.Font).Width;
                        GOOD.Left = ((ClientSize.Width - GOOD.Width) / 2) - 120;

                        OK.Text = (katu + Ok + bad).ToString();
                        OK.Width = TextRenderer.MeasureText(OK.Text, OK.Font).Width;
                        OK.Left = ((ClientSize.Width - OK.Width) / 2) - 61;

                        MISS.Text = Miss.ToString();
                        MISS.Width = TextRenderer.MeasureText(MISS.Text, MISS.Font).Width;
                        MISS.Left = ((ClientSize.Width - MISS.Width) / 2) - 3;
                        break;
                }

                displayFormat = "";
                var ingameoverlayPriorityArray = ingameoverlayPriority.Replace(" ", "").Split('/');
                foreach (var priorityValue in ingameoverlayPriorityArray)
                {
                    var priorityValueResult = int.TryParse(priorityValue, out int priorityValueInt);
                    if (!priorityValueResult) continue;
                    switch (priorityValueInt)
                    {
                        case 1:
                            if (sRToolStripMenuItem.Checked)
                            {
                                displayFormat += "SR: " +
                                                 sr.ToString(CultureInfo.CurrentCulture = new CultureInfo("en-us")) +
                                                 "\n";
                            }

                            break;

                        case 2:
                            if (sSPPToolStripMenuItem.Checked)
                            {
                                displayFormat += "SSPP: " + sspp.ToString("F0") + "pp\n";
                            }

                            break;

                        case 3:
                            if (currentPPToolStripMenuItem.Checked)
                            {
                                if (ifFCPPToolStripMenuItem.Checked && currentGamemode != 3)
                                {
                                    displayFormat += "PP: " + currentPP.ToString("F0") + " / " + ifFCPP.ToString("F0") +
                                                     "pp\n";
                                }
                                else
                                {
                                    displayFormat += "PP: " + currentPP.ToString("F0") + "pp\n";
                                }
                            }

                            break;

                        case 4:
                            if (currentACCToolStripMenuItem.Checked)
                            {
                                displayFormat += "ACC: " + data["PP"]["CurrentACC"] + "%\n";
                            }

                            break;

                        case 5:
                            if (hitsToolStripMenuItem.Checked)
                            {
                                switch (currentGamemode)
                                {
                                    case 0:
                                        displayFormat += $"Hits: {Good}/{Ok}/{bad}/{Miss} ({sliderbreaks})\n";
                                        break;

                                    case 1:
                                        displayFormat += $"Hits: {Good}/{Ok}/{Miss}\n";
                                        break;

                                    case 2:
                                        displayFormat += $"Hits: {Good}/{Ok}/{bad}/{Miss}\n";
                                        break;

                                    case 3:
                                        displayFormat += $"Hits: {geki}/{Good}/{katu}/{Ok}/{bad}/{Miss}\n";
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
                                        displayFormat += $"ifFCHits: {ifFCGood}/{ifFCOk}/{ifFCBad}/{ifFCMiss}\n";
                                        break;

                                    case 1:
                                        displayFormat += $"ifFCHits: {ifFCGood}/{ifFCOk}/{ifFCMiss}\n";
                                        break;

                                    case 2:
                                        displayFormat += $"ifFCHits: {ifFCGood}/{ifFCOk}/{ifFCBad}/{ifFCMiss}\n";
                                        break;
                                }
                            }

                            break;

                        case 7:
                            if (uRToolStripMenuItem.Checked)
                            {
                                displayFormat += "UR: " + ur.ToString("F0") + "\n";
                            }

                            break;

                        case 8:
                            if (offsetHelpToolStripMenuItem.Checked)
                            {
                                displayFormat += "OffsetHelp: " + AvgOffsethelp.ToString("F0") + "\n";
                            }

                            break;

                        case 9:
                            if (expectedManiaScoreToolStripMenuItem.Checked && currentGamemode == 3)
                            {
                                displayFormat += "ManiaScore: " + data["PP"]["expectedManiaScore"] + "\n";
                            }

                            break;

                        case 10:
                            if (avgOffsetToolStripMenuItem.Checked)
                            {
                                displayFormat += "AvgOffset: " +
                                                 AvgOffset.ToString(CultureInfo.CurrentCulture = new CultureInfo("en-us")) + "\n";
                            }
                            
                            break;
                        
                        case 11:
                            if (progressToolStripMenuItem.Checked)
                            {
                                displayFormat += "Progress: " + data["PP"]["progress"] + "%\n";
                            }

                            break;
                    }
                }

                inGameValue.Text = displayFormat;

                response = null;
                json = null;
                data = null;
            }
            catch
            {
                displayFormat = "Error";
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
            finally
            {
                updateLoop();
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

                    if (parts.Length != 2)
                    {
                        continue;
                    }

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
            var fontsizeResult = configDictionary.TryGetValue("FONTSIZE", out string fontsizeValue);
            if (!fontsizeResult)
            {
                MessageBox.Show("Config.txtにFONTSIZEの値がなかったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                MessageBox.Show("フォントのリセットが完了しました！", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var result = float.TryParse(fontsizeValue, out float fontsize);
                if (!result)
                {
                    MessageBox.Show("Config.txtのFONTSIZEの値が不正であったため、初期値の19が適用されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    inGameValue.Font = new Font(FontCollection.Families[0], 19F);
                    MessageBox.Show("フォントのリセットが完了しました！", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    inGameValue.Font = new Font(FontCollection.Families[0], fontsize);
                    MessageBox.Show("フォントのリセットが完了しました！", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
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
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left)
            {
                return;
            }

            Left += e.X - mousePoint.X;
            Top += e.Y - mousePoint.Y;

        }

        private async void githubUpdateChecker()
        {
            try
            {
                const string softwareReleasesLatest = "https://github.com/puk06/RealtimePPUR/releases/latest";
                StreamReader currentVersion = new StreamReader("version");
                string currentVersionString = await currentVersion.ReadToEndAsync();
                currentVersion.Close();
                var githubClient = new GitHubClient(new ProductHeaderValue("RealtimePPUR"));
                var latestRelease = await githubClient.Repository.Release.GetLatest("puk06", "RealtimePPUR");

                if (latestRelease.Name == currentVersionString)
                {
                    return;
                }

                DialogResult result = MessageBox.Show($"最新バージョンがあります！\n\n現在: {currentVersionString} \n更新後: {latestRelease.Name}\n\nダウンロードページを開きますか？", "アップデートのお知らせ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    Process.Start(softwareReleasesLatest);
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
            var lefttest = configDictionary.TryGetValue("LEFT", out string leftvalue);
            var toptest = configDictionary.TryGetValue("TOP", out string topvalue);
            if (!lefttest || !toptest)
            {
                MessageBox.Show("Config.txtにLEFTまたはTOPの値が存在しません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var leftResult = int.TryParse(leftvalue, out int left);
            var topResult = int.TryParse(topvalue, out int top);
            if ((!leftResult || !topResult) && !isosumode)
            {
                MessageBox.Show("Config.txtのLEFT、またはTOPの値が不正であったため、osu! Modeの起動に失敗しました。LEFT、TOPには数値以外入力しないでください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            isosumode = !isosumode;
            osuModeToolStripMenuItem.Checked = isosumode;

            Timer timer = new Timer
            {
                Interval = 1
            };

            if (isosumode)
            {
                timer.Start();
                timer.Tick += (o, args) =>
                {
                    if (isosumode)
                    {
                        var processes = Process.GetProcessesByName("osu!");
                        if (processes.Length > 0)
                        {
                            Process osuProcess = processes[0];
                            IntPtr osuMainWindowHandle = osuProcess.MainWindowHandle;
                            if (GetWindowRect(osuMainWindowHandle, out RECT rect) && status == 2 && GetForegroundWindow() == osuMainWindowHandle && osuMainWindowHandle != IntPtr.Zero)
                            {
                                if (!nowPlaying)
                                {
                                    x = Location.X;
                                    y = Location.Y;
                                    nowPlaying = true;
                                }

                                BackgroundImage = null;
                                currentBackgroundImage = 0;
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
                                Region = null;
                                Size = new Size(inGameValue.Width, inGameValue.Height);
                                Location = new Point(rect.Left + left + 2, rect.Top + top);
                            }
                            else
                            {
                                switch (mode)
                                {
                                    case 0:
                                        if (currentBackgroundImage != 1)
                                        {
                                            ClientSize = new Size(316, 130);
                                            roundCorners();
                                            BackgroundImage = Properties.Resources.PPUR;
                                            currentBackgroundImage = 1;
                                        }
                                        break;

                                    case 1:
                                        if (currentBackgroundImage != 2)
                                        {
                                            ClientSize = new Size(316, 65);
                                            roundCorners();
                                            BackgroundImage = Properties.Resources.PP;
                                            currentBackgroundImage = 2;
                                        }
                                        break;

                                    case 2:
                                        if (currentBackgroundImage != 3)
                                        {
                                            ClientSize = new Size(316, 65);
                                            roundCorners();
                                            BackgroundImage = Properties.Resources.UR;
                                            currentBackgroundImage = 3;
                                        }
                                        break;
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
                        }
                        else
                        {
                            switch (mode)
                            {
                                case 0:
                                    if (currentBackgroundImage != 1)
                                    {
                                        ClientSize = new Size(316, 130);
                                        roundCorners();
                                        BackgroundImage = Properties.Resources.PPUR;
                                        currentBackgroundImage = 1;
                                    }
                                    break;

                                case 1:
                                    if (currentBackgroundImage != 2)
                                    {
                                        ClientSize = new Size(316, 65);
                                        roundCorners();
                                        BackgroundImage = Properties.Resources.PP;
                                        currentBackgroundImage = 2;
                                    }
                                    break;

                                case 2:
                                    if (currentBackgroundImage != 3)
                                    {
                                        ClientSize = new Size(316, 65);
                                        roundCorners();
                                        BackgroundImage = Properties.Resources.UR;
                                        currentBackgroundImage = 3;
                                    }
                                    break;
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
                    }
                    else
                    {
                        timer.Stop();
                    }
                };
            }
            else
            {
                switch (mode)
                {
                    case 0:
                        if (currentBackgroundImage != 1)
                        {
                            ClientSize = new Size(316, 130);
                            roundCorners();
                            BackgroundImage = Properties.Resources.PPUR;
                            currentBackgroundImage = 1;
                        }
                        break;

                    case 1:
                        if (currentBackgroundImage != 2)
                        {
                            ClientSize = new Size(316, 65);
                            roundCorners();
                            BackgroundImage = Properties.Resources.PP;
                            currentBackgroundImage = 2;
                        }
                        break;

                    case 2:
                        if (currentBackgroundImage != 3)
                        {
                            ClientSize = new Size(316, 65);
                            roundCorners();
                            BackgroundImage = Properties.Resources.UR;
                            currentBackgroundImage = 3;
                        }
                        break;
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
        }

        private void roundCorners()
        {
            const int radius = 11;
            const int diameter = radius * 2;
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddPie(0, 0, diameter, diameter, 180, 90);
            gp.AddPie(Width - diameter, 0, diameter, diameter, 270, 90);
            gp.AddPie(0, Height - diameter, diameter, diameter, 90, 90);
            gp.AddPie(Width - diameter, Height - diameter, diameter, diameter, 0, 90);
            gp.AddRectangle(new Rectangle(radius, 0, Width - diameter, Height));
            gp.AddRectangle(new Rectangle(0, radius, radius, Height - diameter));
            gp.AddRectangle(new Rectangle(Width - radius, radius, radius, Height - diameter));
            Region = new Region(gp);
        }

        private void RealtimePPUR_Closed(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void sRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sRToolStripMenuItem.Checked = !sRToolStripMenuItem.Checked;
        }

        private void ifFCPPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ifFCPPToolStripMenuItem.Checked = !ifFCPPToolStripMenuItem.Checked;
        }

        private void ifFCHitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ifFCHitsToolStripMenuItem.Checked = !ifFCHitsToolStripMenuItem.Checked;
        }

        private void expectedManiaScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            expectedManiaScoreToolStripMenuItem.Checked = !expectedManiaScoreToolStripMenuItem.Checked;
        }

        private void currentPPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPPToolStripMenuItem.Checked = !currentPPToolStripMenuItem.Checked;
        }

        private void sSPPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sSPPToolStripMenuItem.Checked = !sSPPToolStripMenuItem.Checked;
        }

        private void hitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hitsToolStripMenuItem.Checked = !hitsToolStripMenuItem.Checked;
        }

        private void uRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uRToolStripMenuItem.Checked = !uRToolStripMenuItem.Checked;
        }

        private void offsetHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            offsetHelpToolStripMenuItem.Checked = !offsetHelpToolStripMenuItem.Checked;
        }

        private void currentACCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentACCToolStripMenuItem.Checked = !currentACCToolStripMenuItem.Checked;
        }

        private void progressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            progressToolStripMenuItem.Checked = !progressToolStripMenuItem.Checked;
        }

        private void avgOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            avgOffsetToolStripMenuItem.Checked = !avgOffsetToolStripMenuItem.Checked;
        }
    }
}
