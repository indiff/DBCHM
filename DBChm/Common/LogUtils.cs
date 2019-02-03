using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Collections.Specialized;


/// <summary>
/// 日志操作类
/// </summary>
public class LogUtils
{

    private static object locker = new object();


    /// <summary>
    /// 获取请求相关信息
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <returns></returns>
    private static List<string> GetRequestData(LogLevel level)
    {
        List<string> lstdata = new List<string>();
        return lstdata;
    }

    /// <summary>
    /// 写入日志
    /// </summary>
    /// <param name="logName">日志名称</param>
    /// <param name="developer">开发记录者</param>
    /// <param name="level">日志级别</param>
    /// <param name="detail">日志详情</param>
    /// <param name="createtime">记录时间</param>
    public static void Write(string logName, Developer developer, LogLevel level, string detail, DateTime createtime)
    {
        Log log = new Log();
        log.LogName = logName;
        log.Level = level;
        log.Developer = developer;
        log.Added = createtime;
        //List<string> lstDetails = GetRequestData(level);
        //lstDetails.Add(detail);
        log.Detail = detail;


        //todo :可以将日志写入 文件、数据库、MongoDB
        //这里写入根目录 log文件夹
        string logText = Log.GetModelData(log) + "\r\n----------------------------------------------------------------------------------------------------\r\n";
        string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
        if (!ZetaLongPaths.ZlpIOHelper.DirectoryExists(dir))
        {
            ZetaLongPaths.ZlpIOHelper.CreateDirectory(dir);
        }
        fileName = Path.Combine(dir, fileName);
        File.AppendAllText(fileName, logText, Encoding.GetEncoding("GBK"));
    }

    /// <summary>
    /// 写入Info 日志
    /// </summary>
    /// <param name="logName">日志名称</param>
    /// <param name="developer">开发记录者</param>
    /// <param name="Info_objs">日志内容</param>
    public static void LogInfo(string logName, Developer developer, params object[] Info_objs)
    {
        lock (locker)
        {
            List<string> lstDetails = new List<string>();
            if (Info_objs != null && Info_objs.Length > 0)
            {
                List<string> lstInfo = new List<string>();
                foreach (var item in Info_objs)
                {
                    lstInfo.Add(Log.GetModelData(item));
                }
                lstDetails.Add("标记信息：" + string.Join(";", lstInfo.ToArray()));
            }
            Write(logName, developer, LogLevel.Info, string.Join("\r\n", lstDetails.ToArray()), DateTime.Now);
        }
    }


    /// <summary>s
    /// 写入带 堆栈执行 的Info 日志
    /// </summary>
    /// <param name="logName">日志名称</param>
    /// <param name="developer">开发记录者</param>
    /// <param name="Info_objs">日志内容</param>
    public static void LogWrite(string logName, Developer developer, params object[] Info_objs)
    {
        lock (locker)
        {
            List<string> lstDetails = new List<string>();
            System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace(1, true);
            System.Diagnostics.StackFrame frame = stack.GetFrame(0);
            string execFile = frame.GetFileName();
            string fullName = frame.GetMethod().DeclaringType.FullName;
            string methodName = frame.GetMethod().Name;
            int execLine = frame.GetFileLineNumber();
            lstDetails.Add("文件路径：" + execFile + "\r\n");
            lstDetails.Add("类全命名：" + fullName + "\r\n");
            lstDetails.Add("执行方法：" + methodName + "\r\n");
            lstDetails.Add("当前行号：" + execLine + "\r\n");

            if (Info_objs != null && Info_objs.Length > 0)
            {
                List<string> lstInfo = new List<string>();
                foreach (var item in Info_objs)
                {
                    lstInfo.Add(Log.GetModelData(item));
                }
                lstDetails.Add("标记信息：" + string.Join(";", lstInfo.ToArray()));
            }
            Write(logName, developer, LogLevel.Info, string.Join("\r\n", lstDetails.ToArray()), DateTime.Now);
        }
    }

