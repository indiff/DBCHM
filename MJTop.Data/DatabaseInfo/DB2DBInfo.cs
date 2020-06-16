using IBM.Data.DB2;
using MJTop.Data.SPI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MJTop.Data.DatabaseInfo
{
    public class DB2DBInfo : IDBInfo
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

        public DB2DBInfo(DB db)
        {
            this.Db = db;
            Refresh();
            this.Tools = new Tool(db, this);
        }
        public string DBName
        {
            get { return (Db.ConnectionStringBuilder as DB2ConnectionStringBuilder).Database; }
        }

        public string Version
        {
            get;
            private set;
        }

        // 8.0.19 => 8.0
        public double VersionNumber
        {
            get
            {
                var mat = Regex.Match(Version, @"\D*(\d{1,}\.\d{1,})\D*", RegexOptions.Compiled);
                double.TryParse(mat?.Groups[1]?.Value, out var res);
                return res;
            }
        }

        public NameValueCollection TableComments { get; private set; } = new NameValueCollection();

        private NameValueCollection TableSchemas { get; set; } = new NameValueCollection();

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

        private DB2ConnectionStringBuilder ConnBuilder { get { return Db.ConnectionStringBuilder as DB2ConnectionStringBuilder; } }

        public bool Refresh()
        {
            this.DictColumnInfo = new IgCaseDictionary<ColumnInfo>(KeyCase.Upper);
            this.TableInfoDict = new IgCaseDictionary<TableInfo>(KeyCase.Upper);
            this.TableColumnNameDict = new IgCaseDictionary<List<string>>(KeyCase.Upper);
            this.TableColumnInfoDict = new IgCaseDictionary<List<ColumnInfo>>(KeyCase.Upper);
            this.TableColumnComments = new IgCaseDictionary<NameValueCollection>(KeyCase.Upper);


            string dbSql = string.Empty;
            string strSql = string.Format("select tabschema,tabname,remarks from syscat.tables where type='T' and OWNER='{0}' order BY tabschema asc, tabname ASC", ConnBuilder.UserID.ToUpper());

            string viewSql = string.Format("select  viewname,text  from syscat.views where OWNER='{0}' order by viewname asc", ConnBuilder.UserID.ToUpper());
            string procSql = string.Format("select procname,text from syscat.procedures where DEFINER='{0}' order by procname asc", ConnBuilder.UserID.ToUpper());

            try
            {
                this.DBNames = new List<string>() { this.DBName };

                this.Version = Db.Scalar("SELECT SERVICE_LEVEL FROM SYSIBMADM.ENV_INST_INFO", string.Empty);

                var data = Db.GetDataTable(strSql);
                foreach (DataRow dr in data.Rows)
                {
                    this.TableComments[dr["tabname"].ToString()] = dr["remarks"].ToString();
                    this.TableSchemas[dr["tabname"].ToString()] = dr["tabschema"].ToString();
                }

                this.Views = Db.ReadNameValues(viewSql);

                this.Procs = Db.ReadNameValues(procSql);

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
                            List<string> lstColName = new List<string>();
                            NameValueCollection nvcColDeText = new NameValueCollection();
                            strSql = @"select (COLNO+1) as Colorder,COLNAME ColumnName,TypeName,
(case when TypeName = 'INTEGER' or TypeName = 'BIGINT'or TypeName='SMALLINT' then  null else length end ) as length,(case when TypeName='NUMBER' or TypeName='DOUBLE' or TypeName='DECIMAL' or TypeName='FLOAT' or TypeName='REAL' then Scale else null end) as Scale,
(case when Identity='Y' then 1 else 0 end) as IsIdentity,(case when KEYSEQ=1 then 1 else 0 end) as IsPK,
(case when NULLS='N' then 0 else 1 end) as CanNull,default as DefaultVal,remarks as DeText from SYSCAT.COLUMNS where TABSCHEMA=? and TABNAME=? order by COLNO asc";

                            try
                            {
                                tabInfo.Colnumns = Db.GetDataTable(strSql, new { t = TableSchemas[tableName.ToUpper()], t1 = tableName.ToUpper() }).ConvertToListObject<ColumnInfo>();
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

                                    Global.Dict_DB2_DbType.TryGetValue(colInfo.TypeName, out DbType type);
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
        
        public Dictionary<string, DateTime> GetTableStruct_Modify()
        {
            string strSql = string.Format("select tabname,alter_time from syscat.tables where type='T' and tabschema='{0}' order by alter_time desc", ConnBuilder.UserID);
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
            tableName = (tableName ?? string.Empty).ToUpper();
            string upsert_sql = string.Empty;
            comment = (comment ?? string.Empty).Replace("'", "");
            try
            {
                upsert_sql = string.Format("comment on table \"{0}\".\"{1}\" is '{2}' ", TableSchemas[tableName], tableName, comment);
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
            tableName = (tableName ?? string.Empty).ToUpper();
            columnName = (columnName ?? string.Empty).ToUpper();
            string upsert_sql = string.Empty;
            comment = (comment ?? string.Empty).Replace("'", "");
            try
            {
                upsert_sql = string.Format("comment on column \"{0}\".\"{1}\".\"{2}\" is '{3}'", TableSchemas[tableName], tableName, columnName, comment);
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

                var strKey = (tableName + "@" + columnName).ToUpper();
                ColumnInfo colInfo = DictColumnInfo[strKey];
                colInfo.DeText = comment;
                DictColumnInfo[strKey] = colInfo;

            }
            catch (Exception ex)
            {
                LogUtils.LogError("DB", Developer.SysDefault, ex, upsert_sql);
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
            tableName = (tableName ?? string.Empty).ToUpper();
            columnName = (columnName ?? string.Empty).ToUpper();

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
