using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCHM
{
    public class DBCHMConfig
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("连接名")]
        public string Name { get; set; }

        [DisplayName("数据库类型")]
        public string DBType { get; set; }

        [DisplayName("主机")]
        public string Server { get; set; }

        [DisplayName("端口")]
        public int? Port { get; set; }
        [DisplayName("数据库")]
        public string DBName { get; set; }

        [DisplayName("用户名")]
        public string Uid { get; set; }

        [DisplayName("密码")]
        public string Pwd { get; set; }

        [DisplayName("连接字符串")]
        public string ConnString { get; set; }
    }
}
