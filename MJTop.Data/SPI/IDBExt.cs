using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SharpDB.SPI
{
    public interface IDBExt
    {

        bool Insert(object obj, string tableName, params string[] excludeColNames);

        bool Update(object obj, string tableName, string pkOrUniqueColName = "Id", params string[] excludeColNames);

        bool Upsert(object obj, string tableName, string pkOrUniqueColName = "Id", params string[] excludeColNames);

        bool UpSingle(string tableName, string columnName, object columnValue, object pkOrUniqueValue, string pkOrUniqueColName = "Id");

        int Delete(string tableName, string columnName, params object[] columnValues);
        
        DataTable GetDataTableByPager(int currentPage, int pageSize, string selColumns, string joinTableName, string whereStr, string orderbyStr, out int totalCount);

        T GetEntity<T>(object IdValue, string pkOrUniqueColName = "Id");

        //获取某个表的 某条数据的 某列的值
        TRet QuerySingle<T, TRet>(string retColumnName, object Idvalue, string pkOrUniqueColName = "Id");

        //新增、编辑的时候，判断唯一键值 使用
        bool ExistByColVal(string tableName, string columnName, object columnValue, params object[] excludeValues);

        List<T> GetList<T>(string whereStr = null, string orderByStr = null);

    }
}
