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

namespace MJTop.Data
{
    public partial class DB
    {
        #region 实体相关
        public virtual TEntity GetById<TEntity>(string tableName, object pkValue) where TEntity : class, new()
        {
            TableInfo tabInfo = Info.TableInfoDict[tableName];
            ColumnInfo colInfo = Info[tableName, tabInfo.PriKeyColName];
            string strSql = "select * from " + tableName + " where " + colInfo.ColumnName + "=" + ParameterSql(colInfo.ColumnName);
            DataTable data = GetDataTable(strSql, CreateParameter(colInfo.ColumnName, pkValue, colInfo).TransArray());
            return BWofter.Converters.Data.DataTableConverter<TEntity>.ToEntities(data).ToList()?.FirstOrDefault();
        }

        public virtual List<TEntity> GetByIds<TEntity>(string tableName, object[] pkValues) where TEntity : class, new()
        {
            TableInfo tabInfo = Info.TableInfoDict[tableName];
            ColumnInfo colInfo = Info[tableName, tabInfo.PriKeyColName];
            string strSql = "select * from " + tableName + " where " + colInfo.ColumnName + " in ({0})";
            if (pkValues.Length <= 0)
            {
                return new List<TEntity>();
            }
            strSql = string.Format(strSql, ParameterSql(colInfo.ColumnName).Repeater(pkValues.Length, ","));

            List<DbParameter> lstpara = new List<DbParameter>();
            foreach (var value in pkValues)
            {
                lstpara.Add(CreateParameter(colInfo.ColumnName, value, colInfo));
            }
            return BWofter.Converters.Data.DataTableConverter<TEntity>.ToEntities(GetDataTable(strSql, lstpara)).ToList();
        }

        public TEntity Get<TEntity>(string tableName, object whereParameters = null)
            where TEntity : class, new()

        {
            return GetList<TEntity>(tableName, whereParameters).FirstOrDefault();
        }

        public List<TEntity> GetList<TEntity>(string tableName, object whereParameters)
            where TEntity : class, new()

