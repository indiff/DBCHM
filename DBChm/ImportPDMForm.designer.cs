namespace DBCHM
{
    partial class ImportPDMForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportPDMForm));
            this.txtMulItem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnBrow = new System.Windows.Forms.Button();
            this.BtnUpdateDisplayName = new System.Windows.Forms.Button();
            this.GP_PDM_Import = new System.Windows.Forms.GroupBox();
            this.GP_PDM_Import.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMulItem
            // 
            this.txtMulItem.Location = new System.Drawing.Point(142, 32);
            this.txtMulItem.Margin = new System.Windows.Forms.Padding(4);
            this.txtMulItem.Multiline = true;
            this.txtMulItem.Name = "txtMulItem";
            this.txtMulItem.ReadOnly = true;
            this.txtMulItem.Size = new System.Drawing.Size(570, 88);
            this.txtMulItem.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "pdm 文件路径：";
            // 
            // BtnBrow
            // 
            this.BtnBrow.Location = new System.Drawing.Point(267, 22);
            this.BtnBrow.Margin = new System.Windows.Forms.Padding(4);
            this.BtnBrow.Name = "BtnBrow";
            this.BtnBrow.Size = new System.Drawing.Size(109, 45);
            this.BtnBrow.TabIndex = 5;
            this.BtnBrow.Text = "选择(可多选)";
            this.BtnBrow.UseVisualStyleBackColor = true;
            this.BtnBrow.Click += new System.EventHandler(this.BtnBrow_Click);
            // 
            // BtnUpdateDisplayName
            // 
            this.BtnUpdateDisplayName.Location = new System.Drawing.Point(512, 22);
            this.BtnUpdateDisplayName.Margin = new System.Windows.Forms.Padding(4);
            this.BtnUpdateDisplayName.Name = "BtnUpdateDisplayName";
            this.BtnUpdateDisplayName.Size = new System.Drawing.Size(119, 45);
            this.BtnUpdateDisplayName.TabIndex = 8;
            this.BtnUpdateDisplayName.Text = "更新表列批注";
            this.BtnUpdateDisplayName.UseVisualStyleBackColor = true;
            this.BtnUpdateDisplayName.Click += new System.EventHandler(this.BtnUpdateDisplayName_Click);
            // 
            // GP_PDM_Import
            // 
            this.GP_PDM_Import.Controls.Add(this.label1);
            this.GP_PDM_Import.Controls.Add(this.txtMulItem);
            this.GP_PDM_Import.Location = new System.Drawing.Point(12, 74);
            this.GP_PDM_Import.Name = "GP_PDM_Import";
            this.GP_PDM_Import.Size = new System.Drawing.Size(757, 141);
            this.GP_PDM_Import.TabIndex = 9;
            this.GP_PDM_Import.TabStop = false;
            this.GP_PDM_Import.Text = "pdm文件导入";
            // 
            // ImportPDMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 236);
            this.Controls.Add(this.GP_PDM_Import);
            this.Controls.Add(this.BtnUpdateDisplayName);
            this.Controls.Add(this.BtnBrow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportPDMForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "pdm文件导入";
            this.GP_PDM_Import.ResumeLayout(false);
            this.GP_PDM_Import.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtMulItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnBrow;
        private System.Windows.Forms.Button BtnUpdateDisplayName;
        private System.Windows.Forms.GroupBox GP_PDM_Import;
    }
}