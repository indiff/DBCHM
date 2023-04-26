using DDTek.Oracle;
using MJTop.Data.DatabaseInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace MJTop.Data.Database
{
    public class OracleDDTekDB : DB
    {
        public OracleDDTekDB(DBType dbType, DbProviderFactory dbFactory, string connectionString, int cmdTimeOut)
            : base(dbType, dbFactory, connectionString)
        {
            this.CmdTimeout = cmdTimeOut;
            this.Info = new OracleDBInfo(this);
        }

        public override bool ValidateSql(string strSql, out Exception ex)
        {
            bool bResult = false;
            ex = null;
            using (DbConnection conn = CreateConn())
            {
                DbCommand cmd = conn.CreateCommand();
                conn.Open();
                try
                {
                    cmd.CommandText = "explain plan for " + strSql;
                    cmd.ExecuteNonQuery();
                    bResult = true;
                }
                catch (Exception e)
                {
                    ex = e;
                    bResult = false;
                }
                finally
                {
                    cmd?.Dispose();
                }
            }
            return bResult;
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

                var result1 = cmd.ExecuteNonQuery();

                if (OnExecuted != null)
                {
                    OnExecuted.Invoke(cmd, result1);
                }

                OracleDBInfo OraInfo = Info as OracleDBInfo;

                string identitySql = Script.IdentitySql(DBType, tableName, OraInfo.IdentitySeqName(tableName));

                cmd.CommandText = identitySql;
                cmd.Parameters.Clear();

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
                    act.Invoke();
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
                    act.Invoke();
                }
            }
            return res;
        }

        public override KeyValuePair<DataTable, long> GetDataTableByPager(int currentPage, int pageSize, string selColumns, string joinTableName, string whereStr, string orderbyStr)
        {
            if (string.IsNullOrEmpty(selColumns))
            {
                selColumns = "*";
            }

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 50;
            }

            string cntSQL = string.Empty, strPageSQL = string.Empty;
            DataTable data = new DataTable();
            long totalCount = 0;

            if (!string.IsNullOrWhiteSpace(whereStr))
            {
                whereStr = Regex.Replace(whereStr, @"(\s)*(where|and)?(\s)*(.+)", "and $3$4", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            if (!string.IsNullOrWhiteSpace(orderbyStr))
            {
                orderbyStr = Regex.Replace(orderbyStr, @"(\s)*(order)(\s)+(by)(.+)", "$5", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            else
            {
                throw new ArgumentNullException("orderbyStr");
            }

            cntSQL = "select count(1) from {0} where 1=1 {1}";
            cntSQL = string.Format(cntSQL, joinTableName, whereStr);

            string strSQL = "select {0} from {1} where 1=1 {2} order by {3}";
            strSQL = string.Format(strSQL, selColumns, joinTableName, whereStr, orderbyStr);

            strPageSQL = string.Format(@"SELECT * FROM (SELECT A.*, ROWNUM RN FROM ({0}) A) WHERE RN BETWEEN {1} AND {2}",
                                       strSQL, (currentPage - 1) * pageSize + 1, (currentPage) * pageSize);

            DataSet ds = new DataSet("ds");
            DbConnection conn = null;
            DbCommand cmd = null;

            try
            {
                conn = CreateConn();
                cmd = BuildCommand(conn, strPageSQL, 300);
                DataAdapter adapter = CreateAdapter(cmd);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    data = ds.Tables[0];
                }

                cmd.CommandText = cntSQL;
                cmd.Parameters.Clear();

                totalCount = cmd.ExecuteScalar().ChangeType<long>();

                KeyValuePair<DataTable, long> res = new KeyValuePair<DataTable, long>(data, totalCount);
                if (OnExecuted != null)
                {
                    OnExecuted.Invoke(cmd, res);
                }
                return res;
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

        public override int BulkInsert(string tableName, DataTable data, int batchSize = 200000, int timeout = 60)
        {
            List<string> lstAllColName = this.Info[tableName];
            OracleBulkCopy bulk = null;
            bulk = new OracleBulkCopy(this.ConnectionString);
            using (bulk)
            {
                int colCount = data.Columns.Count;
                for (int j = 0; j < colCount; j++)
                {
                    if (lstAllColName.Contains(data.Columns[j].ColumnName, StringComparer.OrdinalIgnoreCase))
                    {
                        bulk.ColumnMappings.Add(new OracleBulkCopyColumnMapping(data.Columns[j].ColumnName, data.Columns[j].ColumnName));
                    }
                }
                bulk.DestinationTableName = tableName;
                bulk.BulkCopyTimeout = timeout;
                bulk.BatchSize = batchSize;
                bulk.WriteToServer(data);
            }
            return data.Rows.Count;
        }

        public override int BulkInsert(string tableName, DbDataReader reader, int batchSize = 200000, int timeout = 60)
        {
            List<string> lstAllColName = this.Info[tableName];
            OracleBulkCopy bulk = null;
            bulk = new OracleBulkCopy(this.ConnectionString);
            using (bulk)
            {
                int colCount = reader.FieldCount;
                for (int j = 0; j < colCount; j++)
                {
                    string currName = reader.GetName(j);
                    if (lstAllColName.Contains(currName, StringComparer.OrdinalIgnoreCase))
                    {
                        bulk.ColumnMappings.Add(new OracleBulkCopyColumnMapping(currName, currName));
                    }
                }
                bulk.DestinationTableName = tableName;
                bulk.BulkCopyTimeout = timeout;
                bulk.BatchSize = batchSize;

                bulk.WriteToServer(reader);
                reader.Close();
            }
            return reader.RecordsAffected;
        }

        public override DataTable SelectTop(string tableName, int top = 10, string orderbyStr = null)
        {
            string strSql = "select * from (select * from {0} {1})t where rownum <= {2}";
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