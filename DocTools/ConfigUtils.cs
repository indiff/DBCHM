using MJTop.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace DocTools
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
                if (!ZetaLongPaths.ZlpIOHelper.DirectoryExists(AppPath))
                {
                    ZetaLongPaths.ZlpIOHelper.CreateDirectory(AppPath);                   
                }
                AddSecurityControll2Folder(AppPath);
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
            db = DBMgr.UseDB(DBType.SQLite, ConfigFilePath);

            string strSql = @"create table DBCHMConfig
            (
               Id integer PRIMARY KEY autoincrement,
               Name nvarchar(200) unique,
	             DBType varchar(30),
               Server varchar(100),
               Port integer,
               DBName varchar(100),
	             Uid varchar(50),
               Pwd varchar(100),
                ConnTimeOut integer,
	             ConnString text,
                Modified text
            )";

            //表不存在则创建 连接字符串 配置表
            if (db.Info.TableNames == null || !db.Info.TableNames.Contains(nameof(DBCHMConfig), StringComparer.OrdinalIgnoreCase))
            {
                db.ExecSql(strSql);
                //执行后，刷新实例 表结构信息
                db.Info.Refresh();
            }
            else
            {
                // v1.7.3.7 版本 增加 连接超时 与 最后连接时间
                var info = db.Info;
                if(!info.IsExistColumn(nameof(DBCHMConfig), nameof(DBCHMConfig.Modified)))
                {
                    var configs = db.GetListDictionary("select * from " + nameof(DBCHMConfig));

                    db.Info.DropTable(nameof(DBCHMConfig));

                    db.ExecSql(strSql);

                    //执行后，刷新实例 表结构信息
                    db.Info.Refresh();

                    if (configs != null && configs.Count > 0)
                    {
                        foreach (var config in configs)
                        {
                            try
                            {
                                db.Insert(config, nameof(DBCHMConfig));
                            }
                            catch (Exception ex)
                            {
                                LogUtils.LogError("Init", Developer.SysDefault, ex, config);
                            }
                        }

                        db.ExecSql("update " + nameof(DBCHMConfig) + " set ConnTimeOut = 120 ");
                    }
                }
            }
        }



        /// <summary>
        /// 判断磁盘路径下是否安装存在某个文件，最后返回存在某个文件的路径
        /// </summary>
        /// <param name="installPaths"></param>
        /// <param name="installPath"></param>
        /// <returns></returns>
        public static bool IsInstall(string[] installPaths, out string installPath)
        {
            installPath = string.Empty;
            var driInfos = DriveInfo.GetDrives();
            foreach (DriveInfo dInfo in driInfos)
            {
                if (dInfo.DriveType == DriveType.Fixed)
                {
                    foreach (string ipath in installPaths)
                    {
                        string path = Path.Combine(dInfo.Name, ipath);
                        if (File.Exists(path))
                        {
                            installPath = path;
                            return true;
                        }
                    }
                }
            }
            return false;
        }






        /// <summary>
        /// 搜索获取软件安装目录
        /// </summary>
        /// <param name="orNames">软件名称 或 软件的主程序带exe的文件名</param>
        /// <returns>获取安装目录</returns>
        public static string SearchInstallDir(params string[] orNames)
        {
            //即时刷新注册表
            SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);

            string installDir = null;

            var or_install_addrs = new List<string>
            {
                @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            var or_get_key_names = new List<string>
            {
                "InstallLocation",
                "InstallPath",
                "Install_Dir",
                "UninstallString"
            };


            Microsoft.Win32.RegistryKey regKey = null;
            try
            {
                regKey = Microsoft.Win32.Registry.LocalMachine;

                var arr_exe = orNames.Where(t => t.EndsWith(".exe")).ToList();
                var arr_name = orNames.Where(t => !t.EndsWith(".exe")).ToList();

                if (arr_exe.Any())
                {
                    foreach (var exe_name in arr_exe)
                    {
                        var name_node = regKey.OpenSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{exe_name}", false);
                        if (name_node != null)
                        {
                            var keyValue = name_node.GetValue("Path");

                            if (keyValue == null)
                            {
                                //取 (默认)
                                keyValue = name_node.GetValue("");
                            }

                            if (keyValue != null)
                            {
                                // 值 可能 带双引号，去除双引号
                                installDir = keyValue.ToString().Trim('"');
                                if (!Directory.Exists(installDir))
                                {
                                    // 可能是文件路径，取目录
                                    installDir = Path.GetDirectoryName(installDir);
                                }
                                return installDir;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var regAddr in or_install_addrs)
                    {
                        var regSubKey = regKey.OpenSubKey(regAddr, false);

                        foreach (var name in arr_name)
                        {
                            var name_node = regSubKey.OpenSubKey(name);

                            if (name_node != null)
                            {
                                foreach (var keyName in or_get_key_names)
                                {
                                    var keyValue = name_node.GetValue(keyName);

                                    if (keyValue != null)
                                    {
                                        // 值 可能 带双引号，去除双引号
                                        installDir = keyValue.ToString().Trim('"');
                                        if (!Directory.Exists(installDir))
                                        {
                                            // 可能是文件路径，取目录
                                            installDir = Path.GetDirectoryName(installDir);
                                        }
                                        return installDir;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                LogUtils.LogError(nameof(SearchInstallDir), Developer.SysDefault, ex);
            }
            finally
            {
                regKey?.Close();
            }
            return installDir;
        }


        [DllImport("shell32.dll")]

        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);



        /// <summary>
        ///为文件夹添加users，everyone用户组的完全控制权限
        /// </summary>
        /// <param name="dirPath"></param>
        public static void AddSecurityControll2Folder(string dirPath)
        {
            //获取文件夹信息
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (dir.Exists)
            {
                //获得该文件夹的所有访问权限
                System.Security.AccessControl.DirectorySecurity dirSecurity = dir.GetAccessControl(AccessControlSections.All);
                //设定文件ACL继承
                InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
                //添加ereryone用户组的访问权限规则 完全控制权限
                FileSystemAccessRule everyoneFileSystemAccessRule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
                //添加Users用户组的访问权限规则 完全控制权限
                FileSystemAccessRule usersFileSystemAccessRule = new FileSystemAccessRule("Users", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
                bool isModified = false;
                dirSecurity.ModifyAccessRule(AccessControlModification.Add, everyoneFileSystemAccessRule, out isModified);
                dirSecurity.ModifyAccessRule(AccessControlModification.Add, usersFileSystemAccessRule, out isModified);
                //设置访问权限
                dir.SetAccessControl(dirSecurity);
            }
        }

        /// <summary>
        /// 添加或修改配置连接
        /// </summary>
        /// <param name="dbCHMConfig"></param>
        public static void Save(NameValueCollection dbCHMConfig)
        {
            db.Save(dbCHMConfig, "DBCHMConfig");
        }

        public static void UpLastModified(int id)
        {
            db.ExecSql("update DBCHMConfig set Modified='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + id);
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
            return db.GetDataTable("select * from DBCHMConfig order by Modified desc ").ConvertToListObject<DBCHMConfig>();
        }

        /// <summary>
        /// 得到其中1个连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DBCHMConfig Get(int id)
        {
            return db.GetDataTable("select * from DBCHMConfig where id = " + id).ConvertToListObject<DBCHMConfig>().FirstOrDefault();
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
