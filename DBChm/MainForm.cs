using ComponentFactory.Krypton.Toolkit;
using DocTools;
using DocTools.DBDoc;
using DocTools.Dtos;
using IniParser.Model;
using IniParser;
using MJTop.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;


namespace DBCHM
{
    public partial class MainForm : KryptonForm
    {
        public MainForm()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            KryptonManager kryptonManager = new KryptonManager();
            this.InitializeComponent();
            this.InitializeRibbonTabContainer();
            kryptonManager.GlobalPaletteMode = PaletteModeManager.Office2010Blue;
        }


        #region Ribbon

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
                this._isRibbonTabExpand = false;
                this._isRibbonTabShow = false;
            }
            else
            {
                this.RibbonTabContainer.Height = RIBBON_EXPAND_HEIGHT;
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

        private string selectedCountDesc = "共选择{0}个项目";

        private string selectedItemDesc = "表：{0}，视图：{1}，存储过程：{2}";

        private delegate void ikMethod();

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public List<string> SearchWords { get; set; } = new List<string>();

        public DBDto DbDto { get; set; }

        // INI 配置数据
        public IniData iniData {  get; set; }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Hide();
            if (DBUtils.Instance == null)
            {
                GridFormMgr conMgrForm = new GridFormMgr();
                var diaRes = conMgrForm.ShowDialog(this);
                if (diaRes == DialogResult.OK || FormUtils.IsOK_Close)
                {
                    FormUtils.IsOK_Close = false;
                    InitTree();
                    this.Show();
                }
            }
        }


        #region 初始化树

        private void InitTree()
        {
            if (DBUtils.Instance?.Info == null)
            {
                return;
            }

            lblTip.Text = string.Empty;
            treeDB.Nodes.Clear();

            if (!string.IsNullOrWhiteSpace(DBUtils.Instance?.Info?.DBName))
            {
                var ver = ((System.Reflection.AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)?.FirstOrDefault())?.Version;
                if (ver == null)
                {
                    ver = Assembly.GetExecutingAssembly().GetName()?.Version?.ToString();
                }
                this.Text = DBUtils.Instance?.Info?.DBName + "(" + DBUtils.Instance.DBType.ToString() + ") - " + "DBCHM v" + ver;
            }

            BtnSaveGridData.Enabled = true;

            if (DBUtils.Instance.DBType == MJTop.Data.DBType.SQLite)
            {
                SetMsg(DBUtils.Instance.DBType + "数据库不支持批注功能！", false);
                BtnSaveGridData.Enabled = false;
            }

            if (DBUtils.Instance.Info.TableNames.Count > 0)
            {
                TreeNode tnTable = new TreeNode("表", 0, 0) { Name = "table" };
                // 需要添加的模块节点

                // 最后添加表节点
                List<string> theTables = new List<string>();
                foreach (var tbName in DBUtils.Instance.Info.TableNames)
                {
                    if (SearchWords.Any())
                    {
                        foreach (var word in SearchWords)
                        {
                            if (tbName.ToLower().Contains(word.ToLower()))
                            {
                                var module = getModule(tbName);
                                if (module != null)
                                {
                                    TreeNode moduleNode = createOrGetParent(tnTable, tbName, module);
                                    //   MessageBox.Show(" invoke set module " + tbName + " module " + module + this.DbDto.Tables[0].TableName);
                                    //  setTableModule(tbName,module);
                                    //  MessageBox.Show(" invoke set module " + tbName + " module " + module + this.DbDto.Tables[0].TableModule);
                                    moduleNode.Nodes.Add(tbName, tbName, 1, 1);
                                    break;
                                }
                                else
                                {
                                    theTables.Add(tbName);
                                    // tnTable.Nodes.Add(tbName, tbName, 1, 1);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var module = getModule(tbName);
                        if (!String.IsNullOrEmpty( module ) )
                        {
                            TreeNode moduleNode = createOrGetParent(tnTable, tbName, module);
                            // MessageBox.Show(" invoke set module " + tbName + " module " + module + this.DbDto);
                            // setTableModule(tbName, module);
                            moduleNode.Nodes.Add(tbName, tbName, 1, 1);
                        }
                        else
                        {
                            theTables.Add(tbName);
                            //tnTable.Nodes.Add(tbName, tbName, 1, 1);
                        }
                    }
                }
                // 如果不为空
                if (theTables.Count > 0)
                {
                    foreach( var tbName in theTables)
                    {
                        tnTable.Nodes.Add(tbName, tbName, 1, 1);
                    }
                }

                //try
                {
                    treeDB.Nodes.Add(tnTable);
                }
                //catch (Exception  ex)
                //{

                //}
            }

            if (DBUtils.Instance.Info.Views.Keys.Count > 0)
            {
                TreeNode tnView = new TreeNode("视图", 0, 0) { Name = "view" };
                foreach (var vwName in DBUtils.Instance.Info.Views.AllKeys)
                {
                    if (SearchWords.Any())
                    {
                        foreach (var word in SearchWords)
                        {
                            if (vwName.ToLower().Contains(word.ToLower()))
                            {
                                tnView.Nodes.Add(vwName, vwName, 2, 2);
                                break;
                            }
                        }
                    }
                    else
                    {
                        tnView.Nodes.Add(vwName, vwName, 2, 2);
                    }
                }
                treeDB.Nodes.Add(tnView);
            }

            if (DBUtils.Instance.Info.Procs.Keys.Count > 0)
            {
                TreeNode tnProc = new TreeNode("存储过程", 0, 0) { Name = "proc" };
                foreach (var procName in DBUtils.Instance.Info.Procs.AllKeys)
                {
                    if (SearchWords.Any())
                    {
                        foreach (var word in SearchWords)
                        {
                            if (procName.ToLower().Contains(word.ToLower()))
                            {
                                tnProc.Nodes.Add(procName, procName, 3, 3);
                                break;
                            }
                        }
                    }
                    else
                    {
                        tnProc.Nodes.Add(procName, procName, 3, 3);
                    }
                }
                treeDB.Nodes.Add(tnProc);
            }

            foreach (TreeNode node in treeDB.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    treeDB.SelectedNode = node.Nodes[0];
                    node.Expand();
                    break;
                }
            }

            //treeDB.ExpandAll();
            this.TransToDto();
        }
        
        // 创建或者获取对应的父节点 by tyj
        private TreeNode createOrGetParent(TreeNode tnTable, string tableName, string module)
        {
            foreach (TreeNode node in tnTable.Nodes)
            {
                if ( node.Text.Equals( module ))
                {
                    setTableModule(tableName, module);
                    return node;
                }
            }
            // not found.
            //  TreeNode theModuleNode = new TreeNode(module, 0, 0);
            //   theModuleNode.Name = "module";
            // tnTable.Nodes.Insert(0, theModuleNode); // 插入到头部节点
            tnTable.Nodes.Add(module, module, 0, 0);
            var moduleNode = tnTable.Nodes[module];
            setTableModule(tableName, module);
            moduleNode.Name = "module";
            return moduleNode;
        }

        // 设置表模块名称
        private void setTableModule(string tbName, string module)
        {
         /*   
            this.DbDto.Tables.ForEach(t => { 
                 if ( t.TableName.Equals( tbName ))
                {
                    t.TableModule = module;
                    return;
                }
            });
         */
            try
            {
                var tableDto = this.DbDto.Tables.Where(t => t.TableName == tbName).FirstOrDefault();
                if (tableDto != null)
                {
                    tableDto.TableModule = module;
                }
               //  MessageBox.Show(" set TableName " + tbName + " ,set module " + module);
            }
            catch (Exception ex)
            {
                LogUtils.LogError("Save TableModule", Developer.SysDefault, ex, tbName, module);
                SetMsg(ex.Message, false);
                return;
            }
        }

        private string getModule(string tbName)
        {
            if ( null != this.iniData )
            {
                var type = this.iniData["config"]["type"];
               // MessageBox.Show("tableName : " + tbName + " type:" + type);
                if ( "1".Equals( type.Trim()))  // 标识表名匹配模式
                {
                    //通过所有的段迭代
                    foreach (SectionData section in this.iniData.Sections)
                    {
                        if ( !"config".Equals(section.SectionName ))
                        {
                            //遍历当前节中的所有键以打印值
                            foreach (KeyData key in section.Keys)
                            {

                                if ("1".Equals(key.Value.Trim()))
                                {
                                    if (tbName.Trim().ToLower().Equals(key.KeyName.Trim().ToLower()))
                                    {
                                      //  MessageBox.Show("section.SectionName : " + section.SectionName.Trim() + "key.KeyName : " + key.KeyName.Trim() + " key.Value : " + key.Value.Trim());
                                        return section.SectionName.Trim();
                                    }
                                }
                            }
                           
                        }
                    }
                }

                if ("0".Equals(type.Trim()))  // 标识表前缀方式
                {
                    //通过所有的段迭代
                    foreach (SectionData section in this.iniData.Sections)
                    {
                        if (!"config".Equals(section.SectionName))
                        {
                            //遍历当前节中的所有键以打印值
                            foreach (KeyData key in section.Keys)
                            {
                                if ("1".Equals(key.Value.Trim()))
                                {
                                    var keyTable = key.KeyName.Trim().ToLower();
                                    var tableName = tbName.ToLower();
                                    if (tableName.StartsWith( keyTable ))
                                    {
                                      //  MessageBox.Show("section.SectionName : " + section.SectionName.Trim() + "key.KeyName : " + key.KeyName.Trim() + " key.Value : " + key.Value.Trim());
                                        return section.SectionName;
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 根据选择项转换为DTO数据
        /// </summary>
        public void TransToDto()
        {
            if (DBUtils.Instance == null)
            {
                return;
            }

            this.DbDto = new DBDto()
            {
                DBName = DBUtils.Instance.Info.DBName,
                DBType = DBUtils.Instance.DBType.ToString()
            };

            var tableNode = treeDB.Nodes["table"];

            var viewNode = treeDB.Nodes["view"];

            var procNode = treeDB.Nodes["proc"];

            //只要有任意一个1个勾选，勾选就起作用
            var isHaveChecked = (tableNode != null && tableNode.Nodes.OfType<TreeNode>().Any(t => t.Checked))
                            || (viewNode != null && viewNode.Nodes.OfType<TreeNode>().Any(t => t.Checked))
                            || (procNode != null && procNode.Nodes.OfType<TreeNode>().Any(t => t.Checked));


            var tables = new List<TableDto>();

            //var viewDict = new NameValueCollection();
            //var procDict = new NameValueCollection();

            var viewDict = new Dictionary<string, string>();
            var procDict = new Dictionary<string, string>();

            if (tableNode != null)
            {
                if (isHaveChecked || tableNode.Nodes.OfType<TreeNode>().Any(t => t.Checked))
                {
                    var xh = 1;
                    foreach (TreeNode node in tableNode.Nodes)
                    {
                        if (node.Checked)
                        {
                            if (node.Name.Equals("module")) { 

                                foreach (TreeNode leafNode in node.Nodes)
                                {
                                    if (leafNode.Checked)
                                    {
                                        // 需要判断是否为叶子节点
                                        tables.Add(Trans2Table(leafNode, ref xh));
                                    }
                                }

                            } else
                            {
                                // 需要判断是否为叶子节点
                                tables.Add(Trans2Table(node, ref xh));
                            }
                            
                        }
                    }
                }
                else
                {
                    var xh = 1;
                    foreach (TreeNode node in tableNode.Nodes)
                    {
                        // 需要判断是否为叶子节点
                        if (node.Name.Equals("module"))
                        {

                            foreach (TreeNode leafNode in node.Nodes)
                            {
                                if (leafNode.Checked)
                                {
                                    // 需要判断是否为叶子节点
                                    tables.Add(Trans2Table(leafNode, ref xh));
                                }
                            }

                        }
                        else
                        {
                            // 需要判断是否为叶子节点
                            tables.Add(Trans2Table(node, ref xh));
                        }
                    }
                }
            }

            if (viewNode != null)
            {
                if (isHaveChecked || viewNode.Nodes.OfType<TreeNode>().Any(t => t.Checked))
                {
                    foreach (TreeNode node in viewNode.Nodes)
                    {
                        if (node.Checked)
                        {
                            viewDict.Add(node.Name, DBUtils.Instance.Info.Views[node.Name]);
                        }
                    }
                }
                else
                {
                    viewDict = DBUtils.Instance.Info.Views.ToDictionary();
                }
            }

            if (procNode != null)
            {
                if (isHaveChecked || procNode.Nodes.OfType<TreeNode>().Any(t => t.Checked))
                {
                    foreach (TreeNode node in procNode.Nodes)
                    {
                        if (node.Checked)
                        {
                            procDict.Add(node.Name, DBUtils.Instance.Info.Procs[node.Name]);
                        }
                    }
                }
                else
                {
                    procDict = DBUtils.Instance.Info.Procs.ToDictionary();
                }
            }

            DbDto.Tables = tables;
            DbDto.Views = viewDict;
            DbDto.Procs = procDict;
        }

        private TableDto Trans2Table(TreeNode node, ref int xh)
        {
            TableDto tbDto = new TableDto();
            tbDto.TableOrder = xh.ToString();
            tbDto.TableName = node.Name;
            // 设置表模块信息
            if ( node.Parent != null && !"表".Equals( node.Parent.Text))
            {
                // MessageBox.Show(" set table module : " + node.Parent.Text);
                tbDto.TableModule = node.Parent.Text;
            }
            tbDto.Comment = DBUtils.Instance.Info.TableComments[node.Name];
            tbDto.DBType = DBUtils.Instance.DBType.ToString();

            var lst_col_dto = new List<ColumnDto>();
            // 查找不到表字典数据
            var columns = DBUtils.Instance.Info.TableColumnInfoDict[node.Name];
            foreach (var col in columns)
            {
                ColumnDto colDto = new ColumnDto();
                colDto.ColumnOrder = col.Colorder.ToString();
                colDto.ColumnName = col.ColumnName;
                // 数据类型
                colDto.ColumnTypeName = col.TypeName;
                // 长度
                colDto.Length = (col.Length.HasValue ? col.Length.Value.ToString() : "");
                // 小数位
                colDto.Scale = (col.Scale.HasValue ? col.Scale.Value.ToString() : "");
                // 主键
                colDto.IsPK = (col.IsPK ? "√" : "");
                // 自增
                colDto.IsIdentity = (col.IsIdentity ? "√" : "");
                // 允许空
                colDto.CanNull = (col.CanNull ? "√" : "");
                // 默认值
                colDto.DefaultVal = (!string.IsNullOrWhiteSpace(col.DefaultVal) ? col.DefaultVal : "");
                // 列注释（说明）
                colDto.Comment = (!string.IsNullOrWhiteSpace(col.DeText) ? col.DeText : "");

                lst_col_dto.Add(colDto);
            }

            tbDto.Columns = lst_col_dto;
            xh++;
            return tbDto;
        }

        #endregion


        /// <summary>
        /// 模糊搜索数据表名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtSearchWords_TextChanged(object sender, EventArgs e)
        {
            string searchWords = TxtSearchWords.Text.Trim();
            this.SearchWords = new List<string>();
            if (!string.IsNullOrWhiteSpace(searchWords))
            {
                var words = searchWords.Split(new string[] { ",", "，" }, StringSplitOptions.RemoveEmptyEntries);

                this.SearchWords = new List<string>(words.Distinct());
            }

            ikMethod method = new ikMethod(InitTree);
            this.Invoke(method);
        }


        /// <summary>
        /// 数据连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbConnect_Click(object sender, EventArgs e)
        {
            GridFormMgr conMgrForm = new GridFormMgr();
            var diaRes = conMgrForm.ShowDialog(this);
            if (diaRes == DialogResult.OK || FormUtils.IsOK_Close) //当前窗体 是正常关闭的情况下
            {
                FormUtils.IsOK_Close = false;
            }
            InitTree();
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


        private void tsbBuild_Click(object sender, EventArgs e)
        {
            var btn = sender as System.Windows.Forms.ToolStripButton;
            if (btn.Tag == null)
            {
                MessageBox.Show("菜单未指定Tag值！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var docType = btn.Tag.ToString().GetEnum<DocType>();
            var doc = DocFactory.CreateInstance(docType, this.DbDto);

            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = doc.Filter;
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = doc.Ext;
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = doc.Dto.DBName + "表结构信息" + doc.Ext;
            var diaResult = saveDia.ShowDialog();

            if (diaResult == DialogResult.OK)
            {
                FormUtils.ShowProcessing("正在导出文档，请稍等......", this, arg =>
                {
                    try
                    {
                        doc.Build(saveDia.FileName);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("tsbBuild_Click", Developer.SysDefault, ex, saveDia.FileName, DBUtils.Instance.Info);
                    }

                }, null);
            }
        }

        private void SetMsg(string msg, bool isSuccess = true)
        {
            lblTip.Text = msg;
            lblTip.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        private void BtnSaveGridData_Click(object sender, EventArgs e)
        {
            TableDto tableDto = null;
            try
            {
                DBUtils.Instance?.Info?.SetTableComment(LabCurrTabName.Text, TxtCurrTabComment.Text);
                tableDto = this.DbDto.Tables.Where(t => t.TableName == LabCurrTabName.Text).FirstOrDefault();
                tableDto.Comment = TxtCurrTabComment.Text;
            }
            catch (Exception ex)
            {
                LogUtils.LogError("Save TableComment", Developer.SysDefault, ex, LabCurrTabName.Text, TxtCurrTabComment.Text);
                SetMsg(ex.Message, false);
                return;
            }

            for (int j = 0; j < GV_ColComments.Rows.Count; j++)
            {
                string columnName = GV_ColComments.Rows[j].Cells["ColName"].Value.ToString();

                string colComment = (GV_ColComments.Rows[j].Cells["ColComment"].Value ?? string.Empty).ToString();

                try
                {
                    DBUtils.Instance?.Info?.SetColumnComment(LabCurrTabName.Text, columnName, colComment);
                    tableDto.Columns.Find(t => t.ColumnName == columnName).Comment = colComment;
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("Save ColComment", Developer.SysDefault, ex, columnName, ColComment);
                    SetMsg(ex.Message, false);
                    return;
                }
            }

            SetMsg("保存成功！");
        }




        #region 工具

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

        #endregion


        #region 关于

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox();
            aboutForm.ShowDialog();
        }

        #endregion

        private void MainForm_Resize(object sender, EventArgs e)
        {

        }

        private void GV_ColComments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (GV_ColComments.Columns[e.ColumnIndex].Name.Equals("ColComment", StringComparison.OrdinalIgnoreCase))
            {
                //object obj = GV_ColComments.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                //string content = obj.ToString();

                // 点击单元格 默认选中内容
                this.GV_ColComments.CurrentCell = this.GV_ColComments[e.ColumnIndex, e.RowIndex];
                this.GV_ColComments.BeginEdit(true);
            }
        }

        /**
         *   统计表的个数  视图的个数 存储过程个数
         */
        void TongJi()
        {
            // 不是无限级，不需要用递归
            int totalCkCount = 0, tableCount = 0, viewCount = 0, procCount = 0;
            foreach (TreeNode node in treeDB.Nodes)
            {
                int ckCount = 0;
                foreach (TreeNode item in node.Nodes)
                {
                    if (item.Checked)
                    {
                        totalCkCount++;
                        ckCount++;
                    }
                }

                if (node.Name == "table")
                {
                    tableCount = ckCount;
                }
                else if (node.Name == "view")
                {
                    viewCount = ckCount;
                }
                else if (node.Name == "proc")
                {
                    procCount = ckCount;
                }
            }

            lblSelectRes.Text = string.Format(selectedCountDesc, totalCkCount);
            lblTongJi.Text = string.Format(selectedItemDesc, tableCount, viewCount, procCount);

            TransToDto();
        }

        #region 勾选选中

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

        // 全选操作
        private void CkAll_CheckedChanged(object sender, EventArgs e)
        {
            HandleNode(treeDB.Nodes, CkAll.Checked);
            TongJi();
        }

        // 处理节点
        void HandleNode(TreeNodeCollection nodeColl, bool cked)
        {
            if (nodeColl != null)
            {
                foreach (TreeNode node in nodeColl)
                {
                    if (acceptFilter(node.Text))
                    {
                        node.Checked = cked;
                        if (node.Nodes != null && node.Nodes.Count > 0)
                        {
                            HandleNode(node.Nodes, cked);
                        }
                    }
                    else
                    {
                        node.Checked = !cked;
                        node.ForeColor = Color.Pink;
                    }
                }
            }
        }

        private void CkReverse_CheckedChanged(object sender, EventArgs e)
        {
            CkAll.Checked = false;
            ReverseHandleNode(treeDB.Nodes);
            TongJi();
        }

        void ReverseHandleNode(TreeNodeCollection nodeColl)
        {
            if (nodeColl != null)
            {
                foreach (TreeNode node in nodeColl)
                {
                    if (acceptFilter(node.Text))
                    {
                        if (node.Nodes != null && node.Nodes.Count > 0)
                        {
                            ReverseHandleNode(node.Nodes);

                            //处理受影响的父节点
                            int ckCount = 0;
                            foreach (TreeNode childNode in node.Nodes)
                            {
                                if (childNode.Checked)
                                {
                                    ckCount++;
                                }
                            }
                            // 子节点个数 与 选中的子节点个数 比较
                            node.Checked = node.Nodes.Count == ckCount;

                        }
                        else
                        {
                            node.Checked = !node.Checked;
                        }
                    }
                    else
                    {
                        node.Checked = false;
                        node.ForeColor = Color.Pink;
                    }
                }
            }
        }

        private void treeDB_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.ByKeyboard)
            {
                var node = e.Node;

                HandleNode(node.Nodes, node.Checked);

                if (node.Parent != null)
                {
                    //没有子节点的节点
                    int ckCount = 0;
                    foreach (TreeNode childNode in node.Parent.Nodes)
                    {
                        if (childNode.Checked)
                        {
                            ckCount++;
                        }
                    }
                    // 子节点个数 与 选中的子节点个数 比较
                    node.Parent.Checked = node.Parent.Nodes.Count == ckCount;
                }

                TongJi();
            }
        }

        #endregion


        private void CodeAreaShow()
        {
            codePnl.Visible = true;
            codePnl.Location = new Point(6, 20);
            pizhuPnl.Visible = false;
        }

        private void PiZhuAreaShow()
        {
            pizhuPnl.Visible = true;
            pizhuPnl.Location = new Point(6, 20);
            codePnl.Visible = false;
        }

        private void treeDB_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (DBUtils.Instance.DBType != MJTop.Data.DBType.SQLite)
            {
                lblTip.Text = string.Empty;
            }

            // 都是子项
            if (e.Node.Parent != null)
            {
                if ( ( e.Node.Parent.Name == "table" && !e.Node.Name.Equals( "module") ) ||
                    (e.Node.Parent.Name == "module" && e.Node.Nodes == null ) )
                {
                    PiZhuAreaShow();

                    GV_ColComments.Rows.Clear();

                    var tabInfo = DBUtils.Instance.Info.TableInfoDict[e.Node.Text];
                    LabCurrTabName.Text = tabInfo?.TableName;
                    TxtCurrTabComment.Text = tabInfo?.TabComment;
                    var columnInfos = tabInfo?.Colnumns;
                    if (columnInfos != null)
                    {
                        for (int j = 0; j < columnInfos.Count; j++)
                        {
                            var colInfo = columnInfos[j];

                            GV_ColComments.Rows.Add(j + 1, colInfo.ColumnName, colInfo.TypeName, colInfo.Length, colInfo.DeText);
                        }
                    }

                }
                else
                {
                    CodeAreaShow();

                    var code = string.Empty;

                    if (e.Node.Parent.Name == "view")
                    {
                        code = JS.RunFmtSql(DBUtils.Instance.Info.Views[e.Node.Text], DBUtils.Instance.DBType.ToString());
                    }
                    else if (e.Node.Parent.Name == "proc")
                    {
                        code = JS.RunFmtSql(DBUtils.Instance.Info.Procs[e.Node.Text], DBUtils.Instance.DBType.ToString());
                    }

                    txtCode.Text = code;
                }
            }

        }

        private void GV_ColComments_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V && GV_ColComments.SelectedCells.Count > 0)
            {
                var cell = GV_ColComments.SelectedCells[0];
                var colName = GV_ColComments.Columns[cell.ColumnIndex].Name;
                var text = Clipboard.GetText();
                if (colName == "ColComment" && !string.IsNullOrWhiteSpace(text))
                {
                    var lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        for (int j = 0; j < lines.Length; j++)
                        {
                            GV_ColComments.Rows[cell.RowIndex + j].Cells[cell.ColumnIndex].Value = lines[j];
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  加载数据库分类配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadConfig_Click(object sender, EventArgs e)
        {
            // this.DbDto
            
            OpenFileDialog openDia = new OpenFileDialog();
            openDia.Title = "加载配置";
            //这是系统提供的桌面路径，还可以是其他的路径：比如文档、音乐等文件夹
            openDia.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //过滤不同类型的文件
            openDia.Filter = "(*.ini)|*.ini";
            openDia.InitialDirectory = Environment.CurrentDirectory;
            openDia.FileName = DBUtils.Instance.DBType.ToString() + "_" + this.DbDto.DBName + "_config.ini";
            
            var diaResult = openDia.ShowDialog();
            if (diaResult == DialogResult.OK)
            {

                //Get the path of specified file
             
               var filePath = openDia.FileName;
               var parser = new FileIniDataParser();
               try
               {
                   // 读取Ini数据信息
                   iniData = parser.ReadFile(filePath, System.Text.Encoding.UTF8);
                   InitTree();
               }
               catch (Exception ex)
               {
                   MessageBox.Show("" + ex);
               }


                /*
             FormUtils.ShowProcessing("正在加载自定义数据库INI配置，请稍等......", this, arg =>
            {
                try
                {
                    //Get the path of specified file
                    var filePath = openDia.FileName;
                    var parser = new FileIniDataParser();
                    // 读取Ini数据信息
                    iniData = parser.ReadFile(filePath, System.Text.Encoding.UTF8);
                    InitTree();
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("loadConfig_Click", Developer.SysDefault, ex, openDia.FileName, DBUtils.Instance.Info);
                }

            }, null);  */
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            filterTextBox.Text = "";
        }
    }
}