        {
            string strSql = "select * from " + tableName;
            List<DbParameter> lstPara = new List<DbParameter>();
            List<string> lstWhere = new List<string>();
            if (whereParameters != null)
            {
                if (whereParameters is System.Collections.IDictionary)
                {
                    IDictionary dict = whereParameters as IDictionary;
                    if (dict != null && dict.Count > 0)
                    {
                        foreach (DictionaryEntry kv in dict)
                        {
                            var parameter = CreateParameter();
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
                            lstWhere.Add(kv.Key.ToString() + "=" + ParameterSql(kv.Key.ToString()));
                            lstPara.Add(parameter);
                        }
                    }
                }
                else if (whereParameters is NameValueCollection)
                {
                    NameValueCollection nvc = whereParameters as NameValueCollection;
                    if (nvc != null && nvc.Count > 0)
                    {
                        foreach (var key in nvc.AllKeys)
                        {
                            var parameter = CreateParameter();
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
                            lstWhere.Add(key + "=" + ParameterSql(key));
                            lstPara.Add(parameter);
                        }
                    }
                }
                else
                {
                    Type ty = whereParameters.GetType();

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
                                var parameter = CreateParameter();
                                parameter.ParameterName = ParameterChar + prop.Name;
                                parameter.Value = prop.GetValue(whereParameters, null);

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
                                lstWhere.Add(prop.Name + "=" + ParameterSql(prop.Name));
                                lstPara.Add(parameter);
                            }
                        }
                    }
                    else
                    {
                        //其他类型
                        throw new ArgumentException("不支持其他类型参数！", nameof(whereParameters));
                    }
                }
            }
            strSql = strSql + (lstWhere.Any() ? " where " : "") + string.Join(" and ", lstWhere);
            DataTable data = GetDataTable(strSql, lstPara);
            //return BWofter.Converters.Data.DataTableConverter<TEntity>.ToEntities(data).ToList();
            return data.ConvertToListObject<TEntity>();
        }

        public List<TEntity> GetList<TEntity>(string strSql) where TEntity : class, new()
        {
            DataTable data = GetDataTable(strSql);
            //return BWofter.Converters.Data.DataTableConverter<TEntity>.ToEntities(data).ToList();
            return data.ConvertToListObject<TEntity>();
        }

        #endregion

        public virtual DataTable SelectTable(string joinTableName, string whereStr, string orderbyStr)
        {
            if (!string.IsNullOrWhiteSpace(whereStr))
            {
                whereStr = Regex.Replace(whereStr, @"(\s)*(where)?(\s)*(.+)", "where 1=1 and $3$4", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            if (!string.IsNullOrWhiteSpace(orderbyStr))
            {
                orderbyStr = Regex.Replace(orderbyStr, @"(\s)*(order)(\s)+(by)(.+)", "$5", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            else
            {
                throw new ArgumentNullException("orderbyStr");
            }

            string strSql = "select * from {0}  {1} order by {2}";
            strSql = string.Format(strSql, joinTableName, whereStr, orderbyStr);

            return GetDataTable(strSql);
        }
        public virtual bool Exist(string tableName, string columnName, object columnValue)
        {
            return Exist(tableName, columnName, columnValue, null);
        }
        public virtual bool Exist(string tableName, string columnName, object columnValue, params object[] excludeValues)
        {
            string exist_sql = "select count(1) from " + tableName + " where " + columnName + "='" + columnValue + "' ";
            if (excludeValues != null && excludeValues.Length > 0)
            {
                string in_sql = Script.SqlIn(columnName, excludeValues, true);
                exist_sql += in_sql;
            }
            return Single<int>(exist_sql, 0) > 0;
        }

        public virtual bool UpSingle(string tableName, string columnName, object columnValue, object pkOrUniqueValue, string pkOrUniqueColName = "Id")
        {
            string upSql = string.Empty;
            upSql = "update " + tableName + " set " + columnName + "=" + ParameterSql(columnName) + " where " + pkOrUniqueColName + "=" + ParameterSql(pkOrUniqueColName);

            var p1 = CreateParameter(columnName, columnValue, Info[tableName, columnName]);
            var p2 = CreateParameter(pkOrUniqueColName, pkOrUniqueValue, Info[tableName, pkOrUniqueColName]);

            bool res = ExecSql(upSql, new DbParameter[] { p1, p2 }) > 0;

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
        public virtual int Delete(string tableName, string columnName, params object[] columnValues)
        {
            if (columnValues != null && columnValues.Length <= 0
                && !Info.IsExistColumn(tableName, columnName))
            {
                return Delete(tableName, (object)columnName);
            }

            if (!Info.IsExistTable(tableName))
            {
                return -1;
            }

            string delSql = "delete from " + tableName + " where 1=1 " + Script.SqlInByDBType(Info[tableName, columnName], columnValues, this.DBType);
            int res = ExecSql(delSql);
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

        public virtual bool Delete(string tableName)
        {
            if (!Info.IsExistTable(tableName))
            {
                return false;
            }
            string delSql = "delete from " + tableName;
            //ExecSql(delSql, new DbParameter[] { CreateParameter(tableName, tableName) });
            ExecSql(delSql);
            return true;
        }

        public virtual int Delete(string tableName, object pkValue)
        {
            if (!Info.IsExistTable(tableName))
            {
                return -1;
            }
            string delSql = string.Empty;
            delSql = "delete from " + tableName + " where " + Info.TableInfoDict[tableName].PriKeyColName + "=" + ParameterSql(Info.TableInfoDict[tableName].PriKeyColName);
            int res = ExecSql(delSql, new DbParameter[] {  CreateParameter(Info.TableInfoDict[tableName].PriKeyColName, pkValue) });
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
        /// 根据表名查数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="orderbyStr">排序字段+asc/desc</param>
        /// <returns>DataTable</returns>
        public virtual DataTable SelectAll(string tableName, string orderbyStr = null)
        {
            if (!Info.IsExistTable(tableName))
            {
                return null;
            }
            string selSql = string.Format("select * from {0} ", tableName);
            if (!string.IsNullOrWhiteSpace(orderbyStr))
            {
                selSql += " order by " + orderbyStr;
            }
            return GetDataTable(selSql);
        }

        /// <summary>
        /// 取表中的前top条数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="top">取多少条</param>
        /// <param name="orderbyStr">排序字段+asc/desc</param>
        /// <returns>DataTable</returns>
        public virtual DataTable SelectTop(string tableName, int top, string orderbyStr = null)
        {
            throw new NotImplementedException(DBType + "暂未支持");
        }

        /// <summary>
        /// 获取数据条数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="whereAndStr">and过滤条件</param>
        /// <returns>总条数</returns>
        public virtual long SelectCount(string tableName, string whereAndStr = null)
        {
            if (!Info.IsExistTable(tableName))
            {
                return -1;
            }
            string selSql = string.Format("select count(1) from {0} ", tableName);
            if (!string.IsNullOrWhiteSpace(whereAndStr))
            {
                selSql += " where 1=1  " + whereAndStr;
            }
            return Scalar<long>(selSql, 0);
        }
    }
}
