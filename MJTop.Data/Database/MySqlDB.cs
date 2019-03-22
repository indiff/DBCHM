using MJTop.Data.DatabaseInfo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using MySql.Data.MySqlClient;

namespace MJTop.Data.Database
{
    public class MySqlDB : DB
    {
        public MySqlDB(DBType dbType, DbProviderFactory dbFactory, string connectionString, int cmdTimeOut)
            : base(dbType, dbFactory, connectionString)
        {
            this.CmdTimeout = cmdTimeOut;
            this.Info = new MySqlDBInfo(this);
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
                whereStr = Regex.Replace(whereStr, @"(\s)*(where)?(\s)*(.+)", "and $3$4", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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

            string strSQL = "select {0} from {1} where 1=1 {2} order by {3} ";
            strSQL = string.Format(strSQL, selColumns, joinTableName, whereStr, orderbyStr);

            strPageSQL = string.Format(@"SELECT * FROM ({0}) A limit {1},{2}",
                                       strSQL, (currentPage - 1) * pageSize,  pageSize);

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

                if (OnExecuted != null)
                {
                    OnExecuted.Invoke(cmd, data);
                }

                cmd.CommandText = cntSQL;
                cmd.Parameters.Clear();

                totalCount = cmd.ExecuteScalar().ChangeType<long>();

                if (OnExecuted != null)
                {
                    OnExecuted.Invoke(cmd, totalCount);
                }
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
            return new KeyValuePair<DataTable, long>(data, totalCount);
        }


        public override DataTable SelectTop(string tableName, int top = 10, string orderbyStr = null)
        {
            string strSql = "select * from {0} {1} limit 0,{2}";
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

        ///将DataTable转换为标准的CSV  
        /// </summary>  
        /// <param name="table">数据表</param>  
        /// <returns>返回标准的CSV</returns>  
        private static string DataTableToCsv(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。  
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。  
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。  
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public override int BulkInsert(string tableName, DataTable data, int batchSize = 200000, int timeout = 60)
        {
            if (data.Rows.Count == 0) return 0;
            int insertCount = 0;
            string tmpPath = Path.GetTempFileName();
            string csv = DataTableToCsv(data);
            File.WriteAllText(tmpPath, csv);

            List<string> lstAllColName = this.Info[tableName];

            MySqlConnection conn = null;
            try
            {
                conn = CreateConn() as MySqlConnection;
                conn.Open();
                MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                {
                    FieldTerminator = ",",
                    FieldQuotationCharacter = '"',
                    EscapeCharacter = '"',
                    LineTerminator = "\r\n",
                    FileName = tmpPath,
                    NumberOfLinesToSkip = 0,
                    TableName = tableName,
                    Timeout=timeout
                };

                foreach (DataColumn dc in data.Columns)
                {
                    if (lstAllColName.Contains(dc.ColumnName,StringComparer.OrdinalIgnoreCase))
                    {
                        bulk.Columns.Add(dc.ColumnName);
                    }
                }
                insertCount = bulk.Load();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                File.Delete(tmpPath);
            }
            return insertCount;
        }
    }
}
