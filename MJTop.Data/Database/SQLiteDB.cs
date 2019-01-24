using MJTop.Data.DatabaseInfo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MJTop.Data.Database
{
    public class SQLiteDB : DB
    {
        public SQLiteDB(DBType dbType, DbProviderFactory dbFactory, string connectionString, int cmdTimeOut)
            : base(dbType, dbFactory, connectionString)
        {
            this.CmdTimeout = cmdTimeOut;
            this.Info = new SQLiteDBInfo(this);
        }


        public override DataTable SelectTop(string tableName, int top = 10, string orderbyStr = null)
        {
            string strSql = "select * from {0} {1} desc limit 0,{2}";
            if (!string.IsNullOrWhiteSpace(orderbyStr))
            {
                strSql = string.Format(tableName, " order by " + orderbyStr, top);
            }
            else
            {
                strSql = string.Format(tableName, string.Empty, top);
            }
            return base.SelectTop(tableName, top, orderbyStr);
        }
    }
}
