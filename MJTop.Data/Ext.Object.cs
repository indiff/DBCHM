using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MJTop.Data
{
    /// <summary>
    /// 对象扩展方法类
    /// </summary>
    public static class ExtObject
    {
        #region 数据类型

        /// <summary>
        /// 对象类型转换
        /// </summary>
        /// <typeparam name="T">返回的数据，数据的类型</typeparam>
        /// <param name="this">当前值</param>
        /// <returns>转换后的对象</returns>
        internal static T ChangeType<T>(this object @this)
        {
            object result = null;

            Type toType = typeof(T);

            if (@this == null || @this == DBNull.Value)
            {
                if ((toType.IsGenericType && toType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
                {
                    return default(T);
                }
                else if (toType.IsValueType)
                {
                    throw new Exception("不能将null值转换为" + toType.Name + "类型!");
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                if ((toType.IsGenericType && toType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
                {
                    toType = Nullable.GetUnderlyingType(toType) ?? toType;
                }

                if (toType.Name == "Object")
                {
                    return (T)@this;
                }

                if (toType.IsEnum)
                {
                    result = Enum.Parse(toType, @this.ToString(), true);
                }
                else if (toType.IsAssignableFrom(typeof(Guid)))
                {
                    result = Guid.Parse(@this.ToString());
                }
                else
                {
                    result = Convert.ChangeType(@this, toType);
                }
                return (T)result;
            }
        }

        /// <summary>
        /// 对象类型转换，转换失败，返回默认值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="this">当前值</param>
        /// <param name="def">转换失败，返回的默认值</param>
        /// <returns>返回转换后的对象或转换失败后的默认值</returns>
        internal static T ChangeType<T>(this object @this, T def)
        {
            try
            {
                if (@this == null || string.IsNullOrWhiteSpace(@this.ToString()))
                {
                    return def;
                }
                return ChangeType<T>(@this);
            }
            catch
            {
                return def;
            }
        }

        /// <summary>
        /// 对象类型转换
        /// </summary>
        /// <param name="this">当前值</param>
        /// <param name="conversionType">指定类型的类型</param>
        /// <returns>转换后的对象</returns>
        internal static object ChangeType(this object @this, Type conversionType)
        {
            Type type = conversionType;

            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type != null)
            {
                object result = null;

                if (@this != null && @this != DBNull.Value)
                {
                    if (type.IsAssignableFrom(typeof(string)))
                    {
                        result = @this.ToString();
                    }
                    else if (type.IsEnum)
                    {
                        result = Enum.Parse(type, @this.ToString(), true);
                    }
                    else if (type.IsAssignableFrom(typeof(Guid)))
                    {
                        result = Guid.Parse(@this.ToString());
                    }
                    else
                    {
                        result = Convert.ChangeType(@this, type);
                    }
                }
                else
                {
                    if (type.IsAssignableFrom(typeof(string)) || type.IsAssignableFrom(typeof(object)))
                    {
                        result = null;
                    }
                    else
                    {
                        throw new Exception("不能将null值转换为" + type.Name + "类型!");
                    }
                }
                return result;
            }
            return Convert.ChangeType(@this, type);
        }

        /// <summary>
        /// 对象类型转换
        /// </summary>
        /// <param name="this">当前值</param>
        /// <param name="conversionType">指定类型的类型</param>
        /// <param name="def">转换失败，返回的默认值</param>
        /// <returns>转换后的对象</returns>
        internal static object ChangeType(this object @this, Type conversionType, object def)
        {
            try
            {
                if (@this == null)
                {
                    return def;
                }

                return ChangeType(@this, conversionType);
            }
            catch
            {
                return def;
            }
        }

        /// <summary>
        /// 判定 对象的类型 是否是 可空类型
        /// </summary>
        /// <param name="this">对象类型</param>
        /// <returns>是否是 可空类型</returns>
        public static bool IsNullableType(this Type @this)
        {
            return (@this.IsGenericType && @this.
              GetGenericTypeDefinition().Equals
              (typeof(Nullable<>)));
        }

        /// <summary>
        /// 返回当前对象的数组形式
        /// </summary>
        /// <typeparam name="T">当前数据类型</typeparam>
        /// <param name="this">当前对象</param>
        /// <returns>当前对象的数组形式</returns>
        internal static T[] TransArray<T>(this T @this)
        {
            return new T[] { @this };
        }

        /// <summary>
        /// 返回当前对象的列表形式
        /// </summary>
        /// <typeparam name="T">当前数据类型</typeparam>
        /// <param name="this">当前对象</param>
        /// <returns>当前对象的列表形式</returns>
        internal static List<T> TransList<T>(this T @this)
        {
            List<T> lst = new List<T>();
            lst.Add(@this);
            return lst;
        }

        #endregion 数据类型

        #region NameValueCollection 扩展

        /// <summary>
        /// 返回可写的 NameValueCollection集合
        /// </summary>
        public static NameValueCollection NoReadonly(this NameValueCollection @this)
        {
            if (@this == null)
            {
                return new NameValueCollection();
            }
            return new NameValueCollection(@this);
        }

        /// <summary>
        /// 是否包含某个键
        /// </summary>
        /// <param name="this">NameValueCollection集合</param>
        /// <param name="keyName">键名</param>
        /// <param name="valueIsSpace">value值 是否 允许 为空字符串</param>
        /// <returns>返回bool</returns>
        public static bool ContainsKey(this NameValueCollection @this, string keyName, bool valueIsSpace = false)
        {
            if (@this == null || @this.Count <= 0)
            {
                return false;
            }

            string[] values = @this.GetValues(keyName);

            if (values == null)
            {
                return false;
            }

            if (valueIsSpace)
            {
                return true;
            }

            int ct = 0;
            foreach (var val in values)
            {
                if (string.IsNullOrWhiteSpace(val))
                {
                    ct++;
                }
            }
            return !(ct == values.Length);
        }

        /// <summary>
        /// 是否包含某个值
        /// </summary>
        /// <param name="this">NameValueCollection集合</param>
        /// <param name="value">值</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>返回bool</returns>
        public static bool ContainsValue(this NameValueCollection @this, string value, bool ignoreCase = true)
        {
            if (@this == null || @this.Count <= 0)
            {
                return false;
            }

            StringComparison strCmp = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.CurrentCulture;
            foreach (var keyName in @this.AllKeys)
            {
                foreach (string val in @this.GetValues(keyName))
                {
                    if (val.Equals(value, strCmp))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试获取 某个键的值
        /// </summary>
        /// <param name="this">NameValueCollection集合</param>
        /// <param name="keyName">键名</param>
        /// <param name="values">values值数组</param>
        /// <returns>返回bool</returns>
        public static bool TryGetValues(this NameValueCollection @this, string keyName, out string[] values)
        {
            values = null;
            return TryGetValues(@this, keyName, false, out values);
        }

        /// <summary>
        /// 尝试获取 某个键的值
        /// </summary>
        /// <param name="this">NameValueCollection集合</param>
        /// <param name="keyName">键名</param>
        /// <param name="valueIsSpace">value值 是否 允许 为空字符串</param>
        /// <param name="values">values值数组</param>
        /// <returns>返回bool</returns>
        public static bool TryGetValues(this NameValueCollection @this, string keyName, bool valueIsSpace, out string[] values)
        {
            values = null;

            if (@this == null || @this.Count <= 0)
            {
                return false;
            }

            values = @this.GetValues(keyName);

            if (values == null)
            {
                return false;
            }

            if (valueIsSpace && values.Length > 0)
            {
                return true;
            }
            else
            {
                int ct = 0;
                foreach (var val in values)
                {
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        ct++;
                    }
                }
                return !(ct == values.Length);
            }
        }

        /// <summary>
        /// 尝试获取 某个键的值
        /// </summary>
        /// <param name="this">NameValueCollection集合</param>
        /// <param name="keyName">键名</param>
        /// <param name="value">对应的值</param>
        /// <returns>返回bool</returns>
        public static bool TryGetValue(this NameValueCollection @this, string keyName, out string value)
        {
            value = null;
            return TryGetValue(@this, keyName, false, out value);
        }

        /// <summary>
        /// 尝试获取 某个键的值
        /// </summary>
        /// <param name="this">NameValueCollection集合</param>
        /// <param name="keyName">键名</param>
        /// <param name="valueIsSpace">value值 是否 允许 为空字符串</param>
        /// <param name="value">value值/param>
        /// <returns>返回bool</returns>
        public static bool TryGetValue(this NameValueCollection @this, string keyName, bool valueIsSpace, out string value)
        {
            value = null;

            if (@this == null || @this.Count <= 0)
            {
                return false;
            }

            value = @this.Get(keyName);

            if (value == null)
            {
                return false;
            }

            if (valueIsSpace && value.Length > 0)
            {
                return true;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 移除集合中的某个键的某个值
        /// </summary>
        /// <param name="this">NameValueCollection集合</param>
        /// <param name="keyName">键名</param>
        /// <param name="value">value值</param>
        /// <returns>返回bool</returns>
        public static bool Remove(this NameValueCollection @this, string keyName, string value)
        {
            if (@this == null || @this.Count <= 0)
            {
                return false;
            }

            var lstVals = @this.GetValues(keyName)?.ToList();

            if (lstVals == null)
            {
                return false;
            }

            int index = lstVals.IndexOfCompare(value);

            if (index <= -1)
            {
                return false;
            }

            lstVals.RemoveAt(lstVals.IndexOfCompare(value));
            @this.Remove(keyName);

            for (int j = 0; j < lstVals.Count; j++)
            {
                @this.Add(keyName, lstVals[j]);
            }
            return true;
        }

        #endregion NameValueCollection 扩展

        /// <summary>
        /// 查询集合中的某个元素的索引
        /// </summary>
        /// <param name="this">IEnumerable string集合</param>
        /// <param name="item">元素值</param>
        /// <param name="comparisonType">规则，默认忽略大小写</param>
        /// <returns>索引位置</returns>
        public static int IndexOfCompare(this IEnumerable<string> @this, string item, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (@this == null || !@this.Any())
            {
                return -1;
            }

            int j = -1;
            foreach (string curStr in @this)
            {
                j++;

                if (
                    (curStr == null && item == null) ||
                    (curStr != null && curStr.Equals(item, comparisonType))
                   )

                {
                    return j;
                }
            }
            return -1;
        }

        /// <summary>
        /// 一个Key对应多个Value值的存储
        /// </summary>
        /// <typeparam name="K">key类型</typeparam>
        /// <typeparam name="V">V类型</typeparam>
        /// <param name="this">当前IDictionary</param>
        /// <param name="key">key值</param>
        /// <param name="values">V类型值</param>
        public static void AddRange<K, V>(this IDictionary<K, List<V>> @this, K key, params V[] values)
        {
            List<V> lstValue = new List<V>();

            if (!@this.TryGetValue(key, out lstValue))
            {
                lstValue = new List<V>();
                lstValue.AddRange(values);
                @this.Add(key, lstValue);
            }
            else
            {
                lstValue.AddRange(values);
                @this.Remove(key);
                @this.Add(key, lstValue);
            }
        }

        /// <summary>
        /// 打印 str 加入joinChar 进行cnt次打印
        /// </summary>
        /// <param name="str">当前字符串</param>
        /// <param name="cnt">连发次数</param>
        /// <param name="joinChar">加入的字符</param>
        /// <returns></returns>
        public static string Repeater(this string str, int cnt, string joinChar)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < cnt; j++)
            {
                if (j < cnt - 1)
                {
                    sb.Append(str + joinChar);
                }
                else
                {
                    sb.Append(str);
                }
            }
            return sb.ToString();
        }
    }
}