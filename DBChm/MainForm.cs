//-----------------------------------------------------------------------
// <copyright file="WinFormRibbon.cs" company="YuGuan Corporation">
//     Copyright (c) YuGuan Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DBCHM
{
    using CHM;
    using ComponentFactory.Krypton.Toolkit;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using TryOpenXml.Dtos;

    /// <summary>
    /// Class MainForm.
    /// </summary>
    public partial class MainForm : KryptonForm
    {
        // TODO 已选择的表
        private string selectedTableDesc = "已选择{0}张表";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm" /> class.
        /// </summary>
        public MainForm()
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            KryptonManager kryptonManager = new KryptonManager();
            this.InitializeComponent();
            this.InitializeRibbonTabContainer();
            kryptonManager.GlobalPaletteMode = PaletteModeManager.Office2010Blue;
        }

        /// <summary>
        /// Const Field RIBBON_COLLAPSE_HEIGHT.
        /// </summary>
        private const int RIBBON_COLLAPSE_HEIGHT = 22;

        /// <summary>
        /// Const Field RIBBON_EXPAND_HEIGHT.
        /// </summary>
        private const int RIBBON_EXPAND_HEIGHT = 100;

        /// <summary>
        /// Const Field FORM_BORDER_HEIGHT.
        /// </summary>
        private const int FORM_BORDER_HEIGHT = 60;

        /// <summary>
        /// Field _isRibbonTabExpand.
        /// </summary>
        private bool _isRibbonTabExpand;

        /// <summary>
        /// Field _isRibbonTabShow.
        /// </summary>
        private bool _isRibbonTabShow;

        /// <summary>
        /// Method InitializeRibbonTabContainer.
        /// </summary>
        private void InitializeRibbonTabContainer()
        {
            this._isRibbonTabExpand = true;
            this._isRibbonTabShow = true;
            this.CollapseRibbonTabContainer(!this._isRibbonTabExpand);
            this.RibbonTabContainer.LostFocus += this.HideRibbon;
            this.ribbonPageFile.ItemClicked += this.HideRibbon;
            this.ribbonPageAbout.ItemClicked += this.HideRibbon;
        }

        #region Ribbon
        /// <summary>
        /// Method RibbonTabContainer_MouseDoubleClick.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Instance of MouseEventArgs.</param>
        private void RibbonTabContainer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.CollapseRibbonTabContainer(this._isRibbonTabExpand);
        }

        /// <summary>
        /// Method CollapseRibbonTabContainer.
        /// </summary>
        /// <param name="whetherCollapse">Indicate whether collapse or not.</param>
        private void CollapseRibbonTabContainer(bool whetherCollapse)
        {
            if (whetherCollapse)
            {
                this.RibbonTabContainer.Height = RIBBON_COLLAPSE_HEIGHT;
                this.RibbonPanel.Location = new System.Drawing.Point(0, RIBBON_COLLAPSE_HEIGHT);
                this.RibbonPanel.Height = this.Height - RIBBON_COLLAPSE_HEIGHT - FORM_BORDER_HEIGHT;
                this._isRibbonTabExpand = false;
                this._isRibbonTabShow = false;
            }
            else
            {
                this.RibbonTabContainer.Height = RIBBON_EXPAND_HEIGHT;
                this.RibbonPanel.Location = new System.Drawing.Point(0, RIBBON_EXPAND_HEIGHT);
                this.RibbonPanel.Height = this.Height - RIBBON_EXPAND_HEIGHT - FORM_BORDER_HEIGHT;
                this._isRibbonTabExpand = true;
                this._isRibbonTabShow = true;
            }
        }

        /// <summary>
        /// Method RibbonTabContainer_MouseClick.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Instance of MouseEventArgs.</param>
        private void RibbonTabContainer_MouseClick(object sender, MouseEventArgs e)
        {
            if (!this._isRibbonTabExpand)
            {
                if (!this._isRibbonTabShow)
                {
                    this.RibbonTabContainer.Height = RIBBON_EXPAND_HEIGHT;
                    this.RibbonTabContainer.BringToFront();
                    this._isRibbonTabShow = true;
                }
                else
                {
                    this.RibbonTabContainer.Height = RIBBON_COLLAPSE_HEIGHT;
                    this._isRibbonTabShow = false;
                }
            }
        }

        /// <summary>
        /// Method RibbonTabContainer_Selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Instance of TabControlEventArgs.</param>
        private void RibbonTabContainer_Selected(object sender, TabControlEventArgs e)
        {
            this._isRibbonTabShow = false;
        }

        /// <summary>
        /// Method RibbonTabContainer_Selecting.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Instance of TabControlCancelEventArgs.</param>
        private void RibbonTabContainer_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!this._isRibbonTabExpand)
            {
                this.RibbonTabContainer.Height = RIBBON_EXPAND_HEIGHT;
                this.RibbonTabContainer.BringToFront();
            }
        }

        /// <summary>
        /// Method HideRibbon.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Instance of EventArgs.</param>
        private void HideRibbon(object sender, EventArgs e)
        {
            if (!this._isRibbonTabExpand)
            {
                if (this._isRibbonTabShow)
                {
                    this.RibbonTabContainer.Height = RIBBON_COLLAPSE_HEIGHT;
                    this._isRibbonTabShow = false;
                }
            }
        }

        private void ribbonPageFile_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblTip.Text = string.Empty;

            // 开启 报告进度更新
            bgWork.WorkerReportsProgress = true;

            // 注册 任务执行事件
            bgWork.DoWork += BgWork_DoWork;

            // 注册 报告进度更新事件
            bgWork.ProgressChanged += BgWork_ProgressChanged;

            //初始化窗体
            InitMain();
        }

        private void InitMain()
        {
            GridFormMgr conMgrForm = new GridFormMgr();
            var diaRes = conMgrForm.ShowDialog(this);
            if (diaRes == DialogResult.OK || FormUtils.IsOK_Close) //当前窗体 是正常关闭的情况下
            {
                CkListBox.DataSource = DBUtils.Instance?.Info?.TableNames;
                FormUtils.IsOK_Close = false;
            }
            else
            {
                return;
            }

            //默认全选
            this.CkAll.Checked = true;
            CkAll_Click(null, null);

            if (CkListBox.Items.Count > 0)//默认选择第一张表
            {
                CkListBox.SelectedIndex = 0;
                LabCurrTabName.Text = CkListBox.SelectedItems[0].ToString();
                TxtCurrTabComment.Text = DBUtils.Instance?.Info?.TableComments[LabCurrTabName.Text];
            }
            else//无数据表时，清空 Gird列表
            {
                GV_ColComments.Rows.Clear();
            }

            if (!string.IsNullOrWhiteSpace(DBUtils.Instance?.Info?.DBName))
            {
                this.Text = DBUtils.Instance?.Info?.DBName + "(" + DBUtils.Instance.DBType.ToString() + ") - " + "DBCHM v" + Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".0.0", "");
            }

            //Sqlite 数据库自身 不支持 数据库批注 功能
            if (DBUtils.Instance != null && DBUtils.Instance.DBType == MJTop.Data.DBType.SQLite)
            {
                TxtCurrTabComment.Enabled = false;
                BtnSaveGridData.Enabled = false;
                lblTip.Text = DBUtils.Instance.DBType + "数据库不支持批注功能！";
                lblTip.ForeColor = System.Drawing.Color.Red;
                GV_ColComments.Columns[1].ReadOnly = true;
            }
            else
            {
                TxtCurrTabComment.Enabled = true;
                BtnSaveGridData.Enabled = true;
                lblTip.Text = string.Empty;
                lblTip.ForeColor = System.Drawing.Color.Green;
                GV_ColComments.Columns[1].ReadOnly = false;
            }

            //除了列描述，其余列全部 都居中
            for (int j = 0; j < GV_ColComments.Columns.Count - 1; j++)
            {
                GV_ColComments.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        /// <summary>
        /// 模糊搜索数据表名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtSearchWords_TextChanged(object sender, EventArgs e)
        {
            string searchWords = TxtSearchWords.Text.Trim().ToLower();
            var lstTableName = DBUtils.Instance?.Info?.TableNames ?? new List<string>();
            this.lblSelectRes.Text = string.Format(selectedTableDesc, 0);
            if (!string.IsNullOrWhiteSpace(searchWords))
            {
                var lstContains = new HashSet<string>();
                var words = searchWords.Split(new string[] { ",", "，" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    foreach (var tabName in lstTableName)
                    {
                        if (tabName.ToLower().Contains(word.ToLower()))
                        {
                            lstContains.Add(tabName);
                        }
                    }
                }
                // TODO 清空重置
                CkListBox.DataSource = null;
                CkListBox.Items.Clear();
                var lstNotContains = lstTableName.Except(lstContains).ToList();
                if (lstContains.Any())
                {
                    CkAll.Checked = false;
                    // TODO 搜索到的表名 靠前显示
                    CkListBox.Items.AddRange(lstContains.ToArray());
                    CkListBox.Items.AddRange(lstNotContains.ToArray());
                    for (int j = 0; j < CkListBox.Items.Count; j++)
                    {
                        var item = CkListBox.Items[j].ToString();
                        // TODO 作为排除 参与导出的表名 并且默认不选中
                        if (lstContains.Contains(item))
                        {
                            CkListBox.SetItemChecked(j, true);
                        }
                        else
                        {
                            CkListBox.SetItemChecked(j, false);
                        }
                    }
                    this.lblSelectRes.Text = string.Format(selectedTableDesc, CkListBox.CheckedItems.Count);
                }
            }
            else
            {
                CkListBox.DataSource = DBUtils.Instance?.Info?.TableNames;
                CkAll.Checked = true;
                CkAll_Click(null, null);
            }

            if (CkListBox.Items.Count > 0)
            {
                //默认选择第一张表
                CkListBox.SelectedIndex = 0;
                LabCurrTabName.Text = CkListBox.SelectedItems[0].ToString();
                TxtCurrTabComment.Text = DBUtils.Instance?.Info?.TableComments[LabCurrTabName.Text];
            }
        }


        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CkAll_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int j = 0; j < CkListBox.Items.Count; j++)
            {
               // string itemText = CkListBox.GetItemText(j);
                string itemText = CkListBox.Items[j].ToString();
                // 需要判断文本框里面有的值则过滤
                // if (0 == j)
                // {
                //   MessageBox.Show( itemText + "->" + filterTextBox.Text);
                // }
                if (acceptFilter(itemText))
                {
                    CkListBox.SetItemChecked(j, CkAll.Checked);
                    count++;
                }
                else
                {
                    
                }
            }
           // this.lblSelectRes.Text = string.Format(selectedTableDesc, CkListBox.CheckedItems.Count);
            this.lblSelectRes.Text = string.Format(selectedTableDesc, count);
        }

        // 包含字符串的则过滤不选择
        private bool acceptFilter(string itemText)
        {
            string filterText = filterTextBox.Text.Trim();
            itemText = itemText.Trim();
            if ("".Equals(itemText))
            {
                return false;
            }
            if (!"".Equals(filterText))
            {
                string[] texts = filterText.Split(',');
                foreach (var text in texts)
                {
                    if (itemText.Contains(text))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 反选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CkReverse_Click(object sender, EventArgs e)
        {
            for (int j = 0; j < CkListBox.Items.Count; j++)
            {
                var ck = CkListBox.GetItemChecked(j);
                CkListBox.SetItemChecked(j, !ck);
            }
            CkAll.Checked = CkListBox.Items.Count == CkListBox.CheckedItems.Count;
            this.lblSelectRes.Text = string.Format(selectedTableDesc, CkListBox.CheckedItems.Count);
        }


        private void CkListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CkListBox.SelectedItems.Count > 0)
            {
                //数据重置 
                Prog.Value = 0;
                lblMsg.Text = string.Empty;
                GV_ColComments.Rows.Clear();

                LabCurrTabName.Text = CkListBox.SelectedItems[0].ToString();
                TxtCurrTabComment.Text = DBUtils.Instance?.Info?.TableComments[LabCurrTabName.Text];

                var columnInfos = DBUtils.Instance?.Info?.GetColumns(LabCurrTabName.Text);

                if (columnInfos != null)
                {
                    foreach (var colInfo in columnInfos)
                    {
                        GV_ColComments.Rows.Add(colInfo.ColumnName, colInfo.TypeName, colInfo.Length, colInfo.DeText);
                    }
                }
                this.lblSelectRes.Text = string.Format(selectedTableDesc, CkListBox.CheckedItems.Count);
            }
        }

        
        /// <summary>
        /// 数据连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbConnect_Click(object sender, EventArgs e)
        {
            lblTip.Text = string.Empty;
            InitMain();
        }

        /// <summary>
        /// 重新获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            FormUtils.ShowProcessing("正在查询表结构信息，请稍等......", this, arg =>
            {
                DBUtils.Instance?.Info?.Refresh();
                TxtSearchWords_TextChanged(sender, e);

            }, null);
        }

        /// <summary>
        /// pdm上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbSaveUpload_Click(object sender, EventArgs e)
        {
            ImportForm pdmForm = new ImportForm();
            DialogResult dirRes = pdmForm.ShowDialog(this);
            if (dirRes == DialogResult.OK || FormUtils.IsOK_Close)
            {
                FormUtils.IsOK_Close = false;
                TxtSearchWords_TextChanged(sender, e);
            }
        }

        /// <summary>
        /// chm文件根路径
        /// </summary>
        private string dirPath = string.Empty;
        /// <summary>
        /// CHM 文件绝对路径
        /// </summary>
        private string chm_path = string.Empty;
        /// <summary>
        /// 默认html
        /// </summary>
        private string defaultHtml = "数据库表目录.html";
        /// <summary>
        /// 索引文件路径
        /// </summary>
        private string indexHtmlpath = string.Empty;

        #region 导出方法
        private void ExportToChm()
        {
            #region 使用 HTML Help Workshop 的 hhc.exe 编译 ,先判断系统中是否已经安装有  HTML Help Workshop 

            string hhcPath = string.Empty;

            if (!ConfigUtils.CheckInstall("HTML Help Workshop", "hhc.exe", out hhcPath))
            {
                string htmlhelpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "htmlhelp.exe");
                if (File.Exists(htmlhelpPath))
                {
                    if (MessageBox.Show("导出CHM文档需安装 HTML Help Workshop ，是否现在安装？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                    {
                        var proc = Process.Start(htmlhelpPath);
                    }
                }
                return;
            }

            #endregion

            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "(*.chm)|*.chm";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".chm";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.chm";
            if (saveDia.ShowDialog(this) == DialogResult.OK)
            {
                chm_path = saveDia.FileName;

                System.Diagnostics.Process process;
                if (IsExistProcess(Path.GetFileName(saveDia.FileName), out process))
                {
                    var dia = MessageBox.Show("文件已打开，导出前需关闭，是否继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dia == DialogResult.OK)
                    {
                        process.Kill();
                    }
                }

                try
                {
                    //创建临时文件夹,存在则删除，防止已经存在的文件 会导致生成出来的chm 有问题
                    dirPath = Path.Combine(ConfigUtils.AppPath, DBUtils.Instance.DBType + "_" + DBUtils.Instance.Info.DBName);
                    if (ZetaLongPaths.ZlpIOHelper.DirectoryExists(dirPath))
                    {
                        ZetaLongPaths.ZlpIOHelper.DeleteDirectory(dirPath, true);
                    }
                    ZetaLongPaths.ZlpIOHelper.CreateDirectory(dirPath);
                    ConfigUtils.AddSecurityControll2Folder(dirPath);
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("文件目录创建出错", Developer.SysDefault, ex, dirPath);
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //设置要 进度条 对应的执行方法，以及进度条最大值
                FormUtils.ProgArg = new ProgressArg(() =>
                {
                    try
                    {
                        System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();

                        //生成数据库目录文件
                        indexHtmlpath = Path.Combine(dirPath, defaultHtml);
                        //ChmHtmlHelper.CreateDirHtml("数据库表目录", DBUtils.Instance.Info.TableComments, indexHtmlpath);
                        ChmHtmlHelper.CreateDirHtml("数据库表目录", tableDtos, indexHtmlpath);

                        string structPath = Path.Combine(dirPath, "表结构");
                        if (!ZetaLongPaths.ZlpIOHelper.DirectoryExists(structPath))
                        {
                            ZetaLongPaths.ZlpIOHelper.CreateDirectory(structPath);
                        }

                        bgWork.ReportProgress(2);

                        //生成每张表列结构的html
                        //ChmHtmlHelper.CreateHtml(DBUtils.Instance.Info.TableInfoDict, structPath);
                        ChmHtmlHelper.CreateHtml(tableDtos, structPath);

                        bgWork.ReportProgress(3);

                        ChmHelp c3 = new ChmHelp();
                        c3.HHCPath = hhcPath;
                        c3.DefaultPage = defaultHtml;
                        c3.Title = Path.GetFileName(chm_path);
                        c3.ChmFileName = chm_path;
                        c3.SourcePath = dirPath;
                        c3.Compile();

                        bgWork.ReportProgress(4);

                        if (MessageBox.Show("生成CHM文档成功，是否打开？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(saveDia.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                        bgWork.ReportProgress(4, ex);
                    }

                }, 4);

                bgWork.RunWorkerAsync();
            }
        }

        private void ExportToWord()
        {
            #region 引用Microsoft.Office.Interop.Word.dll导出word文档方法弃用，改为引用Aspose.Words.dll方法导出word文档
            //FormUtils.ShowProcessing("正在导出数据字典Word文档，请稍等......", this, arg =>
            //{
            //    try
            //    {
            //        System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
            //        TryOpenXml.Text.WordUtils.ExportWordByMicrosoftOfficeInteropWord(DBUtils.Instance.Info.DBName, tableDtos);

            //        MessageBox.Show("生成数据库字典Word文档成功！", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //    }
            //    catch (Exception ex)
            //    {
            //        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
            //    }

            //}, null); 
            #endregion

            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "Word files (*.doc)|*.doc";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".doc";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.doc";
            if (saveDia.ShowDialog(this) == DialogResult.OK)
            {
                //System.Diagnostics.Process process;
                //if (IsExistProcess(Path.GetFileName(saveDia.FileName), out process))
                //{
                //    var dia = MessageBox.Show("文件已打开，导出前需关闭，是否继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                //    if (dia == DialogResult.OK)
                //    {
                //        process.Kill();
                //    }
                //}

                FormUtils.ShowProcessing("正在导出数据字典Word文档，请稍等......", this, arg =>
                {
                    try
                    {
                        System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
                        TryOpenXml.Text.WordUtils.ExportWordByAsposeWords(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                        if (MessageBox.Show("生成数据库字典Word文档成功，是否打开？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(saveDia.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                    }

                }, null);
            }
        }

        private void ExportToExcel()
        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".xlsx";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.xlsx";
            if (saveDia.ShowDialog(this) == DialogResult.OK)
            {
                //System.Diagnostics.Process process;
                //if (IsExistProcess(Path.GetFileName(saveDia.FileName), out process))
                //{
                //    var dia = MessageBox.Show("文件已打开，导出前需关闭，是否继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                //    if (dia == DialogResult.OK)
                //    {
                //        process.Kill();
                //    }
                //}

                FormUtils.ShowProcessing("正在导出数据字典Excel文档，请稍等......", this, arg =>
                {
                    try
                    {
                        System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
                        TryOpenXml.Text.ExcelUtils.ExportExcelByEpplus(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                        if (MessageBox.Show("生成数据库字典Excel文档成功，是否打开？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(saveDia.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                    }

                }, null);
            }
        }

        private void ExportToPdf()
        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "Text documents (.pdf)|*.pdf";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".pdf";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.pdf";
            if (saveDia.ShowDialog(this) == DialogResult.OK)
            {
                //System.Diagnostics.Process process;
                //if (IsExistProcess(Path.GetFileName(saveDia.FileName), out process))
                //{
                //    var dia = MessageBox.Show("文件已打开，导出前需关闭，是否继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                //    if (dia == DialogResult.OK)
                //    {
                //        process.Kill();
                //    }
                //}

                FormUtils.ShowProcessing("正在导出数据字典PDF文档，请稍等......", this, arg =>
                {
                    try
                    {
                        // TODO 中文ttf字体库文件（微软雅黑）
                        string baseFontPath = System.Windows.Forms.Application.StartupPath + "\\Fonts\\msyh.ttf";
                        System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
                        TryOpenXml.Text.PdfUtils.ExportPdfByITextSharp(saveDia.FileName, baseFontPath, DBUtils.Instance.Info.DBName, tableDtos);

                        if (MessageBox.Show("生成数据库字典PDF文档成功，是否打开？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(saveDia.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                    }

                }, null);
            }
        }

        private void ExportToXml()
        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "XML files (*.xml)|*.xml";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".xml";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.xml";
            if (saveDia.ShowDialog(this) == DialogResult.OK)
            {
                FormUtils.ShowProcessing("正在导出数据字典XML文档，请稍等......", this, arg =>
                {
                    try
                    {
                        System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
                        TryOpenXml.Text.XmlUtils.ExportXml(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                        if (MessageBox.Show("生成数据库字典XML文档成功，是否打开？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(saveDia.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                    }

                }, null);
            }
        }

        private void ExportToHtml()
        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "html files (*.html)|*.html";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".html";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.html";
            if (saveDia.ShowDialog(this) == DialogResult.OK)
            {
                FormUtils.ShowProcessing("正在导出数据字典html文档，请稍等......", this, arg =>
                {
                    try
                    {
                        System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
                        TryOpenXml.Text.HtmlUtils.ExportHtml(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                        if (MessageBox.Show("生成数据库字典HTML文档成功，是否打开？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(saveDia.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                    }

                }, null);
            }
        }

        private void ExportToMarkDown()
        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "markdown files (*.md)|*.md";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".md";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.md";
            if (saveDia.ShowDialog(this) == DialogResult.OK)
            {
                FormUtils.ShowProcessing("正在导出数据字典markdown文档，请稍等......", this, arg =>
                {
                    try
                    {
                        System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();

                        TryOpenXml.Text.MarkDownUtils.Export(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                        if (MessageBox.Show("生成数据库字典markdown文档成功，是否打开？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(saveDia.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                    }

                }, null);
            }
        }
        #endregion

        #region 导出方法
        /// <summary>
        /// chm文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbBuild_Click(object sender, EventArgs e)
        {
            // TODO 导出chm
            ExportToChm();
        }

        /// <summary>
        /// word文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsWordExp_Click(object sender, EventArgs e)
        {
            // TODO 导出word
            ExportToWord();
        }

        /// <summary>
        /// excel文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsExcelExp_Click(object sender, EventArgs e)
        {
            // TODO 导出excel
            ExportToExcel();
        }

        /// <summary>
        /// pdf文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsPdfExp_Click(object sender, EventArgs e)
        {
            // TODO 导出pdf
            ExportToPdf();
        }

        /// <summary>
        /// xml文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsXmlExp_Click(object sender, EventArgs e)
        {
            // TODO 导出xml
            ExportToXml();
        }

        private void tsHtmlExp_Click(object sender, EventArgs e)
        {
            ExportToHtml();
        }

        private void tsMarkDownExp_Click(object sender, EventArgs e)
        {
            ExportToMarkDown();
        } 
        #endregion

        private void GV_ColComments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (GV_ColComments.Columns[e.ColumnIndex].Name.Equals("ColComment", StringComparison.OrdinalIgnoreCase))
            {
                object obj = GV_ColComments.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                string content = obj.ToString();

                this.GV_ColComments.CurrentCell = this.GV_ColComments[e.ColumnIndex, e.RowIndex];
                this.GV_ColComments.BeginEdit(true);
            }
        }

        private void BgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            if (FormUtils.ProgArg != null)
            {
                lblMsg.Text = string.Empty;

                Prog.Maximum = FormUtils.ProgArg.MaxNum;
                FormUtils.ProgArg.ExecAct.Invoke();

                if (string.IsNullOrWhiteSpace(lblMsg.Text))
                {
                    lblMsg.Text = "操作已完成！";
                    lblMsg.ForeColor = System.Drawing.Color.Green;
                }

                FormUtils.ProgArg = null;
            }
        }

        private void BgWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 设置 进度位置
            Prog.Value = e.ProgressPercentage;
            if (e.UserState != null )
            {
                lblMsg.Text = "操作失败！";
                lblMsg.ForeColor = System.Drawing.Color.Red;

                Exception ex = e.UserState as Exception;
                if (ex != null)
                {
                    LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                    var diaRes = MessageBox.Show("很抱歉，执行过程出现错误，出错原因：\r\n" + e.UserState.ToString() + "\r\n\r\n是否打开错误日志目录？", "程序执行出错", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                    if (diaRes == DialogResult.Yes)
                    {
                        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                        System.Diagnostics.Process.Start(dir);
                    }
                }
            }
        }

        private void BtnSaveGridData_Click(object sender, EventArgs e)
        {
            //设置要 滚动条 对应的执行方法，以及滚动条最大值
            FormUtils.ProgArg = new ProgressArg(() =>
            {
                bool? blRes = DBUtils.Instance?.Info?.SetTableComment(LabCurrTabName.Text, TxtCurrTabComment.Text.Replace("'", "`"));
                if (blRes.HasValue)
                {
                    if (blRes.Value)
                    {
                        bgWork.ReportProgress(1);
                    }
                    else
                    {
                        bgWork.ReportProgress(1 + GV_ColComments.Rows.Count, new Exception("执行更新 表描述过程中，出现异常！（" + LabCurrTabName.Text + "） "));
                        return;
                    }
                }
                else
                {
                    bgWork.ReportProgress(1 + GV_ColComments.Rows.Count, new Exception("执行更新 表描述过程中，出现未知异常！（" + LabCurrTabName.Text + "） "));
                    return;
                }

                for (int j = 0; j < GV_ColComments.Rows.Count; j++)
                {
                    string columnName = GV_ColComments.Rows[j].Cells["ColName"].Value.ToString();
                    string colComment = (GV_ColComments.Rows[j].Cells["ColComment"].Value ?? string.Empty).ToString();
                    if (!string.IsNullOrEmpty(colComment))
                    {
                        blRes = DBUtils.Instance?.Info?.SetColumnComment(LabCurrTabName.Text, columnName, colComment);
                    }

                    if (string.IsNullOrEmpty(colComment))
                    {
                        bgWork.ReportProgress(1 + (1 + j));
                    }
                    else
                    {
                        if (blRes.HasValue)
                        {
                            if (blRes.Value)
                            {
                                bgWork.ReportProgress(1 + (1 + j));
                            }
                            else
                            {
                                bgWork.ReportProgress(1 + GV_ColComments.Rows.Count, new Exception("执行更新 列描述过程中，出现异常！（" + columnName + "） "));
                                return;
                            }
                        }
                        else
                        {
                            bgWork.ReportProgress(1 + GV_ColComments.Rows.Count, new Exception("执行更新 列描述过程中，出现未知异常！（" + columnName + "）  "));
                            return;
                        }
                    }
                }

            }, 1 + GV_ColComments.Rows.Count);

            bgWork.RunWorkerAsync();
        }

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox();
            aboutForm.ShowDialog();
        }

        public bool IsExistProcess(string fileName,out Process process)
        {
            var procs = System.Diagnostics.Process.GetProcessesByName("hh");
            foreach (var proc in procs)
            {
                if (proc.MainWindowTitle.Equals(fileName))
                {
                    process = proc;
                    return true;
                }
            }
            process = null;
            return false;
        }


        /// <summary>
        /// MJTop.Data数据库对象Instance转TableDto
        /// </summary>
        /// <returns>tables</returns>
        private List<TableDto> DBInstanceTransToDto()
        {
            List<TableDto> tables = new System.Collections.Generic.List<TableDto>();

            // TODO 查询数据库表集合
            NameValueCollection dict_tabs = DBUtils.Instance.Info.TableComments;

            // TODO 根据选择的表名进行导出相关处理
            List<string> checkedTableNames = CkListBox.CheckedItems.Cast<string>().ToList();


            if (checkedTableNames.Count == 0)
            {
                checkedTableNames = DBUtils.Instance?.Info?.TableNames ?? new List<string>();
            }

            int i = 1; // 计数器
            foreach (var tableName in checkedTableNames)
            {
                TableDto tableDto = new TableDto();

                tableDto.TableOrder = i + ""; // 序号
                tableDto.TableName = tableName; // 表名
                tableDto.Comment = (!string.IsNullOrWhiteSpace(dict_tabs[tableName]) ? dict_tabs[tableName] : ""); // 表注释（说明）

                // TODO 查询数据库表字段集合
                var columns = new System.Collections.Generic.List<ColumnDto>();
                var dictTabs = DBUtils.Instance.Info.TableInfoDict;
                MJTop.Data.TableInfo tabInfo = null;
                if (dictTabs.Case == MJTop.Data.KeyCase.Lower)
                {
                    if (!dictTabs.ContainsKey(tableName.ToLower()))
                    {
                        throw new Exception(tableName.ToLower());
                    }
                    tabInfo = dictTabs[tableName.ToLower()]; 
                }
                else
                {
                    if (!dictTabs.ContainsKey(tableName.ToUpper()))
                    {
                        throw new Exception(tableName.ToUpper());
                    }
                    tabInfo = dictTabs[tableName.ToUpper()];
                }                
                // TODO 添加数据字段行,循环数据库表字段集合
                foreach (var col in tabInfo.Colnumns)
                {
                    ColumnDto columnDto = new ColumnDto();

                    columnDto.ColumnOrder = col.Colorder + ""; // 序号
                    columnDto.ColumnName = col.ColumnName; // 列名
                    columnDto.ColumnTypeName = col.TypeName; // 数据类型
                    columnDto.Length = (col.Length.HasValue ? col.Length.Value.ToString() : ""); // 长度
                    columnDto.Scale = (col.Scale.HasValue ? col.Scale.Value.ToString() : ""); // 小数位
                    columnDto.IsPK = (col.IsPK ? "√" : ""); // 主键
                    columnDto.IsIdentity = (col.IsIdentity ? "√" : ""); // 自增
                    columnDto.CanNull = (col.CanNull ? "√" : ""); // 允许空
                    columnDto.DefaultVal = (!string.IsNullOrWhiteSpace(col.DefaultVal) ? col.DefaultVal : ""); // 默认值
                    columnDto.Comment = (!string.IsNullOrWhiteSpace(col.DeText) ? col.DeText : ""); // 列注释（说明）

                    columns.Add(columnDto);
                }
                tableDto.Columns = columns; // 数据库表字段集合赋值

                tables.Add(tableDto);

                i++;
            }
            return tables;
        }

        
    }
}
