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
    public class PostgreSqlDBInfo : IDBInfo
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
        public PostgreSqlDBInfo(DB db)
        {
            this.Db = db;
            Refresh();
            this.Tools = new Tool(db, this);
        }

        public string DBName
        {
            get { return (Db.ConnectionStringBuilder as Npgsql.NpgsqlConnectionStringBuilder).Database; }
        }

        public string Version
        {
            get;
            private set;
        }

        // PostgreSQL 9.6.3, compiled by Visual C++ build 1800, 32-bit => 9.6
        public double VersionNumber
        {
            get
            {
                var mat = Regex.Match(Version, @"\D*(\d{1,}\.\d{1,})\D*", RegexOptions.Compiled);
                double.TryParse(mat?.Groups[1]?.Value, out var res);
                return res;
            }
        }

        public string User
        {
            get { return (Db.ConnectionStringBuilder as Npgsql.NpgsqlConnectionStringBuilder).Username; }
        }

        public NameValueCollection TableComments { get; private set; } = new NameValueCollection();

        private NameValueCollection TableSchemas { get; set; } = new NameValueCollection();

        public List<string> TableNames { get; private set; } = new List<string>();
        
        public IgCaseDictionary<TableInfo> TableInfoDict { get; private set; } 

        public IgCaseDictionary<List<string>> TableColumnNameDict { get; private set; } 

        public IgCaseDictionary<List<ColumnInfo>> TableColumnInfoDict { get; private set; } 

        public IgCaseDictionary<NameValueCollection> TableColumnComments { get; private set; } 

        private IgCaseDictionary<ColumnInfo> DictColumnInfo { get; set; } 

        private IgCaseDictionary<string> Dict_Table_Identity_Column { get; set; } = new IgCaseDictionary<string>();

        public NameValueCollection Views { get; private set; }

        public NameValueCollection Procs { get; private set; }

        public List<string> DBNames { get; set; } = new List<string>();

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

            string dbSql = "select datname from pg_database where  datistemplate=false  order by oid desc";
            string strSql = @"select a.*,cast(obj_description(relfilenode,'pg_class') as varchar) as value from (
