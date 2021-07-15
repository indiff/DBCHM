namespace DBCHM
{
    partial class ImportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportForm));
            this.txtMulItem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnBrow = new System.Windows.Forms.Button();
            this.BtnUpdateDisplayName = new System.Windows.Forms.Button();
            this.GP_Import = new System.Windows.Forms.GroupBox();
            this.gpxDesc = new System.Windows.Forms.GroupBox();
            this.txtExplain = new System.Windows.Forms.TextBox();
            this.GP_Import.SuspendLayout();
            this.gpxDesc.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMulItem
            // 
            this.txtMulItem.Location = new System.Drawing.Point(77, 26);
            this.txtMulItem.Multiline = true;
            this.txtMulItem.Name = "txtMulItem";
            this.txtMulItem.ReadOnly = true;
            this.txtMulItem.Size = new System.Drawing.Size(497, 71);
            this.txtMulItem.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "文件路径：";
            // 
            // BtnBrow
            // 
            this.BtnBrow.Location = new System.Drawing.Point(178, 18);
            this.BtnBrow.Name = "BtnBrow";
            this.BtnBrow.Size = new System.Drawing.Size(104, 36);
            this.BtnBrow.TabIndex = 5;
            this.BtnBrow.Text = "选择(可多选)";
            this.BtnBrow.UseVisualStyleBackColor = true;
            this.BtnBrow.Click += new System.EventHandler(this.BtnBrow_Click);
            // 
            // BtnUpdateDisplayName
            // 
            this.BtnUpdateDisplayName.Location = new System.Drawing.Point(371, 18);
            this.BtnUpdateDisplayName.Name = "BtnUpdateDisplayName";
            this.BtnUpdateDisplayName.Size = new System.Drawing.Size(104, 36);
            this.BtnUpdateDisplayName.TabIndex = 8;
            this.BtnUpdateDisplayName.Text = "更新表列批注";
            this.BtnUpdateDisplayName.UseVisualStyleBackColor = true;
            this.BtnUpdateDisplayName.Click += new System.EventHandler(this.BtnUpdateDisplayName_Click);
            // 
            // GP_Import
            // 
            this.GP_Import.Controls.Add(this.label1);
            this.GP_Import.Controls.Add(this.txtMulItem);
            this.GP_Import.Location = new System.Drawing.Point(5, 59);
            this.GP_Import.Margin = new System.Windows.Forms.Padding(2);
            this.GP_Import.Name = "GP_Import";
            this.GP_Import.Padding = new System.Windows.Forms.Padding(2);
            this.GP_Import.Size = new System.Drawing.Size(584, 113);
            this.GP_Import.TabIndex = 9;
            this.GP_Import.TabStop = false;
            this.GP_Import.Text = "批注数据导入";
            // 
            // gpxDesc
            // 
            this.gpxDesc.Controls.Add(this.txtExplain);
            this.gpxDesc.Location = new System.Drawing.Point(5, 177);
            this.gpxDesc.Name = "gpxDesc";
            this.gpxDesc.Size = new System.Drawing.Size(584, 100);
            this.gpxDesc.TabIndex = 10;
            this.gpxDesc.TabStop = false;
            this.gpxDesc.Text = "使用说明";
            // 
            // txtExplain
            // 
            this.txtExplain.ForeColor = System.Drawing.Color.Black;
            this.txtExplain.Location = new System.Drawing.Point(7, 20);
            this.txtExplain.Multiline = true;
            this.txtExplain.Name = "txtExplain";
            this.txtExplain.ReadOnly = true;
            this.txtExplain.Size = new System.Drawing.Size(571, 73);
            this.txtExplain.TabIndex = 0;
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 282);
            this.Controls.Add(this.gpxDesc);
            this.Controls.Add(this.GP_Import);
            this.Controls.Add(this.BtnUpdateDisplayName);
            this.Controls.Add(this.BtnBrow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "批注上载";
            this.Load += new System.EventHandler(this.ImportForm_Load);
            this.GP_Import.ResumeLayout(false);
            this.GP_Import.PerformLayout();
            this.gpxDesc.ResumeLayout(false);
            this.gpxDesc.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtMulItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnBrow;
        private System.Windows.Forms.Button BtnUpdateDisplayName;
        private System.Windows.Forms.GroupBox GP_Import;
        private System.Windows.Forms.GroupBox gpxDesc;
        private System.Windows.Forms.TextBox txtExplain;
    }
}