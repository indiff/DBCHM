using ComponentFactory.Krypton.Toolkit;
using MJTop.Data;
using System;
using System.Windows.Forms;

namespace DBCHM
{
    public partial class GridFormMgr : KryptonForm
    {
        public GridFormMgr()
        {
            InitializeComponent();

            //为KeyDown能应用到所有控件上 注册 KeyDown 事件
            foreach (Control control in this.Controls)
            {
                control.KeyDown += control_KeyDown;
            }
        }

        private void control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private DBForm dbForm = null;

        private void linkAdd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dbForm = new DBForm(OPType.新建);
            var dia = dbForm.ShowDialog();
            if (dia == DialogResult.OK)
            {
                RefreshListView();
            }
        }

        private void linkEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (GV_DBConfigs.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择连接！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int Id = Convert.ToInt32(GV_DBConfigs.SelectedRows[0].Cells[0].Value);
            DBForm dbForm = new DBForm(OPType.编辑, Id);
            var diaResult = dbForm.ShowDialog();
            if (diaResult == DialogResult.OK)
            {
                RefreshListView();
            }
        }

        private void linkRemove_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (GV_DBConfigs.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择连接！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("确定要删除该连接吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                int Id = Convert.ToInt32(GV_DBConfigs.SelectedRows[0].Cells[0].Value);
                ConfigUtils.Delete(Id);

                RefreshListView();
            }
        }

        private void linkClone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (GV_DBConfigs.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择连接！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int Id = Convert.ToInt32(GV_DBConfigs.SelectedRows[0].Cells[0].Value);
            DBForm dbForm = new DBForm(OPType.克隆, Id);
            var diaResult = dbForm.ShowDialog(this);
            if (diaResult == DialogResult.OK)
            {
                RefreshListView();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (GV_DBConfigs.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择连接！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int Id = Convert.ToInt32(GV_DBConfigs.SelectedRows[0].Cells[0].Value);
            DBCHMConfig config = ConfigUtils.Get(Id);

            if ((DBType)Enum.Parse(typeof(DBType), config.DBType) == DBType.SqlServer
                && !GV_DBConfigs.SelectedRows[0].Cells[6].Value.ToString().Equals("sa", StringComparison.OrdinalIgnoreCase))
            {
                var dia = MessageBox.Show("非超级管理员的账号，可能因权限不足，查询不出表结构信息，确定要继续吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dia == DialogResult.Cancel)
                {
                    return;
                }
            }

            FormUtils.ShowProcessing("正在查询表结构信息，请稍等......", this, arg =>
            {
                try
                {
                    DBUtils.Instance = DBMgr.UseDB((DBType)Enum.Parse(typeof(DBType), config.DBType), config.ConnString, 300);
                    ConfigUtils.UpLastModified(Id);
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("连接数据库失败", Developer.SysDefault, ex, config);
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }, null);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GridFormMgr_Load(object sender, EventArgs e)
        {
            RefreshListView();
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        private void RefreshListView()
        {
            var data = ConfigUtils.SelectAll();
            if (data != null)
            {
                GV_DBConfigs.DataSource = data;

                if (data.Count > 0)
                {
                    GV_DBConfigs.Columns[0].Visible = false;
                    GV_DBConfigs.Columns[1].Width = 150;
                }
            }
        }

        private void GV_DBConfigs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnConnect_Click(sender, e);
            //代表已经正常选中
            FormUtils.IsOK_Close = true;
            this.Close();
        }

        private void GV_DBConfigs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 7)
            {
                if (GV_DBConfigs.Columns[e.ColumnIndex].DataPropertyName == "Pwd")
                {
                    string str = e.Value.ToString();
                    string strEncrypt = string.Empty;
                    for (int j = 0; j < str.Length; j++)
                    {
                        strEncrypt += "*";
                    }
                    e.Value = strEncrypt;
                }
            }
        }
    }
}