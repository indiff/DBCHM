using MJTop.Data.SPI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MJTop.Data.DatabaseInfo
{
    public class SqlServerDBInfo : IDBInfo
    {
        internal DB Db { get; set; }

        /// <summary>
        /// 数据库工具
        /// </summary>
        public Tool Tools
        {
            get;
            private set;
        }

        public SqlServerDBInfo(DB db)
        {
            this.Db = db;
            Refresh();
            this.Tools = new Tool(db, this);
        }

        public string DBName
        {
            get { return (Db.ConnectionStringBuilder as System.Data.SqlClient.SqlConnectionStringBuilder).InitialCatalog; }
        }

        public string Version
        {
            get;
            private set;
        }

        //Microsoft SQL Server 2016 (RTM) - 13.0.1601.5 (X64)  =>  2016
        public double VersionNumber
        {
            get
            {
                var mat = Regex.Match(Version, @"\D*(\d{4})\D*", RegexOptions.Compiled);
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

        private IgCaseDictionary<ColumnInfo> DictColumnInfo { get; set; } = new IgCaseDictionary<ColumnInfo>();
        
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
            this.TableNames = new List<string>();
            this.TableInfoDict = new IgCaseDictionary<TableInfo>();
            this.TableColumnNameDict = new IgCaseDictionary<List<string>>();
            this.TableColumnInfoDict = new IgCaseDictionary<List<ColumnInfo>>();
            this.TableColumnComments = new IgCaseDictionary<NameValueCollection>();

            string dbSql = "select name from sys.sysdatabases Order By name asc";
            string strSql = "SELECT * FROM (SELECT '[' + (Select Top 1 c.name From sys.schemas c Where c.schema_id = a.schema_id) + '].[' + a.Name + ']' as Name,(SELECT TOP 1 Value FROM sys.extended_properties b WHERE b.major_id=a.object_id and b.minor_id=0) AS value,(Select top 1 c.name From sys.schemas c Where c.schema_id = a.schema_id) scName From sys.objects a WHERE a.type = 'U' AND a.name <> 'sysdiagrams' AND a.name <> 'dtproperties' )K WHERE K.scName <> 'cdc' ORDER BY K.name ASC;";

            string viewSql = "SELECT TABLE_NAME,VIEW_DEFINITION FROM INFORMATION_SCHEMA.VIEWS Order By TABLE_NAME asc";
            string procSql = "select name,[definition] from sys.objects a Left Join sys.sql_modules b On a.[object_id]=b.[object_id] Where a.type='P' And a.is_ms_shipped=0 And b.execute_as_principal_id Is Null And name !='sp_upgraddiagrams' Order By a.name asc";
            try
            {
                this.DBNames = Db.ReadList<string>(dbSql);

                this.Version = Db.Scalar("select @@version", string.Empty)?.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)?[0];

                var data = Db.GetDataTable(strSql);

                var dictGp = new Dictionary<string, List<string>>();

                foreach (DataRow dr in data.Rows)
                {
                    this.TableComments[dr["name"].ToString()] = dr["value"].ToString();
                    this.TableSchemas[dr["name"].ToString()] = dr["scname"].ToString();

                    dictGp.AddRange(dr["scname"].ToString(), dr["name"].ToString());
                }

                foreach (var item in dictGp)
                {
                    this.TableNames.AddRange(item.Value.OrderBy(t => t));
                }

                this.Views = Db.ReadNameValues(viewSql);
                this.Procs = Db.ReadNameValues(procSql);

                if (this.TableComments != null && this.TableComments.Count > 0)
                {
                    List<Task> lstTask = new List<Task>();
                    foreach (var tableName in this.TableNames)
                    {
                        Task task = Task.Run(() =>
                        {
                            TableInfo tabInfo = new TableInfo();
                            tabInfo.TableName = tableName;
                            tabInfo.TabComment = TableComments[tableName];

                            strSql = @"select s.* FROM (SELECT a.colorder Colorder,a.name ColumnName,b.name TypeName,row_number() over (partition by a.name order by b.name) as group_idx,(case when (SELECT count(*) FROM sysobjects  WHERE (name in (SELECT name FROM sysindexes  WHERE (id = a.id) AND (indid in  (SELECT indid FROM sysindexkeys  WHERE (id = a.id) AND (colid in  (SELECT colid FROM syscolumns WHERE (id = a.id) AND (name = a.name)))))))  AND (xtype = 'PK'))>0 then 1 else 0 end) IsPK,(case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then 1 else 0 end) IsIdentity,  CASE When b.name ='uniqueidentifier' Then 36  WHEN (charindex('int',b.name)>0) OR (charindex('time',b.name)>0) THEN NULL ELSE  COLUMNPROPERTY(a.id,a.name,'PRECISION') end as [Length], CASE WHEN ((charindex('int',b.name)>0) OR (charindex('time',b.name)>0)) THEN NULL ELSE isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),null) end as Scale,(case when a.isnullable=1 then 1 else 0 end) CanNull,Replace(Replace(IsNull(e.text,''),'(',''),')','') DefaultVal,isnull(g.[value], ' ') AS DeText FROM  syscolumns a left join systypes b on a.xtype=b.xusertype inner join sysobjects d on a.id=d.id and d.xtype='U' and d.name<>'dtproperties' left join syscomments e on a.cdefault=e.id left join sys.extended_properties g on a.id=g.major_id AND a.colid=g.minor_id  And g.class=1 left join sys.extended_properties f on d.id=f.class and f.minor_id=0 left join sys.schemas c on d.uid = c.schema_id where b.name is not NULL and '[' + c.name + '].[' + d.name + ']'=@tableName ) s WHERE s.group_idx = 1 order by s.colorder";

                            try
                            {
                                tabInfo.Colnumns = Db.GetDataTable(strSql, new { tableName = tableName }).ConvertToListObject<ColumnInfo>();
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

                                    Global.Dict_SqlServer_DbType.TryGetValue(colInfo.TypeName, out DbType type);
                                    colInfo.DbType = type;
                                }
                                this.TableInfoDict.Add(tableName, tabInfo);
                                this.TableColumnNameDict.Add(tableName, lstColName);
                                this.TableColumnInfoDict.Add(tableName, tabInfo.Colnumns);
                                this.TableColumnComments.Add(tableName, nvcColDeText);
                            }
                            catch (Exception ex)
                            {
                                LogUtils.LogError("DB", Developer.SysDefault, ex, "查询过程出现失败，账号的权限可能不足！！！");
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
                LogUtils.LogError("DB", Developer.SysDefault, ex, "查询过程出现失败，账号的权限可能不足！！！");
                return false;
            }
            return this.TableComments.Count == this.TableInfoDict.Count;
        }


        public Dictionary<string, DateTime> GetTableStruct_Modify()
        {
            string strSql = "select name,modify_date from sys.objects where type='U' and name <> 'dtproperties' and name <>'sysdiagrams'  order by modify_date desc";
            return Db.ReadDictionary<string, DateTime>(strSql);
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
            ColumnInfo colInfo = null;
            var strKey = (tableName + "@" + columnName);
            if (DictColumnInfo.TryGetValue(strKey, out colInfo))
            {
                return colInfo.DeText;
            }
            else
            {
                throw new ArgumentException(tableName + "/" + columnName + ",表的列不存在！");
            }
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
                upsert_sql = @" if exists (
                 SELECT case when a.colorder = 1 then d.name  else '' end as 表名,  case when a.colorder = 1 then isnull(f.value, '')  else '' end as 表说明
                FROM syscolumns a 
                       inner join sysobjects d 
                          on a.id = d.id 
                             and d.xtype = 'U' 
                             and d.name <> 'sys.extended_properties'
                       left join sys.extended_properties   f 
                         on a.id = f.major_id 
                            and f.minor_id = 0
                 where a.colorder = 1 and d.name<>'sysdiagrams'  and d.name='{0}' and f.value is not null
                 )
                 exec sp_updateextendedproperty N'MS_Description', N'{1}', N'schema', N'{2}', N'table', N'{0}', NULL, NULL
                 else
                exec sp_addextendedproperty N'MS_Description', N'{1}', N'schema', N'{2}', N'table', N'{0}', NULL, NULL";
                upsert_sql = string.Format(upsert_sql, tableName, comment, TableSchemas[tableName]);
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

            string upsert_sql = string.Empty;
            comment = (comment ?? string.Empty).Replace("'", "");
            try
            {
                upsert_sql = @"if exists (select * from   ::fn_listextendedproperty (NULL, 'schema', '{3}', 'table', '{0}', 'column', default) where objname = '{1}') EXEC sp_updateextendedproperty   'MS_Description','{2}','schema',{3},'table','{0}','column',{1} else EXEC sp_addextendedproperty @name=N'MS_Description' , @value=N'{2}' ,@level0type=N'SCHEMA', @level0name=N'{3}', @level1type=N'TABLE', @level1name=N'{0}', @level2type=N'COLUMN', @level2name=N'{1}' ";
                upsert_sql = string.Format(upsert_sql, tableName, columnName, comment, this.TableSchemas[tableName]);
                Db.ExecSql(upsert_sql);

                List<ColumnInfo> lstColInfo = TableColumnInfoDict[tableName];

                NameValueCollection nvcColDesc = new NameValueCollection();
                lstColInfo.ForEach(t =>
                {
                    if (t.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        t.DeText = comment;
                    }
                    nvcColDesc.Add(t.ColumnName, t.DeText);
                });

                TableColumnInfoDict.Remove(tableName);
                TableColumnInfoDict.Add(tableName, lstColInfo);

                TableColumnComments.Remove(tableName);
                TableColumnComments.Add(tableName, nvcColDesc);

                ColumnInfo colInfo = DictColumnInfo[tableName + "@" + columnName];
                colInfo.DeText = comment;
                DictColumnInfo[tableName + "@" + columnName] = colInfo;

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
            var strKey = (tableName + "@" + columnName);

            string drop_sql = string.Empty;
            try
            {
                drop_sql = "alter table {0} drop column {1}";
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
            catch (Exception ex)
            {
                LogUtils.LogError("DB", Developer.SysDefault, ex, drop_sql);
                return false;
            }
            return true;
        }


    }
}
