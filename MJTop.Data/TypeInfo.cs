using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace MJTop.Data
{
    internal class TypeInfo<T>
    {
        static TypeInfo()
        {
            TyThis = typeof(T);

            Name = TyThis.Name;

            IsValueType = TyThis.IsValueType;

            IsAnonymousType = Name.Contains("AnonymousType");

            if (Name == "DbParameter[]")
            {
                IsArrayParameter = true;
            }
            else if (Name == "List`1")
            {
                Type typeTmp = TyThis.GetGenericArguments()[0];
                if (typeTmp.Name == "DbParameter")
                {
                    IsListParameter = true;
                }
            }
            else if ((typeof(IDictionary)).IsAssignableFrom(TyThis)) // Dictionary 或 Hashtable
                                                                     //不能是：
                                                                     //|| (typeof(IDictionary<string, string>)).IsAssignableFrom(TyThis)
                                                                     //|| (typeof(IDictionary<string, object>)).IsAssignableFrom(TyThis)
                                                                     //|| (typeof(IDictionary<object, object>)).IsAssignableFrom(TyThis))
            {
                IsDict = true;
            }
            else if ((typeof(NameValueCollection)).IsAssignableFrom(TyThis))
            {
                IsNameValueColl = true;
            }
            else //可能是 匿名对象，也可能时实体对象
            {
                Props = TyThis.GetProperties();
                PropNames = new List<string>();
                PropMapping = new Dictionary<string, PropertyInfo>();
                foreach (var prop in Props)
                {
                    PropMapping.Add(prop.Name, prop);
                    PropNames.Add(prop.Name);
                }
            }

            TableAttribute table = TyThis.GetCustomAttribute<TableAttribute>();
            TableName = (table?.Name) ?? Name;
        }

        public static Type TyThis { get; set; }

        public static bool IsValueType { get; set; }

        public static PropertyInfo[] Props { get; set; }

        public static Dictionary<string, PropertyInfo> PropMapping { get; set; } = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        public static List<string> PropNames { get; set; } = new List<string>();

        public static string Name { get; private set; }

        public static string TableName { get; private set; }

        public static bool IsDict { get; private set; }

        public static bool IsNameValueColl { get; private set; }

        public static bool IsAnonymousType { get; private set; }

        public static bool IsArrayParameter { get; private set; }

        public static bool IsListParameter { get; private set; }

        public static IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
        {
            IDataParameter[] parameterArray = new IDataParameter[originalParameters.Length];
            int index = 0;
            int length = originalParameters.Length;
            while (index < length)
            {
                parameterArray[index] = (IDataParameter)((ICloneable)originalParameters[index]).Clone();
                index++;
            }
            return parameterArray;
        }
    }
}