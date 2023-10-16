namespace RealtimePPUR
{
    partial class RealtimePPUR
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RealtimePPUR));
            this.CurrentPP = new System.Windows.Forms.Label();
            this.SR = new System.Windows.Forms.Label();
            this.SSPP = new System.Windows.Forms.Label();
            this.GOOD = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Label();
            this.MISS = new System.Windows.Forms.Label();
            this.AVGOFFSET = new System.Windows.Forms.Label();
            this.UR = new System.Windows.Forms.Label();
            this.AVGOFFSETHELP = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realtimePPURToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offsetHelperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realtimePPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CurrentPP
            // 
            this.CurrentPP.BackColor = System.Drawing.Color.Transparent;
            this.CurrentPP.ForeColor = System.Drawing.Color.White;
            this.CurrentPP.Location = new System.Drawing.Point(177, 18);
            this.CurrentPP.Name = "CurrentPP";
            this.CurrentPP.Size = new System.Drawing.Size(100, 31);
            this.CurrentPP.TabIndex = 0;
            this.CurrentPP.Text = "0";
            this.CurrentPP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SR
            // 
            this.SR.BackColor = System.Drawing.Color.Transparent;
            this.SR.ForeColor = System.Drawing.Color.White;
            this.SR.Location = new System.Drawing.Point(40, -1);
            this.SR.Name = "SR";
            this.SR.Size = new System.Drawing.Size(100, 23);
            this.SR.TabIndex = 0;
            this.SR.Text = "0";
            // 
            // SSPP
            // 
            this.SSPP.BackColor = System.Drawing.Color.Transparent;
            this.SSPP.ForeColor = System.Drawing.Color.White;
            this.SSPP.Location = new System.Drawing.Point(140, -1);
            this.SSPP.Name = "SSPP";
            this.SSPP.Size = new System.Drawing.Size(100, 23);
            this.SSPP.TabIndex = 0;
            this.SSPP.Text = "0";
            // 
            // GOOD
            // 
            this.GOOD.BackColor = System.Drawing.Color.Transparent;
            this.GOOD.ForeColor = System.Drawing.Color.White;
            this.GOOD.Location = new System.Drawing.Point(-13, 27);
            this.GOOD.Name = "GOOD";
            this.GOOD.Size = new System.Drawing.Size(100, 23);
            this.GOOD.TabIndex = 0;
            this.GOOD.Text = "0";
            this.GOOD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.ForeColor = System.Drawing.Color.White;
            this.OK.Location = new System.Drawing.Point(47, 26);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(100, 23);
            this.OK.TabIndex = 0;
            this.OK.Text = "0";
            this.OK.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MISS
            // 
            this.MISS.BackColor = System.Drawing.Color.Transparent;
            this.MISS.ForeColor = System.Drawing.Color.White;
            this.MISS.Location = new System.Drawing.Point(105, 26);
            this.MISS.Name = "MISS";
            this.MISS.Size = new System.Drawing.Size(100, 23);
            this.MISS.TabIndex = 0;
            this.MISS.Text = "0";
            this.MISS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AVGOFFSET
            // 
            this.AVGOFFSET.AutoSize = true;
            this.AVGOFFSET.BackColor = System.Drawing.Color.Transparent;
            this.AVGOFFSET.ForeColor = System.Drawing.Color.White;
            this.AVGOFFSET.Location = new System.Drawing.Point(38, 106);
            this.AVGOFFSET.Name = "AVGOFFSET";
            this.AVGOFFSET.Size = new System.Drawing.Size(26, 12);
            this.AVGOFFSET.TabIndex = 0;
            this.AVGOFFSET.Text = "0ms";
            // 
            // UR
            // 
            this.UR.AutoSize = true;
            this.UR.BackColor = System.Drawing.Color.Transparent;
            this.UR.ForeColor = System.Drawing.Color.White;
            this.UR.Location = new System.Drawing.Point(219, 72);
            this.UR.Name = "UR";
            this.UR.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.UR.Size = new System.Drawing.Size(11, 12);
            this.UR.TabIndex = 0;
            this.UR.Text = "0";
            // 
            // AVGOFFSETHELP
            // 
            this.AVGOFFSETHELP.AutoSize = true;
            this.AVGOFFSETHELP.BackColor = System.Drawing.Color.Transparent;
            this.AVGOFFSETHELP.ForeColor = System.Drawing.Color.White;
            this.AVGOFFSETHELP.Location = new System.Drawing.Point(82, 69);
            this.AVGOFFSETHELP.Name = "AVGOFFSETHELP";
            this.AVGOFFSETHELP.Size = new System.Drawing.Size(11, 12);
            this.AVGOFFSETHELP.TabIndex = 0;
            this.AVGOFFSETHELP.Text = "0";
            this.AVGOFFSETHELP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(106, 26);
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.realtimePPURToolStripMenuItem,
            this.offsetHelperToolStripMenuItem,
            this.realtimePPToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // realtimePPURToolStripMenuItem
            // 
            this.realtimePPURToolStripMenuItem.Name = "realtimePPURToolStripMenuItem";
            this.realtimePPURToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.realtimePPURToolStripMenuItem.Text = "RealtimePPUR";
            this.realtimePPURToolStripMenuItem.Click += new System.EventHandler(this.realtimePPURToolStripMenuItem_Click);
            // 
            // offsetHelperToolStripMenuItem
            // 
            this.offsetHelperToolStripMenuItem.Name = "offsetHelperToolStripMenuItem";
            this.offsetHelperToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.offsetHelperToolStripMenuItem.Text = "Offset Helper";
            this.offsetHelperToolStripMenuItem.Click += new System.EventHandler(this.offsetHelperToolStripMenuItem_Click);
            // 
            // realtimePPToolStripMenuItem
            // 
            this.realtimePPToolStripMenuItem.Name = "realtimePPToolStripMenuItem";
            this.realtimePPToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.realtimePPToolStripMenuItem.Text = "RealtimePP";
            this.realtimePPToolStripMenuItem.Click += new System.EventHandler(this.realtimePPToolStripMenuItem_Click);
            // 
            // RealtimePPUR
            // 
            this.AVGOFFSETHELP.Font = new System.Drawing.Font(FontCollection.Families[0], 20F, System.Drawing.FontStyle.Bold);
            this.UR.Font = new System.Drawing.Font(FontCollection.Families[0], 25F, System.Drawing.FontStyle.Bold);
            this.AVGOFFSET.Font = new System.Drawing.Font(FontCollection.Families[0], 13F, System.Drawing.FontStyle.Bold);
            this.MISS.Font = new System.Drawing.Font(FontCollection.Families[0], 15F, System.Drawing.FontStyle.Bold);
            this.OK.Font = new System.Drawing.Font(FontCollection.Families[0], 15F, System.Drawing.FontStyle.Bold);
            this.GOOD.Font = new System.Drawing.Font(FontCollection.Families[0], 15F, System.Drawing.FontStyle.Bold);
            this.SSPP.Font = new System.Drawing.Font(FontCollection.Families[0], 13F, System.Drawing.FontStyle.Bold);
            this.SR.Font = new System.Drawing.Font(FontCollection.Families[0], 13F, System.Drawing.FontStyle.Bold);
            this.CurrentPP.Font = new System.Drawing.Font(FontCollection.Families[0], 20F, System.Drawing.FontStyle.Bold);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::RealtimePPUR.Properties.Resources.PPUR;
            this.ClientSize = new System.Drawing.Size(316, 130);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.CurrentPP);
            this.Controls.Add(this.SR);
            this.Controls.Add(this.SSPP);
            this.Controls.Add(this.GOOD);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.MISS);
            this.Controls.Add(this.AVGOFFSET);
            this.Controls.Add(this.UR);
            this.Controls.Add(this.AVGOFFSETHELP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RealtimePPUR";
            this.Text = "RealtimePPUR";
            this.TopMost = true;
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem offsetHelperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem realtimePPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem realtimePPURToolStripMenuItem;
    }
}

