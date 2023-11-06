using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RealtimePPUR
{
    public partial class ChangePriorityForm : Form
    {
        private int _index;
        private readonly int _max;

        public ChangePriorityForm()
        {
            InitializeComponent();
            _max = sortPriorityList.Items.Count;
        }

        private void sortPriorityList_MouseDown(object o, MouseEventArgs e)
        {
            Point p = MousePosition;
            p = sortPriorityList.PointToClient(p);
            _index = sortPriorityList.IndexFromPoint(p);
            if (_index > -1)
            {
                sortPriorityList.DoDragDrop(sortPriorityList.Items[_index].ToString(), DragDropEffects.Copy);
            }
        }

        private void sortPriorityList_DragEnter(object o, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void sortPriorityList_DragDrop(object o, DragEventArgs e)
        {
            string str = e.Data.GetData(DataFormats.Text).ToString();
            Point p = MousePosition;
            p = sortPriorityList.PointToClient(p);
            int ind = sortPriorityList.IndexFromPoint(p);
            if (!(ind > -1 && ind < _max))
            {
                return;
            }
            sortPriorityList.Items[_index] = sortPriorityList.Items[ind];
            sortPriorityList.Items[ind] = str;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            List<int> sortPriority = new List<int>();
            foreach (string item in sortPriorityList.Items)
            {
                int itemNumber = int.Parse(item.Split(':')[0]);
                sortPriority.Add(itemNumber);
            }
            string message = string.Join("/", sortPriority);
            Clipboard.SetText(message);
            MessageBox.Show($"Config.txtのINGAMEOVERLAYPRIORITYの所をクリップボードに自動保存された文章に書き換えてください！再起動したら反映します！\n\nRewrite the INGAMEOVERLAYPRIORITY section of Config.txt with the text automatically saved to the clipboard! It will be reflected after rebooting!\n\nコピーされた文章(Copied text): {message}", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
