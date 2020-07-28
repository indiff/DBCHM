using MJTop.Data.SPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using DDTek.Oracle;
using Npgsql;
using System.Diagnostics;

namespace MJTop.Data
{
    /// <summary>
    /// 数据库连接管理
    /// </summary>
    public static partial class DBMgr
    {
        static DBMgr()
        {
            InitDLL();
        }
        static void InitDLL()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            // 框架加载dll失败后执行，手动加载dll
            AppDomain.CurrentDomain.AssemblyResolve += (sender, senderArgs) =>
            {
                // 当前程序集
                var executingAssembly = Assembly.GetExecutingAssembly();
                // 当前程序集名称
                var assemblyName = new AssemblyName(executingAssembly.FullName).Name;
                // dll名称
                var dllName = new AssemblyName(senderArgs.Name).Name;
                // 待加载dll路径，指向当前程序集资源文件中dll路径。* 根据程序结构调整，使其正确指向dll
                var dllUri = assemblyName + ".lib." + dllName + ".dll";
                // 加载dll
                var resourceStream = executingAssembly.GetManifestResourceStream(dllUri);
                if (resourceStream == null)
                {
                    if (!dllName.EndsWith(".resources"))
                    {
                        throw new ArgumentException(dllName + ".dll" + "未能引用！");
                    }
                    else
                    {
                        Trace.WriteLine(dllName + "，未能找到，未能引用！");
                    }
                }
                if (resourceStream != null)
                {
                    using (resourceStream)
                    {
                        var assemblyData = new Byte[resourceStream.Length];
                        resourceStream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData); //加载dll
                    }
                }
                return null;
            };
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
             
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="dbType">数据库类型（连接字符串默认：ConfigurationManager.ConnectionStrings[dbType.ToString()].ConnectionString）</param>
        /// <returns>数据库操作实例对象</returns>
        public static DB UseDB(DBType dbType, int cmdTimeOut = 30)
        {
            string connectionString = string.Empty;

            if (ConfigurationManager.ConnectionStrings[dbType.ToString()] != null)
            {
                connectionString = ConfigurationManager.ConnectionStrings[dbType.ToString()].ConnectionString;
            }
            else
            {
                throw new ArgumentNullException("connectionString", dbType.ToString() + "的connectionString不能为空！");
            }

            //处理 如果Sqlite 连接字符串只提供 路径的情况
            if (dbType == DBType.SQLite
                    && Regex.IsMatch(connectionString, @"^(\w):\\(.*)(.+\.db)$",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled))
            {
                connectionString = string.Format("Data Source={0};Pooling=True;BinaryGUID=True;Enlist=N;Synchronous=Off;Journal Mode=WAL;Cache Size=5000;", connectionString);
            }

            return DBFactory.CreateInstance(dbType, connectionString, cmdTimeOut);
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="cmdTimeOut">执行超时时间（单位：秒）</param>
        /// <returns>数据库操作实例对象</returns>
        public static DB UseDB(DBType dbType, string connectionString, int cmdTimeOut = 30)
        {
            //处理 如果Sqlite 连接字符串只提供 路径的情况
            if (dbType == DBType.SQLite
                    && Regex.IsMatch(connectionString, @"^(.*)(.+\.db)$",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled))
            {
                connectionString = string.Format("Data Source={0};Pooling=True;BinaryGUID=True;Enlist=N;Synchronous=Off;Journal Mode=WAL;Cache Size=5000;", connectionString);
            }

            return DBFactory.CreateInstance(dbType, connectionString, cmdTimeOut);
        }

        /// <summary>
        /// 连接数据库(拼接连接字符串)
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="server">host地址</param>
        /// <param name="port">SqlServer默认端口：1433，MySql默认端口：3306，SqlServer默认端口：1433，Oracle默认端口：1521，PostgreSql默认端口：5432,DB2默认端口：50000</param>
        /// <param name="databBase">数据库名称</param>
        /// <param name="uid">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="cmdTimeOut">执行超时时间（单位：秒）</param>
        /// <returns>数据库操作实例对象</returns>
        public static DB UseDB(DBType dbType, string server, int? port, string databBase, string uid, string pwd, int connTimeOut = 60, int cmdTimeOut = 30)
        {
            return DBFactory.CreateInstance(dbType, GetConnectionString(dbType, server, port, databBase, uid, pwd, connTimeOut), cmdTimeOut);
        }

        public static DB UseSqlite(string dbPath, string password = null, int cmdTimeOut = 30)
        {
            return DBFactory.CreateInstance(DBType.SQLite, GetConnectionString(DBType.SQLite, null, null, dbPath, null, password), cmdTimeOut);
        }


        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="server">host地址</param>
        /// <param name="port">SqlServer默认端口：1433，MySql默认端口：3306，SqlServer默认端口：1433，Oracle默认端口：1521，PostgreSql默认端口：5432,DB2默认端口：50000</param>
        /// <param name="databBase">数据库名称</param>
        /// <param name="uid">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns>连接字符串</returns>
        public static string GetConnectionString(DBType dbType, string server, int? port, string databBase, string uid, string pwd, int connTimeOut = 60)
        {
            server = (server ?? string.Empty).Trim();
            databBase = (databBase ?? string.Empty).Trim();
            uid = (uid ?? string.Empty).Trim();

            string connectionString = string.Empty;
            switch (dbType)
            {
                case DBType.SqlServer:
                    connectionString = string.Format(@"server={0}{1};database={2};uid={3};pwd={4};connection timeout={5}", server, (port.HasValue ? ("," + port.Value) : string.Empty), databBase, uid, pwd, connTimeOut);
                    break;
                case DBType.MySql:
                    connectionString = string.Format(@"Server={0};{1}Database={2};User={3};Password={4};OldGuids=True;connection timeout={5};SslMode=None;", server, (port.HasValue ? ("Port=" + port.Value + ";") : string.Empty), databBase, uid, pwd, connTimeOut);
                    break;
                case DBType.Oracle:
                    connectionString = string.Format("Data Source={0}:{1}/{2};User Id={3};password={4};Pooling=true;connection timeout={5}", server, (port ?? 1521), databBase, uid, pwd, connTimeOut);
                    break;
                case DBType.OracleDDTek:
                    connectionString = string.Format("Host={0};Port={1};Service Name={2};User ID={3};Password={4};PERSIST SECURITY INFO=True;connection timeout={5}", server, (port ?? 1521), databBase, uid, pwd, connTimeOut);
                    break;
                case DBType.PostgreSql:
                    connectionString = string.Format("host={0};{1}database={2};user id={3};password={4};timeout={5}", server, (port.HasValue ? ("port=" + port.Value + ";") : string.Empty), databBase, uid, pwd, connTimeOut);
                    break;
                case DBType.SQLite:
                    //connectionString = string.Format("Data Source={0};Pooling=True;BinaryGUID=True;Enlist=N;Synchronous=Off;Journal Mode=WAL;Cache Size=5000;", databBase);
                    connectionString = string.Format("Data Source={0};", databBase);
                    if (!string.IsNullOrWhiteSpace(pwd))
                    {
                        connectionString += "version=3;password=" + pwd;
                    }
                    break;
                case DBType.DB2:
                    connectionString = string.Format(@"server={0}:{1};Database={2};Uid={3};Pwd={4};connection timeout={5}", server, (port ?? 50000), databBase, uid, pwd, connTimeOut);
                    break;
                default:
                    throw new ArgumentException("未知数据库类型！");
            }
            return connectionString;
        }
    }
}
