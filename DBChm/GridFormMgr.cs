using ComponentFactory.Krypton.Toolkit;
using System;
using System.Windows.Forms;
using Top._51Try.Data;

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

        void control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }


        private void linkAdd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DBForm dbForm = new DBForm(OPType.新建);
            var diaResult = dbForm.ShowDialog(this);
            if (diaResult == DialogResult.OK)
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
            var diaResult = dbForm.ShowDialog(this);
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
            DBUtils.Instance = DBMgr.Connect((DBType)Enum.Parse(typeof(DBType), config.DBType), config.ConnString);

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
    }
}