    /// <summary>
    /// 写入Warn 日志
    /// </summary>
    /// <param name="logName">日志名称</param>
    /// <param name="developer">开发记录者</param>
    /// <param name="Info_objs">日志内容</param>
    public static void LogWarn(string logName, Developer developer, params object[] Info_objs)
    {
        lock (locker)
        {
            List<string> lstDetails = new List<string>();
            System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace(1, true);
            System.Diagnostics.StackFrame frame = stack.GetFrame(0);
            string execFile = frame.GetFileName();
            string fullName = frame.GetMethod().DeclaringType.FullName;
            string methodName = frame.GetMethod().Name;
            int execLine = frame.GetFileLineNumber();
            lstDetails.Add("文件路径：" + execFile + "\r\n");
            lstDetails.Add("类全命名：" + fullName + "\r\n");
            lstDetails.Add("执行方法：" + methodName + "\r\n");
            lstDetails.Add("当前行号：" + execLine + "\r\n");

            if (Info_objs != null && Info_objs.Length > 0)
            {
                List<string> lstInfo = new List<string>();
                foreach (var item in Info_objs)
                {
                    lstInfo.Add(Log.GetModelData(item));
                }
                lstDetails.Add("标记信息：" + string.Join(";", lstInfo.ToArray()));
            }
            Write(logName, developer, LogLevel.Warn, string.Join("\r\n", lstDetails.ToArray()), DateTime.Now);
        }
    }

    /// <summary>
    /// 写入 Errorr日志
    /// </summary>
    /// <param name="logName">日志名称</param>
    /// <param name="developer">开发记录者</param>
    /// <param name="ex">异常对象(可为null)</param>
    /// <param name="ext_InfoObjs">日志内容</param>
    public static void LogError(string logName, Developer developer, Exception ex, params object[] ext_InfoObjs)
    {
        lock (locker)
        {
            List<string> lstDetails = new List<string>();
            if (ex != null)
            {
                lstDetails.Add("异常信息1：" + Log.GetModelData(ex));

                if (ex.InnerException != null)
                {
                    lstDetails.Add("异常信息2：" + Log.GetModelData(ex.InnerException));
                }
            }

            StringBuilder sb_extInfo = new StringBuilder();
            if (ext_InfoObjs != null && ext_InfoObjs.Length > 0)
            {
                List<string> lst_ext_Inf = new List<string>();
                foreach (var item in ext_InfoObjs)
                {
                    lst_ext_Inf.Add(Log.GetModelData(item));
                }
                lstDetails.Add("标记信息：" + string.Join(";", lst_ext_Inf.ToArray()));
            }
            string detail = string.Join("\r\n\r\n", lstDetails.ToArray());
            Write(logName, developer, LogLevel.Error, detail, DateTime.Now);
        }
    }
}


/// <summary>
/// 程序日志
/// </summary>
public class Log
{
    public Guid Id { get { return Guid.NewGuid(); } }

    /// <summary>
    /// 日志名称
    /// </summary>
    public string LogName { get; set; }

    /// <summary>
    /// 日志级别
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// 当前记录日志者
    /// </summary>
    public Developer Developer { get; set; }

    /// <summary>
    /// 日志详细内容
    /// </summary>
    public string Detail { get; set; }

    /// <summary>
    /// 日志时间
    /// </summary>
    public DateTime Added { get; set; }


    #region  private 反射 对象
    /// <summary>
    /// 得到对象的所有属性值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns></returns>
    public static string GetModelData(object obj)
    {
        string valueParam = string.Empty;
        StringBuilder sb = new StringBuilder();
        if (obj == null || string.IsNullOrEmpty(obj.ToString()))
        {
            return string.Empty;
        }
        Type objType = obj.GetType();

        if (IsSimpleType(objType))
        {
            valueParam = obj.ToString();
        }
        else if (obj is NameValueCollection)
        {
            valueParam = GetCollectionData(obj as ICollection);
        }
        else
        {
            PropertyInfo[] proInfos = objType.GetProperties();
            foreach (PropertyInfo proInfo in proInfos)
            {
                string name = proInfo.Name;
                object objvalue = null;
                string value = string.Empty;
                try
                {
                    objvalue = proInfo.GetValue(obj, null);
                }
                catch
                { }
                if (objvalue == null)
                {
                    value = string.Empty;
                }
                else
                {
                    value = objvalue.ToString();
                }
                sb.AppendLine(name + "：" + value + "\r\n");
            }
            valueParam = sb.ToString().TrimEnd();
        }
        return valueParam;
    }

