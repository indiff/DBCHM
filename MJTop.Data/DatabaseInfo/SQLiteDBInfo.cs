using MJTop.Data.SPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTop.Data.DatabaseInfo
{
    public class SQLiteDBInfo : IDBInfo
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
        public SQLiteDBInfo(DB db)
        {
            this.Db = db;
            Refresh();
            this.Tools = new Tool(db, this);
        }

        public string DBName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension((Db.ConnectionStringBuilder as System.Data.SQLite.SQLiteConnectionStringBuilder).DataSource); }
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

        public List<string> DBNames { get { return DBName.TransList(); } }

        public ColumnInfo this[string tableName, string columnName]
        {
            get
            {
                ColumnInfo colInfo;
                var strKey = (tableName + "@" + columnName).ToLower();
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

            string strSql = "SELECT name FROM sqlite_master WHERE type='table' order by name";
            string viewSql = "SELECT name,sql FROM sqlite_master WHERE type='view' order by name asc";
            string procSql = string.Empty; //Sqlite 没有存储过程功能！
            try
            {
                this.TableComments = Db.GetDataTable(strSql).MapperNameValues("Name", "Name");

                //this.Views = Db.ReadNameValues(viewSql);

                this.Procs = new NameValueCollection();

                if (this.TableComments != null && this.TableComments.Count > 0)
                {
                    this.TableNames = TableComments.AllKeys.ToList();
                    
                    var dbConn = Db.CreateConn();
                    dbConn.Open();
                    foreach (string tableName in TableNames)
                    {
                        TableInfo tabInfo = new TableInfo();
                        tabInfo.TableName = tableName;
                        tabInfo.TabComment = TableComments[tableName];
                        
                        List<ColumnInfo> lstColInfo = new List<ColumnInfo>();
                        List<string> lstColName = new List<string>();
                        NameValueCollection nvcColDeText = new NameValueCollection();

                        DataRow[] columns = dbConn.GetSchema("COLUMNS").Select("TABLE_NAME='" + tableName + "'");
                        foreach (DataRow dr in columns)
                        {
                            ColumnInfo colInfo = new ColumnInfo();
                            colInfo.Colorder = dr["ORDINAL_POSITION"].ToString().ChangeType<int>(0);
                            colInfo.ColumnName = dr["COLUMN_NAME"].ToString();
                            colInfo.Length = dr["CHARACTER_MAXIMUM_LENGTH"].ToString().ChangeType<int?>((int?)null);
                            //colInfo.Preci = dr["NUMERIC_PRECISION"].ToString().ChangeType<int?>(null);
                            colInfo.Scale = dr["NUMERIC_SCALE"].ToString().ChangeType<int?>((int?)null);
                            colInfo.IsPK = dr["PRIMARY_KEY"].ToString().ToLower() == "true" ? true : false;
                            colInfo.CanNull = dr["IS_NULLABLE"].ToString().ToLower() == "true" ? true : false;
                            colInfo.DefaultVal = dr["COLUMN_DEFAULT"].ToString();
                            colInfo.TypeName = dr["DATA_TYPE"].ToString();
                            if (colInfo.IsPK && string.Compare(colInfo.TypeName, "integer", true) == 0)
                            {
                                colInfo.IsIdentity = true;
                            }

                            if (colInfo.TypeName == "integer" || colInfo.TypeName == "bigint")
                            {
                                colInfo.Scale = null;
                            }

                            lstColInfo.Add(colInfo);
                            lstColName.Add(colInfo.ColumnName);
                            nvcColDeText.Add(colInfo.ColumnName, colInfo.ColumnName);
                            
                            var strKey = (tableName + "@" + colInfo.ColumnName).ToLower();
                            DictColumnInfo.Add(strKey, colInfo);

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


                            Global.Dict_Sqlite_DbType.TryGetValue(colInfo.TypeName, out DbType type);
                            colInfo.DbType = type;
                        }
                        tabInfo.Colnumns = lstColInfo;
                        this.TableInfoDict.Add(tableName, tabInfo);
                        this.TableColumnNameDict.Add(tableName, lstColName);
                        this.TableColumnInfoDict.Add(tableName, lstColInfo);
                        this.TableColumnComments.Add(tableName, nvcColDeText);

                    }
                    dbConn.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogError("DB", Developer.SysDefault, ex);
                return false;
            }
            return this.TableComments.Count == this.TableInfoDict.Count;
        }

        #region Sqlite获取列信息
        //private List<ColumnInfo> SqliteColInfo(string tableName)
        //{
        //    List<ColumnInfo> lstCols = new List<ColumnInfo>();
        //    var dbConn = Db.CreateConn();
        //    dbConn.Open();
        //    DataRow[] columns = dbConn.GetSchema("COLUMNS").Select("TABLE_NAME='" + tableName + "'");
        //    foreach (DataRow dr in columns)
        //    {
        //        ColumnInfo colInfo = new ColumnInfo();
        //        colInfo.Colorder = dr["ORDINAL_POSITION"].ToString().ChangeType<int>(0);
        //        colInfo.ColumnName = dr["COLUMN_NAME"].ToString();
        //        colInfo.Length = dr["CHARACTER_MAXIMUM_LENGTH"].ToString().ChangeType<int?>((int?)null);
        //        //colInfo.Preci = dr["NUMERIC_PRECISION"].ToString().ChangeType<int?>(null);
        //        colInfo.Scale = dr["NUMERIC_SCALE"].ToString().ChangeType<int?>((int?)null);
        //        colInfo.IsPK = dr["PRIMARY_KEY"].ToString().ToLower() == "true" ? true : false;
        //        colInfo.CanNull = dr["IS_NULLABLE"].ToString().ToLower() == "true" ? true : false;
        //        colInfo.DefaultVal = dr["COLUMN_DEFAULT"].ToString();
        //        colInfo.TypeName = dr["DATA_TYPE"].ToString();
        //        if (colInfo.IsPK && string.Compare(colInfo.TypeName, "integer", true) == 0)
        //        {
        //            colInfo.IsIdentity = true;
        //        }
        //        lstCols.Add(colInfo);
        //    }
        //    dbConn.Close();
        //    return lstCols;
        //}
        #endregion


        public Dictionary<string, DateTime> GetTableStruct_Modify()
        {
            throw new Exception("SQLite不支持 记录表结构更改时间！");
        }


        public bool IsExistTable(string tableName)
        {
            return TableNames.Contains(tableName, StringComparer.OrdinalIgnoreCase);
        }

        public bool IsExistColumn(string tableName, string columnName)
        {
            var strKey = (tableName + "@" + columnName);
            return DictColumnInfo.ContainsKey(strKey);
        }

        public string GetColumnComment(string tableName, string columnName)
        {
            throw new Exception("Sqlite不支持表列描述！");
        }

        public string GetTableComment(string tableName)
        {
            throw new Exception("Sqlite不支持表列描述！");
        }

        public List<ColumnInfo> GetColumns(string tableName)
        {
            List<ColumnInfo> colInfos = null;
            TableColumnInfoDict.TryGetValue(tableName, out colInfos);
            return colInfos;
        }

        public bool SetTableComment(string tableName, string comment)
        {
            throw new Exception("Sqlite不支持表列描述！");
        }

        public bool SetColumnComment(string tableName, string columnName, string comment)
        {
            throw new Exception("Sqlite不支持表列描述！");
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
                    var strKey = (tableName + "@" + colName).ToLower();
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

            if (Db.Single<int>(string.Format("select count(1) from {0}", tableName), 0) > 0)
            {
                throw new Exception("Sqlite不支持对已有数据的表的结构进行更改！");
            }

            var strKey = (tableName + "@" + columnName);
            
            string ctor_sql = string.Empty;
            string drop_sql = string.Empty;
            string rename_sql = string.Empty;
            try
            {

                //Sqlite 不支持直接删除列 的 处理方式,这里先创查询建 对应表的结构（排除要删除的列），然后删除原来的表，最后重命名新表的 表名称。
                //该方法适用于 表中 没有数据的 情况。
                {
                    var lstColName = this[tableName].Where(t => !t.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                    string temp_Name = tableName + DateTime.Now.ToString("yyyMMddhhmmssfff");
                    ctor_sql = "create table {0} as select {1} from {2}";
                    ctor_sql = string.Format(ctor_sql, temp_Name, string.Join(",", lstColName), tableName);
                    drop_sql = string.Format("drop table if exists {0}", tableName);
                    rename_sql = string.Format("alter table {0} rename to {1}", temp_Name, tableName);
                    Db.ExecSqlTran(ctor_sql, drop_sql, rename_sql);


                    this.DictColumnInfo.Remove(strKey);

                    var nvc = TableColumnComments[tableName];
                    nvc.Remove(columnName);
                    TableColumnNameDict[tableName] = nvc.AllKeys.ToList();

                    var lstColInfo = TableColumnInfoDict[tableName];
                    ColumnInfo curColInfo = null;
                    lstColInfo.ForEach(t =>
                    {
                        if (t.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
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
                
            }
            catch (Exception ex)
            {
                LogUtils.LogError("DB", Developer.SysDefault, ex, ctor_sql, drop_sql, rename_sql);
                return false;
            }
            return true;
        }
    }
}
