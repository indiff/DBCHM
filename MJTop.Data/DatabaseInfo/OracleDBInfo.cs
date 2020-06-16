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
    public class OracleDBInfo : IDBInfo
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

        public OracleDBInfo(DB db)
        {
            this.Db = db;
            Refresh();
            this.Tools = new Tool(db, this);
        }

        public string DBName
        {
            get
            {
                //127.0.0.1:1521/CTMS
                string source = (Db.ConnectionStringBuilder as Oracle.ManagedDataAccess.Client.OracleConnectionStringBuilder).DataSource;
                return Regex.Replace(source, @"(.+/)(.+)", "$2");
            }
        }

        public string Version
        {
            get;
            private set;
        }

        //Oracle Database 11g Enterprise Edition Release 11.2.0.1.0 - 64bit Production  =>  11.2
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

        public List<string> TableNames { get; private set; } = new List<string>();
        
        public IgCaseDictionary<TableInfo> TableInfoDict { get; private set; }

        public IgCaseDictionary<List<string>> TableColumnNameDict { get; private set; }

        public IgCaseDictionary<List<ColumnInfo>> TableColumnInfoDict { get; private set; }

        public IgCaseDictionary<NameValueCollection> TableColumnComments { get; private set; }

        private IgCaseDictionary<ColumnInfo> DictColumnInfo { get; set; }

        private IgCaseDictionary<string> Dict_Table_Sequence { get; set; } = new IgCaseDictionary<string>(KeyCase.Upper);

        public NameValueCollection Views { get; private set; }

        public NameValueCollection Procs { get; private set; }

        public List<string> DBNames { get { return DBName.TransList(); } }
        
        public List<string> Sequences { get; set; } = new List<string>();

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


        public string IdentitySeqName(string tableName)
        {
            string seqName;
            if (Dict_Table_Sequence.TryGetValue(tableName, out seqName))
            {
                return seqName;
            }
            return string.Empty;
        }

        public bool Refresh()
        {
            this.DictColumnInfo = new IgCaseDictionary<ColumnInfo>(KeyCase.Upper);
            this.TableInfoDict = new IgCaseDictionary<TableInfo>(KeyCase.Upper);
            this.TableColumnNameDict = new IgCaseDictionary<List<string>>(KeyCase.Upper);
            this.TableColumnInfoDict = new IgCaseDictionary<List<ColumnInfo>>(KeyCase.Upper);
            this.TableColumnComments = new IgCaseDictionary<NameValueCollection>(KeyCase.Upper);

            string sequence_Sql = "select sequence_name from user_sequences";
            string strSql = "Select table_Name As Name,Comments As Value From User_Tab_Comments Where table_Type='TABLE' Order By table_Name Asc";

            string viewSql = "select view_name,text from user_views order by view_name asc";
            //Oracle 11g 推出 LISTAGG 函数
            string procSql = "select * from (SELECT name,LISTAGG(text,' ') WITHIN  group (order by line asc) text FROM user_source  group by name ) order by name asc";
            
            try
            {
                //查询Oracle的所有序列
                this.Sequences = Db.ReadList<string>(sequence_Sql);

                this.TableComments = Db.ReadNameValues(strSql);

                this.Version = Db.Scalar("select * from v$version where ROWNUM = 1", string.Empty);

                this.Views = Db.ReadNameValues(viewSql);

                if (this.VersionNumber >= 11)
                {
                    this.Procs = Db.ReadNameValues(procSql);
                }
                else
                {
                    this.Procs = new NameValueCollection();
                }

                if (this.TableComments != null && this.TableComments.Count > 0)
                {
                    this.TableNames = this.TableComments.AllKeys.ToList();

                    List<Task> lstTask = new List<Task>();

                    foreach (string tableName in this.TableNames)
                    {
                        Task task = Task.Run(() =>
                        {
                            TableInfo tabInfo = new TableInfo();
                            tabInfo.TableName = tableName;
                            tabInfo.TabComment = this.TableComments[tableName];

                            /** 该语句，包含某列是否自增列，查询慢 **/

                            strSql = @"select a.COLUMN_ID As Colorder,a.COLUMN_NAME As ColumnName,a.DATA_TYPE As TypeName,b.comments As DeText,(Case When a.DATA_TYPE='NUMBER' Then a.DATA_PRECISION When a.DATA_TYPE='NVARCHAR2' Then a.DATA_LENGTH/2 Else a.DATA_LENGTH End )As Length,a.DATA_SCALE As Scale,
	(Case When (select Count(1)  from user_cons_columns aa, user_constraints bb where aa.constraint_name = bb.constraint_name 
	 and bb.constraint_type = 'P' and aa.table_name = '{0}' And aa.column_name=a.COLUMN_NAME)>0 Then 1 Else 0 End
	 ) As IsPK,(
			 case when (select count(1) from user_triggers tri INNER JOIN user_source src on tri.trigger_Name=src.Name 
				where (triggering_Event='INSERT' and table_name='{0}')
			and regexp_like(text,	concat(concat('nextval\s+into\s*?:\s*?new\s*?\.\s*?',a.COLUMN_NAME),'\s+?'),'i'))>0 
			then 1 else 0 end 
	) As IsIdentity, 
		Case a.NULLABLE  When 'Y' Then 1 Else 0 End As CanNull,
		a.data_default As DefaultVal from user_tab_columns a Inner Join user_col_comments b On a.TABLE_NAME=b.table_name 
	Where b.COLUMN_NAME= a.COLUMN_NAME   and a.Table_Name='{0}'  order by a.column_ID Asc";
                            try
                            {
                                //if (Db.DBType == DBType.OracleDDTek)
                                //{
                                //    strSql = strSql.Replace("'{0}'", "?");
                                //    tabInfo.Colnumns = Db.GetDataTable(strSql, new { t1 = tableName, t2 = tableName, t3 = tableName }).ConvertToListObject<ColumnInfo>();

                                //}
                                //else
                                {
                                    strSql = strSql.Replace("'{0}'", ":" + tableName);
                                    tabInfo.Colnumns = Db.GetDataTable(strSql, new { tableName = tableName }).ConvertToListObject<ColumnInfo>();
                                }

                                List<string> lstColName = new List<string>();
                                NameValueCollection nvcColDeText = new NameValueCollection();
                                foreach (ColumnInfo colInfo in tabInfo.Colnumns)
                                {
                                    lstColName.Add(colInfo.ColumnName);
                                    nvcColDeText.Add(colInfo.ColumnName, colInfo.DeText);

                                    var strKey = (tableName + "@" + colInfo.ColumnName);
                                    this.DictColumnInfo.Add(strKey, colInfo);

                                    //自增的列，需要查询序列名称
                                    if (colInfo.IsIdentity)
                                    {
                                        AddColSeq(tableName, colInfo.ColumnName);
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

                                    Global.Dict_Oracle_DbType.TryGetValue(colInfo.TypeName, out DbType type);
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

        private void AddColSeq(string tableName,string colName)
        {
            tableName = (tableName ?? string.Empty);
            colName = (colName ?? string.Empty);
            string strSql = string.Empty;
            if (Sequences != null && Sequences.Count > 0)
            {
                foreach (string  seqName in Sequences)
                {
                    strSql = @"select count(1) from user_triggers tri INNER JOIN user_source src on tri.trigger_Name=src.Name where (triggering_Event='INSERT' and table_name='" + tableName + "') and regexp_like(text,concat(concat('" + seqName + @"\s*?\.\s*?nextval\s+into\s*?:\s*?new\s*?\.\s*?','" + colName + @"'),'\s+?'),'i')";
                    int res = Db.Single<int>(strSql,0);
                    if (res > 0)
                    {
                        Dict_Table_Sequence[tableName] = seqName;
                        break;
                    }
                }
            }
        }

        public Dictionary<string, DateTime> GetTableStruct_Modify()
        {
            string strSql = "select object_name as name ,last_ddl_time as modify_date from user_objects Where object_Type='TABLE' Order By last_ddl_time Desc";
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

        /// <summary>
        /// 如果表名 或 列名 小写 则加双引号
        /// </summary>
        /// <param name="name">表名或列名</param>
        /// <returns></returns>
        private string FormatName(string name)
        {
            if (Regex.IsMatch(name, ".*?[a-z].*?", RegexOptions.Compiled))
            {
                return "\"" + name + "\"";
            }
            return name;
        }

        public bool SetTableComment(string tableName, string comment)
        {
            Db.CheckTabStuct(tableName);

            //tableName = (tableName ?? string.Empty);
           
            string upsert_sql = string.Empty;
            comment = (comment ?? string.Empty).Replace("'", "");
            try
            {
                upsert_sql = "comment on table \"" + tableName + "\" is '" + comment + "'";
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

            //tableName = (tableName ?? string.Empty);
            //columnName = (columnName ?? string.Empty);

            string upsert_sql = string.Empty;
            comment = (comment ?? string.Empty).Replace("'", "");
            try
            {
                upsert_sql = "comment on column \"" + tableName + "\".\"" + columnName + "\" is '" + comment + "'";
                Db.ExecSql(upsert_sql);

                List<ColumnInfo> lstColInfo = TableColumnInfoDict[tableName];

                NameValueCollection nvcColDesc = new NameValueCollection();
                lstColInfo.ForEach(t =>
                {
                    if (t.ColumnName.Equals(columnName,StringComparison.OrdinalIgnoreCase))
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

            tableName = (tableName ?? string.Empty);
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

            tableName = (tableName ?? string.Empty);
            columnName = (columnName ?? string.Empty);

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
