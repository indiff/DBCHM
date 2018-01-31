using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Top._51Try.Data;

namespace DBCHM
{
    public static class ConfigUtils
    {
        private static string ConfigFileName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName).Replace(".vshost", "");
        public static string AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), ConfigFileName);

        private static string ConfigFilePath = string.Empty;
        private static DB db = null;

        /// <summary>
        /// 初始化静态数据
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

                db.Info.Refresh();
            }
        }


        public static void Save(NameValueCollection dbCHMConfig)
        {
            db.Upsert(dbCHMConfig, "DBCHMConfig");
        }


        public static void Delete(int id)
        {
            db.Delete("DBCHMConfig", "Id", id);
        }


        public static List<DBCHMConfig> SelectAll()
        {
            return db.QueryTable("select Id,Name,DBType,Server,Port,DBName,Uid,Pwd,ConnString from DBCHMConfig order by Id desc ").ConvertToListObject<DBCHMConfig>();
        }

        public static DBCHMConfig Get(int id)
        {
            return db.FirstRow("select Id,Name,DBType,Server,Port,DBName,Uid,Pwd,ConnString from DBCHMConfig where id = " + id).ConvertToObjectFromDR<DBCHMConfig>();
        }


        public static bool HasValue()
        {
            string strSql = "select count(1) from DBCHMConfig";
            return db.Single<int>(strSql, 0) > 0;
        }

    }
}
