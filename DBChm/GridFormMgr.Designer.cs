namespace DBCHM
{
    partial class GridFormMgr
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridFormMgr));
            this.linkAdd = new System.Windows.Forms.LinkLabel();
            this.linkEdit = new System.Windows.Forms.LinkLabel();
            this.linkRemove = new System.Windows.Forms.LinkLabel();
            this.linkClone = new System.Windows.Forms.LinkLabel();
            this.BtnConnect = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.GV_DBConfigs = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.GV_DBConfigs)).BeginInit();
            this.SuspendLayout();
            // 
            // linkAdd
            // 
            this.linkAdd.AutoSize = true;
            this.linkAdd.Location = new System.Drawing.Point(30, 13);
            this.linkAdd.Name = "linkAdd";
            this.linkAdd.Size = new System.Drawing.Size(37, 15);
            this.linkAdd.TabIndex = 0;
            this.linkAdd.TabStop = true;
            this.linkAdd.Text = "新建";
            this.linkAdd.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAdd_LinkClicked);
            // 
            // linkEdit
            // 
            this.linkEdit.AutoSize = true;
            this.linkEdit.Location = new System.Drawing.Point(84, 13);
            this.linkEdit.Name = "linkEdit";
            this.linkEdit.Size = new System.Drawing.Size(37, 15);
            this.linkEdit.TabIndex = 0;
            this.linkEdit.TabStop = true;
            this.linkEdit.Text = "编辑";
            this.linkEdit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEdit_LinkClicked);
            // 
            // linkRemove
            // 
            this.linkRemove.AutoSize = true;
            this.linkRemove.Location = new System.Drawing.Point(137, 13);
            this.linkRemove.Name = "linkRemove";
            this.linkRemove.Size = new System.Drawing.Size(37, 15);
            this.linkRemove.TabIndex = 0;
            this.linkRemove.TabStop = true;
            this.linkRemove.Text = "删除";
            this.linkRemove.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkRemove_LinkClicked);
            // 
            // linkClone
            // 
            this.linkClone.AutoSize = true;
            this.linkClone.Location = new System.Drawing.Point(189, 13);
            this.linkClone.Name = "linkClone";
            this.linkClone.Size = new System.Drawing.Size(37, 15);
            this.linkClone.TabIndex = 0;
            this.linkClone.TabStop = true;
            this.linkClone.Text = "克隆";
            this.linkClone.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkClone_LinkClicked);
            // 
            // BtnConnect
            // 
            this.BtnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnConnect.Location = new System.Drawing.Point(521, 319);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(80, 32);
            this.BtnConnect.TabIndex = 3;
            this.BtnConnect.Text = "连接";
            this.BtnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(653, 319);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(78, 32);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "取消";
            this.BtnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // GV_DBConfigs
            // 
            this.GV_DBConfigs.AllowUserToAddRows = false;
            this.GV_DBConfigs.AllowUserToDeleteRows = false;
            this.GV_DBConfigs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GV_DBConfigs.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.GV_DBConfigs.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GV_DBConfigs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.GV_DBConfigs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GV_DBConfigs.Location = new System.Drawing.Point(12, 43);
            this.GV_DBConfigs.MultiSelect = false;
            this.GV_DBConfigs.Name = "GV_DBConfigs";
            this.GV_DBConfigs.ReadOnly = true;
            this.GV_DBConfigs.RowHeadersVisible = false;
            this.GV_DBConfigs.RowTemplate.Height = 27;
            this.GV_DBConfigs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GV_DBConfigs.Size = new System.Drawing.Size(735, 260);
            this.GV_DBConfigs.TabIndex = 5;
            this.GV_DBConfigs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GV_DBConfigs_CellDoubleClick);
            // 
            // GridFormMgr
            // 
            this.AcceptButton = this.BtnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 363);
            this.Controls.Add(this.GV_DBConfigs);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnConnect);
            this.Controls.Add(this.linkClone);
            this.Controls.Add(this.linkRemove);
            this.Controls.Add(this.linkEdit);
            this.Controls.Add(this.linkAdd);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GridFormMgr";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据库连接管理";
            this.Load += new System.EventHandler(this.GridFormMgr_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GV_DBConfigs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkAdd;
        private System.Windows.Forms.LinkLabel linkEdit;
        private System.Windows.Forms.LinkLabel linkRemove;
        private System.Windows.Forms.LinkLabel linkClone;
        private System.Windows.Forms.Button BtnConnect;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.DataGridView GV_DBConfigs;
    }
}