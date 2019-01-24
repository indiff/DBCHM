using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTop.Data
{
    /// <summary>
    /// 拼接Sql脚本类
    /// </summary>
    public class Script
    {
        #region SqlIn/SqlLike

        /// <summary>
        /// 拼接in sql语句
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="columnName">列名</param>
        /// <param name="values">元素值</param>
        /// <param name="isNotIn">not in 或 in </param>
        /// <returns>and开头的 in sql语句</returns>
        public static string SqlIn<T>(string columnName, T[] values, bool isNotIn = false)
        {
            string result = string.Empty;
            if (values == null || values.Length <= 0)
            {
                return string.Empty;
            }
            bool isValueType = TypeInfo<T>.IsValueType;
            List<string> lst = new List<string>();
            foreach (T obj in values)
            {
                if (obj != null)
                {
                    string val = obj.ToString();
                    if (val.StartsWith("'") && val.EndsWith("'"))
                    {
                        val = val.Replace("'", "'''");
                        lst.Add(val);
                        continue;
                    }
                    if (!isValueType)
                    {
                        val = "'" + val + "'";
                    }
                    lst.Add(val);
                }
            }
            if (lst.Count > 0)
            {
                result = " and " + columnName + " " + (isNotIn ? "not" : "") + " in (" + string.Join(",", lst) + ") ";
            }
            return result;
        }


        /// <summary>
        /// 拼接in sql语句
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="columnName">列名</param>
        /// <param name="values">元素值</param>
        /// <param name="isNotIn">not in 或 in </param>
        /// <returns>and开头的 in sql语句</returns>
        public static string SqlInByDBType(ColumnInfo columnInfo, object[] values, DBType dBType = DBType.SqlServer, bool isNotIn = false)
        {
            string result = string.Empty;
            if (values == null || values.Length <= 0)
            {
                return string.Empty;
            }
            List<string> lst = new List<string>();
            foreach (object obj in values)
            {
                if (obj != null)
                {
                    string val = obj.ToString();
                    if (val.StartsWith("'") && val.EndsWith("'"))
                    {
                        val = val.Replace("'", "'''");
                        lst.Add(val);
                        continue;
                    }
                    if (columnInfo.LikeType != LikeType.Number)
                    {
                        val = "'" + val + "'";
                    }
                    lst.Add(val);
                }
            }
            if (lst.Count > 0)
            {
                result = " and " + columnInfo.ColumnName + " " + (isNotIn ? "not" : "") + " in (" + string.Join(",", lst) + ") ";
            }
            return result;
        }


        /// <summary>
        /// 拼接多条 like 语句
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="columnName">列名</param>
        /// <param name="values">元素值</param>
        /// <param name="isOrLike">or like 或 and like</param>
        /// <returns>and开头的 多条 like 语句</returns>
        public static string SqlLike<T>(string columnName, T[] values, bool isOrLike = true)
        {
            string result = string.Empty;
            if (values == null || values.Length <= 0)
            {
                return string.Empty;
            }
            List<string> lst = new List<string>();
            foreach (T obj in values)
            {
                if (obj != null)
                {
                    string like_sql = columnName + " like '%{0}%' ";
                    string temp_sql = string.Empty;
                    string val = obj.ToString();
                    if (val.StartsWith("'") && val.EndsWith("'"))
                    {
                        val = val.Replace("'", "''");
                        temp_sql = string.Format(like_sql, val);
                        lst.Add(temp_sql);
                        continue;
                    }
                    temp_sql = string.Format(like_sql, val);
                    lst.Add(temp_sql);
                }
            }
            if (lst.Count > 0)
            {
                result = " and (" + (string.Join((isOrLike ? " or" : " and "), lst)) + ") ";
            }
            return result;
        }

        #endregion
        

        /// <summary>
        /// 插入后查询自增列的值
        /// </summary>
        /// <param name="type">数据库类型</param>
        /// <param name="tableName">表名</param>
        /// <param name="sequenceName">序列名称</param>
        /// <param name="identityColName">自增列的列名</param>
        /// <returns>查询自增Id的脚本</returns>
        public static string IdentitySql(DBType type, string tableName = null, string sequenceName = null, string identityColName = null)
        {
            switch (type)
            {
                case DBType.SqlServer:
                    if (string.IsNullOrWhiteSpace(tableName))
                    {
                        throw new ArgumentException(type + "，查询自增Id的值，不能为空！", "tableName");
                    }
                    return string.Format("select Ident_Current('{0}')", tableName);

                case DBType.MySql:
                    return string.Format("select @@Identity");

                case DBType.Oracle:
                case DBType.OracleDDTek:

                    if (string.IsNullOrWhiteSpace(sequenceName))
                    {
                        throw new ArgumentException(type + "，查询自增Id的值，不能为空！", "sequenceName");
                    }
                    return string.Format("select {0}.currval from dual", sequenceName);

                case DBType.PostgreSql:

                    if (string.IsNullOrWhiteSpace(tableName))
                    {
                        throw new ArgumentException(type + "，查询自增Id的值，不能为空！", "tableName");
                    }

                    if (string.IsNullOrWhiteSpace(identityColName))
                    {
                        throw new ArgumentException(type + "，查询自增Id的值，不能为空！", "identityColName");
                    }
                    //只支持表定义时定义的：smallserial，serial，bigserial
                    //不支持创建 序列+触发器 时的实现。
                    return string.Format("select currval(pg_get_serial_sequence('{0}', '{1}'));", tableName.ToLower(), identityColName.ToLower());

                case DBType.SQLite:
                    return string.Format("select last_insert_rowid()");
                default:
                    throw new ArgumentException("不支持的数据库类型！");
            }
        }
    }
}
