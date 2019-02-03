using System;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace DBCHM.CHM
{

    /// <summary>
    /// Chm辅助类
    /// 
    /// </summary>
    public class ChmHelp
    {

        public ChmHelp()
        {
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            if (!ZetaLongPaths.ZlpIOHelper.DirectoryExists(logDir))
            {
                ZetaLongPaths.ZlpIOHelper.CreateDirectory(logDir);
            }
        }

        /// <summary>
        /// Chm文件保存路径
        /// </summary>
        public string ChmFileName { get; set; }

        /// <summary>
        /// Chm文件Titie
        /// </summary>
        public string Title { get; set; }

        private string sourcePath;

        /// <summary>
        /// 编印所在目录
        /// </summary>
        public string SourcePath
        {
            get { return sourcePath; }
            set//赋值时保证路径最后有斜杠否则在获取文件相对路径的时候会意外的多第一个斜杠
            {
                sourcePath = Path.GetFullPath(value);
                if (!sourcePath.EndsWith("\\"))
                {
                    sourcePath += "\\";
                }
            }
        }

        /// <summary>
        /// 默认页面 相对编译文件夹的路径
        /// </summary>
        public string DefaultPage { get; set; }

        private StringBuilder hhcBody = new StringBuilder();
        private StringBuilder hhpBody = new StringBuilder();
        private StringBuilder hhkBody = new StringBuilder();       

        #region 构造所需要的文件

        /// <summary>
        /// 生成 HHC 的Body 主体内容
        /// </summary>
        /// <param name="path"></param>
        private void Create(string path)
        {
            //获取文件
            var strFileNames = Directory.GetFiles(path);
            //获取子目录
            var strDirectories = Directory.GetDirectories(path);
            //给该目录添加UL标记
            if (strFileNames.Length > 0 || strDirectories.Length > 0)
                hhcBody.AppendLine("	<UL>");
            //处理获取的文件
            foreach (string filename in strFileNames)
            {
                var fileItem = new StringBuilder();
                fileItem.AppendLine("	<LI> <OBJECT type=\"text/sitemap\">");
                fileItem.AppendLine("		<param name=\"Name\" value=\"{0}\">".FormatString(Path.GetFileNameWithoutExtension(filename)));
                fileItem.AppendLine("		<param name=\"Local\" value=\"{0}\">".FormatString(filename.Replace(SourcePath, string.Empty)));
                fileItem.AppendLine("		<param name=\"ImageNumber\" value=\"11\">");
                fileItem.AppendLine("		</OBJECT>");
                //添加文件列表到hhp
                hhpBody.AppendLine(filename);
                hhcBody.Append(fileItem.ToString());
                hhkBody.Append(fileItem.ToString());
            }
            //遍历获取的目录
            foreach (string dirname in strDirectories)
            {
                hhcBody.AppendLine("	<LI> <OBJECT type=\"text/sitemap\">");
                hhcBody.AppendLine("		<param name=\"Name\" value=\"{0}\">".FormatString(Path.GetFileName(dirname)));
                hhcBody.AppendLine("		<param name=\"ImageNumber\" value=\"1\">");
                hhcBody.AppendLine("		</OBJECT>");
                //递归遍历子文件夹
                Create(dirname);
            }
            //给该目录添加/UL标记
            if (strFileNames.Length > 0 || strDirectories.Length > 0)
            {
                hhcBody.AppendLine("	</UL>");
            }
        }

        /// <summary>
        /// 创建HHC文件：列表文件,确定目标文件中左侧树形列表中"目录"选项卡下的内容.
        /// </summary>
        private void CreateHHC()
        {
            var code = new StringBuilder();
            code.AppendLine("<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML//EN\">");
            code.AppendLine("<HTML>");
            code.AppendLine("<HEAD>");
            code.AppendLine("<meta name=\"GENERATOR\" content=\"DBCHM.exe  www.51try.top\">");
            code.AppendLine("<!-- Sitemap 1.0 -->");
            code.AppendLine("</HEAD><BODY>");
            code.AppendLine("<OBJECT type=\"text/site properties\">");
            code.AppendLine("	<param name=\"ExWindow Styles\" value=\"0x200\">");
            code.AppendLine("	<param name=\"Window Styles\" value=\"0x800025\">");
            code.AppendLine("	<param name=\"Font\" value=\"MS Sans Serif,9,0\">");
            code.AppendLine("</OBJECT>");

            //遍历文件夹 构建hhc文件内容
            code.Append(hhcBody.ToString());

            code.AppendLine("</BODY></HTML>");
            ZetaLongPaths.ZlpIOHelper.WriteAllText(Path.Combine(SourcePath, "chm.hhc"), code.ToString(), Encoding.GetEncoding("gbk"));
        }

        /// <summary>
        /// 创建HHP配置项文件
        /// </summary>
        private void CreateHHP()
        {
            var code = new StringBuilder();
            code.AppendLine("[OPTIONS]");
            code.AppendLine("Auto Index=Yes");
            code.AppendLine("CITATION=Made by lztkdr");//制作人
            code.AppendLine("Compatibility=1.1 or later");//版本
            code.AppendLine(@"Compiled file=" + ChmFileName);//生成chm文件路径
            code.AppendLine("Contents file=chm.HHC");//hhc文件路径
            code.AppendLine("COPYRIGHT=www.51try.top");//版权所有
            code.AppendLine(@"Default topic={1}");//CHM文件的首页
            code.AppendLine("Default Window=Main");//目标文件窗体控制参数,这里跳转到Windows小节中，与其一致即可
            code.AppendLine("Display compile notes=Yes");//显示编译信息
            code.AppendLine("Display compile progress=Yes");//显示编译进度
            //code.AppendLine("Error log file=" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log","chm.log"));//错误日志文件
            code.AppendLine("Full-text search=Yes");//是否支持全文检索信息
            code.AppendLine("Language=0x804 中文(中国)");// 国家语言代码Locale ID (LCID)
            code.AppendLine("Index file=chm.HHK");//hhk文件路径
            code.AppendLine("Title={0}");//CHM文件标题
            //code.AppendLine("Flat=NO");//编译文件不包括文件夹
            code.AppendLine("Enhanced decompilation=yes");//编译文件不包括文件夹
            code.AppendLine();
            code.AppendLine("[WINDOWS]");
            //例子中使用的参数 0x20 表示只显示目录和索引
            code.AppendLine("Main=\"{0}\",\"chm.hhc\",\"chm.hhk\",\"{1}\",\"{1}\",,,,20000,0x63520,180,0x104E, [0,0,745,509],0x0,0x0,,,,,0");
            code.AppendLine();
            code.AppendLine("[MERGE FILES]");
            code.AppendLine();
            code.AppendLine("[FILES]");
            code.Append(hhpBody.ToString());
            
            ZetaLongPaths.ZlpIOHelper.WriteAllText(Path.Combine(SourcePath, "chm.hhp"), code.ToString().FormatString(Title, DefaultPage), Encoding.GetEncoding("gbk"));
        }
        private void CreateHHK()
        {
            var code = new StringBuilder();
            code.AppendLine("<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML//EN\">");
            code.AppendLine("<HTML>");
            code.AppendLine("<HEAD>");
            code.AppendLine("<meta name=\"GENERATOR\" content=\"DBCHM.exe  www.51try.top\">");
            code.AppendLine("<!-- Sitemap 1.0 -->");
            code.AppendLine("</HEAD><BODY>");
            code.AppendLine("<OBJECT type=\"text/site properties\">");
            code.AppendLine("	<param name=\"ExWindow Styles\" value=\"0x200\">");
            code.AppendLine("	<param name=\"Window Styles\" value=\"0x800025\">");
            code.AppendLine("	<param name=\"Font\" value=\"MS Sans Serif,9,0\">");
            code.AppendLine("</OBJECT>");
            code.AppendLine("<UL>");
            //遍历文件夹 构建hhc文件内容
            code.Append(hhkBody.ToString());
            code.AppendLine("</UL>");
            code.AppendLine("</BODY></HTML>");
            ZetaLongPaths.ZlpIOHelper.WriteAllText(Path.Combine(SourcePath, "chm.hhk"), code.ToString(), Encoding.GetEncoding("gbk"));
        }
        #endregion


        /// <summary>
        /// 编译
        /// </summary>
        /// <returns></returns>
        public void Compile()
        {
            #region 使用 HTML Help Workshop 的 hhc.exe 编译 ,先判断系统中是否已经安装有  HTML Help Workshop 

            string hhcPath = string.Empty;

            string[] installPaths ={
                                       @"Program Files (x86)\HTML Help Workshop\hhc.exe",
                                       @"Program Files\HTML Help Workshop\hhc.exe"
                                  };

            if (!ConfigUtils.IsInstall(installPaths, out hhcPath))
            {
                throw new FileNotFoundException("未安装HTML Help Workshop！", "hhc.exe");
            } 
            #endregion

            //准备hhp hhc hhk文件
            Create(SourcePath);
            CreateHHC();
            CreateHHK();
            CreateHHP();
            
            string res = StartRun(hhcPath, Path.Combine(SourcePath, "chm.hhp"), Encoding.GetEncoding("gbk"));
            ZetaLongPaths.ZlpIOHelper.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", "chm.log"), res);
        }



        public static string StartRun(string hhcPath,string arguments,Encoding encoding)
        {
            string str = "";
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = hhcPath,  //调入HHC.EXE文件 
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardErrorEncoding = encoding,
                StandardOutputEncoding = encoding
            };

            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    str = reader.ReadToEnd();
                }
                process.WaitForExit();
            }
            return str.Trim();
        }

        /// <summary>
        /// 反编译
        /// </summary>
        /// <returns></returns>
        public bool DeCompile()
        {
            //反编译时，Path作为CHM文件路径
            //得到chm文件的绝对路径
            string ExtportPath = Path.GetDirectoryName(ChmFileName);
            //命令参数含义
            //Path:导出的文件保存的路径
            //ChmPath:Chm文件所在的路径
            string cmd = " -decompile " + ExtportPath + " " + ChmFileName;//反编译命令
            Process p = Process.Start("hh.exe", cmd);//调用hh.exe进行反编译
            p.WaitForExit();
            return true;
        }
    }
}