    /// <summary>
    /// 得到集合 数组中所有值
    /// </summary>
    /// <param name="obj">集合对象</param>
    /// <returns></returns>
    public static string GetCollectionData(ICollection obj)
    {
        if (obj == null || string.IsNullOrEmpty(obj.ToString()))
        {
            return string.Empty;
        }
        string valueParam = string.Empty;
        Type objType = obj.GetType();
        string typeName = objType.Name;
        Type[] argumentsTypes = objType.GetGenericArguments();

        #region isLstMark isDictMark
        bool isLstMark = false;
        if (argumentsTypes.Length == 1)
        {
            if (IsSimpleType(argumentsTypes[0]))
            {
                isLstMark = true;
            }
        }
        else
        {
            isLstMark = (obj as IList) != null;
        }


        bool isDictMark = false;
        if (argumentsTypes.Length == 2)
        {
            if (IsSimpleType(argumentsTypes[0]) && IsSimpleType(argumentsTypes[1]))
            {
                isDictMark = true;
            }
        }
        else
        {
            isDictMark = ((obj as IDictionary) != null);
        }
        #endregion

        if (objType.IsArray)
        {
            #region 数组类型
            int arrRank = objType.GetArrayRank();
            if (arrRank == 1)
            {
                Array arr = (Array)obj;
                if (arr != null && arr.LongLength > 0)
                {
                    List<string> lst = new List<string>();
                    foreach (var item in arr)
                    {
                        if (item != null)
                        {
                            lst.Add(item.ToString());
                        }
                    }
                    valueParam = string.Join(",", lst.ToArray());
                }
            }
            #endregion
        }
        else if (isLstMark)
        {
            #region List
            IEnumerable enumlst = obj as IEnumerable;
            if (enumlst != null)
            {
                List<string> lsts = new List<string>();
                foreach (var item in enumlst)
                {
                    if (item != null)
                    {
                        lsts.Add(item.ToString());
                    }
                }
                if (lsts.Count > 0)
                {
                    valueParam = string.Join(",", lsts.ToArray());
                }
            }
            #endregion
        }
        else if (isDictMark)
        {
            #region Dictionary
            IDictionary dict = obj as IDictionary;
            if (dict != null && dict.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DictionaryEntry item in dict)
                {
                    sb.AppendLine(item.Key + "：" + item.Value + "\r\n");
                }
                valueParam = sb.ToString();
            }
            #endregion
        }
        else if (obj is NameValueCollection)
        {
            #region NameValueCollection
            NameValueCollection nvc = (NameValueCollection)obj;
            if (nvc != null && nvc.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string key in nvc.AllKeys)
                {
                    sb.AppendLine(key + "：" + nvc[key] + "\r\n");
                }
                valueParam = sb.ToString();
            }
            #endregion
        }
        else if (obj is ICollection)
        {
            #region ICollection
            ICollection coll = obj as ICollection;
            if (coll != null)
            {
                List<string> lstObjs = new List<string>();
                foreach (var item in coll)
                {
                    if (item != null)
                    {
                        lstObjs.Add(item.ToString());
                    }
                }
                if (lstObjs.Count > 0)
                {
                    valueParam = string.Join(",", lstObjs.ToArray());
                }
            }

            #endregion
        }
        return valueParam.TrimEnd();
    }

    public static bool IsSimpleType(Type type)
    {
        //IsPrimitive 判断是否为基础类型。
        //基元类型为 Boolean、 Byte、 SByte、 Int16、 UInt16、 Int32、 UInt32、 Int64、 UInt64、 IntPtr、 UIntPtr、 Char、 Double 和 Single。          
        Type t = Nullable.GetUnderlyingType(type) ?? type;
        if (t.IsPrimitive || t.IsEnum || t == typeof(string)) return true;
        return false;
    }

    #endregion


    #region 枚举 处理
    /// <summary>
    /// 根据枚举对象得到 枚举键值对
    /// </summary>
    /// <typeparam name="T">枚举</typeparam>
    /// <returns></returns>
    public static Dictionary<string, string> GetAllEnums<T>()
    {
        Dictionary<string, string> dict = null;
        Type type = typeof(T);
        string[] enums = Enum.GetNames(type);
        if (enums != null && enums.Length > 0)
        {
            dict = new Dictionary<string, string>();
            foreach (string item in enums)
            {
                string str = Enum.Parse(typeof(T), item).ToString();
                T deve = (T)Enum.Parse(typeof(T), item);
                string uid = Convert.ToInt32(deve).ToString();
                dict.Add(str, uid);
            }
        }
        return dict;
    }


    /// <summary>
    /// 根据枚举val获取枚举name
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="enumVal">枚举val</param>
    /// <returns>枚举name</returns>
    public static T GetEnumName<T>(int enumVal)
    {
        T t = (T)Enum.Parse(typeof(T), enumVal.ToString());
        return t;
    }
    #endregion
}

/// <summary>
/// 日志级别
/// </summary>
public enum LogLevel
{
    Info = 0,
    Warn = 1,
    Error = 2
}

/// <summary>
/// 日志记录开发者
/// </summary>
public enum Developer
{
    /// <summary>
    /// 系统默认
    /// </summary>
    SysDefault = 0,

    /// <summary>
    /// 其他用户 
    /// </summary>

    MJ = 1,
}