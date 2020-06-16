using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace MJTop.Data.SPI
{
    public interface IDB
    {
        //void BeginTran();
        //void BeginTran(IsolationLevel isolationLevel);
        //void Commit();
        //void Rollback();



        #region AOP拦截

        Action<DbCommand> OnExecuting { get; set; }

        Action<DbCommand, object> OnExecuted { get; set; }

        Action<DbCommand, Exception> OnError { get; set; }

        #endregion

        TableTrigger DataChangeTriggers { get; }

        NameValueCollection GetInsertExcludeColumns();

        ExcludeColumn InsertExcludeColumns { get; }

        NameValueCollection GetUpdateExcludeColumns();

        ExcludeColumn UpdateExcludeColumns { get; }

        #region Bool查询

        bool TryConnect();

        bool ValidateSql(string strSql, out Exception ex);

        void CheckTabStuct(string tableName, params string[] columnNames);

        #endregion


        int RunStoreProc(string storeProcName, object parms = null);

        DataTable RunStoreProcGetDT(string storeProcName, object parms = null);

        DataSet RunStoreProcGetDS(string storeProcName, object parms = null);


        #region 基础查询

        TRet Scalar<TRet>(string strSql, TRet defRet, object parms = null);

        NameValueCollection GetFirstRow(string strSql, object parms = null);

        DataTable GetDataTable(string strSql, object parms = null);

        List<DataTable> GetListTable(string strSql, object parms = null);

        DataSet GetDataSet(string strSql, object parms = null);
        
        DbDataReader ExecReader(string commandText, object parms = null, CommandType commandType = CommandType.Text);

        TRet Single<TRet>(string strSql, TRet defRet, object parms = null);

        List<Dictionary<string, object>> GetListDictionary(string strSql, object parms = null);

        DataTable ReadTable(string strSql, object parms = null);

        List<TRet> ReadList<TRet>(string strSql, object parms = null);

        NameValueCollection ReadNameValues(string strSql, object parms = null);

        Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(string strSql, object parms = null, IEqualityComparer<TKey> comparer = null);

        #endregion

        #region 执行

        int ExecSql(string strSql, object parms = null);
        int ExecSqlTran(string strSql, object parms = null);
        int ExecSqlTran(params string[] sqlCmds);
        int ExecSqlTran(List<KeyValuePair<string, List<DbParameter>>> strSqlList);


        #endregion

        int BulkInsert(string tableName, DataTable data, int batchSize = 200000, int timeout = 60);

        int BulkInsert(string tableName, DbDataReader reader, int batchSize = 200000, int timeout = 60);

 
        #region 根据表名，列名，列相关操作

        bool Exist(string tableName, string columnName, object columnValue, params object[] excludeValues);

        bool Insert<DT>(DT data, string tableName, params string[] excludeColNames);

        int InsertGetInt<DT>(DT data, string tableName, params string[] excludeColNames);

        long InsertGetLong<DT>(DT data, string tableName, params string[] excludeColNames);

        bool Update<DT>(DT data, string tableName, string pkOrUniqueColName = "Id", params string[] excludeColNames);

        KeyValuePair<SaveType, bool> Save<DT>(DT data, string tableName, string pkOrUniqueColName = "Id", params string[] excludeColNames);

        bool UpSingle(string tableName, string columnName, object columnValue, object pkOrUniqueValue, string pkOrUniqueColName = "Id");

        bool DeleteAll(string tableName);

        int Delete(string tableName, object pkOrUniqueColName);

        int Delete(string tableName, string columnName, params object[] columnValues);

        #endregion

        T Get<T>(string tableName, object parms) where T : class, new();

        T GetById<T>(string tableName, object pkValue) where T : class, new();

        List<T> GetByIds<T>(string tableName, object[] pkValues) where T : class, new();

        List<T> GetList<T>(string tableName, object parms) where T : class, new();

        List<T> GetList<T>(string strSql) where T : class, new();

        DataTable SelectAll(string tableName, string orderbyStr = null);

        DataTable SelectTop(string tableName, int top, string orderbyStr = null);

        long SelectCount(string tableName, string whereAndStr = null);

        DataTable SelectTable(string joinTableName, string whereStr, string orderbyStr);
               
        KeyValuePair<DataTable, long> GetDataTableByPager(int currentPage, int pageSize, string selColumns, string joinTableName, string whereStr, string orderbyStr);
    }
}
