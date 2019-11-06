using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MJTop.Data
{
    /// <summary>
    /// 数据对象扩展方法类
    /// </summary>
    public static class ExtData
    {
        #region  DataTable 转实体类型数据 方法1


        /// <summary>
        /// 根据类型的属性集合，匹配修改DataTable列名（用来处理属性名与列名不一致的大小写问题）
        /// </summary>
        /// <typeparam name="P">对象类型</typeparam>
        /// <param name="data">要修改列名的DataTable</param>
        //public static void CompareModify<P>(this DataTable data)
        //{
        //    var propNames = TypeInfo<P>.PropNames;

        //    foreach (string propName in propNames)
        //    {
        //        if (data.Columns[propName] != null)
        //        {
        //            data.Columns[propName].ColumnName = propName;
        //        }
        //    }
        //}

        /// <summary>
        /// DataTable 转实体类型数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="dt">数据源</param>
        /// <param name="IsCompareModify">是否匹配修改DataTable列名（用来处理属性名与列名不一致的大小写问题）</param>
        /// <returns>对象集合</returns>
        //public static List<T> ToList<T>(this DataTable dt, bool IsCompareModify = false) where T : class
        //{
        //    if (dt == null || dt.Rows.Count <= 0)
        //    {
        //        return new List<T>();
        //    }

        //    if (IsCompareModify)
        //    {
        //        CompareModify<T>(dt);
        //    }

        //    List<T> list = new List<T>();
        //    if (dt == null) return list;
        //    DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilder(dt.Rows[0]);
        //    foreach (DataRow info in dt.Rows)
        //        list.Add(eblist.Build(info));
        //    dt.Dispose();
        //    dt = null;
        //    return list;
        //}

        //internal class DataTableEntityBuilder<T>
        //{
        //    private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
        //    private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });
        //    private delegate T Load(DataRow dataRecord);

        //    private Load handler;
        //    private DataTableEntityBuilder() { }

        //    public T Build(DataRow dataRecord)
        //    {
        //        return handler(dataRecord);
        //    }

        //    public static DataTableEntityBuilder<T> CreateBuilder(DataRow dataRow)
        //    {
        //        DataTableEntityBuilder<T> dynamicBuilder = new DataTableEntityBuilder<T>();
        //        DynamicMethod method = new DynamicMethod("DynamicCreateEntity", typeof(T), new Type[] { typeof(DataRow) }, typeof(T), true);
        //        ILGenerator generator = method.GetILGenerator();
        //        LocalBuilder result = generator.DeclareLocal(typeof(T));
        //        generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
        //        generator.Emit(OpCodes.Stloc, result);

        //        for (int index = 0; index < dataRow.ItemArray.Length; index++)
        //        {
        //            PropertyInfo propertyInfo = typeof(T).GetProperty(dataRow.Table.Columns[index].ColumnName);
        //            Label endIfLabel = generator.DefineLabel();
        //            if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
        //            {
        //                generator.Emit(OpCodes.Ldarg_0);
        //                generator.Emit(OpCodes.Ldc_I4, index);
        //                generator.Emit(OpCodes.Callvirt, isDBNullMethod);
        //                generator.Emit(OpCodes.Brtrue, endIfLabel);
        //                generator.Emit(OpCodes.Ldloc, result);
        //                generator.Emit(OpCodes.Ldarg_0);
        //                generator.Emit(OpCodes.Ldc_I4, index);
        //                generator.Emit(OpCodes.Callvirt, getValueMethod);
        //                generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
        //                generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
        //                generator.MarkLabel(endIfLabel);
        //            }
        //        }
        //        generator.Emit(OpCodes.Ldloc, result);
        //        generator.Emit(OpCodes.Ret);
        //        dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
        //        return dynamicBuilder;
        //    }
        //}
        #endregion

        #region DataTable 转实体类型数据 方法2

        /// <summary>
        /// 反射 将行数据转换为实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="row">行数据</param>
        /// <returns>实体对象</returns>
        public static T ConvertToObjectFromDR<T>(this DataRow row)
        {
            if (row == null)
            {
                return default(T);
            }
            T obj = (T)Activator.CreateInstance(typeof(T));
            obj = ConvertToObjectFromDR<T>(row, obj);
            return obj;
        }

        /// <summary>
        /// 反射 将行数据转换为实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="row">行数据</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        private static T ConvertToObjectFromDR<T>(this DataRow row, T obj)
        {
            if (row == null)
            {
                return default(T);
            }
            Type type = TypeInfo<T>.TyThis;
            System.Reflection.PropertyInfo[] propInfo = TypeInfo<T>.Props;
            for (int i = 0; i < propInfo.Length; i++)
            {
                if (row.Table.Columns[propInfo[i].Name] != null && row[propInfo[i].Name] != System.DBNull.Value && propInfo[i].CanWrite)
                {
                    object objVal = row[propInfo[i].Name];
                    Type typeVal = Nullable.GetUnderlyingType(propInfo[i].PropertyType) ?? propInfo[i].PropertyType;
                    int mark = 0;
                    try
                    {
                        if (typeVal.Name == "Guid")
                        {
                            mark = 1;
                            propInfo[i].SetValue(obj, Guid.Parse(objVal.ToString()), null);
                        }
                        else
                        {
                            if (typeVal.IsEnum && objVal != null)
                            {
                                Type tyEnum = Enum.GetUnderlyingType(typeVal);
                                if (tyEnum.IsAssignableFrom(typeof(int)))
                                {
                                    mark = 2;
                                    propInfo[i].SetValue(obj, Enum.Parse(typeVal, objVal.ToString()), null);
                                }
                                else
                                {
                                    mark = 3;
                                    propInfo[i].SetValue(obj, Convert.ChangeType(objVal, typeVal), null);
                                }
                            }
                            else
                            {
                                if (objVal == null || string.IsNullOrWhiteSpace(objVal.ToString()))
                                {
                                    mark = 4;
                                    if (propInfo[i].PropertyType.IsNullableType())
                                    {
                                        objVal = null;
                                    }
                                    propInfo[i].SetValue(obj, objVal, null);
                                }
                                else
                                {
                                    mark = 5;
                                    propInfo[i].SetValue(obj, Convert.ChangeType(objVal, typeVal), null);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("SetValue出错！(" + mark + ")", propInfo[i].Name + ":" + objVal, ex);
                    }
                }
            }
            return obj;
        }


        /// <summary>
        /// 反射将datatable转换为List对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="this">datatable数据</param>
        /// <returns>List对象</returns>
        public static List<T> ConvertToListObject<T>(this DataTable @this)
        {
            if (@this == null || @this.Rows.Count <= 0)
            {
                return new List<T>();
            }
            List<T> objs = new List<T>();
            for (int i = 0; i < @this.Rows.Count; i++)
            {
                T obj = (T)Activator.CreateInstance(typeof(T));
                obj = ConvertToObjectFromDR(@this.Rows[i], obj);
                objs.Add(obj);
            }
            return objs;
        }


        #endregion


        #region DataTable 转实体类型数据 方法3

        /// <summary>
        /// DataTable生成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToListModel<T>(this DataTable dataTable) where T : class, new()
        {
            return BWofter.Converters.Data.DataTableConverter<T>.ToEntities(dataTable);
        }

        #endregion

        #region DataTable DataRow DataColumn 扩展

        /// <summary>
        /// 改变:将DataTable的其中 某列(默认第一列)数据 存储为 List
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="this">数据源</param>
        /// <param name="columnName">列名,默认第一列</param>
        /// <returns>List数据</returns>
        public static List<T> TransList<T>(this DataTable @this, string columnName = null)
        {
            List<T> lst = new List<T>();
            if (@this == null || @this.Rows.Count < 0)
            {
                return lst;
            }
            else
            {
                foreach (DataRow dr in @this.Rows)
                {
                    if (columnName == null)
                    {
                        lst.Add(dr[0].ChangeType<T>());
                    }
                    else
                    {
                        lst.Add(dr[columnName].ChangeType<T>());
                    }
                }
            }
            return lst;
        }


        /// <summary>
        /// 将DataTable的其中 两列数据 存储为 Dictionary
        /// </summary>
        /// <param name="this">数据源</param>
        /// <param name="ColumnNameKey">列1(键)</param>
        /// <param name="ColumnVal">列2(值)</param>
        /// <returns>Dictionary集合</returns>
        public static Dictionary<TKey, TVal> TransDict<TKey, TVal>(this DataTable @this, string ColumnNameKey, string ColumnVal)
        {
            Dictionary<TKey, TVal> dict = new Dictionary<TKey, TVal>();
            if (@this == null || @this.Rows.Count <= 0)
            {
                return dict;
            }
            else
            {
                foreach (DataRow dr in @this.Rows)
                {
                    TKey k = (TKey)dr[ColumnNameKey].ChangeType(typeof(TKey));
                    TVal v = (TVal)dr[ColumnVal].ChangeType(typeof(TVal));
                    dict.Add(k, v);
                }
            }
            return dict;
        }


        /// <summary>
        /// 将DataTable的其中 两列数据 存储为 NameValueCollection
        /// </summary>
        /// <param name="this">数据源</param>
        /// <param name="ColumnNameKey">列1(键)</param>
        /// <param name="ColumnVal">列2(值)</param>
        /// <returns>NameValueCollection集合</returns>
        public static NameValueCollection MapperNameValues(this DataTable @this, string ColumnNameKey, string ColumnVal)
        {
            NameValueCollection nvc = new NameValueCollection();
            if (@this == null || @this.Rows.Count <= 0)
            {
                return nvc;
            }
            else
            {
                foreach (DataRow dr in @this.Rows)
                {
                    nvc.Add(dr[ColumnNameKey].ToString(), dr[ColumnVal].ToString());
                }
            }
            return nvc;
        }


        /// <summary>
        ///给当前DataTable增加列名 
        /// </summary>
        /// <param name="this">DataTable对象</param>
        /// <param name="columns">列信息</param>
        /// <returns>增加列后的DataTable</returns>
        public static DataTable AddColumns(this DataTable @this, params KeyValuePair<string, Type>[] columns)
        {
            if (@this == null)
            {
                @this = new DataTable();
            }
            if (columns != null && columns.Length > 0)
            {
                foreach (var col in columns)
                {
                    @this.Columns.Add(new DataColumn(col.Key, col.Value));
                }
            }
            return @this;
        }

        /// <summary>
        /// 获取首行数据
        /// </summary>
        /// <param name="this">DataTable数据</param>
        /// <returns>首行数据</returns>
        public static DataRow FirstRow(this DataTable @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("DataTable不能为null");
            }
            if (@this.Rows.Count > 0)
            {
                return @this.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 获取最后一行数据
        /// </summary>
        /// <param name="this">DataTable数据</param>
        /// <returns>最后一行数据</returns>
        public static DataRow LastRow(this DataTable @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("DataTable不能为null");
            }

            if (@this.Rows.Count > 0)
            {
                return @this.Rows[@this.Rows.Count - 1];
            }
            return null;
        }

        /// <summary>
        /// 添加列数据
        /// </summary>
        /// <param name="this">行集合</param>
        /// <param name="drs">行数据</param>
        public static void AddRange(this DataRowCollection @this, IEnumerable<DataRow> drs)
        {
            foreach (DataRow dr in drs)
            {
                @this.Add(dr.ItemArray);
            }
        }

        /// <summary>
        /// 获取所有DataColumn
        /// </summary>
        /// <param name="columnCollection">列集合</param>
        /// <returns>列数组</returns>
        public static DataColumn[] ToArray(this DataColumnCollection columnCollection)
        {
            List<DataColumn> lstDC = new List<DataColumn>();
            foreach (DataColumn dc in columnCollection)
            {
                lstDC.Add(dc);
            }
            return lstDC.ToArray();
        }



        /// <summary>
        /// 将DataRow转为 dynamic 类型对象
        /// </summary>
        /// <param name="this">行数据</param>
        /// <returns></returns>
        public static dynamic ToExpandoObject(this DataRow @this)
        {
            dynamic entity = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)entity;
            foreach (DataColumn column in @this.Table.Columns)
            {
                expandoDict.Add(column.ColumnName, @this[column]);
            }
            return expandoDict;
        }

        #endregion

        #region DbParameterCollection 扩展
        /// <summary>
        /// DbParameterCollection 转数组
        /// </summary>
        /// <param name="parameterCollection">DbParameter集合</param>
        /// <returns>数组形式的 DbParameter </returns>
        public static DbParameter[] ToArray(this DbParameterCollection parameterCollection)
        {
            if (parameterCollection == null || parameterCollection.Count <= 0)
            {
                return null;
            }

            DbParameter[] paras = new DbParameter[parameterCollection.Count];

            for (int j = 0; j < parameterCollection.Count; j++)
            {
                paras[j] = parameterCollection[j];
            }

            return paras;
        }
        #endregion

        #region DBType 扩展

        /// <summary>
        /// 获取数据库类型对应的 数据类型/Dbtype的字典
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns>数据类型/Dbtype的字典</returns>
        public static Dictionary<string, DbType> DictDbType(this DBType dbType)
        {
            switch (dbType)
            {
                case DBType.SqlServer:
                    return Global.Dict_SqlServer_DbType;
                case DBType.MySql:
                    return Global.Dict_MySql_DbType;
                case DBType.Oracle:
                    return Global.Dict_Oracle_DbType;
                case DBType.OracleDDTek:
                    return Global.Dict_Oracle_DbType;
                case DBType.PostgreSql:
                    return Global.Dict_PostgreSql_DbType;
                case DBType.SQLite:
                    return Global.Dict_Sqlite_DbType;
                default:
                    throw new ArgumentException("未知数据库类型！");
            }
        }

        /// <summary>
        /// 数据库类型，列 对应的DbType
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="colInfo"></param>
        /// <returns>DbType</returns>
        //public static DbType GetDbType(this DBType dbType, ColumnInfo colInfo)
        //{
        //    DbType dType;
        //    if (dbType.DictDbType().TryGetValue(colInfo.TypeName, out dType))
        //    {
        //        return dType;
        //    }
        //    return DbType.AnsiString;
        //}

        /// <summary>
        /// 获取当前数据库类型的参数化字符
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns>参数化字符</returns>
        public static string ParameterChar(this DBType dbType)
        {
            return Global.ParameterCharMap[dbType];
        }


        #endregion

        /// <summary>
        /// 引用类型对象的序列化（深度克隆）
        /// 注：使用之前先将对象标记为  [Serializable] 可序列化。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="RealObject">对象</param>
        /// <returns>返回深度克隆后的新对象</returns>
        internal static T Clone<T>(this T RealObject)
        {
            if (RealObject == null)
            {
                return default(T);
            }
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }
    }
}
