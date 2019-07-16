namespace DBCHM
{
    partial class FrmProcessing
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbMessage = new System.Windows.Forms.Label();
            this.panImage = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lbMessage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panImage, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(776, 398);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lbMessage
            // 
            this.lbMessage.BackColor = System.Drawing.Color.Transparent;
            this.lbMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbMessage.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbMessage.Location = new System.Drawing.Point(4, 0);
            this.lbMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Padding = new System.Windows.Forms.Padding(0, 0, 0, 38);
            this.lbMessage.Size = new System.Drawing.Size(768, 199);
            this.lbMessage.TabIndex = 1;
            this.lbMessage.Text = "lbMessage\r\nadsfadsf";
            this.lbMessage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // panImage
            // 
            this.panImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panImage.Location = new System.Drawing.Point(4, 203);
            this.panImage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panImage.Name = "panImage";
            this.panImage.Size = new System.Drawing.Size(768, 191);
            this.panImage.TabIndex = 2;
            // 
            // FrmProcessing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(776, 398);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmProcessing";
            this.Opacity = 0.85D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FrmProcessing";
            this.Load += new System.EventHandler(this.FrmProcessing_Load);
            this.Shown += new System.EventHandler(this.FrmProcessing_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.Panel panImage;


    }


}
