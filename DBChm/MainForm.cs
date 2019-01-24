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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// Class MainForm.
    /// </summary>
    public partial class MainForm : KryptonForm
    {
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
                LstBox.DataSource = DBUtils.Instance?.Info?.TableNames;
                FormUtils.IsOK_Close = false;
            }
            else
            {
                return;
            }
            if (LstBox.Items.Count > 0)//默认选择第一张表
            {
                LstBox.SelectedIndex = 0;
                LabCurrTabName.Text = LstBox.SelectedItems[0].ToString();
                TxtCurrTabComment.Text = DBUtils.Instance?.Info?.TableComments[LabCurrTabName.Text];
            }
            else//无数据表时，清空 Gird列表
            {
                GV_ColComments.Rows.Clear();
            }
            if (!string.IsNullOrWhiteSpace(DBUtils.Instance?.Info?.DBName))
            {
                this.Text = DBUtils.Instance?.Info?.DBName + " - " + "DBCHM v" + Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".0.0", "");
            }
        }

        /// <summary>
        /// 模糊搜索数据表名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtTabName_TextChanged(object sender, EventArgs e)
        {
            string strName = TxtTabName.Text.Trim().ToLower();
            var lstTableName = DBUtils.Instance?.Info?.TableNames;

            LstBox.DataSource = null;
            LstBox.Items.Clear();

            if (!string.IsNullOrWhiteSpace(strName))
            {
                lstTableName.ForEach(t =>
                {
                    if (t.ToLower().Contains(strName))//模糊匹配
                    {
                        LstBox.Items.Add(t);
                    }

                });
            }
            else//默认所有数据表
            {
                LstBox.DataSource = DBUtils.Instance?.Info?.TableNames;
            }

            if (LstBox.Items.Count > 0)//默认选择第一张表
            {
                LstBox.SelectedIndex = 0;
                LabCurrTabName.Text = LstBox.SelectedItems[0].ToString();
                TxtCurrTabComment.Text = DBUtils.Instance?.Info?.TableComments[LabCurrTabName.Text];
            }
        }

        private void LstBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LstBox.SelectedItems.Count > 0)
            {
                //数据重置 
                Prog.Value = 0;
                lblMsg.Text = string.Empty;
                GV_ColComments.Rows.Clear();

                LabCurrTabName.Text = LstBox.SelectedItems[0].ToString();
                TxtCurrTabComment.Text = DBUtils.Instance?.Info?.TableComments[LabCurrTabName.Text];

                var nvc = DBUtils.Instance?.Info?.TableColumnComments[LabCurrTabName.Text];

                if (nvc != null)
                {
                    foreach (var colName in nvc.AllKeys)
                    {
                        GV_ColComments.Rows.Add(colName, nvc[colName]);
                    }
                }
                else
                {
                    DBUtils.Instance?.Info?.Refresh();
                    TxtTabName_TextChanged(sender, e);
                }
            }
        }

        private void tsbConnect_Click(object sender, EventArgs e)
        {
            InitMain();
        }
        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            FormUtils.ShowProcessing("正在查询表结构信息，请稍等......", this, arg =>
            {
                DBUtils.Instance?.Info?.Refresh();

                TxtTabName_TextChanged(sender, e);

            }, null);

        }
        private void tsbSaveUpload_Click(object sender, EventArgs e)
        {
            ImportPDMForm pdmForm = new ImportPDMForm();
            DialogResult dirRes = pdmForm.ShowDialog(this);
            if (dirRes == DialogResult.OK || FormUtils.IsOK_Close)
            {
                FormUtils.IsOK_Close = false;
                TxtTabName_TextChanged(sender, e);
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

        private void ExportToChm()
        {
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
            bool isOpen = false;
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
                        isOpen = true;
                    }
                }

                try
                {
                    //创建临时文件夹,存在则删除，防止已经存在的文件 会导致生成出来的chm 有问题
                    dirPath = Path.Combine(ConfigUtils.AppPath, DBUtils.Instance.DBType + "_" + DBUtils.Instance.Info.DBName);
                    if (Directory.Exists(dirPath))
                    {
                        Directory.Delete(dirPath, true);
                    }
                    Directory.CreateDirectory(dirPath);
                    ConfigUtils.AddSecurityControll2Folder(dirPath);
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("文件目录创建出错", Developer.SysDefault, ex, dirPath);
                    return;
                }

                //设置要 滚动条 对应的执行方法，以及滚动条最大值
                FormUtils.ProgArg = new ProgressArg(() =>
                {
                    try
                    {
                        //生成数据库目录文件
                        indexHtmlpath = Path.Combine(dirPath, defaultHtml);
                        ChmHtmlHelper.CreateDirHtml("数据库表目录", DBUtils.Instance.Info.TableComments, indexHtmlpath);

                        string structPath = Path.Combine(dirPath, "表结构");
                        if (!Directory.Exists(structPath))
                        {
                            Directory.CreateDirectory(structPath);
                        }

                        bgWork.ReportProgress(2);

                        //生成每张表列结构的html
                        ChmHtmlHelper.CreateHtml(DBUtils.Instance.Info.TableInfoDict, structPath);

                        bgWork.ReportProgress(3);

                        ChmHelp c3 = new ChmHelp();
                        c3.DefaultPage = defaultHtml;
                        c3.Title = Path.GetFileName(chm_path);
                        c3.ChmFileName = chm_path;
                        c3.SourcePath = dirPath;
                        c3.Compile();

                        if (isOpen)
                        {
                            System.Diagnostics.Process.Start(chm_path);
                        }

                        bgWork.ReportProgress(4);
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
            FormUtils.ShowProcessing("正在导出数据字典Word文档，请稍等......", this, arg =>
            {
                try
                {
                    string docTitle = "数据库名：" + DBUtils.Instance.Info.DBName;
                    object template = Missing.Value;
                    object oEndOfDoc = @"\endofdoc"; // \endofdoc是预定义的bookmark

                    // TODO 依赖冲突，所以用了全类名
                    Microsoft.Office.Interop.Word._Application application = new Microsoft.Office.Interop.Word.Application();
                    application.Visible = false;
                    Microsoft.Office.Interop.Word._Document document = application.Documents.Add(ref template, ref template, ref template, ref template);
                    application.ActiveWindow.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdOutlineView;
                    application.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekPrimaryHeader;
                    application.ActiveWindow.ActivePane.Selection.InsertAfter("DBCHM https://gitee.com/lztkdr/DBCHM");
                    application.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                    application.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;

                    Microsoft.Office.Interop.Word.Paragraph paragraph = document.Content.Paragraphs.Add(ref template);
                    paragraph.Range.Text = docTitle;
                    paragraph.Range.Font.Bold = 1;
                    paragraph.Range.Font.Name = "宋体";
                    paragraph.Range.Font.Size = 12f;
                    paragraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    paragraph.Format.SpaceAfter = 5f;
                    paragraph.OutlineLevel = Microsoft.Office.Interop.Word.WdOutlineLevel.wdOutlineLevel1;
                    paragraph.Range.InsertParagraphAfter();

                    // TODO 循环数据库表名
                    System.Collections.Specialized.NameValueCollection dict_tabs = DBUtils.Instance.Info.TableComments;
                    foreach (var tableName in dict_tabs.AllKeys)
                    {
                        string docTableName = "表名：" + tableName + " " + (!string.IsNullOrWhiteSpace(dict_tabs[tableName]) ? dict_tabs[tableName] : "");
                        // TODO 一级标题
                        object oRng = document.Bookmarks[oEndOfDoc].Range;
                        Microsoft.Office.Interop.Word.Paragraph paragraph2 = document.Content.Paragraphs.Add(ref oRng);
                        paragraph2.Range.Text = docTableName;
                        paragraph2.Range.Font.Bold = 1;
                        paragraph2.Range.Font.Name = "宋体";
                        paragraph2.Range.Font.Size = 10f;
                        paragraph2.OutlineLevel = Microsoft.Office.Interop.Word.WdOutlineLevel.wdOutlineLevel2;
                        paragraph2.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        paragraph2.Format.SpaceBefore = 15f;
                        paragraph2.Format.SpaceAfter = 5f;
                        paragraph2.Range.InsertParagraphAfter();

                        // TODO 循环数据库表字段
                        System.Collections.Generic.Dictionary<string, MJTop.Data.TableInfo> dictTabs = DBUtils.Instance.Info.TableInfoDict;
                        MJTop.Data.TableInfo tabInfo = dictTabs[tableName.ToLower()];
                        // TODO 创建表格
                        Microsoft.Office.Interop.Word.Range range = document.Bookmarks[oEndOfDoc].Range;
                        Microsoft.Office.Interop.Word.Table table2 = document.Tables.Add(range, tabInfo.Colnumns.Count + 1, 10, ref template, ref template);
                        table2.Range.Font.Name = "宋体";
                        table2.Range.Font.Bold = 0;
                        table2.Range.Font.Size = 9f;
                        table2.Borders.Enable = 1;
                        table2.Rows.Height = 10f;
                        table2.AllowAutoFit = true;
                        table2.Cell(1, 1).Range.Text = "序号";
                        table2.Cell(1, 2).Range.Text = "列名";
                        table2.Cell(1, 3).Range.Text = "数据类型";
                        table2.Cell(1, 4).Range.Text = "长度";
                        table2.Cell(1, 5).Range.Text = "小数位";
                        table2.Cell(1, 6).Range.Text = "主键";
                        table2.Cell(1, 7).Range.Text = "自增";
                        table2.Cell(1, 8).Range.Text = "允许空";
                        table2.Cell(1, 9).Range.Text = "默认值";
                        table2.Cell(1, 10).Range.Text = "列说明";
                        // TODO 分别设置word文档中表格的列宽
                        //table2.Columns[1].Width = 33f;

                        int j = 0;
                        foreach (var col in tabInfo.Colnumns)
                        {
                            table2.Cell(j + 2, 1).Range.Text = col.Colorder + "";
                            table2.Cell(j + 2, 2).Range.Text = col.ColumnName;
                            table2.Cell(j + 2, 3).Range.Text = col.TypeName;
                            table2.Cell(j + 2, 4).Range.Text = (col.Length.HasValue ? col.Length.Value.ToString() : "");
                            table2.Cell(j + 2, 5).Range.Text = (col.Scale.HasValue ? col.Scale.Value.ToString() : "");
                            table2.Cell(j + 2, 6).Range.Text = (col.IsPK ? "√" : "");
                            table2.Cell(j + 2, 7).Range.Text = (col.IsIdentity ? "√" : "");
                            table2.Cell(j + 2, 8).Range.Text = (col.CanNull ? "√" : "");
                            table2.Cell(j + 2, 9).Range.Text = (!string.IsNullOrWhiteSpace(col.DefaultVal) ? col.DefaultVal : "");
                            table2.Cell(j + 2, 10).Range.Text = (!string.IsNullOrWhiteSpace(col.DeText) ? col.DeText : "");
                            j++;
                        }
                    }

                    application.Visible = true;
                    document.Activate();

                    MessageBox.Show("生成数据库字典Word文档成功！", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                }

            }, null);
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
                FormUtils.ShowProcessing("正在导出数据字典Excel文档，请稍等......", this, arg =>
                {
                    try
                    {
                        fileName = saveDia.FileName;
                        FileInfo xlsFileInfo = new FileInfo(fileName);
                        if (xlsFileInfo.Exists)
                        {
                            // TODO 存在Excel文档即删除再创建一个
                            xlsFileInfo.Delete();
                            xlsFileInfo = new FileInfo(fileName);
                        }
                        // TODO 创建并添加Excel文档信息
                        using (OfficeOpenXml.ExcelPackage epck = new OfficeOpenXml.ExcelPackage(xlsFileInfo))
                        {
                            // TODO 循环数据库表名
                            System.Collections.Specialized.NameValueCollection dict_tabs = DBUtils.Instance.Info.TableComments;
                            foreach (var tableName in dict_tabs.AllKeys)
                            {
                                string docTableName = tableName + " " + (!string.IsNullOrWhiteSpace(dict_tabs[tableName]) ? dict_tabs[tableName] : "");
                                // TODO 创建sheet
                                OfficeOpenXml.ExcelWorksheet tbWorksheet = epck.Workbook.Worksheets.Add(tableName);
                                //OfficeOpenXml.ExcelWorksheet tbWorksheet = null;
                                //try
                                //{
                                //    tbWorksheet = epck.Workbook.Worksheets.Add(tableName);
                                //}
                                //catch (Exception)
                                //{
                                //    // TODO ignore
                                //    epck.Workbook.Worksheets.Delete(tableName);
                                //    tbWorksheet = epck.Workbook.Worksheets.Add(tableName);
                                //}
                                // TODO 查询数据库表
                                System.Collections.Generic.Dictionary<string, MJTop.Data.TableInfo> dictTabs = DBUtils.Instance.Info.TableInfoDict;
                                MJTop.Data.TableInfo tabInfo = dictTabs[tableName.ToLower()];
                                // TODO 添加列标题
                                tbWorksheet.Cells[1, 1].Value = "序号";
                                tbWorksheet.Cells[1, 2].Value = "列名";
                                tbWorksheet.Cells[1, 3].Value = "数据类型";
                                tbWorksheet.Cells[1, 4].Value = "长度";
                                tbWorksheet.Cells[1, 5].Value = "小数位";
                                tbWorksheet.Cells[1, 6].Value = "主键";
                                tbWorksheet.Cells[1, 7].Value = "自增";
                                tbWorksheet.Cells[1, 8].Value = "允许空";
                                tbWorksheet.Cells[1, 9].Value = "默认值";
                                tbWorksheet.Cells[1, 10].Value = "列说明";
                                // TODO 添加数据行,循环数据库表字段
                                int rowNum = 2;
                                foreach (var col in tabInfo.Colnumns)
                                {
                                    string Colorder = col.Colorder + ""; // 序号
                                    string ColumnName = col.ColumnName; // 列名
                                    string TypeName = col.TypeName; // 数据类型
                                    string Length = (col.Length.HasValue ? col.Length.Value.ToString() : ""); // 长度
                                    string Scale = (col.Scale.HasValue ? col.Scale.Value.ToString() : ""); // 小数位
                                    string IsPK = (col.IsPK ? "√" : ""); // 主键
                                    string IsIdentity = (col.IsIdentity ? "√" : ""); // 自增
                                    string CanNull = (col.CanNull ? "√" : ""); // 允许空
                                    string DefaultVal = (!string.IsNullOrWhiteSpace(col.DefaultVal) ? col.DefaultVal : ""); // 默认值
                                    string DeText = (!string.IsNullOrWhiteSpace(col.DeText) ? col.DeText : ""); // 列说明
                                    tbWorksheet.Cells[rowNum, 1].Value = Colorder;
                                    tbWorksheet.Cells[rowNum, 2].Value = ColumnName;
                                    tbWorksheet.Cells[rowNum, 3].Value = TypeName;
                                    tbWorksheet.Cells[rowNum, 4].Value = Length;
                                    tbWorksheet.Cells[rowNum, 5].Value = Scale;
                                    tbWorksheet.Cells[rowNum, 6].Value = IsPK;
                                    tbWorksheet.Cells[rowNum, 7].Value = IsIdentity;
                                    tbWorksheet.Cells[rowNum, 8].Value = CanNull;
                                    tbWorksheet.Cells[rowNum, 9].Value = DefaultVal;
                                    tbWorksheet.Cells[rowNum, 10].Value = DeText;
                                    // 行号+1
                                    rowNum++;
                                }
                                // TODO 设置表格样式
                                tbWorksheet.Cells.Style.WrapText = true; // 自动换行
                                tbWorksheet.Cells.Style.ShrinkToFit = true; // 单元格自动适应大小
                                tbWorksheet.Cells[1, 1, 1, 10].Style.Font.Bold = true; // 列标题字体为粗体
                                tbWorksheet.Cells[1, 1, rowNum - 1, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // 水平居中
                                tbWorksheet.Cells[1, 1, rowNum - 1, 10].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // 垂直居中
                                tbWorksheet.Cells[1, 1, rowNum - 1, 10].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin; // 上下左右边框线
                                tbWorksheet.Cells[1, 1, rowNum - 1, 10].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                tbWorksheet.Cells[1, 1, rowNum - 1, 10].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                tbWorksheet.Cells[1, 1, rowNum - 1, 10].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            }
                            epck.Save(); // 保存excel
                            epck.Dispose();
                            MessageBox.Show("生成数据库字典Excel文档成功！", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                FormUtils.ShowProcessing("正在导出数据字典PDF文档，请稍等......", this, arg =>
                {
                    try
                    {
                        fileName = saveDia.FileName;
                        // TODO 创建并添加文档信息
                        iTextSharp.text.Document pdfDocument = new iTextSharp.text.Document();
                        pdfDocument.AddTitle(fileName);
                        pdfDocument.AddSubject("Database Dictionary Document of PDF");
                        pdfDocument.AddKeywords("DBCHM, PDF");
                        pdfDocument.AddCreator("trycache,lztkdr");
                        pdfDocument.AddAuthor("trycache,lztkdr");
                        pdfDocument.AddHeader("DBCHM", "https://gitee.com/lztkdr/DBCHM");

                        iTextSharp.text.pdf.PdfWriter pdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDocument, new FileStream(fileName, FileMode.Create));
                        pdfDocument.Open(); // 打开文档

                        // TODO 循环数据库表名
                        System.Collections.Specialized.NameValueCollection dict_tabs = DBUtils.Instance.Info.TableComments;
                        int chapterNum = 1; // PDF书签章节序号
                        // TODO 字体设置，处理iTextSharp中文不识别显示问题
                        iTextSharp.text.pdf.BaseFont chinese = iTextSharp.text.pdf.BaseFont.CreateFont(Application.StartupPath + "\\Fonts\\msyh.ttf", iTextSharp.text.pdf.BaseFont.IDENTITY_H, true);
                        iTextSharp.text.Font pdfFont = new iTextSharp.text.Font(chinese, 12, iTextSharp.text.Font.NORMAL);

                        // TODO 标题
                        iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("数据库字典文档\n\n", new iTextSharp.text.Font(chinese, 30, iTextSharp.text.Font.BOLD));
                        title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        pdfDocument.Add(title);
                        iTextSharp.text.Paragraph subTitle = new iTextSharp.text.Paragraph(" —— " + DBUtils.Instance.Info.DBName, new iTextSharp.text.Font(chinese, 20, iTextSharp.text.Font.NORMAL));
                        subTitle.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        pdfDocument.Add(subTitle);

                        foreach (var tableName in dict_tabs.AllKeys)
                        {
                            string docTableName = tableName + " " + (!string.IsNullOrWhiteSpace(dict_tabs[tableName]) ? dict_tabs[tableName] : "");
                            // TODO 创建添加书签章节
                            iTextSharp.text.Chapter chapter = new iTextSharp.text.Chapter(new iTextSharp.text.Paragraph(docTableName, pdfFont), chapterNum);
                            pdfDocument.Add(chapter);
                            pdfDocument.Add(new iTextSharp.text.Paragraph("\n", pdfFont)); // 换行

                            // TODO 查询数据库表
                            System.Collections.Generic.Dictionary<string, MJTop.Data.TableInfo> dictTabs = DBUtils.Instance.Info.TableInfoDict;
                            MJTop.Data.TableInfo tabInfo = dictTabs[tableName.ToLower()];

                            // TODO 创建表格
                            iTextSharp.text.pdf.PdfPTable pdfTable = new iTextSharp.text.pdf.PdfPTable(10);
                            // TODO 添加列标题
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("序号", pdfFont)));
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("列名", pdfFont)));
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("数据类型", pdfFont)));
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("长度", pdfFont)));
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("小数位", pdfFont)));
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("主键", pdfFont)));
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("自增", pdfFont)));
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("允许空", pdfFont)));
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("默认值", pdfFont)));
                            pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("列说明", pdfFont)));
                            // TODO 添加数据行,循环数据库表字段
                            foreach (var col in tabInfo.Colnumns)
                            {
                                string Colorder = col.Colorder + ""; // 序号
                                string ColumnName = col.ColumnName; // 列名
                                string TypeName = col.TypeName; // 数据类型
                                string Length = (col.Length.HasValue ? col.Length.Value.ToString() : ""); // 长度
                                string Scale = (col.Scale.HasValue ? col.Scale.Value.ToString() : ""); // 小数位
                                string IsPK = (col.IsPK ? "√" : ""); // 主键
                                string IsIdentity = (col.IsIdentity ? "√" : ""); // 自增
                                string CanNull = (col.CanNull ? "√" : ""); // 允许空
                                string DefaultVal = (!string.IsNullOrWhiteSpace(col.DefaultVal) ? col.DefaultVal : ""); // 默认值
                                string DeText = (!string.IsNullOrWhiteSpace(col.DeText) ? col.DeText : ""); // 列说明
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Colorder, pdfFont)));
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(ColumnName, pdfFont)));
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(TypeName, pdfFont)));
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Length, pdfFont)));
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Scale, pdfFont)));
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(IsPK, pdfFont)));
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(IsIdentity, pdfFont)));
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(CanNull, pdfFont)));
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(DefaultVal, pdfFont)));
                                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(DeText, pdfFont)));
                            }
                            // TODO 添加表格
                            pdfDocument.Add(pdfTable);

                            // TODO PDF换页
                            pdfDocument.NewPage();

                            // TODO PDF书签章节序号自增
                            chapterNum++;
                        }

                        // TODO 关闭释放PDF文档资源
                        pdfDocument.Close();

                        MessageBox.Show("生成数据库字典Pdf文档成功！", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                    }

                }, null);
            }
        }

        private void tsbBuild_Click(object sender, EventArgs e)
        {
            // TODO 导出chm
            ExportToChm();
        }

        private void tsWordExp_Click(object sender, EventArgs e)
        {
            // TODO 导出word
            ExportToWord();
        }

        private void tsExcelExp_Click(object sender, EventArgs e)
        {
            // TODO 导出excel
            ExportToExcel();
        }

        private void tsPdfExp_Click(object sender, EventArgs e)
        {
            // TODO 导出pdf
            ExportToPdf();
        }

        private void GV_ColComments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0 && e.RowIndex > -1)
            {
                object obj = GV_ColComments.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (obj != null)
                {
                    string content = obj.ToString();
                    this.GV_ColComments.CurrentCell = this.GV_ColComments[e.ColumnIndex, e.RowIndex];
                    this.GV_ColComments.BeginEdit(true);
                }
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
                    string columnName = GV_ColComments[0, j].Value.ToString();
                    string colComment = (GV_ColComments[1, j].Value ?? string.Empty).ToString();
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
    }
}