select b.oid as schid, a.table_schema as scName,concat(a.table_schema,'.',a.table_name) as Name,a.table_name from information_schema.tables  a
left join pg_namespace b on b.nspname = a.table_schema
where a.table_schema not in ('pg_catalog','information_schema') and a.table_schema=:schema and a.table_type='BASE TABLE'
) a inner join pg_class b on a.table_name = b.relname and a.schid = b.relnamespace
order by name asc";

            string viewSql = "SELECT viewname,definition FROM pg_views where schemaname='public' order by viewname asc";
            string procSql = "select proname,prosrc from  pg_proc where pronamespace=(SELECT pg_namespace.oid FROM pg_namespace WHERE nspname = 'public') order by proname asc";

            try
            {
               
                this.DBNames = Db.ReadList<string>(dbSql);

                this.Version = Db.Scalar("select version()", string.Empty);

                var data = Db.GetDataTable(strSql, new { schema = User });

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

                            strSql = @"SELECT
	col.ordinal_position AS Colorder,
	col.table_schema AS SCHEMA_NAME,
	col.TABLE_NAME,
	col.COLUMN_NAME AS ColumnName,
	col.udt_name AS TypeName,
	col_description ( C.oid, col.ordinal_position ) AS DeText,	
	COALESCE(col.character_maximum_length, col.numeric_precision, -1) AS Length,
	col.numeric_scale AS Scale,
	(CASE col.is_identity WHEN 'YES' THEN 1 ELSE 0 END) AS IsIdentity,	
	CASE
		col.is_nullable 
		WHEN 'NO' THEN
		0 ELSE 1 
	END AS CanNull,
	col.column_default AS DefaultVal
FROM
	information_schema.COLUMNS AS col
	LEFT JOIN pg_namespace ns ON ns.nspname = col.table_schema
	LEFT JOIN pg_class C ON col.TABLE_NAME = C.relname 
	AND C.relnamespace = ns.oid
	LEFT JOIN pg_attrdef A ON C.oid = A.adrelid 
	AND col.ordinal_position = A.adnum
	LEFT JOIN pg_attribute b ON b.attrelid = C.oid 
	AND b.attname = col.
	COLUMN_NAME LEFT JOIN pg_type et ON et.oid = b.atttypid
	LEFT JOIN pg_collation coll ON coll.oid = b.attcollation
	LEFT JOIN pg_namespace colnsp ON coll.collnamespace = colnsp.oid
	LEFT JOIN pg_type bt ON et.typelem = bt.oid
	LEFT JOIN pg_namespace nbt ON bt.typnamespace = nbt.oid
WHERE
	col.table_schema = :schema 
	AND col.TABLE_NAME = :tableName
ORDER BY
	col.table_schema,
	col.TABLE_NAME,
	col.ordinal_position";
                            try
                            {
                                var arr = tableName.Split('.');
                                var scName = arr.FirstOrDefault();
                                var tabName = arr.LastOrDefault();
                                tabInfo.Colnumns = Db.GetDataTable(strSql, new { tableName = tabName, schema = scName }).ConvertToListObject<ColumnInfo>();
                                List<string> lstColName = new List<string>();
                                NameValueCollection nvcColDeText = new NameValueCollection();
                                foreach (ColumnInfo colInfo in tabInfo.Colnumns)
                                {
                                    lstColName.Add(colInfo.ColumnName);
                                    nvcColDeText.Add(colInfo.ColumnName, colInfo.DeText);

                                    var strKey = (tableName + "@" + colInfo.ColumnName);
                                    this.DictColumnInfo.Add(strKey, colInfo);
                                    if (colInfo.IsIdentity)
                                    {
                                        this.Dict_Table_Identity_Column[tableName] = colInfo.ColumnName;
                                    }

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

                                    Global.Dict_PostgreSql_DbType.TryGetValue(colInfo.TypeName, out DbType type);
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

        internal string IdentityColumnName(string tableName)
        {
            tableName = (tableName ?? string.Empty).ToLower();
            string colName;
            if (Dict_Table_Identity_Column.TryGetValue(tableName, out colName))
            {
                return colName;
            }
            return string.Empty;
        }

        public Dictionary<string, DateTime> GetTableStruct_Modify()
        {
            throw new Exception("暂未提供查询PostgreSql表结构修改时间的功能！");
        }

        public bool IsExistTable(string tableName)
        {
            return TableNames.Contains(tableName);
        }

        public bool IsExistColumn(string tableName, string columnName)
        {
            var strKey = (tableName + "@" + columnName).ToLower();
            return DictColumnInfo.ContainsKey(strKey);
        }

        public string GetColumnComment(string tableName, string columnName)
        {
            ColumnInfo colInfo = null;
            var strKey = (tableName + "@" + columnName).ToLower();
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
            tableName = (tableName ?? string.Empty).ToLower();
            List<ColumnInfo> colInfos = null;
            TableColumnInfoDict.TryGetValue(tableName, out colInfos);
            return colInfos;
        }

        public bool SetTableComment(string tableName, string comment)
        {
            Db.CheckTabStuct(tableName);

            tableName = TableInfoDict[tableName].TableName;
           
            string upsert_sql = string.Empty;
            comment = (comment ?? string.Empty).Replace("'", "");
            try
            {
                var tabName = tableName.Split('.').LastOrDefault();
                //切换schema，更新表描述
                upsert_sql = "set search_path to " + TableSchemas[tableName] + ";comment on table \"" + tabName + "\" is '" + comment + "'";
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

            tableName = TableInfoDict[tableName].TableName;
            columnName = this[tableName, columnName].ColumnName;
            
            string upsert_sql = string.Empty;
            comment = (comment ?? string.Empty).Replace("'", "");
            try
            {
                var tabName = tableName.Split('.').LastOrDefault();
                //切换schema，更新列描述
                upsert_sql = "set search_path to " + TableSchemas[tableName] + ";comment on column \"" + tabName + "\".\"" + columnName + "\" is '" + comment + "'";
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
            tableName = (tableName ?? string.Empty).ToLower();
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

            tableName = (tableName ?? string.Empty).ToLower();
            columnName = (columnName ?? string.Empty).ToLower();

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
