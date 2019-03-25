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


        #endregion


        int RunStoreProc(string storeProcName, object parameters = null);

        DataTable RunStoreProcGetDT(string storeProcName, object parameters = null);

        DataSet RunStoreProcGetDS(string storeProcName, object parameters = null);


        #region 基础查询

        TRet Scalar<TRet>(string strSql, TRet defRet, object parameters = null);

        NameValueCollection GetFirstRow(string strSql, object parameters = null);

        DataTable GetDataTable(string strSql, object parameters = null);

        List<DataTable> GetListTable(string strSql, object parameters = null);

        DataSet GetDataSet(string strSql, object parameters = null);
        
        DbDataReader ExecReader(string commandText, object parameters = null, CommandType commandType = CommandType.Text);

        TRet Single<TRet>(string strSql, TRet defRet, object parameters = null);

        List<Dictionary<string, object>> GetListDictionary(string strSql, object parameters = null);

        DataTable ReadTable(string strSql, object parameters = null);

        List<TRet> ReadList<TRet>(string strSql, object parameters = null);

        NameValueCollection ReadNameValues(string strSql, object parameters = null);

        Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(string strSql, object parameters = null, IEqualityComparer<TKey> comparer = null);

        #endregion

        #region 执行

        int ExecSql(string strSql, object parameters = null);
        int ExecSqlTran(string strSql, object parameters = null);
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

        int Delete(string tableName, object pkColName);

        int Delete(string tableName, string columnName, params object[] columnValues);

        #endregion

        TEntity Get<TEntity>(string tableName, object whereParameters) where TEntity : class, new();

        TEntity GetById<TEntity>(string tableName, object pkValue) where TEntity : class, new();

        List<TEntity> GetByIds<TEntity>(string tableName, object[] pkValues) where TEntity : class, new();

        List<TEntity> GetList<TEntity>(string tableName, object whereParameters) where TEntity : class, new();

        List<TEntity> GetList<TEntity>(string strSql) where TEntity : class, new();

        DataTable SelectAll(string tableName, string orderbyStr = null);

        DataTable SelectTop(string tableName, int top, string orderbyStr = null);

        long SelectCount(string tableName, string whereAndStr = null);

        DataTable SelectTable(string joinTableName, string whereStr, string orderbyStr);
               
        KeyValuePair<DataTable, long> GetDataTableByPager(int currentPage, int pageSize, string selColumns, string joinTableName, string whereStr, string orderbyStr);
    }
}
