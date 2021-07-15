using ComponentFactory.Krypton.Toolkit;
using MJTop.Data;
using System;
using System.Collections.Generic;
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
                    foreach (var item in FormUtils.DictDBType)
                    {
                        cboDBType.Items.Add(item.Value.ToString());
                    }

                    this.Id = id.Value;
                    DBCHMConfig config = ConfigUtils.Get(id.Value);
                    TxtConnectName.Text = config.Name;
                    cboDBType.Text = config.DBType;
                    TxtHost.Text = config.Server;
                    TxtPort.Text = config.Port?.ToString();
                    TxtUName.Text = config.Uid;
                    TxtPwd.Text = config.Pwd;
                    cboDBName.Text = config.DBName;
                    txtConnTimeOut.Text = config.ConnTimeOut?.ToString();

                    if (this.OpType == OPType.克隆)
                    {
                        TxtConnectName.Text += "_Clone";
                    }

                    if (config.DBType == DBType.SQLite.ToString())
                    {
                        btnSelectFile.Visible = true;

                        TxtHost.Enabled = false;
                        TxtPort.Enabled = false;
                        TxtUName.Enabled = false;

                        //暂不支持 加密的 Sqlite数据库
                        TxtPwd.Enabled = false;
                    }

                    //编辑时，确定后刷新连接列表
                    BtnOk.DialogResult = DialogResult.OK;
                }
                else
                {
                    btnSelectFile.Visible = false;
                }

                if (string.IsNullOrWhiteSpace(txtConnTimeOut.Text))
                {
                    txtConnTimeOut.Text = "30";
                }
            }

            //为KeyDown能应用到所有控件上 注册 KeyDown 事件 
            foreach (Control control in this.Controls)
            {
                control.KeyDown += control_KeyDown;
            }
            lblMsg.Text = string.Empty;
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
                TxtHost.Text = "127.0.0.1";
                // TODO 设置默认用户名等
                SetUserNameByDbType();
            }
        }
        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDia = new OpenFileDialog();
            var dia = fileDia.ShowDialog();
            if (dia == DialogResult.OK)
            {
                cboDBName.Text = fileDia.FileName;
            }
        }

        public void SetMsg(string msg, bool isSuccess = false)
        {
            lblMsg.Text = msg;
            lblMsg.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        private void BtnTestConnect_Click(object sender, EventArgs e)
        {
            DBType type = (DBType)Enum.Parse(typeof(DBType), cboDBType.Text);
            try
            {
                if (type == DBType.Oracle && string.IsNullOrWhiteSpace(cboDBName.Text))
                {
                    throw new Exception("Oracle没有提供数据库名称查询支持，请输入服务名！");
                }

                var connString = InitConnectionStr(type);

                List<string> dbNames = null;

                DBMgr.TryConnect(type, connString, out dbNames);

                if (dbNames != null && dbNames.Count > 0)
                {
                    cboDBName.Items.Clear();
                    foreach (var dbName in dbNames)
                    {
                        cboDBName.Items.Add(dbName);
                    }
                }

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
            try
            {
                if (string.IsNullOrWhiteSpace(TxtConnectName.Text))
                {
                    MessageBox.Show("请输入连接名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(cboDBName.Text))
                {
                    MessageBox.Show("请输入数据库名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DBType type = (DBType)Enum.Parse(typeof(DBType), cboDBType.Text);


                string connString = InitConnectionStr(type);
                NameValueCollection nvc = new NameValueCollection();
                if (OpType == OPType.新建 || OpType == OPType.克隆)
                {
                    nvc.Add("Name", TxtConnectName.Text.Trim());
                    nvc.Add("DBType", cboDBType.Text.Trim());

                    nvc.Add("Server", TxtHost.Enabled ? TxtHost.Text.Trim() : string.Empty);
                    nvc.Add("Port", TxtPort.Enabled ? TxtPort.Text : string.Empty);
                    nvc.Add("DBName", cboDBName.Text.Trim());
                    nvc.Add("Uid", TxtUName.Enabled ? TxtUName.Text.Trim() : string.Empty);
                    nvc.Add("Pwd", TxtPwd.Enabled ? TxtPwd.Text : string.Empty);
                    nvc.Add("ConnTimeOut", txtConnTimeOut.Enabled ? txtConnTimeOut.Text : "30");
                    nvc.Add("ConnString", connString);
                    nvc.Add("Modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    ConfigUtils.Save(nvc);

                }
                else if (OpType == OPType.编辑)
                {
                    nvc.Add("Id", Id.ToString());
                    nvc.Add("Name", TxtConnectName.Text.Trim());
                    nvc.Add("DBType", cboDBType.Text.Trim());

                    nvc.Add("Server", TxtHost.Enabled ? TxtHost.Text.Trim() : string.Empty);
                    nvc.Add("Port", TxtPort.Enabled ? TxtPort.Text : string.Empty);
                    nvc.Add("DBName", cboDBName.Text.Trim());
                    nvc.Add("Uid", TxtUName.Enabled ? TxtUName.Text.Trim() : string.Empty);
                    nvc.Add("Pwd", TxtPwd.Enabled ? TxtPwd.Text : string.Empty);
                    nvc.Add("ConnTimeOut", txtConnTimeOut.Enabled ? txtConnTimeOut.Text : "30");
                    nvc.Add("ConnString", connString);
                    nvc.Add("Modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    ConfigUtils.Save(nvc);
                }


                FormUtils.ShowProcessing("正在查询表结构信息，请稍等......", this, arg =>
                {
                    try
                    {
                        InitDb(type);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("BtnOk_Click", Developer.SysDefault, ex, connString);
                    }

                }, null);

                this.DialogResult = DialogResult.OK;                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void cboDBType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = "连接数据库";
            string port;
            if (FormUtils.DictPort.TryGetValue(cboDBType.Text, out port))
            {
                TxtPort.Text = port;
            }
            // TODO 设置默认用户名等
            SetUserNameByDbType();
        }

        /// <summary>
        /// 根据数据库类型设置默认用户名等 扩展修改 2019-01-24 21:23
        /// </summary>
        private void SetUserNameByDbType()
        {
            btnSelectFile.Visible = false;
            
            TxtHost.Enabled = true;
            TxtPort.Enabled = true;
            TxtUName.Enabled = true;

            sslLabel.Visible = false;
            noneSSLCB.Visible = false;
            requiredSSLCB.Visible = false;

            labDBName.Text = "数据库";
            DBType dbtype = (DBType)Enum.Parse(typeof(DBType), cboDBType.Text.ToString());
            if (dbtype == DBType.SqlServer)
            {
                TxtUName.Text = "sa";
            }
            else if (dbtype == DBType.MySql)
            {
                TxtUName.Text = "root";

                sslLabel.Visible = true;
                noneSSLCB.Visible = true;
                requiredSSLCB.Visible = true;

            }
            else if (dbtype == DBType.Oracle || dbtype == DBType.OracleDDTek)
            {
                TxtUName.Text = "scott";
                labDBName.Text = "服务名";
            }
            else if (dbtype == DBType.PostgreSql)
            {
                TxtUName.Text = "postgres";
            }
            else if (dbtype == DBType.DB2)
            {
                TxtUName.Text = "db2admin";
            }
            else if(dbtype == DBType.SQLite)
            {
                btnSelectFile.Visible = true;

                TxtHost.Enabled = false;
                TxtPort.Enabled = false;
                TxtUName.Enabled = false;

                //暂不支持 加密的 Sqlite数据库
                TxtPwd.Enabled = false;
            }
            else
            {
                TxtUName.Text = "";
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

        private void txtConnectionOut_KeyPress(object sender, KeyPressEventArgs e)
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
                    int timtOut = 0;
                    int.TryParse(TxtPort.Text + e.KeyChar.ToString(), out timtOut);
                    if (timtOut > -1)
                    {
                        e.Handled = false;
                    }
                }
            }

        }

        private void cboDBName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = "连接数据库";
        }

        private void noneSSLCB_Click(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
            {
                requiredSSLCB.Checked = false;
            }
            else
            {
                requiredSSLCB.Checked = true;
            }
        }

        private void requiredSSLCB_Click(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
            {
                noneSSLCB.Checked = false;
            }
            else
            {
                noneSSLCB.Checked = true;
            }
        }

        private string extraParam = ""; // 额外参数

        /// <summary>
        /// 临时处理
        /// TODO 创建额外参数
        /// </summary>
        private void CreateExtraParam()
        {
            // TODO 额外参数
            if (noneSSLCB.Checked == true)
            {
                this.extraParam = "SslMode=None;";
            }
            else if (requiredSSLCB.Checked == true)
            {
                this.extraParam = "SslMode=Required;";
            }
            else
            {
                this.extraParam = "";
            }
        }

        /// <summary>
        /// 临时处理
        /// TODO 初始化DB连接
        /// </summary>
        /// <param name="type"></param>
        /// <param name="strDBName"></param>
        private void InitDb(DBType type) 
        {
            if (type == DBType.MySql)
            {
                CreateExtraParam();
                DBUtils.Instance = DBMgr.UseDB(type, TxtHost.Text,
                    (string.IsNullOrWhiteSpace(TxtPort.Text) ? null : new Nullable<int>(Convert.ToInt32(TxtPort.Text))),
                    cboDBName.Text.Trim(), TxtUName.Text, TxtPwd.Text,
                    (string.IsNullOrWhiteSpace(txtConnTimeOut.Text) ? 30 : Convert.ToInt32(txtConnTimeOut.Text))
                    , 300, this.extraParam);
            }
            else
            {
                DBUtils.Instance = DBMgr.UseDB(type, TxtHost.Text,
                    (string.IsNullOrWhiteSpace(TxtPort.Text) ? null : new Nullable<int>(Convert.ToInt32(TxtPort.Text))),
                    cboDBName.Text.Trim(), TxtUName.Text, TxtPwd.Text,
                    (string.IsNullOrWhiteSpace(txtConnTimeOut.Text) ? 30 : Convert.ToInt32(txtConnTimeOut.Text))
                    , 300);
            }
        }

        /// <summary>
        /// 临时处理
        /// TODO 初始化连接串
        /// </summary>
        private string InitConnectionStr(DBType type) {
            CreateExtraParam();
            string connString = "";
            if (type == DBType.MySql)
            {
                connString = DBMgr.GetConnectionString(type, TxtHost.Text,
                    (string.IsNullOrWhiteSpace(TxtPort.Text) ? null : new Nullable<int>(Convert.ToInt32(TxtPort.Text))),
                    cboDBName.Text, TxtUName.Text, TxtPwd.Text,
                    (string.IsNullOrWhiteSpace(txtConnTimeOut.Text) ? 30 : Convert.ToInt32(txtConnTimeOut.Text)), 
                    this.extraParam);
            }
            else
            {
                connString = DBMgr.GetConnectionString(type, TxtHost.Text,
                    (string.IsNullOrWhiteSpace(TxtPort.Text) ? null : new Nullable<int>(Convert.ToInt32(TxtPort.Text))),
                    cboDBName.Text, TxtUName.Text, TxtPwd.Text,
                    (string.IsNullOrWhiteSpace(txtConnTimeOut.Text) ? 30 : Convert.ToInt32(txtConnTimeOut.Text))
                    );
            }
            return connString;
        }
    }
}
