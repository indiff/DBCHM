using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Top._51Try.Data;

namespace DBCHM
{
    /// <summary>
    /// 管理 配置的连接字符串
    /// </summary>
    public static class ConfigUtils
    {
        /// <summary>
        /// 当前应用程序的名称
        /// </summary>
        private static string ConfigFileName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName).Replace(".vshost", "");
        /// <summary>
        /// 定义配置存放的路径
        /// </summary>
        public static string AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), ConfigFileName);

        /// <summary>
        /// sqlite db文件的存放路径
        /// </summary>
        private static string ConfigFilePath = string.Empty;

        /// <summary>
        /// 针对配置的 数据库操作对象
        /// </summary>
        private static DB db = null;

        /// <summary>
        /// 初始化静态数据
        /// 将sqlite数据库写入  C:\Users\用户名\AppData\Local\DBChm 目录中
        /// </summary>
        static ConfigUtils()
        {
            try
            {
                if (!Directory.Exists(AppPath))
                {
                    Directory.CreateDirectory(AppPath);
                }
                ConfigFilePath = Path.Combine(AppPath, ConfigFileName + ".db");
                Init();
            }
            catch (Exception ex)
            {
                LogUtils.LogError("ConfigUtils初始化", Developer.SysDefault, ex);
            }
        }

        /// <summary>
        /// 初始化创建配置数据库
        /// </summary>
        private static void Init()
        {
            db = DBMgr.Connect(DBType.SQLite, ConfigFilePath);
            string strSql = string.Empty;
            //表不存在则创建 连接字符串 配置表
            if (db.Info.TableNames == null || !db.Info.TableNames.Contains("DBCHMConfig", StringComparer.OrdinalIgnoreCase))
            {
                strSql = @"create table DBCHMConfig
(
   Id integer PRIMARY KEY autoincrement,
   Name nvarchar(200) unique,
	 DBType varchar(30),
   Server varchar(100),
   Port integer,
   DBName varchar(100),
	 Uid varchar(50),
   Pwd varchar(100),
	 ConnString text 
)";
                db.ExecSql(strSql);

                //执行后，刷新实例 表结构信息
                db.Info.Refresh();
            }
        }

        /// <summary>
        /// 添加或修改配置连接
        /// </summary>
        /// <param name="dbCHMConfig"></param>
        public static void Save(NameValueCollection dbCHMConfig)
        {
            db.Upsert(dbCHMConfig, "DBCHMConfig");
        }

        /// <summary>
        /// 删除连接
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(int id)
        {
            db.Delete("DBCHMConfig", "Id", id);
        }

        /// <summary>
        /// 查询出所有配置的连接
        /// </summary>
        /// <returns></returns>
        public static List<DBCHMConfig> SelectAll()
        {
            return db.QueryTable("select Id,Name,DBType,Server,Port,DBName,Uid,Pwd,ConnString from DBCHMConfig order by Id desc ").ConvertToListObject<DBCHMConfig>();
        }

        /// <summary>
        /// 得到其中1个连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DBCHMConfig Get(int id)
        {
            return db.FirstRow("select Id,Name,DBType,Server,Port,DBName,Uid,Pwd,ConnString from DBCHMConfig where id = " + id).ConvertToObjectFromDR<DBCHMConfig>();
        }

        /// <summary>
        /// 判断配置表是否存在连接字符串
        /// </summary>
        /// <returns></returns>
        public static bool HasValue()
        {
            string strSql = "select count(1) from DBCHMConfig";
            return db.Single<int>(strSql, 0) > 0;
        }

    }
}
