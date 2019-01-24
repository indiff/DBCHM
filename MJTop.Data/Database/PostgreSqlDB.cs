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
    public class PostgreSqlDB : DB
    {
        public PostgreSqlDB(DBType dbType, DbProviderFactory dbFactory, string connectionString, int cmdTimeOut)
            : base(dbType, dbFactory, connectionString)
        {
            this.CmdTimeout = cmdTimeOut;
            this.Info = new PostgreSqlDBInfo(this);
        }

        internal override Ret InsertGet<DT, Ret>(DT data, string tableName, params string[] excludeColNames)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            try
            {
                var kv = InsertScript(data, tableName, excludeColNames);

                conn = CreateConn();
                cmd = BuildCommandByParam(conn, kv.Key, kv.Value, 30);

                PostgreSqlDBInfo postgreInfo = Info as PostgreSqlDBInfo;

                cmd.CommandText = kv.Key + ";" + Script.IdentitySql(DBType, tableName, null, postgreInfo.IdentityColumnName(tableName));
               
                var result2 = cmd.ExecuteScalar().ChangeType<Ret>();

                if (OnExecuted != null)
                {
                    OnExecuted.Invoke(cmd, result2);
                }

                return result2;
            }
            catch (Exception ex)
            {
                if (this.OnError != null)
                    this.OnError.Invoke(cmd, ex);
                throw ex;
            }
            finally
            {
                conn?.Close();
            }
        }

        public override int InsertGetInt<DT>(DT data, string tableName, params string[] excludeColNames)
        {
            int res = InsertGet<DT, int>(data, tableName, excludeColNames);
            var lstAct = DataChangeTriggers.GetActions(tableName);
            if (lstAct.Any())
            {
                foreach (var act in lstAct)
                {
                    try
                    {
                        act.Invoke();
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return res;
        }

        public override long InsertGetLong<DT>(DT data, string tableName, params string[] excludeColNames)
        {
            long res = InsertGet<DT, long>(data, tableName, excludeColNames);
            var lstAct = DataChangeTriggers.GetActions(tableName);
            if (lstAct.Any())
            {
                foreach (var act in lstAct)
                {
                    try
                    {
                        act.Invoke();
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return res;
        }

        public override DataTable SelectTop(string tableName, int top = 10, string orderbyStr = null)
        {
            string strSql = "select * from {0} {1} desc limit {2}";
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
