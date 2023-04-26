using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZetaLongPaths;

/// <summary>
/// 日志操作类
/// </summary>
public class LogUtils
{
    private static object locker = new object();

    /// <summary>
    /// 写入日志
    /// </summary>
    /// <param name="logName">日志名称</param>
    /// <param name="developer">开发记录者</param>
    /// <param name="level">日志级别</param>
    /// <param name="details">日志详情</param>
    /// <param name="createtime">记录时间</param>
    public static void Write(string logName, Developer developer, LogLevel level, List<object> details, DateTime createtime)
    {
        Log log = new Log();
        log.LogName = logName;
        log.Level = level;
        log.Developer = developer;
        log.Added = createtime;
        log.Details = details;

        string logText = log.ToString() + "\r\n----------------------------------------------------------------------------------------------------\r\n";
        string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
        if (!ZetaLongPaths.ZlpIOHelper.DirectoryExists(dir))
        {
            ZetaLongPaths.ZlpIOHelper.CreateDirectory(dir);
        }
        fileName = Path.Combine(dir, fileName);
        ZlpIOHelper.AppendText(fileName, logText, Encoding.GetEncoding("GBK"));
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
            Write(logName, developer, LogLevel.Info, Info_objs?.ToList(), DateTime.Now);
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
            List<object> lstDetails = new List<object>();
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
                lstDetails.AddRange(Info_objs);
            }
            Write(logName, developer, LogLevel.Info, lstDetails, DateTime.Now);
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
            List<object> lstDetails = new List<object>();
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
                lstDetails.AddRange(Info_objs);
            }
            Write(logName, developer, LogLevel.Warn, lstDetails, DateTime.Now);
        }
    }

    /// <summary>
    /// 写入 Errorr日志
    /// </summary>
    /// <param name="logName">日志名称</param>
    /// <param name="developer">开发记录者</param>
    /// <param name="ex">异常对象(可为null)</param>
    /// <param name="Info_objs">日志内容</param>
    public static void LogError(string logName, Developer developer, Exception ex, params object[] Info_objs)
    {
        lock (locker)
        {
            List<object> lstDetails = new List<object>();
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
            lstDetails.Add(ex);
            if (ex.InnerException != null)
            {
                lstDetails.Add(ex.InnerException);
            }
            if (Info_objs != null && Info_objs.Length > 0)
            {
                lstDetails.AddRange(Info_objs);
            }
            Write(logName, developer, LogLevel.Error, lstDetails, DateTime.Now);
        }
    }
}

/// <summary>
/// 程序日志
/// </summary>
public class Log
{
    public Guid Id
    { get { return Guid.NewGuid(); } }

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
    public List<object> Details { get; set; }

    /// <summary>
    /// 日志时间
    /// </summary>
    public DateTime Added { get; set; }

    public override string ToString()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(this ?? default(Log));
    }

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

    #endregion 枚举 处理
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