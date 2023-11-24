using System.Drawing;

namespace RealtimePPUR
{
    sealed partial class RealtimePpur
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RealtimePpur));
            this._currentPp = new System.Windows.Forms.Label();
            this._sr = new System.Windows.Forms.Label();
            this._sspp = new System.Windows.Forms.Label();
            this._good = new System.Windows.Forms.Label();
            this._ok = new System.Windows.Forms.Label();
            this._miss = new System.Windows.Forms.Label();
            this._avgoffset = new System.Windows.Forms.Label();
            this._ur = new System.Windows.Forms.Label();
            this._avgoffsethelp = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realtimePPURToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offsetHelperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realtimePPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.osuModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inGameOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sSPPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentPPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentACCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ifFCHitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offsetHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expectedManiaScoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.avgOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ifFCPPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changePriorityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inGameValue = new System.Windows.Forms.Label();
            this.healthPercentageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _currentPp
            // 
            this._currentPp.BackColor = System.Drawing.Color.Transparent;
            this._currentPp.ForeColor = System.Drawing.Color.White;
            this._currentPp.Location = new System.Drawing.Point(181, 18);
            this._currentPp.Name = "_currentPp";
            this._currentPp.Size = new System.Drawing.Size(100, 31);
            this._currentPp.TabIndex = 0;
            this._currentPp.Text = "0";
            this._currentPp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._currentPp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this._currentPp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            // 
            // _sr
            // 
            this._sr.BackColor = System.Drawing.Color.Transparent;
            this._sr.ForeColor = System.Drawing.Color.White;
            this._sr.Location = new System.Drawing.Point(40, -1);
            this._sr.Name = "_sr";
            this._sr.Size = new System.Drawing.Size(100, 23);
            this._sr.TabIndex = 0;
            this._sr.Text = "0";
            this._sr.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this._sr.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            // 
            // _sspp
            // 
            this._sspp.BackColor = System.Drawing.Color.Transparent;
            this._sspp.ForeColor = System.Drawing.Color.White;
            this._sspp.Location = new System.Drawing.Point(140, -1);
            this._sspp.Name = "_sspp";
            this._sspp.Size = new System.Drawing.Size(100, 23);
            this._sspp.TabIndex = 0;
            this._sspp.Text = "0";
            this._sspp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this._sspp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            // 
            // _good
            // 
            this._good.BackColor = System.Drawing.Color.Transparent;
            this._good.ForeColor = System.Drawing.Color.White;
            this._good.Location = new System.Drawing.Point(-13, 27);
            this._good.Name = "_good";
            this._good.Size = new System.Drawing.Size(100, 23);
            this._good.TabIndex = 0;
            this._good.Text = "0";
            this._good.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._good.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this._good.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            // 
            // _ok
            // 
            this._ok.BackColor = System.Drawing.Color.Transparent;
            this._ok.ForeColor = System.Drawing.Color.White;
            this._ok.Location = new System.Drawing.Point(47, 26);
            this._ok.Name = "_ok";
            this._ok.Size = new System.Drawing.Size(100, 23);
            this._ok.TabIndex = 0;
            this._ok.Text = "0";
            this._ok.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._ok.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this._ok.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            // 
            // _miss
            // 
            this._miss.BackColor = System.Drawing.Color.Transparent;
            this._miss.ForeColor = System.Drawing.Color.White;
            this._miss.Location = new System.Drawing.Point(105, 26);
            this._miss.Name = "_miss";
            this._miss.Size = new System.Drawing.Size(100, 23);
            this._miss.TabIndex = 0;
            this._miss.Text = "0";
            this._miss.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._miss.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this._miss.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            // 
            // _avgoffset
            // 
            this._avgoffset.AutoSize = true;
            this._avgoffset.BackColor = System.Drawing.Color.Transparent;
            this._avgoffset.ForeColor = System.Drawing.Color.White;
            this._avgoffset.Location = new System.Drawing.Point(38, 106);
            this._avgoffset.Name = "_avgoffset";
            this._avgoffset.Size = new System.Drawing.Size(26, 12);
            this._avgoffset.TabIndex = 0;
            this._avgoffset.Text = "0ms";
            this._avgoffset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this._avgoffset.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            // 
            // _ur
            // 
            this._ur.AutoSize = true;
            this._ur.BackColor = System.Drawing.Color.Transparent;
            this._ur.ForeColor = System.Drawing.Color.White;
            this._ur.Location = new System.Drawing.Point(219, 72);
            this._ur.Name = "_ur";
            this._ur.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._ur.Size = new System.Drawing.Size(11, 12);
            this._ur.TabIndex = 0;
            this._ur.Text = "0";
            this._ur.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this._ur.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            // 
            // _avgoffsethelp
            // 
            this._avgoffsethelp.AutoSize = true;
            this._avgoffsethelp.BackColor = System.Drawing.Color.Transparent;
            this._avgoffsethelp.ForeColor = System.Drawing.Color.White;
            this._avgoffsethelp.Location = new System.Drawing.Point(82, 69);
            this._avgoffsethelp.Name = "_avgoffsethelp";
            this._avgoffsethelp.Size = new System.Drawing.Size(11, 12);
            this._avgoffsethelp.TabIndex = 0;
            this._avgoffsethelp.Text = "0";
            this._avgoffsethelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._avgoffsethelp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this._avgoffsethelp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.modeToolStripMenuItem, this.osuModeToolStripMenuItem, this.inGameOverlayToolStripMenuItem, this.changePriorityToolStripMenuItem, this.changeFontToolStripMenuItem, this.loadFontToolStripMenuItem, this.resetFontToolStripMenuItem, this.closeToolStripMenuItem });
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 202);
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.realtimePPURToolStripMenuItem, this.offsetHelperToolStripMenuItem, this.realtimePPToolStripMenuItem });
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
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
            // osuModeToolStripMenuItem
            // 
            this.osuModeToolStripMenuItem.Name = "osuModeToolStripMenuItem";
            this.osuModeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.osuModeToolStripMenuItem.Text = "osu! mode";
            this.osuModeToolStripMenuItem.Click += new System.EventHandler(this.osuModeToolStripMenuItem_Click);
            // 
            // inGameOverlayToolStripMenuItem
            // 
            this.inGameOverlayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.sRToolStripMenuItem, this.sSPPToolStripMenuItem, this.currentPPToolStripMenuItem, this.currentACCToolStripMenuItem, this.hitsToolStripMenuItem, this.ifFCHitsToolStripMenuItem, this.uRToolStripMenuItem, this.offsetHelpToolStripMenuItem, this.expectedManiaScoreToolStripMenuItem, this.avgOffsetToolStripMenuItem, this.progressToolStripMenuItem, this.ifFCPPToolStripMenuItem, this.healthPercentageToolStripMenuItem });
            this.inGameOverlayToolStripMenuItem.Name = "inGameOverlayToolStripMenuItem";
            this.inGameOverlayToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.inGameOverlayToolStripMenuItem.Text = "InGameOverlay";
            // 
            // sRToolStripMenuItem
            // 
            this.sRToolStripMenuItem.Name = "sRToolStripMenuItem";
            this.sRToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.sRToolStripMenuItem.Text = "SR";
            this.sRToolStripMenuItem.Click += new System.EventHandler(this.sRToolStripMenuItem_Click);
            // 
            // sSPPToolStripMenuItem
            // 
            this.sSPPToolStripMenuItem.Name = "sSPPToolStripMenuItem";
            this.sSPPToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.sSPPToolStripMenuItem.Text = "SSPP";
            this.sSPPToolStripMenuItem.Click += new System.EventHandler(this.sSPPToolStripMenuItem_Click);
            // 
            // currentPPToolStripMenuItem
            // 
            this.currentPPToolStripMenuItem.Name = "currentPPToolStripMenuItem";
            this.currentPPToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.currentPPToolStripMenuItem.Text = "CurrentPP";
            this.currentPPToolStripMenuItem.Click += new System.EventHandler(this.currentPPToolStripMenuItem_Click);
            // 
            // currentACCToolStripMenuItem
            // 
            this.currentACCToolStripMenuItem.Name = "currentACCToolStripMenuItem";
            this.currentACCToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.currentACCToolStripMenuItem.Text = "CurrentACC";
            this.currentACCToolStripMenuItem.Click += new System.EventHandler(this.currentACCToolStripMenuItem_Click);
            // 
            // hitsToolStripMenuItem
            // 
            this.hitsToolStripMenuItem.Name = "hitsToolStripMenuItem";
            this.hitsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.hitsToolStripMenuItem.Text = "Hits";
            this.hitsToolStripMenuItem.Click += new System.EventHandler(this.hitsToolStripMenuItem_Click);
            // 
            // ifFCHitsToolStripMenuItem
            // 
            this.ifFCHitsToolStripMenuItem.Name = "ifFCHitsToolStripMenuItem";
            this.ifFCHitsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.ifFCHitsToolStripMenuItem.Text = "ifFCHits";
            this.ifFCHitsToolStripMenuItem.Click += new System.EventHandler(this.ifFCHitsToolStripMenuItem_Click);
            // 
            // uRToolStripMenuItem
            // 
            this.uRToolStripMenuItem.Name = "uRToolStripMenuItem";
            this.uRToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.uRToolStripMenuItem.Text = "UR";
            this.uRToolStripMenuItem.Click += new System.EventHandler(this.uRToolStripMenuItem_Click);
            // 
            // offsetHelpToolStripMenuItem
            // 
            this.offsetHelpToolStripMenuItem.Name = "offsetHelpToolStripMenuItem";
            this.offsetHelpToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.offsetHelpToolStripMenuItem.Text = "OffsetHelp";
            this.offsetHelpToolStripMenuItem.Click += new System.EventHandler(this.offsetHelpToolStripMenuItem_Click);
            // 
            // expectedManiaScoreToolStripMenuItem
            // 
            this.expectedManiaScoreToolStripMenuItem.Name = "expectedManiaScoreToolStripMenuItem";
            this.expectedManiaScoreToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.expectedManiaScoreToolStripMenuItem.Text = "ExpectedManiaScore";
            this.expectedManiaScoreToolStripMenuItem.Click += new System.EventHandler(this.expectedManiaScoreToolStripMenuItem_Click);
            // 
            // avgOffsetToolStripMenuItem
            // 
            this.avgOffsetToolStripMenuItem.Name = "avgOffsetToolStripMenuItem";
            this.avgOffsetToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.avgOffsetToolStripMenuItem.Text = "AvgOffset";
            this.avgOffsetToolStripMenuItem.Click += new System.EventHandler(this.avgOffsetToolStripMenuItem_Click);
            // 
            // progressToolStripMenuItem
            // 
            this.progressToolStripMenuItem.Name = "progressToolStripMenuItem";
            this.progressToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.progressToolStripMenuItem.Text = "Progress";
            this.progressToolStripMenuItem.Click += new System.EventHandler(this.progressToolStripMenuItem_Click);
            // 
            // ifFCPPToolStripMenuItem
            // 
            this.ifFCPPToolStripMenuItem.Name = "ifFCPPToolStripMenuItem";
            this.ifFCPPToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.ifFCPPToolStripMenuItem.Text = "ifFCPP";
            this.ifFCPPToolStripMenuItem.Click += new System.EventHandler(this.ifFCPPToolStripMenuItem_Click);
            // 
            // changePriorityToolStripMenuItem
            // 
            this.changePriorityToolStripMenuItem.Name = "changePriorityToolStripMenuItem";
            this.changePriorityToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.changePriorityToolStripMenuItem.Text = "Change Priority";
            this.changePriorityToolStripMenuItem.Click += new System.EventHandler(this.changePriorityToolStripMenuItem_Click);
            // 
            // changeFontToolStripMenuItem
            // 
            this.changeFontToolStripMenuItem.Name = "changeFontToolStripMenuItem";
            this.changeFontToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.changeFontToolStripMenuItem.Text = "Change Font";
            this.changeFontToolStripMenuItem.Click += new System.EventHandler(this.changeFontToolStripMenuItem_Click);
            // 
            // loadFontToolStripMenuItem
            // 
            this.loadFontToolStripMenuItem.Name = "loadFontToolStripMenuItem";
            this.loadFontToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.loadFontToolStripMenuItem.Text = "Load Font";
            this.loadFontToolStripMenuItem.Click += new System.EventHandler(this.loadFontToolStripMenuItem_Click);
            // 
            // resetFontToolStripMenuItem
            // 
            this.resetFontToolStripMenuItem.Name = "resetFontToolStripMenuItem";
            this.resetFontToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.resetFontToolStripMenuItem.Text = "Reset Font";
            this.resetFontToolStripMenuItem.Click += new System.EventHandler(this.resetFontToolStripMenuItem_Click_1);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // inGameValue
            // 
            this.inGameValue.AutoSize = true;
            this.inGameValue.ForeColor = System.Drawing.Color.White;
            this.inGameValue.Location = new System.Drawing.Point(0, 0);
            this.inGameValue.Name = "inGameValue";
            this.inGameValue.Size = new System.Drawing.Size(0, 12);
            this.inGameValue.TabIndex = 1;
            this.inGameValue.Visible = false;
            // 
            // healthPercentageToolStripMenuItem
            // 
            this.healthPercentageToolStripMenuItem.Name = "healthPercentageToolStripMenuItem";
            this.healthPercentageToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.healthPercentageToolStripMenuItem.Text = "Health Percentage";
            this.healthPercentageToolStripMenuItem.Click += new System.EventHandler(this.healthPercentageToolStripMenuItem_Click);
            // 
            // RealtimePpur
            // 
            this._avgoffsethelp.Font = new System.Drawing.Font(_fontCollection.Families[1], 20F, System.Drawing.FontStyle.Bold);
            this._ur.Font = new System.Drawing.Font(_fontCollection.Families[1], 25F, System.Drawing.FontStyle.Bold);
            this._avgoffset.Font = new System.Drawing.Font(_fontCollection.Families[1], 13F, System.Drawing.FontStyle.Bold);
            this._miss.Font = new System.Drawing.Font(_fontCollection.Families[1], 15F, System.Drawing.FontStyle.Bold);
            this._ok.Font = new System.Drawing.Font(_fontCollection.Families[1], 15F, System.Drawing.FontStyle.Bold);
            this._good.Font = new System.Drawing.Font(_fontCollection.Families[1], 15F, System.Drawing.FontStyle.Bold);
            this._sspp.Font = new System.Drawing.Font(_fontCollection.Families[1], 13F, System.Drawing.FontStyle.Bold);
            this._sr.Font = new System.Drawing.Font(_fontCollection.Families[1], 13F, System.Drawing.FontStyle.Bold);
            this._currentPp.Font = new System.Drawing.Font(_fontCollection.Families[1], 20F, System.Drawing.FontStyle.Bold);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::RealtimePPUR.Properties.Resources.PPUR;
            this.ClientSize = new System.Drawing.Size(316, 130);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.inGameValue);
            this.Controls.Add(this._currentPp);
            this.Controls.Add(this._sr);
            this.Controls.Add(this._sspp);
            this.Controls.Add(this._good);
            this.Controls.Add(this._ok);
            this.Controls.Add(this._miss);
            this.Controls.Add(this._avgoffset);
            this.Controls.Add(this._ur);
            this.Controls.Add(this._avgoffsethelp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RealtimePpur";
            this.Text = "RealtimePPUR";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.Closed += new System.EventHandler(this.RealtimePPUR_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RealtimePPUR_MouseMove);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
            RoundCorners();
        }

        private System.Windows.Forms.ToolStripMenuItem healthPercentageToolStripMenuItem;

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem offsetHelperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem realtimePPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem realtimePPURToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem osuModeToolStripMenuItem;
        private System.Windows.Forms.Label inGameValue;
        private System.Windows.Forms.ToolStripMenuItem inGameOverlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentPPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sSPPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem offsetHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem avgOffsetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentACCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem progressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ifFCPPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ifFCHitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expectedManiaScoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changePriorityToolStripMenuItem;
    }
}

