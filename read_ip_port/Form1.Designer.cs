namespace read_ip_port
{
    partial class Form1
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
            this.btn_openpath = new System.Windows.Forms.Button();
            this.txt_import = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_start = new System.Windows.Forms.Button();
            this.txt_info = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // btn_openpath
            // 
            this.btn_openpath.Location = new System.Drawing.Point(54, 58);
            this.btn_openpath.Margin = new System.Windows.Forms.Padding(6);
            this.btn_openpath.Name = "btn_openpath";
            this.btn_openpath.Size = new System.Drawing.Size(188, 50);
            this.btn_openpath.TabIndex = 0;
            this.btn_openpath.Text = "打开文件夹";
            this.btn_openpath.UseVisualStyleBackColor = true;
            this.btn_openpath.Click += new System.EventHandler(this.button1_Click);
            // 
            // txt_import
            // 
            this.txt_import.Location = new System.Drawing.Point(253, 62);
            this.txt_import.Margin = new System.Windows.Forms.Padding(6);
            this.txt_import.Name = "txt_import";
            this.txt_import.Size = new System.Drawing.Size(812, 42);
            this.txt_import.TabIndex = 1;
            this.txt_import.Text = "可以自动搜索目录下的所有host(s)文件夹";
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(54, 120);
            this.btn_start.Margin = new System.Windows.Forms.Padding(6);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(188, 50);
            this.btn_start.TabIndex = 5;
            this.btn_start.Text = "开始";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // txt_info
            // 
            this.txt_info.Location = new System.Drawing.Point(54, 182);
            this.txt_info.Margin = new System.Windows.Forms.Padding(6);
            this.txt_info.Multiline = true;
            this.txt_info.Name = "txt_info";
            this.txt_info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_info.Size = new System.Drawing.Size(1011, 438);
            this.txt_info.TabIndex = 6;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 624);
            this.Controls.Add(this.txt_info);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.txt_import);
            this.Controls.Add(this.btn_openpath);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1097, 712);
            this.MinimumSize = new System.Drawing.Size(1097, 712);
            this.Name = "Form1";
            this.Text = "IP端口导出工具v1.4   ——By MC";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_openpath;
        private System.Windows.Forms.TextBox txt_import;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.TextBox txt_info;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

