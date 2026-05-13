using System;using System.Collections.Generic;using System.ComponentModel;using System.Drawing;using System.Windows.Forms;using System.Reflection;using ComponentFactory.Krypton.Toolkit;namespace DBCHM{    partial class AboutBox : KryptonForm    {        public AboutBox()        {            InitializeComponent();            this.Text = String.Format("关于 {0}", AssemblyTitle);            this.labelProductName.Text = AssemblyProduct;            this.labelVersion.Text = String.Format("版本号： {0}", AssemblyVersion);            this.labelCopyright.Text = AssemblyCopyright;            this.labelCompanyName.Text = AssemblyCompany;
            //this.textBoxDescription.Text = "更新时间：2024-07-31" + "\r\n\r\n" +            //    "更新内容：\r\n\r\n" +            //    "1、支持达梦数据库连接配置及数据库字典文档生成。\r\n\r\n" +            //    "2、MySQL数据库连接配置优化，数据库连接字符串支持添加自定义参数。\r\n\r\n" +             //    "3、数据库表、视图、存储过程查询优化。\r\n\r\n" +            //    "4、MySql.Data.6.9.12升级为MySql.Data.8.0.31，解决“Character set 'utf8mb3' is not supported by .Net Framework.”问题。\r\n\r\n" +            //    "5、修复了一些已知问题。\r\n\r\n" +            //    this.textBoxDescription.Text;            this.textBoxDescription.Text = "更新时间：2024-10-08" + "\r\n\r\n" +                "更新内容：\r\n\r\n" +                "1、支持神通数据库连接配置及数据库字典文档生成。\r\n\r\n" +                "2、支持人大金仓（电科金仓）数据库连接配置及数据库字典文档生成。\r\n\r\n" +                "3、修复了一些已知问题。\r\n\r\n" +                 this.textBoxDescription.Text;        }

        #region Assembly Attribute Accessors
        public string AssemblyTitle        {            get            {
                // Get all Title attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")                        return titleAttribute.Title;                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);            }        }        public string AssemblyVersion        {            get            {                return Assembly.GetExecutingAssembly().GetName().Version.ToString();            }        }        public string AssemblyDescription        {            get            {
                // Get all Description attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;            }        }        public string AssemblyProduct        {            get            {
                // Get all Product attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;            }        }        public string AssemblyCopyright        {            get            {
                // Get all Copyright attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;            }        }        public string AssemblyCompany        {            get            {
                // Get all Company attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;            }        }
        #endregion
        private void okButton_Click(object sender, EventArgs e)        {            this.Dispose();        }        private void textBoxDescription_Click(object sender, EventArgs e)        {
            // System.Diagnostics.Process.Start("http://shang.qq.com/wpa/qunwpa?idkey=43619cbe3b2a10ded01b5354ac6928b30cc91bda45176f89a191796b7a7c0e26");			System.Diagnostics.Process.Start("https://github.com/indiff/DBCHM/issues");
        }    }}