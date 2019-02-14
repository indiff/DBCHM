using MJTop.Data.SPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MJTop.Data.DatabaseInfo
{
    public class MySqlDBInfo : IDBInfo
    {
        private DB Db { get; set; }

        /// <summary>
        /// 数据库工具
        /// </summary>
        public Tool Tools
        {
            get;
            private set;
        }

        public MySqlDBInfo(DB db)
        {
            this.Db = db;
            Refresh();
            this.Tools = new Tool(db, this);
        }

        public string DBName
        {
            get { return (Db.ConnectionStringBuilder as MySql.Data.MySqlClient.MySqlConnectionStringBuilder).Database; }
        }

        public NameValueCollection TableComments { get; private set; } = new NameValueCollection();

        public List<string> TableNames { get; private set; } = new List<string>();
        
        public IgCaseDictionary<TableInfo> TableInfoDict { get; private set; }

        public IgCaseDictionary<List<string>> TableColumnNameDict { get; private set; }

        public IgCaseDictionary<List<ColumnInfo>> TableColumnInfoDict { get; private set; }

        public IgCaseDictionary<NameValueCollection> TableColumnComments { get; private set; }

        private IgCaseDictionary<ColumnInfo> DictColumnInfo { get; set; }

        public NameValueCollection Views { get; private set; }

        public NameValueCollection Procs { get; private set; }

        public List<string> DBNames { get; private set; } = new List<string>();

        public ColumnInfo this[string tableName, string columnName]
        {
            get
            {
                ColumnInfo colInfo;
                var strKey = (tableName + "@" + columnName);
                DictColumnInfo.TryGetValue(strKey, out colInfo);
                return colInfo;
            }
        }

        public List<string> this[string tableName]
        {
            get
            {
                List<string> colNames;
                TableColumnNameDict.TryGetValue(tableName, out colNames);
                return colNames;
            }
        }

        public bool Refresh()
        {
            this.DictColumnInfo = new IgCaseDictionary<ColumnInfo>();
            this.TableInfoDict = new IgCaseDictionary<TableInfo>();
            this.TableColumnNameDict = new IgCaseDictionary<List<string>>();
            this.TableColumnInfoDict = new IgCaseDictionary<List<ColumnInfo>>();
            this.TableColumnComments = new IgCaseDictionary<NameValueCollection>();


            string dbSql = "SELECT SCHEMA_NAME FROM information_schema.SCHEMATA order by  SCHEMA_NAME asc";
            string strSql = string.Format("SELECT table_name name,TABLE_COMMENT value FROM INFORMATION_SCHEMA.TABLES WHERE table_type='base table' and  table_schema = '{0}' order by table_name asc ", DBName);

            string viewSql = string.Format("SELECT  table_name,VIEW_DEFINITION  FROM  information_schema.views where TABLE_SCHEMA='{0}' order by table_name asc", DBName);
            string procSql = string.Format("SELECT  ROUTINE_NAME,ROUTINE_DEFINITION   FROM   INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='{0}' AND ROUTINE_TYPE='PROCEDURE' ORDER BY ROUTINE_NAME ASC", DBName);

            try
            {
                this.DBNames = Db.ReadList<string>(dbSql);

                this.TableComments = Db.ReadNameValues(strSql);

                //this.Views = Db.ReadNameValues(viewSql);

                //this.Procs = Db.ReadNameValues(procSql);

                if (this.TableComments != null && this.TableComments.Count > 0)
                {
                    this.TableNames = this.TableComments.AllKeys.ToList();

                    List<Task> lstTask = new List<Task>();

                    foreach (var tableName in TableNames)
                    {
                        Task task = Task.Run(() =>
                        {
                            TableInfo tabInfo = new TableInfo();
                            tabInfo.TableName = tableName;
                            tabInfo.TabComment = TableComments[tableName];

                            //strSql = "SHOW FULL COLUMNS FROM " + tableName;

                            strSql = @"select ORDINAL_POSITION as Colorder,Column_Name as ColumnName,data_type as TypeName,COLUMN_COMMENT as DeText,
(case when data_type = 'float' or data_type = 'double' or data_type = 'decimal' then  NUMERIC_PRECISION else CHARACTER_MAXIMUM_LENGTH end ) as length,
NUMERIC_SCALE as Scale,( case when EXTRA='auto_increment' then 1 else 0 end) as IsIdentity,(case when COLUMN_KEY='PRI' then 1 else 0 end) as IsPK,
(case when IS_NULLABLE = 'NO' then 0 else 1 end)as CanNull,COLUMN_DEFAULT as DefaultVal
from information_schema.columns where table_schema = ?DBName and table_name = ?tableName order by ORDINAL_POSITION asc";

                            try
                            {
                                tabInfo.Colnumns = Db.GetDataTable(strSql, new { DBName = DBName, tableName = tableName }).ConvertToListObject<ColumnInfo>();
                                List<string> lstColName = new List<string>();
                                NameValueCollection nvcColDeText = new NameValueCollection();
                                foreach (ColumnInfo colInfo in tabInfo.Colnumns)
                                {
                                    lstColName.Add(colInfo.ColumnName);
                                    nvcColDeText.Add(colInfo.ColumnName, colInfo.DeText);

                                    var strKey = (tableName + "@" + colInfo.ColumnName);
                                    this.DictColumnInfo.Add(strKey, colInfo);

                                    if (colInfo.IsPK)
                                    {
                                        tabInfo.PriKeyColName = colInfo.ColumnName;
                                        if (colInfo.IsIdentity)
                                        {
                                            tabInfo.PriKeyType = PrimaryKeyType.AUTO;
                                        }
                                        else
                                        {
                                            tabInfo.PriKeyType = PrimaryKeyType.SET;
                                        }
                                    }

                                    Global.Dict_MySql_DbType.TryGetValue(colInfo.TypeName, out DbType type);
                                    colInfo.DbType = type;
                                }

                                this.TableInfoDict.Add(tableName, tabInfo);
                                this.TableColumnNameDict.Add(tableName, lstColName);
                                this.TableColumnInfoDict.Add(tableName, tabInfo.Colnumns);
                                this.TableColumnComments.Add(tableName, nvcColDeText);
                            }
                            catch (Exception ex)
                            {
                                LogUtils.LogError("DB", Developer.SysDefault, ex);
                            }
                        });
                        lstTask.Add(task);
                        if (lstTask.Count(t => t.Status != TaskStatus.RanToCompletion) >= 50)
                        {
                            Task.WaitAny(lstTask.ToArray());
                            lstTask = lstTask.Where(t => t.Status != TaskStatus.RanToCompletion).ToList();
                        }
                    }
                    Task.WaitAll(lstTask.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogError("DB", Developer.SysDefault, ex);
                return false;
            }
            return this.TableComments.Count == this.TableInfoDict.Count;
        }


        #region MySql 获取列信息
        //private List<ColumnInfo> MySqlReadColInfo(string strSql)
        //{
        //    List<ColumnInfo> lstCols = new List<ColumnInfo>();
        //    DbDataReader reader = null;
        //    reader = Db.Reader(strSql);
        //    int colorder = 1;
        //    while (reader.Read())
        //    {
        //        ColumnInfo colInfo = new ColumnInfo();
        //        colInfo.Colorder = colorder;
        //        colInfo.ColumnName = reader["Field"].ToString();

        //        {
        //            string typename = reader["Type"].ToString();
        //            string len = "", pre = "", scal = "";
        //            TypeNameProcess(typename, out typename, out len, out pre, out scal);

        //            colInfo.TypeName = typename;
        //            colInfo.Length = len.ChangeType<int?>((int?)null);
        //            colInfo.Scale = scal.ChangeType<int?>((int?)null);
        //        }

        //        colInfo.IsPK = (reader["Key"].ToString() == "PRI") ? true : false;

        //        colInfo.CanNull = (reader["Null"].ToString() == "YES") ? true : false;

        //        colInfo.DefaultVal = reader["Default"].ToString();

        //        colInfo.DeText = reader["Comment"].ToString();

        //        colInfo.IsIdentity = (reader["Extra"].ToString() == "auto_increment") ? true : false;

        //        lstCols.Add(colInfo);

        //        colorder++;
        //    }

        //    if (reader != null && !reader.IsClosed)
        //    {
        //        reader.Close();
        //    }

        //    return lstCols;
        //}

        //对类型名称 解析
        private void TypeNameProcess(string strName, out string TypeName, out string Length, out string Preci, out string Scale)
        {
            TypeName = strName;
            Length = string.Empty;
            Preci = string.Empty;
            Scale = string.Empty;

            if (strName.Contains("("))
            {
                if (!strName.Contains(","))
                {
                    TypeName = Regex.Replace(strName, @"(\w+)\((\d+)\)", "$1", RegexOptions.Compiled);
                    Length = Regex.Replace(strName, @"(\w+)\((\d+)\)", "$2", RegexOptions.Compiled);
                }
                else
                {
                    TypeName = Regex.Replace(strName, @"(\w+)\((\d+)\)", "$1", RegexOptions.Compiled);
                    Length = Regex.Replace(strName, @"(\w+)\((\d+),(\d+)\)", "$2", RegexOptions.Compiled);
                    Scale = Regex.Replace(strName, @"(\w+)\((\d+),(\d+)\)", "$3", RegexOptions.Compiled);
                }
            }
        }
        #endregion


        public bool IsAllMyISAM
        {
            get
            {
                string strSql = string.Format("select case when ((SELECT count(1) FROM information_schema.tables where table_type='base table' and TABLE_SCHEMA='{0}' )= (SELECT count(1) FROM information_schema.tables where table_type='base table' and TABLE_SCHEMA = '{0}' and ENGINE = 'MyISAM')) then true else FALSE end as res", DBName);
                return Db.Single<bool>(strSql, false);
            }
        }


        public Dictionary<string, DateTime> GetTableStruct_Modify()
        {
            string strSql = string.Format("SELECT TABLE_NAME name,UPDATE_TIME modify_date FROM information_schema.tables where TABLE_SCHEMA='{0}' and table_type='base table' and update_time is not null ORDER BY UPDATE_TIME asc", DBName);
            return Db.ReadDictionary<string, DateTime>(strSql);
        }

        public bool IsExistTable(string tableName)
        {
            tableName = (tableName ?? string.Empty);
            return TableNames.Contains(tableName);
        }

        public bool IsExistColumn(string tableName, string columnName)
        {
            var strKey = (tableName + "@" + columnName);
            return DictColumnInfo.ContainsKey(strKey);
        }

        public string GetColumnComment(string tableName, string columnName)
        {
            Db.CheckTabStuct(tableName, columnName);
            ColumnInfo colInfo = null;
            var strKey = (tableName + "@" + columnName);
            DictColumnInfo.TryGetValue(strKey, out colInfo);
            return colInfo?.DeText;
        }

        public string GetTableComment(string tableName)
        {
            Db.CheckTabStuct(tableName);
            return TableComments[tableName];
        }

        public List<ColumnInfo> GetColumns(string tableName)
        {
            Db.CheckTabStuct(tableName);
            List<ColumnInfo> colInfos = null;
            TableColumnInfoDict.TryGetValue(tableName, out colInfos);
            return colInfos;
        }

        public bool SetTableComment(string tableName, string comment)
        {
            Db.CheckTabStuct(tableName);
            string upsert_sql = string.Empty;
            comment = (comment ?? string.Empty).Replace("'", "");
            try
            {
                upsert_sql = "ALTER TABLE " + tableName + " COMMENT='" + comment + "';";
                Db.ExecSql(upsert_sql);

                TableComments[tableName] = comment;

                var tabInfo = TableInfoDict[tableName];
                tabInfo.TabComment = comment;
                TableInfoDict[tableName] = tabInfo;

            }
            catch (Exception ex)
            {
                LogUtils.LogError("DB", Developer.SysDefault, ex, upsert_sql);
                return false;
            }
            return true;
        }

        public bool SetColumnComment(string tableName, string columnName, string comment)
        {
            Db.CheckTabStuct(tableName, columnName);

            string selsql = string.Empty;
            string upsert_sql = string.Empty;
            comment = (comment ?? string.Empty).Replace("'", "");

            try
            {
                var colInfo = this[tableName, columnName];
                string setSql = string.Empty;
                if (!colInfo.CanNull)
                {
                    setSql += " not null ";
                }
                if (!string.IsNullOrWhiteSpace(colInfo.DefaultVal))
                {
                    setSql += " default '" + colInfo.DefaultVal + "' ";
                }

                selsql = "USE INFORMATION_SCHEMA;SELECT COLUMN_TYPE,EXTRA FROM COLUMNS WHERE TABLE_NAME = '" + tableName + "' AND COLUMN_NAME = '" + columnName + "';";
                var dict = Db.GetFirstRow(selsql);
                if (colInfo.DefaultVal != null && colInfo.DefaultVal.Equals("CURRENT_TIMESTAMP", StringComparison.OrdinalIgnoreCase))
                {
                    upsert_sql = "USE `" + DBName + "`;ALTER TABLE `" + DBName + "`.`" + tableName + "` CHANGE `" + columnName + "` `" + columnName + "` TIMESTAMP DEFAULT CURRENT_TIMESTAMP " + dict["EXTRA"] + " COMMENT '" + comment + "'; ";
                }
                else
                {
                    string col_type = dict["COLUMN_TYPE"].ToString();
                    upsert_sql = "USE `" + DBName + "`;ALTER TABLE " + tableName + " MODIFY COLUMN " + columnName + " " + col_type + " " + setSql + " COMMENT '" + comment + "';";
                }
                Db.ExecSql(upsert_sql);

                List<ColumnInfo> lstColInfo = TableColumnInfoDict[tableName];

                NameValueCollection nvcColDesc = new NameValueCollection();
                lstColInfo.ForEach(t =>
                {
                    if (t.ColumnName.Equals(columnName))
                    {
                        t.DeText = comment;
                    }
                    nvcColDesc.Add(t.ColumnName, t.DeText);
                });

                TableColumnInfoDict.Remove(tableName);
                TableColumnInfoDict.Add(tableName, lstColInfo);

                TableColumnComments.Remove(tableName);
                TableColumnComments.Add(tableName, nvcColDesc);

                var strKey = (tableName + "@" + columnName);
                colInfo = DictColumnInfo[strKey];
                colInfo.DeText = comment;
                DictColumnInfo[strKey] = colInfo;
            }
            catch (Exception ex)
            {
                LogUtils.LogError("DB", Developer.SysDefault, ex, selsql, upsert_sql);
                return false;
            }
            return true;
        }

        public bool DropTable(string tableName)
        {
            Db.CheckTabStuct(tableName);

            string drop_sql = string.Empty;
            try
            {

                drop_sql = "drop table " + tableName;
                Db.ExecSql(drop_sql);

                this.TableComments.Remove(tableName);

                this.TableNames = TableComments.AllKeys.ToList();

                this.TableInfoDict.Remove(tableName);
                this.TableColumnInfoDict.Remove(tableName);
                this.TableColumnComments.Remove(tableName);

                var lstColName = TableColumnNameDict[tableName];

                foreach (var colName in lstColName)
                {
                    var strKey = (tableName + "@" + colName);
                    this.DictColumnInfo.Remove(strKey);
                }

                this.TableColumnNameDict.Remove(tableName);

            }
            catch (Exception ex)
            {
                LogUtils.LogError("DB", Developer.SysDefault, ex, drop_sql);
                return false;
            }
            return true;
        }

        public bool DropColumn(string tableName, string columnName)
        {
            Db.CheckTabStuct(tableName, columnName);

            var strKey = (tableName + "@" + columnName);

            string drop_sql = "alter table {0} drop column {1}";
            try
            {
               
                drop_sql = string.Format(drop_sql, tableName, columnName);
                Db.ExecSql(drop_sql);

                this.DictColumnInfo.Remove(strKey);

                var nvc = TableColumnComments[tableName];
                nvc.Remove(columnName);
                TableColumnNameDict[tableName] = nvc.AllKeys.ToList();

                var lstColInfo = TableColumnInfoDict[tableName];
                ColumnInfo curColInfo = null;
                lstColInfo.ForEach(t =>
                {
                    if (t.ColumnName.Equals(columnName))
                    {
                        curColInfo = t;

                        //tabInfo 对应的 主键类型和主键列 也需要 跟着修改。
                        if (curColInfo.IsPK)
                        {
                            var tabInfo = TableInfoDict[tableName];
                            tabInfo.PriKeyType = PrimaryKeyType.UNKNOWN;
                            tabInfo.PriKeyColName = null;
                            TableInfoDict[tableName] = tabInfo;
                        }
                        return;
                    }
                });
                lstColInfo.Remove(curColInfo);
                TableColumnInfoDict[tableName] = lstColInfo;

            }
            catch (Exception ex)
            {
                LogUtils.LogError("DB", Developer.SysDefault, ex, drop_sql);
                return false;
            }
            return true;
        }

    }
}
