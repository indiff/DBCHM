using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Top._51Try.Data.SPI
{
    public interface IDBEntity
    {
        //用一个算法，解析出一种插入的解决方案，然后进行缓存，下次插入时直接调用
        /**
         *  1. 先用 表达式树的 方式 能够将字段 高效的处理好
         *  2. 
         *  
         */

        bool Insert<DT>(DT data, string tableName, params string[] excludeColNames);

        bool Update<DT>(DT data, string tableName, string pkOrUniqueColName = "Id", params string[] excludeColNames);

        bool Upsert<DT>(DT data, string tableName, string pkOrUniqueColName = "Id", params string[] excludeColNames);

        bool UpSingle(string tableName, string columnName, object columnValue, object pkOrUniqueValue, string pkOrUniqueColName = "Id");

        int Delete<P>(string tableName, string columnName, params P[] columnValues);

        DataTable GetDataTableByPager(int currentPage, int pageSize, string selColumns, string joinTableName, string whereStr, string orderbyStr, out int totalCount);

        T GetById<T,P>(P IdValue, string pkOrUniqueColName = "Id");

        List<T> GetByIds<T, P>(string pkOrUniqueColName = "Id", params P[] ids);


        //获取某个表的 某条数据的 某列的值
        TReturn QuerySingleById<TReturn>(string tableName, string retColumnName, object Idvalue, string pkOrUniqueColName = "Id");


        //新增、编辑的时候，判断唯一键值 使用
        bool ExistByColVal<P>(string tableName, string columnName, object columnValue, params P[] excludeValues);

        //List<T> GetList<T>(string whereStr, string orderByStr = null);

        //List<T> GetAll<T>(string orderByStr = null);

    }
}
