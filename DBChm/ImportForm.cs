using ComponentFactory.Krypton.Toolkit;
using DBCHM.Common;
using DocTools;
using DocTools.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace DBCHM
{
    public partial class ImportForm : KryptonForm
    {
        public ImportForm()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            //为KeyDown能应用到所有控件上 注册 KeyDown 事件 
            foreach (Control control in this.Controls)
            {
                control.KeyDown += control_KeyDown;
            }
        }
        public void control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            txtExplain.Text = @"本工具目前支持 pdm/xml 文件来 进行更新批注(注释)：
 pdm 由powerdesigner设计数据库时产生。
 xml 由visual studio设置 实体类库的项目属性，勾选 XML文档文件 后生成项目时产生。
 xml 由dbchm的 XML导出 而产生。";
        }

        private void BtnBrow_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                //支持 pdm，实体类注释产生的xml文档，dbchm导出的xml文件
                Filter = "支持文件类型(*.pdm;*.xml)|*.pdm;*.xml|PowerDesigner文件(*.pdm)|*.pdm|xml文件|*.xml",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtMulItem.Text = string.Join("\r\n", openFileDialog.FileNames);
            }
        }

        private void BtnUpdateDisplayName_Click(object sender, EventArgs e)
        {
            if (DBUtils.Instance == null)
            {
                MessageBox.Show("更新批注，需连接数据库，请切换到要更新批注的数据库！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMulItem.Text))
            {
                MessageBox.Show("请先选择批注数据文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }


            FormUtils.ShowProcessing("正在更新批注到数据库，请稍等......", this, arg =>
            {
                string[] paths = txtMulItem.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string ph in paths)
                {
                    string extName = Path.GetExtension(ph).ToLower();
                    try
                    {
                        if (File.Exists(ph))
                        {
                            if (extName == ".pdm")
                            {
                                UpdateCommentByPDM(ph);
                            }
                            else if (extName == ".xml")
                            {
                                UpdateCommentByXML(ph);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "出错", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                MessageBox.Show("更新表列批注完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                FormUtils.IsOK_Close = true;
                this.Close();

            }, null);
        }


        /// <summary>
        /// 通过pdm文件更新批注
        /// </summary>
        /// <param name="path"></param>
        void UpdateCommentByPDM(string path)
        {
            var lstTabs = GetTables(path);
            var dbInfo = DBUtils.Instance?.Info;
            foreach (var tab in lstTabs)
            {
                string tab_Comment = tab.Name;
                if (!string.IsNullOrWhiteSpace(tab_Comment)
                    && !tab.Code.Equals(tab_Comment, StringComparison.OrdinalIgnoreCase))
                {
                    dbInfo.SetTableComment(tab.Code, tab_Comment);
                }
                var lstCols = tab.Columns;
                foreach (var col in lstCols)
                {
                    string col_Comment = col.Name;
                    if (!string.IsNullOrWhiteSpace(col_Comment)
                        && !col.Code.Equals(col_Comment, StringComparison.OrdinalIgnoreCase))
                    {
                        dbInfo.SetColumnComment(tab.Code, col.Code, col_Comment);
                    }
                }
            }
        }


        static IList<PdmModels.TableInfo> GetTables(params string[] pdmPaths)
        {
            List<PdmModels.TableInfo> lstTables = new List<PdmModels.TableInfo>();
            var pdmReader = new PDM.PdmReader();
            foreach (string path in pdmPaths)
            {
                if (File.Exists(path))
                {
                    var models = pdmReader.ReadFromFile(path);
                    lstTables.AddRange(models.Tables);
                }
            }
            lstTables = lstTables.OrderBy(t => t.Code).ToList();
            return lstTables;
        }

        void UpdateCommentByXML(string path)
        {
            var xmlContent = File.ReadAllText(path, Encoding.UTF8);
            if (xmlContent.Contains("ArrayOfTableDto"))
            {
                //通过 dbchm 导出的 XML文件 来更新 表列批注

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                
                var dbName = doc.DocumentElement.GetAttribute("databaseName");

                if (!DBUtils.Instance.Info.DBName.Equals(dbName, StringComparison.OrdinalIgnoreCase))
                {
                    if (MessageBox.Show("检测到数据库名称不一致，确定要继续吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                var lstDTO = typeof(List<TableDto>).DeserializeXml(xmlContent) as List<TableDto>;

                foreach (var tabInfo in lstDTO)
                {
                    if (DBUtils.Instance.Info.IsExistTable(tabInfo.TableName) && !string.IsNullOrWhiteSpace(tabInfo.Comment))
                    {
                        DBUtils.Instance.Info.SetTableComment(tabInfo.TableName, tabInfo.Comment);
                    }

                    foreach (var colInfo in tabInfo.Columns)
                    {
                        if (DBUtils.Instance.Info.IsExistColumn(tabInfo.TableName, colInfo.ColumnName) && !string.IsNullOrWhiteSpace(colInfo.Comment))
                        {
                            DBUtils.Instance.Info.SetColumnComment(tabInfo.TableName, colInfo.ColumnName, colInfo.Comment);
                        }
                    }
                }
            }
            else
            {

                //通过 有 VS 生成的 实体类库 XML文档文件 来更新 表列批注

                XmlAnalyze analyze = new XmlAnalyze(path);

                var data = analyze.Data;

                foreach (var item in data)
                {
                    if (DBUtils.Instance.Info.IsExistTable(item.Key.Key) && !string.IsNullOrWhiteSpace(item.Key.Value))
                    {
                        DBUtils.Instance.Info.SetTableComment(item.Key.Key, item.Key.Value);
                    }

                    foreach (var colKV in item.Value)
                    {
                        if (DBUtils.Instance.Info.IsExistColumn(item.Key.Key, colKV.Key) && !string.IsNullOrWhiteSpace(colKV.Value))
                        {
                            DBUtils.Instance.Info.SetColumnComment(item.Key.Key, colKV.Key, colKV.Value);
                        }
                    }
                }

            }
        }
    }
}
