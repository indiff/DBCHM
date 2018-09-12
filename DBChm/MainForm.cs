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
        private void tsbBuild_Click(object sender, EventArgs e)
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
            if (saveDia.ShowDialog(this) == DialogResult.OK)
            {
                chm_path = saveDia.FileName;
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


        private void GV_ColComments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0)
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
    }
}
