using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace MJTop.Data
{
    public partial class DBExtend
    {

        #region 将集合实体转为DataTable

        /// <summary>
        /// 将集合实体转为DataTable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="lstEntity">集合</param>
        /// <returns></returns>
        public static DataTable EntityToDataTable<T>(IEnumerable<T> lstEntity)
            where T:class,new()
        {
            return EntityToDataTable(lstEntity, null);
        }

        /// <summary>
        /// 将集合实体转为DataTable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="lstEntity">集合</param>
        /// <param name="tableName">DataTable表名</param>
        /// <returns></returns>
        public static DataTable EntityToDataTable<T>(IEnumerable<T> lstEntity, string tableName)
            where T : class, new()
        {
            DataTable data = new DataTable();
            Type ty = typeof(T);
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                data.TableName = tableName;
            }
            else
            {
                data.TableName = ty.Name;
            }            
            var props = ty.GetProperties();

            foreach (var prop in props)
            {
                data.Columns.Add(prop.Name, prop.PropertyType);
            }
            foreach (var entity in lstEntity)
            {
                DataRow dr = data.NewRow();
                foreach (DataColumn dc in data.Columns)
                {
                    var prop = ty.GetProperty(dc.ColumnName);
                    var value = prop.GetValue(entity, null);
                    dr[dc.ColumnName] = value;
                }
                data.Rows.Add(dr);
            }
            return data;
        }

        #endregion
        
        #region DataTable =》 GetCreateSqlScript

        /// <summary>
        /// 根据DataTable获取创建Sql语句
        /// </summary>
        /// <param name="data">DataTable</param>
        /// <param name="tableName">表名</param>
        /// <param name="DBType">数据库类型</param>
        /// <returns>创建表的Sql脚本</returns>
        public static string GetCreateSqlScript(DataTable data, string tableName, DBType DBType = DBType.SqlServer)
        {
            if (data == null)
            {
                throw new ArgumentException("数据表不能为null！", "data");
            }
            data.TableName = tableName;
            return GetCreateSqlScript(data, DBType);
        }

        /// <summary>
        /// 根据集合实体得到 Sql创建表脚本
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="lstEntity">集合</param>
        /// <param name="tableName">表名</param>
        /// <param name="DBType">数据库类型</param>
        /// <returns></returns>
        public static string GetCreateSqlScript<T>(IEnumerable<T> lstEntity, string tableName, DBType DBType = DBType.SqlServer)
            where T : class, new()
        {
            DataTable data = EntityToDataTable(lstEntity, tableName);
            return GetCreateSqlScript(data, DBType);
        }

        #region private
        /// <summary>
        /// 获取列的值的最大小数点占用位数
        /// </summary>
        /// <param name="data">DataTable</param>
        /// <returns></returns>
        private static Dictionary<string, int> GetColDecimals(DataTable data)
        {
            Dictionary<string, int> dictDecimals = new Dictionary<string, int>();
            List<Type> lstTy = new List<Type>()
            {
                typeof(double),
                typeof(decimal),
                typeof(float)
            };
            var rowColl = data.AsEnumerable();
            var lstCol = data.Columns.ToArray().Where(t => lstTy.Contains(t.DataType));//获取小数类型的列
            foreach (DataColumn dc in lstCol)
            {
                long lngTemp;
                var currArr = rowColl.Select(t => t[dc.ColumnName].ToString()); //得到当前列的 ToString()
                currArr = currArr.Where(t => t.Length > 0 && !long.TryParse(t, out lngTemp));//不是空字符串，并且Parse失败则才算是真正的小数，过滤 小数点后都是0的数值
                if (currArr.Any())
                {
                    var currArrInt = currArr.Select(t => (Regex.Replace(t, @"(\d+)\.(\d+)", "$2", RegexOptions.Compiled)).Length);
                    int decimals = currArrInt.Max();
                    dictDecimals[dc.ColumnName] = decimals;
                }
            }
            return dictDecimals;
        }

        /// <summary>
        /// 根据DataTable获取创建Sql语句
        /// </summary>
        /// <param name="data">DataTable</param>
        /// <param name="DBType">数据库类型</param>
        /// <returns>创建表的Sql脚本</returns>
        private static string GetCreateSqlScript(DataTable data, DBType DBType = DBType.SqlServer)
        {
            if (data == null)
            {
                throw new ArgumentException("数据表不能为null！", "data");
            }
            if (string.IsNullOrWhiteSpace(data.TableName))
            {
                throw new ArgumentException("表名不能为空！");
            }
            return GetCreateSqlScript(data, data.Columns, DBType);
        }

        /// <summary>
        /// 根据 表名，DataColumn[]获取创建Sql语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dcs">所有列</param>
        /// <param name="DBType">数据库类型</param>
        /// <returns>创建表的Sql脚本</returns>
        private static string GetCreateSqlScript(DataTable data, DataColumnCollection dcs, DBType DBType = DBType.SqlServer)
        {
            //获取主键
            DataColumn[] primaryKey = data.PrimaryKey;
            if (dcs == null)
            {
                throw new ArgumentException("列集合不能为null！");
            }
            if (dcs.Count <= 0)
            {
                throw new ArgumentException("列集合个数必须大于0！");
            }
            Dictionary<string, int> dictDecimals = GetColDecimals(data);

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("create table {0}", data.TableName);
            sbSql.Append("(");
            for (int j = 0; j < dcs.Count; j++)
            {
                var dc = dcs[j];
                string colSqlType = GetColSqlType(dc, DBType, dictDecimals);
                if (dc.AutoIncrement) //是自增列
                {
                    colSqlType += " identity(" + dc.AutoIncrementSeed + "," + dc.AutoIncrementStep + ") ";
                }
                if (primaryKey.Contains(dc))
                {
                    colSqlType += " primary key ";
                }
                colSqlType = colSqlType + ((j != dcs.Count - 1) ? "," : "");
                sbSql.Append(colSqlType);
            }
            sbSql.Append(")");
            string sql = sbSql.ToString();
            return sql;
        }

        /// <summary>
        /// 返回Sql列类型
        /// </summary>
        /// <param name="dc">列</param>
        /// <param name="DBType">数据库类型</param>
        /// <param name="dictDecimals">列的值的最大小数点占用位数</param>
        /// <returns>Sql列类型</returns>
        private static string GetColSqlType(DataColumn dc, DBType DBType, Dictionary<string, int> dictDecimals)
        {
            string res = string.Empty;
            string columnName = dc.ColumnName;
            Type typecol = Nullable.GetUnderlyingType(dc.DataType) ?? dc.DataType;
            switch (typecol.FullName)
            {
                case "System.Guid":
                    if (DBType == DBType.SqlServer)
                    {
                        res = columnName + " uniqueidentifier";
                    }
                    else
                    {
                        res = columnName + " char(36)";
                    }
                    break;
                case "System.Boolean":
                    if (DBType == DBType.SqlServer || DBType == DBType.MySql)
                    {
                        res = columnName + " bit";
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        res = columnName + " number(1,0)";
                    }
                    else if (DBType == DBType.SQLite)
                    {
                        res = columnName + " integer";
                    }
                    break;
                case "System.Int32":
                case "System.Int64":
                    if (DBType == DBType.SqlServer || DBType == DBType.MySql)
                    {
                        res = columnName + " bigint";
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        res = columnName + " number(38,0)";
                    }
                    else if (DBType == DBType.SQLite)
                    {
                        res = columnName + " integer";
                    }
                    break;
                case "System.Decimal":
                case "System.Double":
                case "System.Single":
                    if (DBType == DBType.SqlServer || DBType == DBType.MySql)
                    {
                        if (dictDecimals != null && dictDecimals.ContainsKey(dc.ColumnName))
                        {
                            res = columnName + " decimal(38," + dictDecimals[dc.ColumnName] + ")";
                        }
                        else
                        {
                            res = columnName + " bigint";
                        }
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        if (dictDecimals != null && dictDecimals.ContainsKey(dc.ColumnName))
                        {
                            res = columnName + " number(38," + dictDecimals[dc.ColumnName] + ")";
                        }
                        else
                        {
                            res = columnName + " number(38,0)";
                        }

                    }
                    else if (DBType == DBType.SQLite)
                    {
                        if (dictDecimals != null && dictDecimals.ContainsKey(dc.ColumnName))
                        {
                            res = columnName + " real(38," + dictDecimals[dc.ColumnName] + ")";
                        }
                        else
                        {
                            res = columnName + " integer";
                        }
                    }
                    break;
                case "System.DateTime":
                    if (DBType == DBType.SqlServer || DBType == DBType.MySql || DBType == DBType.SQLite)
                    {
                        res = columnName + " datetime";
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        res = columnName + " date";
                    }
                    break;
                case "System.Byte[]":
                    //先把数据存进去，使用max
                    if (DBType == DBType.SqlServer)
                    {
                        res = columnName + " varbinary(max)";
                    }
                    else if (DBType == DBType.MySql || DBType == DBType.SQLite)
                    {
                        res = columnName + " blob";
                    }
                    break;
                default:
                    //先把数据存进去，使用max
                    if (DBType == DBType.SqlServer)
                    {
                        res = columnName + " nvarchar(max)";
                    }
                    else if (DBType == DBType.MySql || DBType == DBType.SQLite)
                    {
                        res = columnName + " text";
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        res = columnName + " nclob";
                    }
                    break;
            }

            return res;
        }

        #endregion

        #endregion

        #region 反射对象类型的属性生成创建脚本
        public static string GetCreateSqlScript(Type ty, DBType DBType = DBType.SqlServer)
        {
            var props = ty.GetProperties();
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("create table {0}", ty.Name);
            sbSql.Append("(");
            for (int j = 0; j < props.Length; j++)
            {
                PropertyInfo pInfo = props[j];
                string colType = GetColTypeByProperty(pInfo, DBType);
                sbSql.Append(colType + " " + ((j == props.Length - 1) ? "" : ","));
            }
            sbSql.Append(")");
            return sbSql.ToString();
        }
        private static string GetColTypeByProperty(PropertyInfo pInfo, DBType DBType)
        {
            string res = string.Empty;
            string columnName = pInfo.Name;
            Type typecol = Nullable.GetUnderlyingType(pInfo.PropertyType) ?? pInfo.PropertyType;
            switch (typecol.FullName)
            {
                case "System.Guid":
                    if (DBType == DBType.SqlServer)
                    {
                        res = columnName + " uniqueidentifier";
                    }
                    else
                    {
                        res = columnName + " char(36)";
                    }
                    break;
                case "System.Boolean":
                    if (DBType == DBType.SqlServer || DBType == DBType.MySql)
                    {
                        res = columnName + " bit";
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        res = columnName + " number(1,0)";
                    }
                    else if (DBType == DBType.SQLite)
                    {
                        res = columnName + " integer";
                    }
                    break;
                case "System.Int32":
                case "System.Int64":
                    if (DBType == DBType.SqlServer || DBType == DBType.MySql)
                    {
                        res = columnName + " bigint";
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        res = columnName + " number(38,0)";
                    }
                    else if (DBType == DBType.SQLite)
                    {
                        res = columnName + " integer";
                    }
                    break;
                case "System.Decimal":
                case "System.Double":
                case "System.Single":
                    if (DBType == DBType.SqlServer || DBType == DBType.MySql)
                    {
                        res = columnName + " decimal(38," + 2 + ")";
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        res = columnName + " number(38,," + 2 + ")";

                    }
                    else if (DBType == DBType.SQLite)
                    {
                        res = columnName + " real(38," + 2 + ")";
                    }
                    break;
                case "System.DateTime":
                    if (DBType == DBType.SqlServer || DBType == DBType.MySql || DBType == DBType.SQLite)
                    {
                        res = columnName + " datetime";
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        res = columnName + " date";
                    }
                    break;
                case "System.Byte[]":
                    //先把数据存进去，使用max
                    if (DBType == DBType.SqlServer)
                    {
                        res = columnName + " varbinary(max)";
                    }
                    else if (DBType == DBType.MySql || DBType == DBType.SQLite)
                    {
                        res = columnName + " blob";
                    }
                    break;
                default:
                    //先把数据存进去，使用max
                    if (DBType == DBType.SqlServer)
                    {
                        res = columnName + " nvarchar(max)";
                    }
                    else if (DBType == DBType.MySql || DBType == DBType.SQLite)
                    {
                        res = columnName + " text";
                    }
                    else if (DBType == DBType.OracleDDTek || DBType == DBType.Oracle)
                    {
                        res = columnName + " nclob";
                    }
                    break;
            }

            return res;
        } 
        #endregion
        
    }
}
