namespace DBCHM
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.Prog = new System.Windows.Forms.ToolStripProgressBar();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ribbonPageAbout = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAbout = new System.Windows.Forms.ToolStripButton();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ribbonPageFile = new System.Windows.Forms.ToolStrip();
            this.tsbConnect = new System.Windows.Forms.ToolStripButton();
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsbPDMUpload = new System.Windows.Forms.ToolStripButton();
            this.tsbBuild = new System.Windows.Forms.ToolStripButton();
            this.tsWordExp = new System.Windows.Forms.ToolStripButton();
            this.tsExcelExp = new System.Windows.Forms.ToolStripButton();
            this.tsPdfExp = new System.Windows.Forms.ToolStripButton();
            this.tsHtmlExp = new System.Windows.Forms.ToolStripButton();
            this.tsXmlExp = new System.Windows.Forms.ToolStripButton();
            this.RibbonTabContainer = new System.Windows.Forms.TabControl();
            this.bgWork = new System.ComponentModel.BackgroundWorker();
            this.lblMsg = new System.Windows.Forms.Label();
            this.GpTableCol = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.gpCurrTable = new System.Windows.Forms.GroupBox();
            this.labCurrTabComment = new System.Windows.Forms.Label();
            this.TxtCurrTabComment = new System.Windows.Forms.TextBox();
            this.LabCurrTabName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.GpColumns = new System.Windows.Forms.GroupBox();
            this.lblTip = new System.Windows.Forms.Label();
            this.GV_ColComments = new System.Windows.Forms.DataGridView();
            this.BtnSaveGridData = new System.Windows.Forms.Button();
            this.TxtTabName = new System.Windows.Forms.TextBox();
            this.RibbonPanel = new System.Windows.Forms.Panel();
            this.ColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStripMain.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.ribbonPageAbout.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.ribbonPageFile.SuspendLayout();
            this.RibbonTabContainer.SuspendLayout();
            this.GpTableCol.SuspendLayout();
            this.gpCurrTable.SuspendLayout();
            this.GpColumns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GV_ColComments)).BeginInit();
            this.RibbonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripMain
            // 
            this.statusStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Prog});
            this.statusStripMain.Location = new System.Drawing.Point(0, 678);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(1076, 22);
            this.statusStripMain.TabIndex = 2;
            this.statusStripMain.Text = "statusStripMain";
            // 
            // Prog
            // 
            this.Prog.Name = "Prog";
            this.Prog.Size = new System.Drawing.Size(950, 16);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.ribbonPageAbout);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage3.Size = new System.Drawing.Size(1069, 73);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "关于";
            this.tabPage3.ToolTipText = "关于";
            // 
            // ribbonPageAbout
            // 
            this.ribbonPageAbout.BackColor = System.Drawing.SystemColors.Control;
            this.ribbonPageAbout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ribbonPageAbout.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ribbonPageAbout.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ribbonPageAbout.ImageScalingSize = new System.Drawing.Size(48, 48);
            this.ribbonPageAbout.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAbout});
            this.ribbonPageAbout.Location = new System.Drawing.Point(3, 2);
            this.ribbonPageAbout.Name = "ribbonPageAbout";
            this.ribbonPageAbout.Size = new System.Drawing.Size(1063, 70);
            this.ribbonPageAbout.TabIndex = 0;
            this.ribbonPageAbout.Text = "toolStripAbout";
            // 
            // toolStripButtonAbout
            // 
            this.toolStripButtonAbout.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAbout.Image")));
            this.toolStripButtonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAbout.Name = "toolStripButtonAbout";
            this.toolStripButtonAbout.Size = new System.Drawing.Size(52, 67);
            this.toolStripButtonAbout.Text = "About";
            this.toolStripButtonAbout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonAbout.Click += new System.EventHandler(this.toolStripButtonAbout_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ribbonPageFile);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Size = new System.Drawing.Size(1069, 73);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "文件";
            this.tabPage1.ToolTipText = "文件";
            // 
            // ribbonPageFile
            // 
            this.ribbonPageFile.BackColor = System.Drawing.SystemColors.Control;
            this.ribbonPageFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ribbonPageFile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ribbonPageFile.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ribbonPageFile.ImageScalingSize = new System.Drawing.Size(48, 48);
            this.ribbonPageFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbConnect,
            this.tsbRefresh,
            this.tsbPDMUpload,
            this.tsbBuild,
            this.tsWordExp,
            this.tsExcelExp,
            this.tsPdfExp,
            this.tsHtmlExp,
            this.tsXmlExp});
            this.ribbonPageFile.Location = new System.Drawing.Point(3, 2);
            this.ribbonPageFile.Name = "ribbonPageFile";
            this.ribbonPageFile.Size = new System.Drawing.Size(1063, 70);
            this.ribbonPageFile.TabIndex = 0;
            this.ribbonPageFile.Text = "toolStripFile";
            this.ribbonPageFile.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ribbonPageFile_ItemClicked);
            // 
            // tsbConnect
            // 
            this.tsbConnect.Image = ((System.Drawing.Image)(resources.GetObject("tsbConnect.Image")));
            this.tsbConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConnect.Name = "tsbConnect";
            this.tsbConnect.Size = new System.Drawing.Size(63, 67);
            this.tsbConnect.Text = "数据连接";
            this.tsbConnect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbConnect.Click += new System.EventHandler(this.tsbConnect_Click);
            // 
            // tsbRefresh
            // 
            this.tsbRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsbRefresh.Image")));
            this.tsbRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(63, 67);
            this.tsbRefresh.Text = "重新获取";
            this.tsbRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click);
            // 
            // tsbPDMUpload
            // 
            this.tsbPDMUpload.Image = ((System.Drawing.Image)(resources.GetObject("tsbPDMUpload.Image")));
            this.tsbPDMUpload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPDMUpload.Name = "tsbPDMUpload";
            this.tsbPDMUpload.Size = new System.Drawing.Size(62, 67);
            this.tsbPDMUpload.Text = "pdm上传";
            this.tsbPDMUpload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbPDMUpload.Click += new System.EventHandler(this.tsbSaveUpload_Click);
            // 
            // tsbBuild
            // 
            this.tsbBuild.Image = ((System.Drawing.Image)(resources.GetObject("tsbBuild.Image")));
            this.tsbBuild.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbBuild.Name = "tsbBuild";
            this.tsbBuild.Size = new System.Drawing.Size(65, 67);
            this.tsbBuild.Text = "CHM导出";
            this.tsbBuild.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbBuild.Click += new System.EventHandler(this.tsbBuild_Click);
            // 
            // tsWordExp
            // 
            this.tsWordExp.AutoSize = false;
            this.tsWordExp.Image = ((System.Drawing.Image)(resources.GetObject("tsWordExp.Image")));
            this.tsWordExp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsWordExp.Name = "tsWordExp";
            this.tsWordExp.Size = new System.Drawing.Size(60, 67);
            this.tsWordExp.Text = "Word导出";
            this.tsWordExp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsWordExp.Click += new System.EventHandler(this.tsWordExp_Click);
            // 
            // tsExcelExp
            // 
            this.tsExcelExp.AutoSize = false;
            this.tsExcelExp.Image = ((System.Drawing.Image)(resources.GetObject("tsExcelExp.Image")));
            this.tsExcelExp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsExcelExp.Name = "tsExcelExp";
            this.tsExcelExp.Size = new System.Drawing.Size(60, 67);
            this.tsExcelExp.Text = "Excel导出";
            this.tsExcelExp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsExcelExp.Click += new System.EventHandler(this.tsExcelExp_Click);
            // 
            // tsPdfExp
            // 
            this.tsPdfExp.AutoSize = false;
            this.tsPdfExp.Image = ((System.Drawing.Image)(resources.GetObject("tsPdfExp.Image")));
            this.tsPdfExp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPdfExp.Name = "tsPdfExp";
            this.tsPdfExp.Size = new System.Drawing.Size(60, 67);
            this.tsPdfExp.Text = "PDF导出";
            this.tsPdfExp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsPdfExp.Click += new System.EventHandler(this.tsPdfExp_Click);
            // 
            // tsHtmlExp
            // 
            this.tsHtmlExp.AutoSize = false;
            this.tsHtmlExp.Image = ((System.Drawing.Image)(resources.GetObject("tsHtmlExp.Image")));
            this.tsHtmlExp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsHtmlExp.Name = "tsHtmlExp";
            this.tsHtmlExp.Size = new System.Drawing.Size(64, 67);
            this.tsHtmlExp.Text = "Html导出";
            this.tsHtmlExp.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsHtmlExp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsHtmlExp.ToolTipText = "Html导出";
            this.tsHtmlExp.Click += new System.EventHandler(this.tsHtmlExp_Click);
            // 
            // tsXmlExp
            // 
            this.tsXmlExp.AutoSize = false;
            this.tsXmlExp.Image = ((System.Drawing.Image)(resources.GetObject("tsXmlExp.Image")));
            this.tsXmlExp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsXmlExp.Name = "tsXmlExp";
            this.tsXmlExp.Size = new System.Drawing.Size(60, 67);
            this.tsXmlExp.Text = "XML导出";
            this.tsXmlExp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsXmlExp.Click += new System.EventHandler(this.tsXmlExp_Click);
            // 
            // RibbonTabContainer
            // 
            this.RibbonTabContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RibbonTabContainer.Controls.Add(this.tabPage1);
            this.RibbonTabContainer.Controls.Add(this.tabPage3);
            this.RibbonTabContainer.ItemSize = new System.Drawing.Size(65, 20);
            this.RibbonTabContainer.Location = new System.Drawing.Point(0, 0);
            this.RibbonTabContainer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.RibbonTabContainer.Name = "RibbonTabContainer";
            this.RibbonTabContainer.SelectedIndex = 0;
            this.RibbonTabContainer.Size = new System.Drawing.Size(1077, 101);
            this.RibbonTabContainer.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.RibbonTabContainer.TabIndex = 3;
            this.RibbonTabContainer.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.RibbonTabContainer_Selecting);
            this.RibbonTabContainer.Selected += new System.Windows.Forms.TabControlEventHandler(this.RibbonTabContainer_Selected);
            this.RibbonTabContainer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.RibbonTabContainer_MouseClick);
            this.RibbonTabContainer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.RibbonTabContainer_MouseDoubleClick);
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Location = new System.Drawing.Point(965, 682);
            this.lblMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(0, 12);
            this.lblMsg.TabIndex = 5;
            // 
            // GpTableCol
            // 
            this.GpTableCol.Controls.Add(this.checkBox1);
            this.GpTableCol.Controls.Add(this.checkedListBox1);
            this.GpTableCol.Controls.Add(this.gpCurrTable);
            this.GpTableCol.Controls.Add(this.label1);
            this.GpTableCol.Controls.Add(this.GpColumns);
            this.GpTableCol.Controls.Add(this.TxtTabName);
            this.GpTableCol.Location = new System.Drawing.Point(6, 7);
            this.GpTableCol.Margin = new System.Windows.Forms.Padding(2);
            this.GpTableCol.Name = "GpTableCol";
            this.GpTableCol.Padding = new System.Windows.Forms.Padding(2);
            this.GpTableCol.Size = new System.Drawing.Size(1051, 562);
            this.GpTableCol.TabIndex = 0;
            this.GpTableCol.TabStop = false;
            this.GpTableCol.Text = "表列批注";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(3, 55);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(78, 16);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "全选/反选";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkedListBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(1, 80);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(273, 468);
            this.checkedListBox1.Sorted = true;
            this.checkedListBox1.TabIndex = 8;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // gpCurrTable
            // 
            this.gpCurrTable.Controls.Add(this.labCurrTabComment);
            this.gpCurrTable.Controls.Add(this.TxtCurrTabComment);
            this.gpCurrTable.Controls.Add(this.LabCurrTabName);
            this.gpCurrTable.Controls.Add(this.label2);
            this.gpCurrTable.Location = new System.Drawing.Point(304, 18);
            this.gpCurrTable.Margin = new System.Windows.Forms.Padding(2);
            this.gpCurrTable.Name = "gpCurrTable";
            this.gpCurrTable.Padding = new System.Windows.Forms.Padding(2);
            this.gpCurrTable.Size = new System.Drawing.Size(726, 73);
            this.gpCurrTable.TabIndex = 7;
            this.gpCurrTable.TabStop = false;
            this.gpCurrTable.Text = "表批注";
            // 
            // labCurrTabComment
            // 
            this.labCurrTabComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.labCurrTabComment.AutoSize = true;
            this.labCurrTabComment.Location = new System.Drawing.Point(210, 42);
            this.labCurrTabComment.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labCurrTabComment.Name = "labCurrTabComment";
            this.labCurrTabComment.Size = new System.Drawing.Size(53, 12);
            this.labCurrTabComment.TabIndex = 7;
            this.labCurrTabComment.Text = "表批注：";
            // 
            // TxtCurrTabComment
            // 
            this.TxtCurrTabComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.TxtCurrTabComment.Location = new System.Drawing.Point(265, 39);
            this.TxtCurrTabComment.Margin = new System.Windows.Forms.Padding(2);
            this.TxtCurrTabComment.Name = "TxtCurrTabComment";
            this.TxtCurrTabComment.Size = new System.Drawing.Size(300, 21);
            this.TxtCurrTabComment.TabIndex = 3;
            // 
            // LabCurrTabName
            // 
            this.LabCurrTabName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.LabCurrTabName.AutoSize = true;
            this.LabCurrTabName.Location = new System.Drawing.Point(270, 17);
            this.LabCurrTabName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabCurrTabName.Name = "LabCurrTabName";
            this.LabCurrTabName.Size = new System.Drawing.Size(0, 12);
            this.LabCurrTabName.TabIndex = 5;
            this.LabCurrTabName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(223, 17);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "表名：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "查询:";
            // 
            // GpColumns
            // 
            this.GpColumns.Controls.Add(this.lblTip);
            this.GpColumns.Controls.Add(this.GV_ColComments);
            this.GpColumns.Controls.Add(this.BtnSaveGridData);
            this.GpColumns.Location = new System.Drawing.Point(304, 95);
            this.GpColumns.Margin = new System.Windows.Forms.Padding(2);
            this.GpColumns.Name = "GpColumns";
            this.GpColumns.Padding = new System.Windows.Forms.Padding(2);
            this.GpColumns.Size = new System.Drawing.Size(733, 453);
            this.GpColumns.TabIndex = 2;
            this.GpColumns.TabStop = false;
            this.GpColumns.Text = "列批注";
            // 
            // lblTip
            // 
            this.lblTip.AutoSize = true;
            this.lblTip.Location = new System.Drawing.Point(53, 411);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(41, 12);
            this.lblTip.TabIndex = 9;
            this.lblTip.Text = "lblTip";
            // 
            // GV_ColComments
            // 
            this.GV_ColComments.AllowUserToAddRows = false;
            this.GV_ColComments.AllowUserToDeleteRows = false;
            this.GV_ColComments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GV_ColComments.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.GV_ColComments.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GV_ColComments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.GV_ColComments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GV_ColComments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColName,
            this.ColDataType,
            this.ColLength,
            this.ColComment});
            this.GV_ColComments.Location = new System.Drawing.Point(4, 18);
            this.GV_ColComments.Margin = new System.Windows.Forms.Padding(2);
            this.GV_ColComments.MultiSelect = false;
            this.GV_ColComments.Name = "GV_ColComments";
            this.GV_ColComments.RowHeadersVisible = false;
            this.GV_ColComments.RowTemplate.Height = 27;
            this.GV_ColComments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.GV_ColComments.Size = new System.Drawing.Size(722, 363);
            this.GV_ColComments.TabIndex = 0;
            this.GV_ColComments.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GV_ColComments_CellClick);
            // 
            // BtnSaveGridData
            // 
            this.BtnSaveGridData.Location = new System.Drawing.Point(529, 400);
            this.BtnSaveGridData.Margin = new System.Windows.Forms.Padding(2);
            this.BtnSaveGridData.Name = "BtnSaveGridData";
            this.BtnSaveGridData.Size = new System.Drawing.Size(98, 34);
            this.BtnSaveGridData.TabIndex = 8;
            this.BtnSaveGridData.Text = "保存";
            this.BtnSaveGridData.Click += new System.EventHandler(this.BtnSaveGridData_Click);
            // 
            // TxtTabName
            // 
            this.TxtTabName.Location = new System.Drawing.Point(40, 26);
            this.TxtTabName.Margin = new System.Windows.Forms.Padding(2);
            this.TxtTabName.Name = "TxtTabName";
            this.TxtTabName.Size = new System.Drawing.Size(221, 21);
            this.TxtTabName.TabIndex = 0;
            this.TxtTabName.TextChanged += new System.EventHandler(this.TxtTabName_TextChanged);
            // 
            // RibbonPanel
            // 
            this.RibbonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RibbonPanel.Controls.Add(this.GpTableCol);
            this.RibbonPanel.Location = new System.Drawing.Point(0, 98);
            this.RibbonPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.RibbonPanel.Name = "RibbonPanel";
            this.RibbonPanel.Size = new System.Drawing.Size(1070, 578);
            this.RibbonPanel.TabIndex = 4;
            // 
            // ColName
            // 
            this.ColName.Frozen = true;
            this.ColName.HeaderText = "列名";
            this.ColName.Name = "ColName";
            this.ColName.ReadOnly = true;
            this.ColName.Width = 220;
            // 
            // ColDataType
            // 
            this.ColDataType.Frozen = true;
            this.ColDataType.HeaderText = "数据类型";
            this.ColDataType.Name = "ColDataType";
            this.ColDataType.ReadOnly = true;
            // 
            // ColLength
            // 
            this.ColLength.Frozen = true;
            this.ColLength.HeaderText = "长度";
            this.ColLength.Name = "ColLength";
            this.ColLength.ReadOnly = true;
            this.ColLength.Width = 80;
            // 
            // ColComment
            // 
            this.ColComment.HeaderText = "列批注";
            this.ColComment.Name = "ColComment";
            this.ColComment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColComment.Width = 450;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 700);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.RibbonPanel);
            this.Controls.Add(this.RibbonTabContainer);
            this.Controls.Add(this.statusStripMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DBCHM";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ribbonPageAbout.ResumeLayout(false);
            this.ribbonPageAbout.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ribbonPageFile.ResumeLayout(false);
            this.ribbonPageFile.PerformLayout();
            this.RibbonTabContainer.ResumeLayout(false);
            this.GpTableCol.ResumeLayout(false);
            this.GpTableCol.PerformLayout();
            this.gpCurrTable.ResumeLayout(false);
            this.gpCurrTable.PerformLayout();
            this.GpColumns.ResumeLayout(false);
            this.GpColumns.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GV_ColComments)).EndInit();
            this.RibbonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ToolStrip ribbonPageAbout;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbout;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStrip ribbonPageFile;
        private System.Windows.Forms.ToolStripButton tsbConnect;
        private System.Windows.Forms.ToolStripButton tsbBuild;
        private System.Windows.Forms.TabControl RibbonTabContainer;
        private System.Windows.Forms.ToolStripButton tsbRefresh;
        private System.Windows.Forms.ToolStripButton tsbPDMUpload;
        private System.Windows.Forms.ToolStripProgressBar Prog;
        private System.ComponentModel.BackgroundWorker bgWork;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.ToolStripButton tsWordExp;
        private System.Windows.Forms.ToolStripButton tsExcelExp;
        private System.Windows.Forms.ToolStripButton tsPdfExp;
        private System.Windows.Forms.ToolStripButton tsXmlExp;
        private System.Windows.Forms.GroupBox GpTableCol;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.GroupBox gpCurrTable;
        private System.Windows.Forms.Label labCurrTabComment;
        private System.Windows.Forms.TextBox TxtCurrTabComment;
        private System.Windows.Forms.Label LabCurrTabName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox GpColumns;
        private System.Windows.Forms.Label lblTip;
        private System.Windows.Forms.DataGridView GV_ColComments;
        private System.Windows.Forms.Button BtnSaveGridData;
        private System.Windows.Forms.TextBox TxtTabName;
        private System.Windows.Forms.Panel RibbonPanel;
        private System.Windows.Forms.ToolStripButton tsHtmlExp;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColComment;
    }
}