namespace AdvancedGIS
{
    partial class AGIS
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.模型ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生成格网ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tIN模型ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生成等值线ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tIN模型ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.生成凸包ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生成TINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生成等值线ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.拓扑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.建立拓扑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出拓扑数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.模型ToolStripMenuItem,
            this.tIN模型ToolStripMenuItem1,
            this.拓扑ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(827, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.OpenToolStripMenuItem.Text = "打开";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // 模型ToolStripMenuItem
            // 
            this.模型ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.生成格网ToolStripMenuItem,
            this.tIN模型ToolStripMenuItem,
            this.生成等值线ToolStripMenuItem1});
            this.模型ToolStripMenuItem.Name = "模型ToolStripMenuItem";
            this.模型ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.模型ToolStripMenuItem.Text = "格网模型";
            // 
            // 生成格网ToolStripMenuItem
            // 
            this.生成格网ToolStripMenuItem.Name = "生成格网ToolStripMenuItem";
            this.生成格网ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.生成格网ToolStripMenuItem.Text = "生成格网";
            this.生成格网ToolStripMenuItem.Click += new System.EventHandler(this.生成格网ToolStripMenuItem_Click);
            // 
            // tIN模型ToolStripMenuItem
            // 
            this.tIN模型ToolStripMenuItem.Name = "tIN模型ToolStripMenuItem";
            this.tIN模型ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.tIN模型ToolStripMenuItem.Text = "加密格网";
            this.tIN模型ToolStripMenuItem.Click += new System.EventHandler(this.tIN模型ToolStripMenuItem_Click);
            // 
            // 生成等值线ToolStripMenuItem1
            // 
            this.生成等值线ToolStripMenuItem1.Name = "生成等值线ToolStripMenuItem1";
            this.生成等值线ToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
            this.生成等值线ToolStripMenuItem1.Text = "生成等值线";
            this.生成等值线ToolStripMenuItem1.Click += new System.EventHandler(this.生成等值线ToolStripMenuItem1_Click);
            // 
            // tIN模型ToolStripMenuItem1
            // 
            this.tIN模型ToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.生成凸包ToolStripMenuItem,
            this.生成TINToolStripMenuItem,
            this.生成等值线ToolStripMenuItem});
            this.tIN模型ToolStripMenuItem1.Name = "tIN模型ToolStripMenuItem1";
            this.tIN模型ToolStripMenuItem1.Size = new System.Drawing.Size(76, 24);
            this.tIN模型ToolStripMenuItem1.Text = "TIN模型";
            // 
            // 生成凸包ToolStripMenuItem
            // 
            this.生成凸包ToolStripMenuItem.Name = "生成凸包ToolStripMenuItem";
            this.生成凸包ToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.生成凸包ToolStripMenuItem.Text = "生成凸包";
            this.生成凸包ToolStripMenuItem.Click += new System.EventHandler(this.生成凸包ToolStripMenuItem_Click);
            // 
            // 生成TINToolStripMenuItem
            // 
            this.生成TINToolStripMenuItem.Name = "生成TINToolStripMenuItem";
            this.生成TINToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.生成TINToolStripMenuItem.Text = "生成TIN";
            this.生成TINToolStripMenuItem.Click += new System.EventHandler(this.生成TINToolStripMenuItem_Click);
            // 
            // 生成等值线ToolStripMenuItem
            // 
            this.生成等值线ToolStripMenuItem.Name = "生成等值线ToolStripMenuItem";
            this.生成等值线ToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.生成等值线ToolStripMenuItem.Text = "生成等值线";
            this.生成等值线ToolStripMenuItem.Click += new System.EventHandler(this.生成等值线ToolStripMenuItem_Click);
            // 
            // 拓扑ToolStripMenuItem
            // 
            this.拓扑ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.建立拓扑ToolStripMenuItem,
            this.导出拓扑数据ToolStripMenuItem});
            this.拓扑ToolStripMenuItem.Name = "拓扑ToolStripMenuItem";
            this.拓扑ToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.拓扑ToolStripMenuItem.Text = "拓扑";
            // 
            // 建立拓扑ToolStripMenuItem
            // 
            this.建立拓扑ToolStripMenuItem.Name = "建立拓扑ToolStripMenuItem";
            this.建立拓扑ToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.建立拓扑ToolStripMenuItem.Text = "建立拓扑";
            this.建立拓扑ToolStripMenuItem.Click += new System.EventHandler(this.建立拓扑ToolStripMenuItem_Click);
            // 
            // 导出拓扑数据ToolStripMenuItem
            // 
            this.导出拓扑数据ToolStripMenuItem.Name = "导出拓扑数据ToolStripMenuItem";
            this.导出拓扑数据ToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.导出拓扑数据ToolStripMenuItem.Text = "导出拓扑关系表";
            this.导出拓扑数据ToolStripMenuItem.Click += new System.EventHandler(this.导出拓扑数据ToolStripMenuItem_Click);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.帮助ToolStripMenuItem.Text = "帮助";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(0, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(827, 689);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // AGIS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 721);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AGIS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced GIS Assignment";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem 模型ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生成格网ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tIN模型ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tIN模型ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 生成TINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生成等值线ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 拓扑ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生成等值线ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 建立拓扑ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出拓扑数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生成凸包ToolStripMenuItem;
    }
}

