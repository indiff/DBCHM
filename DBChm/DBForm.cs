using ComponentFactory.Krypton.Toolkit;
using MJTop.Data;
using System;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace DBCHM
{
    public partial class DBForm : KryptonForm
    {
        private DBForm()
            : this(OPType.新建)
        {

        }

        /// <summary>
        /// 当前操作类型
        /// </summary>
        public OPType OpType { get; private set; }

        public int Id { get; private set; }

        public DBForm(OPType opType, int? id = null)
        {

            InitializeComponent();

            this.OpType = opType;

            if ((this.OpType == OPType.编辑 || this.OpType == OPType.克隆) && !id.HasValue)
            {
                throw new ArgumentNullException(this.OpType + "操作必须传递要操作的Id！");
            }
            else
            {
                if (id.HasValue)
                {
                    this.Id = id.Value;

                    DBCHMConfig config = ConfigUtils.Get(id.Value);
                    TxtConnectName.Text = config.Name;
                    cboDBType.Text = config.DBType;
                    TxtHost.Text = config.Server;
                    TxtPort.Text = config.Port.ToString();
                    TxtUName.Text = config.Uid;
                    TxtPwd.Text = config.Pwd;
                    cboDBName.Text = config.DBName;

                    if (this.OpType == OPType.克隆)
                    {
                        TxtConnectName.Text += "_Clone";
                    }
                }
            }

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



        private void DBFrom_Load(object sender, EventArgs e)
        {
            if (OpType == OPType.新建)
            {
                foreach (var item in FormUtils.DictDBType)
                {
                    cboDBType.Items.Add(item.Value.ToString());
                }

                cboDBType.SelectedIndex = 0;
                string port;
                if (FormUtils.DictPort.TryGetValue(cboDBType.Text, out port))
                {
                    TxtPort.Text = port;
                }
            }

        }

        private void BtnTestConnect_Click(object sender, EventArgs e)
        {
            DBType type = (DBType)Enum.Parse(typeof(DBType), cboDBType.Text);
            if (type == DBType.Oracle && string.IsNullOrWhiteSpace(cboDBName.Text))
            {
                MessageBox.Show("Oracle没有提供数据库名称查询支持，请输入服务名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            if (type == DBType.SqlServer && !TxtUName.Text.Equals("sa", StringComparison.OrdinalIgnoreCase))
            {
                var dia = MessageBox.Show("非超级管理员的账号，可能因权限不足，查询不出表结构信息，确定要继续吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dia == DialogResult.Cancel)
                {
                    return;
                }
            }

            try
            {
                DBUtils.Instance = DBMgr.UseDB(type, TxtHost.Text, Convert.ToInt32(TxtPort.Text), cboDBName.Text, TxtUName.Text, TxtPwd.Text);

                var info = DBUtils.Instance.Info;

                foreach (var dbName in info.DBNames)
                {
                    if (!cboDBName.Items.Contains(dbName))
                    {
                        cboDBName.Items.Add(dbName);
                    }
                }
                cboDBName.SelectedItem = cboDBName.Text;
                this.Text = "连接服务器成功！";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtConnectName.Text))
            {
                MessageBox.Show("请输入连接名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            if (string.IsNullOrWhiteSpace(cboDBName.Text))
            {
                MessageBox.Show("请输入数据库名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            DBType type = (DBType)Enum.Parse(typeof(DBType), cboDBType.Text);
            string connString = DBMgr.GetConnectionString(type, TxtHost.Text, Convert.ToInt32(TxtPort.Text), cboDBName.Text, TxtUName.Text, TxtPwd.Text);
            NameValueCollection nvc = new NameValueCollection();
            if (OpType == OPType.新建 || OpType == OPType.克隆)
            {
                nvc.Add("Name", TxtConnectName.Text.Trim());
                nvc.Add("DBType", cboDBType.Text.Trim());

                nvc.Add("Server", TxtHost.Text.Trim());
                nvc.Add("Port", TxtPort.Text);
                nvc.Add("DBName", cboDBName.Text.Trim());
                nvc.Add("Uid", TxtUName.Text.Trim());
                nvc.Add("Pwd", TxtPwd.Text);

                nvc.Add("ConnString", connString);

                ConfigUtils.Save(nvc);

                this.Close();

            }
            else if (OpType == OPType.编辑)
            {
                nvc.Add("Id", Id.ToString());
                nvc.Add("Name", TxtConnectName.Text.Trim());
                nvc.Add("DBType", cboDBType.Text.Trim());

                nvc.Add("Server", TxtHost.Text.Trim());
                nvc.Add("Port", TxtPort.Text);
                nvc.Add("DBName", cboDBName.Text.Trim());
                nvc.Add("Uid", TxtUName.Text.Trim());
                nvc.Add("Pwd", TxtPwd.Text);

                nvc.Add("ConnString", connString);
                ConfigUtils.Save(nvc);
            }
        }

        private void cboDBType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = "连接数据库";
            string port;
            if (FormUtils.DictPort.TryGetValue(cboDBType.Text, out port) && string.IsNullOrWhiteSpace(TxtPort.Text))
            {
                TxtPort.Text = port;
            }

            if (cboDBType.Text.ToString() == DBType.Oracle.ToString())
            {
                labDBName.Text = "服务名";
            }
            else
            {
                labDBName.Text = "数据库";
            }           
        }

        /// <summary>
        /// 端口验证 只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == 8 || e.KeyChar == 127)//退格删除，delete删除
            {
                e.Handled = false;
            }
            else
            {
                if (e.KeyChar >= '0' && e.KeyChar <= '9')//只能输入数字
                {
                    int maxPort = 0;
                    int.TryParse(TxtPort.Text + e.KeyChar.ToString(), out maxPort);
                    if (maxPort > 0 && maxPort <= 65535)
                    {
                        e.Handled = false;
                    }
                }
            }

        }


    }
}
