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
                if (Db.ConnectionStringBuilder is Oracle.ManagedDataAccess.Client.OracleConnectionStringBuilder)
                {
                    //127.0.0.1:1521/CTMS
                    string source = (Db.ConnectionStringBuilder as Oracle.ManagedDataAccess.Client.OracleConnectionStringBuilder).DataSource;
                    return Regex.Replace(source, @"(.+/)(.+)", "$2");
                }
                else
                {
                    return (Db.ConnectionStringBuilder as DDTek.Oracle.OracleConnectionStringBuilder).ServiceName;
                }
            }
        }

        public string User
        {
            get
            {
                if (Db.ConnectionStringBuilder is Oracle.ManagedDataAccess.Client.OracleConnectionStringBuilder)
                {
                    return (Db.ConnectionStringBuilder as Oracle.ManagedDataAccess.Client.OracleConnectionStringBuilder).UserID?.ToUpper();
                }
                else
                {
                    return (Db.ConnectionStringBuilder as DDTek.Oracle.OracleConnectionStringBuilder).UserID?.ToUpper();
                }
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

        public Dictionary<string, int> TableRows { get; private set; } = new Dictionary<string, int>();

        public List<string> TableNames { get; private set; } = new List<string>();

        public IgCaseDictionary<TableInfo> TableInfoDict { get; private set; }

        public IgCaseDictionary<List<string>> TableColumnNameDict { get; private set; }

        public IgCaseDictionary<List<ColumnInfo>> TableColumnInfoDict { get; private set; }

        public IgCaseDictionary<NameValueCollection> TableColumnComments { get; private set; }

        private IgCaseDictionary<ColumnInfo> DictColumnInfo { get; set; }

        private IgCaseDictionary<string> Dict_Table_Sequence { get; set; } = new IgCaseDictionary<string>(KeyCase.Upper);

        public NameValueCollection Views { get; private set; }

        public NameValueCollection Procs { get; private set; }

        public List<string> DBNames
        { get { return DBName.TransList(); } }

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

            string sequence_Sql = string.Format("SELECT SEQUENCE_NAME FROM ALL_SEQUENCES WHERE SEQUENCE_OWNER = '{0}' ORDER BY SEQUENCE_NAME", User);

            // 读取表名称和注释信息 // 去除空格  // T.NUM_ROWS DESC, 按照行数降序
            string tableCommentSql = string.Format("SELECT T.TABLE_NAME as Name, TRIM(TC.COMMENTS) AS Value  " +
                                                   "FROM SYS.ALL_ALL_TABLES T, SYS.ALL_TAB_COMMENTS TC " +
                                                   "WHERE T.IOT_NAME IS NULL  " +
                                                   "AND T.NESTED = 'NO'  " +
                                                   "AND T.SECONDARY = 'N'  " +
                                                   "AND NOT EXISTS ( SELECT 1 FROM SYS.ALL_MVIEWS MV WHERE MV.OWNER = T.OWNER AND MV.MVIEW_NAME = T.TABLE_NAME ) " +
                                                   "AND TC.OWNER ( + ) = T.OWNER  " +
                                                   "AND TC.TABLE_NAME ( + ) = T.TABLE_NAME  " +
                                                   "AND T.OWNER = '{0}' ORDER BY T.NUM_ROWS DESC,T.TABLE_NAME ASC", User);

            // 添加内置的行数表 by indiff PROD_TABLE_ROWS  注意：使用下面的语句的话必须保证表结构存在 PROD_TABLE_ROWS
            /*  // 创建表记录的语句，创建表的名称和行数表
                    drop table PROD_TABLE_ROWS;
                    create table PROD_TABLE_ROWS as
                    SELECT
	                    T.TABLE_NAME AS Name,
	                    T.NUM_ROWS AS TableRows,
	                    TRIM(TC.COMMENTS) AS Value
                    FROM
	                    SYS.ALL_ALL_TABLES T,
	                    SYS.ALL_TAB_COMMENTS TC
                    WHERE
	                    T.IOT_NAME IS NULL
	                    AND T.NESTED = 'NO'
	                    AND T.SECONDARY = 'N'
	                    AND NOT EXISTS ( SELECT 1 FROM SYS.ALL_MVIEWS MV WHERE MV.OWNER = T.OWNER AND MV.MVIEW_NAME = T.TABLE_NAME )
	                    AND TC.OWNER ( + ) = T.OWNER
	                    AND TC.TABLE_NAME ( + ) = T.TABLE_NAME
	                    AND T.OWNER = 'XXX'
                    ORDER BY
                      T.NUM_ROWS DESC,
	                    T.TABLE_NAME ASC
             */
            tableCommentSql = string.Format(@"SELECT
	                                T.TABLE_NAME AS Name,
	                                TRIM( TC.COMMENTS ) AS Value
                                FROM
	                                SYS.ALL_ALL_TABLES T,
	                                SYS.ALL_TAB_COMMENTS TC,
	                                PROD_TABLE_ROWS R
                                WHERE
	                                T.IOT_NAME IS NULL
	                                AND T.NESTED = 'NO'
	                                AND T.SECONDARY = 'N'
	                                AND T.TABLE_NAME NOT IN ( 'PROD_TABLE_ROWS', 'PROD_TABLE_ROWS20220804', 'PROD_TABLE_ROWS20220813', 'TEST1')
	                                AND NOT EXISTS ( SELECT 1 FROM SYS.ALL_MVIEWS MV WHERE MV.OWNER = T.OWNER AND MV.MVIEW_NAME = T.TABLE_NAME )
	                                AND TC.OWNER ( + ) = T.OWNER
	                                AND TC.TABLE_NAME ( + ) = T.TABLE_NAME
	                                AND TC.TABLE_NAME = R.NAME
	                                AND T.OWNER = '{0}'
                                ORDER BY
	                                R.TABLEROWS DESC", User);  // T.NUM_ROWS DESC, 按照行数降序

            // 读取表名称和注释信息 添加表行数信息
            string tableRowSql = string.Format(@"SELECT
	                                            T.TABLE_NAME AS Name,
	                                            T.NUM_ROWS AS Value
                                            FROM
	                                            SYS.ALL_ALL_TABLES T,
	                                            SYS.ALL_TAB_COMMENTS TC
                                            WHERE
	                                            T.IOT_NAME IS NULL
	                                            AND T.NESTED = 'NO'
	                                            AND T.SECONDARY = 'N'
	                                            AND NOT EXISTS ( SELECT 1 FROM SYS.ALL_MVIEWS MV WHERE MV.OWNER = T.OWNER AND MV.MVIEW_NAME = T.TABLE_NAME )
	                                            AND TC.OWNER ( + ) = T.OWNER
	                                            AND TC.TABLE_NAME ( + ) = T.TABLE_NAME
	                                            AND T.OWNER = '{0}'
                                            ORDER BY
                                              T.NUM_ROWS DESC,
                                              T.TABLE_NAME ASC", User);

            // 使用测试库创建的临时表来存储数据行数  by indiff
            tableRowSql = string.Format(@"SELECT NAME,TABLEROWS FROM prod_table_rows order by TABLEROWS DESC,NAME ASC", User);

            string viewSql = string.Format("select view_name,text from ALL_VIEWS WHERE OWNER = '{0}' order by view_name asc", User);

            //Oracle 11g 推出 LISTAGG 函数，有可能会报：ora-01489 字符串连接的结果过长
            string procSql = string.Format("select * from (SELECT name,LISTAGG(text,' ') WITHIN  group (order by line asc) text FROM all_source where OWNER = '{0}'  group by name ) order by name asc", User);

            //https://blog.csdn.net/rczrj/article/details/74977010
            procSql = string.Format("select * from (SELECT name,xmlagg(xmlparse(content text||' ' wellformed) order by line asc).getclobval() text FROM all_source where OWNER = '{0}' group by name ) order by name asc", User);
            procSql = string.Format("select(name || '(' || type || ')') as name , text from(SELECT name, type, xmlagg(xmlparse(content text|| ' ' wellformed) order by line asc).getclobval() text FROM all_source where OWNER = '{0}' group by name, type ) order by type,   name asc", User);

            try
            {
                //查询Oracle的所有序列
                this.Sequences = Db.ReadList<string>(sequence_Sql);

                this.TableComments = Db.ReadNameValues(tableCommentSql);

                this.TableRows = Db.ReadDictionary<string, int>(tableRowSql); ;

                this.Version = Db.Scalar("select * from v$version where ROWNUM = 1", string.Empty);

                this.Views = Db.ReadNameValues(viewSql);

                try
                {
                    this.Procs = Db.ReadNameValues(procSql);
                    handleProcsByIndiff();
                }
                catch (Exception e)
                {
                    LogUtils.LogError("查询存储过程报错", Developer.SysDefault, e, this.Version, procSql);
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
                            if (!this.TableRows.ContainsKey(tableName))  // 如果通过表名查找行数字典找不到数据，则赋0
                            {
                                tabInfo.TableRows = 0;
                            }
                            else
                            {
                                tabInfo.TableRows = this.TableRows[tableName];
                            }

                            /** 该语句，包含某列是否自增列，查询慢 **/

                            tableCommentSql = @"select a.COLUMN_ID As Colorder,a.COLUMN_NAME As ColumnName,a.DATA_TYPE As TypeName,b.comments As DeText,(Case When a.DATA_TYPE='NUMBER' Then a.DATA_PRECISION When a.DATA_TYPE='NVARCHAR2' Then a.DATA_LENGTH/2 Else a.DATA_LENGTH End )As Length,a.DATA_SCALE As Scale,
	(Case When (select Count(1)  from all_cons_columns aa, all_constraints bb where aa.OWNER = '{0}' and bb.OWNER = '{0}' and aa.constraint_name = bb.constraint_name and bb.constraint_type = 'P' and aa.table_name = '{1}' And aa.column_name=a.COLUMN_NAME)>0 Then 1 Else 0 End
	 ) As IsPK,(
			 case when (select count(1) from all_triggers tri INNER JOIN all_source src on tri.trigger_Name=src.Name
				where tri.OWNER = '{0}' and src.OWNER = '{0}' and (triggering_Event='INSERT' and table_name='{1}')
			and regexp_like(text,concat(concat('into\s*?:\s*?new\s*?\.\s*?',a.COLUMN_NAME),'\s+?'),'i'))>0
			then 1 else 0 end
	) As IsIdentity,
		Case a.NULLABLE  When 'Y' Then 1 Else 0 End As CanNull,
		a.data_default As DefaultVal from all_tab_columns a Inner Join all_col_comments b On a.TABLE_NAME=b.table_name
	Where a.OWNER = '{0}' and b.OWNER = '{0}' and b.COLUMN_NAME= a.COLUMN_NAME and a.Table_Name='{1}'  order by a.column_ID Asc";

                            // 忽略 IsIdentity 查询
                            tableCommentSql = @"select a.COLUMN_ID As Colorder,a.COLUMN_NAME As ColumnName,a.DATA_TYPE As TypeName,b.comments As DeText,(Case When a.DATA_TYPE='NUMBER' Then a.DATA_PRECISION When a.DATA_TYPE='NVARCHAR2' Then a.DATA_LENGTH/2 Else a.DATA_LENGTH End )As Length,a.DATA_SCALE As Scale,
	(Case When (select Count(1)  from all_cons_columns aa, all_constraints bb where aa.OWNER = '{0}' and bb.OWNER = '{0}' and aa.constraint_name = bb.constraint_name and bb.constraint_type = 'P' and aa.table_name = '{1}' And aa.column_name=a.COLUMN_NAME)>0 Then 1 Else 0 End
	 ) As IsPK,0 As IsIdentity,
		Case a.NULLABLE  When 'Y' Then 1 Else 0 End As CanNull,
		a.data_default As DefaultVal from all_tab_columns a Inner Join all_col_comments b On a.TABLE_NAME=b.table_name
	Where a.OWNER = '{0}' and b.OWNER = '{0}' and b.COLUMN_NAME= a.COLUMN_NAME and a.Table_Name='{1}'  order by a.column_ID Asc";

                            tableCommentSql = string.Format(tableCommentSql, User, tableName);

                            try
                            {
                                tabInfo.Colnumns = Db.GetDataTable(tableCommentSql).ConvertToListObject<ColumnInfo>();

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
                                LogUtils.LogError("DB", Developer.SysDefault, ex, tableCommentSql);
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

        /**
         * 处理存储过程的名称
         */

        private void handleProcsByIndiff()
        {
            NameValueCollection nvc = new NameValueCollection();
            foreach (string k in this.Procs)
            {
                foreach (string v in this.Procs.GetValues(k))
                {
                    // Console.WriteLine("{0} {1}", s, v);
                    nvc.Add(getComment(k, v), v);
                }
            }
            if (this.Procs.Count == nvc.Count)
            {
                this.Procs = nvc;
            }
        }

        /**
         * 提取功能描述
         */

        private string getComment(string k, string v)
        {
            string pattern = @"功能描述：(.+)\s+";

            Match match = Regex.Match(v, pattern);
            if (match.Success && match.Groups.Count > 1)
            {   // 判断是否找到功能描述，进行提取日志
                Group g = match.Groups[1];
                string comment = g.ToString().Trim();
                // 影响文件名的字符进行替换
                comment = comment.Replace("/", "");
                comment = comment.Replace(@"\", "");
                comment = comment.Replace(@"&", "_");
                return k.Trim() + "_" + comment;
            }
            return k.Trim();
        }

        private void AddColSeq(string tableName, string colName)
        {
            tableName = (tableName ?? string.Empty);
            colName = (colName ?? string.Empty);
            string strSql = string.Empty;
            if (Sequences != null && Sequences.Count > 0)
            {
                foreach (string seqName in Sequences)
                {
                    strSql = @"select count(1) from all_triggers tri INNER JOIN all_source src on tri.trigger_Name=src.Name where tri.OWNER = '" + User + "' and src.OWNER = '" + User + "'  and (triggering_Event='INSERT' and table_name='" + tableName + "') and regexp_like(text,concat(concat('" + seqName + @"\s*?\.\s*?nextval\s+into\s*?:\s*?new\s*?\.\s*?','" + colName + @"'),'\s+?'),'i')";
                    int res = Db.Single<int>(strSql, 0);
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
            string strSql = "select object_name as name ,last_ddl_time as modify_date from all_objects Where OWNER = '" + User + "' and object_Type='TABLE' Order By last_ddl_time Desc";
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