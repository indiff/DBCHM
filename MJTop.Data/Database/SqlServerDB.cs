using MJTop.Data.DatabaseInfo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace MJTop.Data.Database
{
    public class SqlServerDB : DB
    {
        public SqlServerDB(DBType dbType, DbProviderFactory dbFactory, string connectionString, int cmdTimeOut)
            : base(dbType, dbFactory, connectionString)
        {
            this.CmdTimeout = cmdTimeOut;
            this.Info = new SqlServerDBInfo(this);
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
                    cmd.CommandText = "set noexec on;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = strSql;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "set noexec off;";
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

        public override KeyValuePair<DataTable,long> GetDataTableByPager(int currentPage, int pageSize, string selColumns, string joinTableName, string whereStr, string orderbyStr)
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
                whereStr = Regex.Replace(whereStr, @"(\s)*(where)?(\s)*(.+)", "where 1=1 and $3$4", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            if (!string.IsNullOrWhiteSpace(orderbyStr))
            {
                orderbyStr = Regex.Replace(orderbyStr, @"(\s)*(order)(\s)+(by)(.+)", "$5", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            else
            {
                throw new ArgumentNullException("orderbyStr");
            }

            cntSQL = "select count(1) from {0}  {1}";
            cntSQL = string.Format(cntSQL, joinTableName, whereStr);

            string strSQL = "select {0},ROW_NUMBER() OVER ( ORDER BY {3} ) RN from {1}  {2} ";
            strSQL = string.Format(strSQL, selColumns, joinTableName, whereStr, orderbyStr);

            strPageSQL = string.Format(@"SELECT * FROM ({0}) A WHERE   RN BETWEEN {1} AND {2}",
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

        public override int BulkInsert(string tableName, DataTable data,int batchSize = 200000, int timeout = 60)
        {
            List<string> lstAllColName = this.Info[tableName];
            SqlBulkCopy bulk = null;
            bulk = new SqlBulkCopy(this.ConnectionString);
            using (bulk)
            {
                int colCount = data.Columns.Count;
                for (int j = 0; j < colCount; j++)
                {
                    if (lstAllColName.Contains(data.Columns[j].ColumnName, StringComparer.OrdinalIgnoreCase))
                    {
                        bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(data.Columns[j].ColumnName, data.Columns[j].ColumnName));
                    }
                }
                bulk.DestinationTableName = tableName;
                bulk.BulkCopyTimeout = timeout;
                bulk.BatchSize = batchSize;
                bulk.WriteToServer(data);
            }
            return data.Rows.Count;
        }

        public override int BulkInsert(string tableName, DbDataReader reader,int batchSize = 200000, int timeout = 60)
        {
            List<string> lstAllColName = this.Info[tableName];
            SqlBulkCopy bulk = null;
            bulk = new SqlBulkCopy(this.ConnectionString);
            using (bulk)
            {
                int colCount = reader.FieldCount;
                for (int j = 0; j < colCount; j++)
                {
                    string currName = reader.GetName(j);
                    if (lstAllColName.Contains(currName, StringComparer.OrdinalIgnoreCase))
                    {
                        bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(currName, currName));
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
            string strSql = string.Format("select top {0} from {1} ", top, tableName);
            if (!string.IsNullOrWhiteSpace(orderbyStr))
            {
                strSql += " order by " + orderbyStr;
            }
            return base.SelectTop(tableName, top, orderbyStr);
        }
    }
}
