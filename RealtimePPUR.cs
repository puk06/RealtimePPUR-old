using System;
using System.Drawing;
using System.Drawing.Text;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RealtimePPUR
{
    public partial class RealtimePPUR : Form
    {
        private Label CurrentPP;
        private Label SR;
        private Label SSPP;
        private Label GOOD;
        private Label OK;
        private Label MISS;
        private Label AVGOFFSET;
        private Label UR;
        private Label AVGOFFSETHELP;
        private int mode;

        //フォントをローカルファイルから読み込みます。
        public PrivateFontCollection FontCollection;

        public RealtimePPUR()
        {
            FontCollection = new PrivateFontCollection();
            FontCollection.AddFontFile("./src/Fonts/MPLUSRounded1c-ExtraBold.ttf");
            InitializeComponent();

            Timer timer = new Timer();
            timer.Interval = 5;
            timer.Tick += async (sender, e) =>
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = null;
                    string dataurl = "http://127.0.0.1:3000/";
                    try
                    {
                        response = await client.GetAsync(dataurl);
                    }
                    catch (HttpRequestException)
                    {
                        SR.Text = "0";
                        SSPP.Text = "0";
                        CurrentPP.Text = "0";
                        GOOD.Text = "0";
                        MISS.Text = "0";
                        AVGOFFSET.Text = "0ms";
                        UR.Text = "0";
                        AVGOFFSETHELP.Text = "0";
                        return;
                    }

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
                }
            };
            timer.Start();
        }

        private void realtimePPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientSize = new Size(316, 65);
            BackgroundImage = Properties.Resources.PP;
            if (mode == 2)
            {
                foreach (Control control in Controls)
                {
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
                    control.Location = new Point(control.Location.X, control.Location.Y - 65);
                }
            }
            else if (mode == 1)
            {
                foreach (Control control in Controls)
                {
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
                    control.Location = new Point(control.Location.X, control.Location.Y + 65);
                }
            }
            mode = 0;
        }
    }
}
