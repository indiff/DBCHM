using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTop.Data.SPI
{
    public interface IDBInfo
    {
        Tool Tools { get; }

        string DBName { get; }

        string Version { get; }

        double VersionNumber { get; }

        /// <summary>
        /// 注意：刷新数据失败的情况，根据返回值做对应处理。
        /// </summary>
        /// <returns></returns>
        bool Refresh();

        List<string> TableNames { get; }
        
        NameValueCollection TableComments { get; }

        IgCaseDictionary<TableInfo> TableInfoDict { get; }

        IgCaseDictionary<List<string>> TableColumnNameDict { get; }

        IgCaseDictionary<List<ColumnInfo>> TableColumnInfoDict { get; }
        
        IgCaseDictionary<NameValueCollection> TableColumnComments { get; }

        NameValueCollection Views { get; }

        NameValueCollection Procs { get; }

        [Obsolete("注意：Oralce暂不支持查询所有数据库名称。", false)]
        List<string> DBNames { get; }

        List<string> this[string tableName] { get; }

        ColumnInfo this[string tableName, string columnName] { get; }
        
        bool IsExistTable(string tableName);

        bool IsExistColumn(string tableName, string columnName);

        string GetColumnComment(string tableName, string columnName);
        
        string GetTableComment(string tableName);

        List<ColumnInfo> GetColumns(string tableName);

        bool SetTableComment(string tableName, string comment);

        bool SetColumnComment(string tableName, string columnName, string comment);

        bool DropTable(string tableName);

        bool DropColumn(string tableName, string columnName);

        [Obsolete("注意：MySql的所有表存储引擎必须为MyISAM 方才查询支持，方才查询准确。", false)]
        Dictionary<string, DateTime> GetTableStruct_Modify();
        
    }
}
