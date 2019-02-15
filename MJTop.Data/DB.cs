using MJTop.Data.SPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Collections.Specialized;
using MJTop.Data.DatabaseInfo;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace MJTop.Data
{
    public partial class DB : IDB
    {
        /// <summary>
        /// 数据提供程序工厂
        /// </summary>
        internal DbProviderFactory DBFactory
        {
            get;
            set;
        }

        /// <summary>
        /// 连接字符串基类对象
        /// </summary>
        public DbConnectionStringBuilder ConnectionStringBuilder { get; protected set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBType DBType
        {
            get; protected set;
        }

        /// <summary>
        /// 数据库表列相关信息
        /// </summary>
        public IDBInfo Info
        {
            get;
            protected set;
        }


        /// <summary>
        /// 超时时间
        /// </summary>
        internal int CmdTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        internal string ConnectionString { get; set; }

        #region AOP
        /// <summary>
        /// 执行前 
        /// </summary>
        public Action<DbCommand> OnExecuting { get; set; }

        /// <summary>
        /// 执行后，附带返回值
        /// </summary>
        public Action<DbCommand, object> OnExecuted { get; set; }

        /// <summary>
        /// 报异常时
        /// </summary>
        public Action<DbCommand, Exception> OnError { get; set; }
        #endregion

        /// <summary>
        /// 对数据 增删改 时触发
        /// </summary>
        public TableTrigger DataChangeTriggers
        {
            get;
            internal set;
        }

        #region 插入或更新排除列 设置与获取
        /// <summary>
        /// 设置 插入时 要排除表列 集合
        /// </summary>
        public ExcludeColumn InsertExcludeColumns
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取 插入时 排除表列 集合
        /// </summary>
        /// <returns></returns>
        public NameValueCollection GetInsertExcludeColumns()
        {
            return InsertExcludeColumns.Coll;
        }

        /// <summary>
        /// 设置 更新时 要排除表列 集合
        /// </summary>
        public ExcludeColumn UpdateExcludeColumns
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取 更新时 排除表列 集合
        /// </summary>
        /// <returns></returns>
        public NameValueCollection GetUpdateExcludeColumns()
        {
            return InsertExcludeColumns.Coll;
        }


        public string ParameterChar
        {
            get
            {
                return DBType.ParameterChar();
            }
        }


        #endregion


        internal string ParameterSql(string parameterName)
        {
            if (DBType == DBType.OracleDDTek)
            {
                return "?";
            }
            return ParameterChar + parameterName;
        }

        /// <summary>
        /// 构建 参数化 对象 
        /// 不同数据库 参数化 时所使用的字符：Global.ParameterCharMap
        /// </summary>
        /// <param name="parameterName">参数化 变量名</param>
        /// <param name="value">参数化 值</param>
        /// <param name="colInfo">列信息</param>
        /// <returns></returns>
        public DbParameter CreateParameter(string parameterName, object value, ColumnInfo colInfo = null)
        {
            DbParameter dbparameter = DBFactory.CreateParameter();
            dbparameter.ParameterName = DBType.ParameterChar() + parameterName;
            dbparameter.Value = value;

            if (value == null || value == DBNull.Value)
            {
                dbparameter.Value = DBNull.Value;
            }
            else
            {
                DbType dbType;
                if (colInfo != null)
                {
                    dbType = colInfo.DbType;
                    object val = null;
                    val = Global.Dict_Convert_Type[dbType].Invoke(dbparameter.Value);
                    if (val is Exception)
                    {
                        throw val as Exception;
                    }
                    dbparameter.Value = val;
                    dbparameter.DbType = dbType;
                }
                else
                {
                    Type tyValue = dbparameter.Value.GetType();
                    if (Global.TypeMap.TryGetValue(tyValue, out dbType))
                    {
                        dbparameter.DbType = dbType;
                    }
                    else
                    {
                        dbparameter.DbType = DbType.AnsiString;
                    }
                }
            }
            return dbparameter;
        }

        /// <summary>
        /// 构建 参数化 对象
        /// </summary>
        /// <param name="parameterName">参数化 变量名</param>
        /// <param name="value">参数化 值</param>
        /// <param name="type">数据类型</param>
        /// <param name="direction">参数化的方式</param>
        /// <returns></returns>
        public DbParameter CreateParameter(string parameterName, object value, DbType type, ParameterDirection direction = ParameterDirection.Input)
        {
            DbParameter dbparameter = DBFactory.CreateParameter();
            dbparameter.ParameterName = DBType.ParameterChar() + parameterName;
            dbparameter.Value = value;
            dbparameter.DbType = type;
            dbparameter.Direction = direction;
            return dbparameter;
        }

        /// <summary>
        /// 创建 Connection 对象
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConn()
        {
            return CreateConn(this.ConnectionString);
        }


        #region internal ado.net的四大核心对象
        internal DbConnection CreateConn(string connectionString)
        {
            DbConnection _DBConn = DBFactory.CreateConnection();
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                _DBConn.ConnectionString = connectionString;
            }
            return _DBConn;
        }

        internal DbCommand CreateCmd()
        {
            DbCommand _DBCmd = DBFactory.CreateCommand();
            return _DBCmd;
        }

        internal DbCommand CreateCmd(string commandText = null, DbConnection DbConn = null)
        {
            DbCommand _DBCmd = DBFactory.CreateCommand();
            if (DbConn != null)
            {
                _DBCmd.Connection = DbConn;
            }
            if (!string.IsNullOrWhiteSpace(commandText))
            {
                _DBCmd.CommandText = commandText;
            }
            return _DBCmd;
        }

        internal DbDataAdapter CreateAdapter(DbCommand dbCmd = null)
        {
            DbDataAdapter dbadapter = DBFactory.CreateDataAdapter();
            if (dbCmd != null)
            {
                dbadapter.SelectCommand = dbCmd;
            }
            return dbadapter;
        }

        internal DbParameter CreateParameter()
        {
            DbParameter dbparameter = DBFactory.CreateParameter();
            return dbparameter;
        }

        internal void CheckTabStuct(string tableName, params string[] columnNames)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "不能为空！");
            }

            if (!Info.TableNames.Contains(tableName, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(string.Format("不存在该表！{0}", "[" + tableName + "]"), "tableName:" + tableName);
            }

            if (columnNames != null && columnNames.Length > 0)
            {
                List<string> lstAllColName = Info[tableName];

                foreach (string columnName in columnNames)
                {
                    if (!lstAllColName.Contains(columnName, StringComparer.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException(string.Format("不存在该列！{0}", "[" + tableName + "." + columnName + "]"), "columnName:" + columnName, null);
                    }
                }
            }
        }

        #endregion


        public DB(DBType dbType, DbProviderFactory dbFactory, string connectionString)
        {
            this.DBType = dbType;
            this.DBFactory = dbFactory;
            this.ConnectionString = connectionString;

            this.ConnectionStringBuilder = DBFactory.CreateConnectionStringBuilder();           
            this.ConnectionStringBuilder.ConnectionString = connectionString;

            this.InsertExcludeColumns = new ExcludeColumn(this);
            this.UpdateExcludeColumns = new ExcludeColumn(this);

            this.DataChangeTriggers = new TableTrigger(this);

            {//查询数据库表列信息前，先验证数据库能否可以连接成功！

                try
                {
                    using (DbConnection conn = CreateConn())
                    {
                        conn.Open();
                    }
                }
                catch (Exception ex)
                {
                    if (this.OnError != null)
                        this.OnError.Invoke(null, ex);

                    throw ex;
                }
            }
        }

        internal DbCommand BuildCommand(DbConnection conn, string cmdText, int timeOut = 30, CommandType cmdType = CommandType.Text)
        {
            return BuildCommandByParam(conn, cmdText, null, timeOut, cmdType);
        }

        static ConcurrentDictionary<Type, PropertyInfo[]> Dict_Type_Props = new ConcurrentDictionary<Type, PropertyInfo[]>();
        internal DbCommand BuildCommandByParam(DbConnection conn, string cmdText, object cmdParms, int timeOut = 30, CommandType cmdType = CommandType.Text, string tableName = null)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (timeOut > 0)//（以秒为单位）。默认为 30 秒
            {
                cmd.CommandTimeout = timeOut;
            }
            else
            {
                cmd.CommandTimeout = this.CmdTimeout;
            }

            if (cmdParms != null)
            {
                if (cmdParms is DbParameter[])
                {
                    DbParameter[] arr = cmdParms as DbParameter[];
                    cmd.Parameters.AddRange(arr);
                }
                else if (cmdParms is List<DbParameter>)
                {
                    List<DbParameter> lstDp = cmdParms as List<DbParameter>;
                    cmd.Parameters.AddRange(lstDp.ToArray());
                }
                else if (cmdParms is System.Collections.IDictionary)
                {
                    IDictionary dict = cmdParms as IDictionary;
                    if (dict != null && dict.Count > 0)
                    {
                        foreach (DictionaryEntry kv in dict)
                        {
                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = ParameterChar + kv.Key.ToString();
                            parameter.Value = kv.Value;

                            if (parameter.Value == null || parameter.Value == DBNull.Value)
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else
                            {
                                if (tableName != null)
                                {
                                    ColumnInfo colInfo = Info[tableName, kv.Key.ToString()];
                                    if (colInfo != null)
                                    {
                                        parameter.DbType = colInfo.DbType;

                                        parameter.Value = Global.Dict_Convert_Type[colInfo.DbType].Invoke(parameter.Value);
                                    }
                                }
                                else
                                {
                                    Type tyValue = parameter.Value.GetType();
                                    DbType tmpType;
                                    if (Global.TypeMap.TryGetValue(tyValue, out tmpType))
                                    {
                                        parameter.DbType = tmpType;
                                    }
                                    else
                                    {
                                        parameter.DbType = DbType.AnsiString;
                                    }
                                }
                            }
                            cmd.Parameters.Add(parameter);
                        }
                    }
                }
                else if (cmdParms is NameValueCollection)
                {
                    NameValueCollection nvc = cmdParms as NameValueCollection;
                    if (nvc != null && nvc.Count > 0)
                    {
                        foreach (var key in nvc.AllKeys)
                        {
                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = ParameterChar + key;
                            parameter.Value = nvc[key];

                            if (parameter.Value == null || parameter.Value == DBNull.Value)
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else
                            {
                                if (tableName != null)
                                {
                                    ColumnInfo colInfo = Info[tableName, key];
                                    if (colInfo != null)
                                    {
                                        parameter.DbType = colInfo.DbType;

                                        parameter.Value = Global.Dict_Convert_Type[colInfo.DbType].Invoke(parameter.Value);
                                    }
                                }
                                else
                                {
                                    Type tyValue = parameter.Value.GetType();
                                    DbType tmpType;
                                    if (Global.TypeMap.TryGetValue(tyValue, out tmpType))
                                    {
                                        parameter.DbType = tmpType;
                                    }
                                    else
                                    {
                                        parameter.DbType = DbType.AnsiString;
                                    }
                                }
                            }
                            cmd.Parameters.Add(parameter);
                        }
                    }
                }
                else
                {
                    Type ty = cmdParms.GetType();

                    if (ty.Name.Contains("AnonymousType"))
                    {
                        PropertyInfo[] props;
                        if (!Dict_Type_Props.TryGetValue(ty, out props))
                        {
                            Dict_Type_Props[ty] = ty.GetProperties();
                            props = Dict_Type_Props[ty];
                        }

                        if (props != null && props.Length > 0)
                        {
                            foreach (var prop in props)
                            {
                                var parameter = cmd.CreateParameter();
                                parameter.ParameterName = ParameterChar + prop.Name;
                                parameter.Value = prop.GetValue(cmdParms, null);

                                if (parameter.Value == null || parameter.Value == DBNull.Value)
                                {
                                    parameter.Value = DBNull.Value;
                                }
                                else
                                {
                                    if (tableName != null)
                                    {
                                        ColumnInfo colInfo = Info[tableName, prop.Name];
                                        if (colInfo != null)
                                        {
                                            parameter.DbType = colInfo.DbType;

                                            parameter.Value = Global.Dict_Convert_Type[colInfo.DbType].Invoke(parameter.Value);
                                        }
                                    }
                                    else
                                    {
                                        Type tyValue = parameter.Value.GetType();
                                        DbType tmpType;
                                        if (Global.TypeMap.TryGetValue(tyValue, out tmpType))
                                        {
                                            parameter.DbType = tmpType;
                                        }
                                        else
                                        {
                                            parameter.DbType = DbType.AnsiString;
                                        }
                                    }
                                }
                                cmd.Parameters.Add(parameter);
                            }
                        }
                    }
                    else
                    {
                        //其他类型
                    }

                }
            }

            if (OnExecuting != null)
            {
                OnExecuting.Invoke(cmd);
            }
            return cmd;
        }

        /// <summary>
        /// 尝试 连接字符串 能否 连接成功
        /// </summary>
        /// <returns></returns>
        public virtual bool TryConnect()
        {
            try
            {
                using (DbConnection conn = CreateConn())
                {
                    conn.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sqlite3 收缩数据库（缓存数据写入到db文件中）
        /// </summary>
        public void Shrink()
        {
            ExecSql("pragma journal_mode=delete;");
        }


        /// <summary>
        /// 验证 执行语句 是否 能执行通过
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual bool ValidateSql(string strSql, out Exception ex)
        {
            bool bResult = false;
            ex = null;
            DbConnection conn = null;
            DbCommand cmd = null;
            int res = -1;
            try
            {
                conn = CreateConn();
                strSql = "explain  " + strSql;
                cmd = BuildCommand(conn, strSql);
                res = cmd.ExecuteNonQuery();
                bResult = true;
            }
            catch (Exception e)
            {
                ex = e;
                bResult = false;
            }
            finally
            {
                conn?.Close();
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, bResult);
            }
            return bResult;
        }


        public virtual int RunStoreProc(string storeProcName, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            int cnt = -1;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, storeProcName, parameters, this.CmdTimeout, CommandType.StoredProcedure);
                cnt = cmd.ExecuteNonQuery();
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

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, cnt);
            }
            return cnt;
        }
        public virtual DataTable RunStoreProcGetDT(string storeProcName, object parameters = null)
        {
            DataSet ds = new DataSet("ds");
            DbConnection conn = null;
            DbCommand cmd = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, storeProcName, parameters, this.CmdTimeout, CommandType.StoredProcedure);
                DataAdapter adapter = CreateAdapter(cmd);
                adapter.Fill(ds);
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

            DataTable dt = null;
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                dt.TableName = "data";
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, dt);
            }
            return dt;
        }
        public virtual DataSet RunStoreProcGetDS(string storeProcName, object parameters = null)
        {
            DataSet ds = new DataSet("ds");
            DbConnection conn = null;
            DbCommand cmd = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, storeProcName, parameters, this.CmdTimeout, CommandType.StoredProcedure);
                DataAdapter adapter = CreateAdapter(cmd);
                adapter.Fill(ds);
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

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, ds);
            }
            return ds;
        }



        #region 基础查询

        public virtual TRet Scalar<TRet>(string strSql, TRet defRet, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            object obj = null;
            TRet result;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                obj = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                result = defRet;
                if (this.OnError != null)
                    this.OnError.Invoke(cmd, ex);
            }
            finally
            {
                conn?.Close();
            }

            result = obj.ChangeType<TRet>(default(TRet));

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, result);
            }
            return result;
        }

        public virtual NameValueCollection GetFirstRow(string strSql, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader myReader = null;
            NameValueCollection dict = new NameValueCollection;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                myReader = cmd.ExecuteReader();
                if (myReader.HasRows)
                {
                    myReader.Read();
                    for (int j = 0; j < myReader.FieldCount; j++)
                    {
                        string columnName = myReader.GetName(j);
                        dict.Add(columnName, myReader[columnName] == null ? string.Empty : myReader[columnName].ToString());
                    }
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
                myReader?.Close();
                conn?.Close();
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, dict);
            }
            return dict;
        }

        public virtual DataTable GetDataTable(string strSql, object parameters = null)
        {
            DataSet ds = new DataSet("ds");
            DbConnection conn = null;
            DbCommand cmd = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                DataAdapter adapter = CreateAdapter(cmd);
                adapter.Fill(ds);
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

            DataTable dt = null;
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                dt.TableName = "data";
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, dt);
            }
            return dt;
        }

        public virtual List<DataTable> GetListTable(string strSql, object parameters = null)
        {
            List<DataTable> lstTabs = new List<DataTable>();
            DataSet ds = new DataSet("ds");
            DbConnection conn = null;
            DbCommand cmd = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                DataAdapter adapter = CreateAdapter(cmd);
                adapter.Fill(ds);
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

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataTable tb in ds.Tables)
                {
                    lstTabs.Add(tb);
                }
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, lstTabs);
            }
            return lstTabs;
        }

        public virtual DataSet GetDataSet(string strSql, object parameters = null)
        {
            DataSet ds = new DataSet("ds");
            DbConnection conn = null;
            DbCommand cmd = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                DataAdapter adapter = CreateAdapter(cmd);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                if (this.OnError != null)
                    this.OnError.Invoke(cmd, ex);
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, ds);
            }
            return ds;
        }

        public virtual DbDataReader ExecReader(string commandText, object parameters = null, CommandType commandType = CommandType.Text)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader myReader = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, commandText, parameters, CmdTimeout, commandType);
                myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                if (this.OnError != null)
                    this.OnError.Invoke(cmd, ex);
                throw ex;
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, myReader);
            }
            return myReader;
        }

        public virtual TRet Single<TRet>(string strSql, TRet defRet, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader myReader = null;
            TRet result;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                myReader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                if (myReader.HasRows)
                {
                    myReader.Read();
                    // ret = myReader.GetFieldValue<TRet>(0);
                    result = myReader.GetValue(0).ChangeType<TRet>(default(TRet));
                }
                else
                {
                    result = defRet;
                }
            }
            catch (Exception ex)
            {
                result = defRet;
                if (this.OnError != null)
                    this.OnError.Invoke(cmd, ex);
            }
            finally
            {
                myReader?.Close();
                conn?.Close();
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, result);
            }
            return result;
        }

        public virtual List<Dictionary<string, object>> GetListDictionary(string strSql, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader myReader = null;
            List<Dictionary<string, object>> lstDict = new List<Dictionary<string, object>>();
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                myReader = cmd.ExecuteReader();
                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>();
                        for (int j = 0; j < myReader.FieldCount; j++)
                        {
                            string columnName = myReader.GetName(j);
                            dict.Add(columnName, myReader[columnName]);
                        }
                        lstDict.Add(dict);
                    }
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
                myReader?.Close();
                conn?.Close();
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, lstDict);
            }
            return lstDict;
        }

        public virtual DataTable ReadTable(string strSql, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader myReader = null;
            DataTable data = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                myReader = cmd.ExecuteReader();
                data = new DataTable("data");
                if (myReader.HasRows)
                {
                    bool isAddCol = false;
                    while (myReader.Read())
                    {
                        DataRow dr = data.NewRow();
                        for (int j = 0; j < myReader.FieldCount; j++)
                        {
                            string columnName = myReader.GetName(j);
                            if (!isAddCol)
                            {
                                data.Columns.Add(columnName, myReader.GetFieldType(j));
                            }

                            dr[columnName] = myReader[columnName];
                        }
                        data.Rows.Add(dr);
                        isAddCol = true;
                    }
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
                myReader?.Close();
                conn?.Close();
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, data);
            }
            return data;
        }

        public virtual List<TRet> ReadList<TRet>(string strSql, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader myReader = null;
            List<TRet> lstVal = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                myReader = cmd.ExecuteReader();
                lstVal = new List<TRet>();
                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        lstVal.Add(myReader.GetFieldValue<TRet>(0));
                    }
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
                myReader?.Close();
                conn?.Close();
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, lstVal);
            }
            return lstVal;
        }

        public virtual NameValueCollection ReadNameValues(string strSql, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader myReader = null;
            NameValueCollection nvc = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                myReader = cmd.ExecuteReader();
                nvc = new NameValueCollection();
                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        nvc.Add((myReader.GetValue(0) ?? string.Empty).ToString(), (myReader.GetValue(1) ?? string.Empty).ToString());
                    }
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
                myReader?.Close();
                conn?.Close();
            }
            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, nvc);
            }
            return nvc;
        }

        public virtual Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(string strSql, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader myReader = null;
            Dictionary<TKey, TValue> dict = null;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                myReader = cmd.ExecuteReader();
                dict = new Dictionary<TKey, TValue>();
                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        dict.Add((TKey)myReader.GetValue(0).ChangeType(typeof(TKey)), (TValue)myReader.GetValue(1).ChangeType(typeof(TValue)));
                    }
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
                myReader?.Close();
                conn?.Close();
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, dict);
            }
            return dict;
        }

        #endregion

        #region 执行

        public virtual int ExecSql(string strSql, object parameters = null)
        {
            if (this.DBType == DBType.SQLite && !strSql.Contains("journal_mode=delete"))
            {
                return ExecSqlTran(strSql, parameters);
            }
            DbConnection conn = null;
            DbCommand cmd = null;
            int cnt = -1;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                cnt = cmd.ExecuteNonQuery();
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

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, cnt);
            }
            return cnt;
        }

        public int ExecSqlTran(string strSql, object parameters = null)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbTransaction tran = null;
            int cnt = -1;
            try
            {
                conn = CreateConn();
                cmd = BuildCommandByParam(conn, strSql, parameters);
                tran = conn.BeginTransaction();               
                cnt = cmd.ExecuteNonQuery();
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran?.Rollback();
                if (this.OnError != null)
                    this.OnError.Invoke(cmd, ex);
            }
            finally
            {
                conn?.Close();
            }

            if (OnExecuted != null)
            {
                OnExecuted.Invoke(cmd, cnt);
            }
            return cnt;
        }

        public virtual int ExecSqlTran(params string[] sqlCmds)
        {
            if (sqlCmds == null || sqlCmds.Length == 0)
            {
                return -1;
            }

            DbConnection conn = null;
            DbCommand cmd = null;
            DbTransaction tran = null;
            int cnt = 0;
            try
            {
                conn = CreateConn();
                conn.Open();
                tran = conn.BeginTransaction();
                cmd = conn.CreateCommand();
                cmd.CommandTimeout = this.CmdTimeout;

                for (int n = 0; n < sqlCmds.Length; n++)
                {
                    string strsql = sqlCmds[n];
                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;

                        if (OnExecuting != null)
                        {
                            OnExecuting.Invoke(cmd);
                        }
                        int res = cmd.ExecuteNonQuery();
                        cnt += res;

                        if (OnExecuted != null)
                        {
                            OnExecuted.Invoke(cmd, res);
                        }
                    }
                }
                tran.Commit();

            }
            catch (Exception ex)
            {
                tran.Rollback();
                if (this.OnError != null)
                    this.OnError.Invoke(cmd, ex);
            }
            finally
            {
                conn?.Close();
            }
            return cnt;
        }
        public virtual int ExecSqlTran(List<KeyValuePair<string, List<DbParameter>>> strSqlList)
        {
            if (strSqlList == null || strSqlList.Count <= 0)
            {
                return -1;
            }
            DbConnection conn = null;
            DbCommand cmd = null;
            DbTransaction tran = null;
            int cnt = 0;
            try
            {
                conn = CreateConn();
                conn.Open();
                tran = conn.BeginTransaction();
                cmd = conn.CreateCommand();
                cmd.CommandTimeout = this.CmdTimeout;

                for (int n = 0; n < strSqlList.Count; n++)
                {
                    var kv = strSqlList[n];
                    if (!string.IsNullOrWhiteSpace(kv.Key))
                    {
                        cmd.CommandText = kv.Key;

                        if (kv.Value != null && kv.Value.Count > 0)
                        {
                            cmd.Parameters.AddRange(kv.Value.ToArray());
                        }

                        if (OnExecuting != null)
                        {
                            OnExecuting.Invoke(cmd);
                        }
                        int res = cmd.ExecuteNonQuery();
                        cnt += res;

                        if (OnExecuted != null)
                        {
                            OnExecuted.Invoke(cmd, res);
                        }
                    }
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                if (this.OnError != null)
                    this.OnError.Invoke(cmd, ex);
            }
            finally
            {
                conn?.Close();
            }
            return cnt;
        }



        #endregion


        public virtual int BulkInsert(string tableName, DataTable data,int batchSize = 200000, int timeout = 60)
        {
            throw new NotImplementedException("该方法未支持！(" + this.DBType + ")");
        }

        public virtual int BulkInsert(string tableName, DbDataReader reader, int batchSize = 200000, int timeout = 60)
        {
            throw new NotImplementedException("该方法未支持！(" + this.DBType + ")");
        }

        public KeyValuePair<string, List<DbParameter>> InsertScript<DT>(DT data, string tableName, params string[] excludeColNames)
        {
            if (data == null)
                throw new ArgumentException("data", "不能为null!");
            CheckTabStuct(tableName);

            StringBuilder sb_beforeSQL = new StringBuilder();
            sb_beforeSQL.Append("insert into " + tableName + " (");
            StringBuilder sb_afterSQl = new StringBuilder();
            sb_afterSQl.Append(") values (");

            string insert_sql = string.Empty;

            List<DbParameter> lstPara = new List<DbParameter>();
            var lstColName = Info[tableName];

            IEnumerable<string> enumNames = new List<string>();
            if (TypeInfo<DT>.IsAnonymousType)
            {
                #region 匿名对象
                var curNames = TypeInfo<DT>.PropNames;
                enumNames = curNames.Intersect(lstColName, StringComparer.OrdinalIgnoreCase);

                if (excludeColNames != null && excludeColNames.Length > 0)
                {
                    enumNames = enumNames.Except(excludeColNames, StringComparer.OrdinalIgnoreCase);
                }

                string[] columnNames = InsertExcludeColumns.Coll.GetValues(tableName);

                if (columnNames != null && columnNames.Length > 0)
                {
                    enumNames = enumNames.Except(columnNames, StringComparer.OrdinalIgnoreCase);
                }

                foreach (var colName in enumNames)
                {
                    ColumnInfo colInfo = Info[tableName, colName];
                    if (colInfo.IsIdentity)
                    {
                        continue;
                    }
                    object obj = TypeInfo<DT>.PropMapping[colName].GetValue(data, null);
                    if (!string.IsNullOrWhiteSpace(colInfo.DefaultVal)
                        && (obj == null || string.IsNullOrEmpty(obj.ToString())))
                    {
                        continue;
                    }
                    DbParameter dp = CreateParameter(colName, obj, colInfo);
                    lstPara.Add(dp);
                    sb_beforeSQL.Append(colName + ",");
                    sb_afterSQl.Append(ParameterSql(colName) + ",");
                }
                #endregion
            }
            else if (TypeInfo<DT>.IsNameValueColl)
            {
                #region NameValueCollection
                var curNames = (data as NameValueCollection).AllKeys;
                enumNames = curNames.Intersect(lstColName, StringComparer.OrdinalIgnoreCase);

                if (excludeColNames != null && excludeColNames.Length > 0)
                {
                    enumNames = enumNames.Except(excludeColNames, StringComparer.OrdinalIgnoreCase);
                }

                foreach (var colName in enumNames)
                {
                    ColumnInfo colInfo = Info[tableName, colName];
                    if (colInfo.IsIdentity)
                    {
                        continue;
                    }
                    object obj = (data as NameValueCollection)[colName];
                    if (!string.IsNullOrWhiteSpace(colInfo.DefaultVal)
                        && (obj == null || string.IsNullOrEmpty(obj.ToString())))
                    {
                        continue;
                    }
                    DbParameter dp = CreateParameter(colName, obj, colInfo);
                    lstPara.Add(dp);

                    sb_beforeSQL.Append(colName + ",");
                    sb_afterSQl.Append(ParameterSql(colName) + ",");
                }
                #endregion
            }
            else if (TypeInfo<DT>.IsDict)
            {
                #region IDictionary
                var curNames = (data as IDictionary).Keys;

                string[] arrKeys = new string[curNames.Count];

                curNames.CopyTo(arrKeys, 0);

                enumNames = arrKeys.Intersect(lstColName, StringComparer.OrdinalIgnoreCase);

                if (excludeColNames != null && excludeColNames.Length > 0)
                {
                    enumNames = enumNames.Except(excludeColNames, StringComparer.OrdinalIgnoreCase);
                }

                foreach (var colName in enumNames)
                {
                    ColumnInfo colInfo = Info[tableName, colName];
                    if (colInfo.IsIdentity)
                    {
                        continue;
                    }
                    object obj = (data as IDictionary)[colName];
                    if (!string.IsNullOrWhiteSpace(colInfo.DefaultVal)
                        && (obj == null || string.IsNullOrEmpty(obj.ToString())))
                    {
                        continue;
                    }
                    DbParameter dp = CreateParameter(colName, obj, colInfo);
                    lstPara.Add(dp);

                    sb_beforeSQL.Append(colName + ",");
                    sb_afterSQl.Append(ParameterSql(colName) + ",");
                }
                #endregion
            }
            else
            {
                throw new ArgumentException("未知数据类型插入！", "data");
            }

            if (!enumNames.Any())
            {
                throw new ArgumentException("至少有1列的值，才能够插入！", "data");
            }

            insert_sql = sb_beforeSQL.ToString().TrimEnd(',') + sb_afterSQl.ToString().TrimEnd(',') + ")";

            return new KeyValuePair<string, List<DbParameter>>(insert_sql, lstPara);
        }

        public virtual bool Insert<DT>(DT data, string tableName, params string[] excludeColNames)
        {
            var kv = InsertScript(data, tableName, excludeColNames);
            bool res = false;
            res = ExecSql(kv.Key, kv.Value) > 0;
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

        internal virtual Ret InsertGet<DT, Ret>(DT data, string tableName, params string[] excludeColNames)
        {
            var kv = InsertScript(data, tableName, excludeColNames);
            string insert_Sql = kv.Key + ";" + Script.IdentitySql(DBType, tableName);
            var res = Single<Ret>(insert_Sql, default(Ret), kv.Value);

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

        public virtual int InsertGetInt<DT>(DT data, string tableName, params string[] excludeColNames)
        {
            return InsertGet<DT, int>(data, tableName, excludeColNames);
        }

        public virtual long InsertGetLong<DT>(DT data, string tableName, params string[] excludeColNames)
        {
            return InsertGet<DT, long>(data, tableName, excludeColNames);
        }

        public KeyValuePair<string, List<DbParameter>> UpdateScript<DT>(DT data, string tableName, string pkOrUniqueColName = "Id", params string[] excludeColNames)
        {
            if (data == null)
                throw new ArgumentException("data", "不能为null!");
            CheckTabStuct(tableName, pkOrUniqueColName);

            string parameterChar = DBType.ParameterChar();

            StringBuilder sb_beforeSQL = new StringBuilder();
            sb_beforeSQL.Append("update " + tableName + " set ");


            string update_sql = string.Empty;
            DbParameter paraPKOrUnique = null;
            string paraPKOrUniqueName = string.Empty;

            List<DbParameter> lstPara = new List<DbParameter>();
            var lstColName = Info[tableName];

            IEnumerable<string> enumNames = new List<string>();
            if (TypeInfo<DT>.IsAnonymousType)
            {
                #region 匿名对象
                var curNames = TypeInfo<DT>.PropNames;
                enumNames = curNames.Intersect(lstColName, StringComparer.OrdinalIgnoreCase);

                if (excludeColNames != null && excludeColNames.Length > 0)
                {
                    enumNames = enumNames.Except(excludeColNames, StringComparer.OrdinalIgnoreCase);
                }

                foreach (var colName in enumNames)
                {
                    ColumnInfo colInfo = Info[tableName, colName];
                    object obj = TypeInfo<DT>.PropMapping[colName].GetValue(data, null);
                    if (!colName.Equals(pkOrUniqueColName, StringComparison.OrdinalIgnoreCase))
                    {
                        sb_beforeSQL.Append(colName + "=" + ParameterSql(colName) + ",");
                        DbParameter dp = CreateParameter(colName, obj, colInfo);
                        lstPara.Add(dp);
                    }
                    else
                    {
                        paraPKOrUnique = CreateParameter(colName, obj, colInfo);
                        paraPKOrUniqueName = colName;
                    }
                }
                #endregion

            }
            else if (TypeInfo<DT>.IsNameValueColl)
            {
                #region NameValueCollection
                var curNames = (data as NameValueCollection).AllKeys;
                enumNames = curNames.Intersect(lstColName, StringComparer.OrdinalIgnoreCase);

                if (excludeColNames != null && excludeColNames.Length > 0)
                {
                    enumNames = enumNames.Except(excludeColNames, StringComparer.OrdinalIgnoreCase);
                }

                foreach (var colName in enumNames)
                {
                    ColumnInfo colInfo = Info[tableName, colName];
                    object obj = (data as NameValueCollection)[colName];
                    if (!colName.Equals(pkOrUniqueColName, StringComparison.OrdinalIgnoreCase))
                    {
                        sb_beforeSQL.Append(colName + "=" + ParameterSql(colName) + ",");
                        DbParameter dp = CreateParameter(colName, obj, colInfo);
                        lstPara.Add(dp);
                    }
                    else
                    {
                        paraPKOrUnique = CreateParameter(colName, obj, colInfo);
                        paraPKOrUniqueName = colName;
                    }
                }
                #endregion
            }
            else if (TypeInfo<DT>.IsDict)
            {
                #region IDictionary
                var curNames = (data as IDictionary).Keys;

                string[] arrKeys = new string[curNames.Count];

                curNames.CopyTo(arrKeys, 0);

                enumNames = arrKeys.Intersect(lstColName, StringComparer.OrdinalIgnoreCase);

                if (excludeColNames != null && excludeColNames.Length > 0)
                {
                    enumNames = enumNames.Except(excludeColNames, StringComparer.OrdinalIgnoreCase);
                }

                foreach (var colName in enumNames)
                {
                    ColumnInfo colInfo = Info[tableName, colName];
                    object obj = (data as IDictionary)[colName];
                    if (!colName.Equals(pkOrUniqueColName, StringComparison.OrdinalIgnoreCase))
                    {
                        sb_beforeSQL.Append(colName + "=" + ParameterSql(colName) + ",");
                        DbParameter dp = CreateParameter(colName, obj, colInfo);
                        lstPara.Add(dp);
                    }
                    else
                    {
                        paraPKOrUnique = CreateParameter(colName, obj, colInfo);
                        paraPKOrUniqueName = colName;
                    }
                }
                #endregion
            }
            else
            {
                throw new ArgumentException("未知数据类型插入！", "data");
            }

            if (!enumNames.Any())
            {
                throw new ArgumentException("至少有1列的值才能够更新！", "data");
            }

            update_sql = sb_beforeSQL.ToString().TrimEnd(',') + (" where " + paraPKOrUniqueName + "=" + ParameterSql(pkOrUniqueColName));
            lstPara.Add(paraPKOrUnique);
            return new KeyValuePair<string, List<DbParameter>>(update_sql, lstPara);
        }

        public virtual bool Update<DT>(DT data, string tableName, string pkOrUniqueColName = "Id", params string[] excludeColNames)
        {
            var kv = UpdateScript(data, tableName, pkOrUniqueColName, excludeColNames);
            var res = ExecSql(kv.Key, kv.Value) > 0;

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

        /// <summary>
        /// 保存策略：
        /// 1.如果表没有设置主键，则进行的是插入操作
        /// 2.如果表的主键是自增，如果data中主键有值，则进行更新操作，没有值则进行插入操作
        /// 3.如果表有主键，不是自增，则从数据查询，存在则更新，不存在则插入。注：如果主键列的值为空，则报异常！
        /// </summary>
        /// <typeparam name="DT"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <param name="pkOrUniqueColName"></param>
        /// <param name="excludeColNames"></param>
        /// <returns></returns>
        private KeyValuePair<string, List<DbParameter>> SaveScript<DT>(DT data, string tableName, out SaveType saveType, string pkOrUniqueColName = "Id", params string[] excludeColNames)
        {
            saveType = SaveType.Insert;
            if (data == null)
                throw new ArgumentException("data", "不能为null!");
            CheckTabStuct(tableName, pkOrUniqueColName);

            var tableInfo = Info.TableInfoDict[tableName];

            if (tableInfo.PriKeyType == PrimaryKeyType.UNKNOWN)
            {
                return InsertScript<DT>(data, tableName, pkOrUniqueColName);
            }
            else
            {
                object pkOrUniqueColValue = null;

                if (TypeInfo<DT>.IsAnonymousType)
                {
                    PropertyInfo pkOrUniqueColPy;
                    if (TypeInfo<DT>.PropMapping.TryGetValue(pkOrUniqueColName, out pkOrUniqueColPy))
                    {
                        pkOrUniqueColValue = pkOrUniqueColPy.GetValue(data, null);
                    }
                }
                else if (TypeInfo<DT>.IsNameValueColl)
                {
                    var nvc = (data as NameValueCollection);
                    pkOrUniqueColValue = nvc[pkOrUniqueColName];
                }
                else if (TypeInfo<DT>.IsDict)
                {
                    var dict = (data as IDictionary<string, object>);
                    dict.TryGetValue(pkOrUniqueColName, out pkOrUniqueColValue);
                }
                else
                {
                    throw new ArgumentException("未知数据类型插入！", "data");
                }

                if (tableInfo.PriKeyType == PrimaryKeyType.AUTO)
                {
                    if (pkOrUniqueColValue == null || string.IsNullOrWhiteSpace(pkOrUniqueColValue.ToString()))
                    {
                        saveType = SaveType.Insert;
                        return InsertScript<DT>(data, tableName, pkOrUniqueColName);
                    }
                    else
                    {
                        saveType = SaveType.Update;
                        return UpdateScript<DT>(data, tableName, pkOrUniqueColName, excludeColNames);
                    }
                }
                else//PrimaryKeyType.SET
                {
                    if (pkOrUniqueColValue == null || string.IsNullOrWhiteSpace(pkOrUniqueColValue.ToString()))
                    {
                        throw new ArgumentException("主键列的值不能为空！[" + pkOrUniqueColName + "]");
                    }

                    if (!Exist(tableName, pkOrUniqueColName, pkOrUniqueColValue, null))
                    {
                        saveType = SaveType.Insert;
                        //return InsertScript<DT>(data, tableName, pkOrUniqueColName);
                        return InsertScript<DT>(data, tableName);
                    }
                    else
                    {
                        saveType = SaveType.Update;
                        return UpdateScript<DT>(data, tableName, pkOrUniqueColName, excludeColNames);
                    }
                }
            }
        }

        /// <summary>
        /// 保存策略：
        /// 1.如果表没有设置主键，则进行的是插入操作
        /// 2.如果表的主键是自增，如果data中主键有值，则进行更新操作，没有值则进行插入操作
        /// 3.如果表有主键，不是自增，则从数据查询，存在则更新，不存在则插入。注：如果主键列的值为空，则报异常！
        /// </summary>
        /// <typeparam name="DT"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <param name="pkOrUniqueColName"></param>
        /// <param name="excludeColNames"></param>
        /// <returns></returns>
        public virtual KeyValuePair<SaveType, bool> Save<DT>(DT data, string tableName, string pkOrUniqueColName = "Id", params string[] excludeColNames)
        {
            SaveType saveType;
            var kv = SaveScript(data, tableName, out saveType, pkOrUniqueColName, excludeColNames);
            bool res = ExecSql(kv.Key, kv.Value) > 0;
            var lstAct = DataChangeTriggers.GetActions(tableName);
            if (lstAct.Any())
            {
                foreach (var act in lstAct)
                {
                    act.Invoke();
                }
            }
            return new KeyValuePair<SaveType, bool>(saveType, res);
        }



        public virtual KeyValuePair<DataTable, long> GetDataTableByPager(int currentPage, int pageSize, string selColumns, string joinTableName, string whereStr, string orderbyStr)
        {
            long totalCount;
            DataTable data = GetDataTableByPager(currentPage, pageSize, selColumns, joinTableName, whereStr, orderbyStr, out totalCount);
            return new KeyValuePair<DataTable, long>(data, totalCount);
        }

        protected virtual DataTable GetDataTableByPager(int currentPage, int pageSize, string selColumns, string joinTableName, string whereStr, string orderbyStr, out long totalCount)
        {
            throw new NotImplementedException(DBType + "暂未支持");
        }

        public virtual bool DeleteAll(string tableName)
        {
            string strSql = "delete from " + ParameterSql(tableName);
            ExecSql(strSql, CreateParameter(tableName, tableName).TransArray());
            return true;
        }       
    }
}
