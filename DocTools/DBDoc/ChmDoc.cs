using DocTools.Dtos;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ZetaLongPaths;

namespace DocTools.DBDoc
{
    public class ChmDoc : Doc
    {
        public ChmDoc(DBDto dto, string filter = "chm files (*.chm)|*.chm") : base(dto, filter)
        {
        }

        private Encoding CurrEncoding
        {
            get
            {
                return Encoding.GetEncoding("gbk");
            }
        }

        private string HHCPath
        {
            get
            {
                var hhcPath = string.Empty;
                var hhwDir = ConfigUtils.SearchInstallDir("HTML Help Workshop", "hhw.exe");
                if (!string.IsNullOrWhiteSpace(hhwDir) && ZlpIOHelper.DirectoryExists(hhwDir))
                {
                    hhcPath = Path.Combine(hhwDir, "hhc.exe");
                }
                return hhcPath;
            }
        }

        // 创建目录结构
        private void InitDirFiles(string tableStr, string viewStr, string procStr)
        {
            var dirNames = new string[] {
                tableStr,
                viewStr,
                procStr,
                //"函数",
                "resources\\js"
            };

            foreach (var name in dirNames)
            {
                var tmpDir = Path.Combine(this.WorkTmpDir, name);

                if (ZlpIOHelper.DirectoryExists(tmpDir))
                {
                    ZlpIOHelper.DeleteDirectory(tmpDir, true);
                }
                ZlpIOHelper.CreateDirectory(tmpDir);
            }

            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TplFile\\chm\\");

            var files = Directory.GetFiles(dir, "*.js", SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                var fileName = Path.GetFileName(filePath);
                ZlpIOHelper.CopyFile(filePath, Path.Combine(this.WorkTmpDir, "resources\\js\\", fileName), true);
            }
        }

        public override void Build(string filePath)
        {
            #region 使用 HTML Help Workshop 的 hhc.exe 编译 ,先判断系统中是否已经安装有  HTML Help Workshop

            if (this.HHCPath.IsNullOrWhiteSpace())
            {
                string htmlhelpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "htmlhelp.exe");

                if (File.Exists(htmlhelpPath))
                {
                    if (MessageBox.Show("导出CHM文档需安装 HTML Help Workshop ，是否现在安装？", "提示",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                    {
                        var proc = Process.Start(htmlhelpPath);
                    }
                }
                return;
            }

            #endregion 使用 HTML Help Workshop 的 hhc.exe 编译 ,先判断系统中是否已经安装有  HTML Help Workshop

            var tableStr = "表结构(" + this.Dto.Tables.Count + ")";
            var viewStr = "视图(" + this.Dto.Views.Count + ")";
            var procStr = "存储过程(" + this.Dto.Procs.Count + ")";
            this.InitDirFiles(tableStr, viewStr, procStr);

            //MessageBox.Show("InitDirFiles");

            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TplFile\\chm");

            var hhc_tpl = File.ReadAllText(Path.Combine(dir, "hhc.cshtml"), CurrEncoding);
            var hhk_tpl = File.ReadAllText(Path.Combine(dir, "hhk.cshtml"), CurrEncoding);
            var hhp_tpl = File.ReadAllText(Path.Combine(dir, "hhp.cshtml"), CurrEncoding);
            var list_tpl = File.ReadAllText(Path.Combine(dir, "list.cshtml"), CurrEncoding);
            var table_tpl = File.ReadAllText(Path.Combine(dir, "table.cshtml"), CurrEncoding);
            var sqlcode_tpl = File.ReadAllText(Path.Combine(dir, "sqlcode.cshtml"), CurrEncoding);
            // MessageBox.Show("ReadAllText");

            //  MessageBox.Show(" hhk_tpl RazorRender LI");

            try
            {
                var hhk = hhk_tpl.RazorRender(this.Dto).Replace("</LI>", "");
                var hhc = hhc_tpl.RazorRender(this.Dto).Replace("</LI>", "");
                ZlpIOHelper.WriteAllText(Path.Combine(this.WorkTmpDir, "chm.hhc"), hhc, CurrEncoding);
                ZlpIOHelper.WriteAllText(Path.Combine(this.WorkTmpDir, "chm.hhk"), hhk, CurrEncoding);
                ZlpIOHelper.WriteAllText(Path.Combine(this.WorkTmpDir, "数据库目录.html"), list_tpl.RazorRender(this.Dto), CurrEncoding);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // MessageBox.Show("write all text");
            foreach (var tab in this.Dto.Tables)
            {
                if (!String.IsNullOrWhiteSpace(tab.TableModule))
                {
                    // 文件实际目录
                    var tab_path_dir = Path.Combine(this.WorkTmpDir, tableStr, tab.TableModule);
                    var tab_path = Path.Combine(tab_path_dir, $"{tab.TableName} {tab.Comment}.html");
                    // 如果目录不存在则创建目录
                    if (!ZlpIOHelper.DirectoryExists(tab_path_dir))
                    {
                        ZlpIOHelper.CreateDirectory(tab_path_dir);
                        //  ZlpIOHelper.DeleteDirectory(tab_path_dir, true);
                    }
                    // 输出文件内容
                    var content = table_tpl.RazorRender(tab);
                    //  File.WriteAllText(Path.Combine(this.WorkTmpDir, tableStr, tab.TableModule, "content.html"), content);
                    ZlpIOHelper.WriteAllText(tab_path, content, CurrEncoding);
                    //   MessageBox.Show(content);
                }
                else
                {
                    // MessageBox.Show( " table module is empty ");
                    var tab_path = Path.Combine(this.WorkTmpDir, tableStr, $"{tab.TableName} {tab.Comment}.html");
                    var content = table_tpl.RazorRender(tab);
                    ZlpIOHelper.WriteAllText(tab_path, content, CurrEncoding);
                }
            }

            foreach (var item in Dto.Views)
            {
                var vw_path = Path.Combine(this.WorkTmpDir, viewStr, $"{item.Key}.html");
                var content = sqlcode_tpl.RazorRender(
                     new SqlCode() { DBType = Dto.DBType, CodeName = item.Key, Content = item.Value.Trim() }
                     );
                ZlpIOHelper.WriteAllText(vw_path, content, CurrEncoding);
            }

            int i = 0;
            foreach (var item in Dto.Procs)
            {
                Console.WriteLine("start :" + item.Key + (i++));
                var proc_path = Path.Combine(this.WorkTmpDir, procStr, $"{item.Key}.html");
                var content = sqlcode_tpl.RazorRender(
                    new SqlCode() { DBType = Dto.DBType, CodeName = item.Key, Content = item.Value.Trim() }
                    );
                ZlpIOHelper.WriteAllText(proc_path, content, CurrEncoding);
                Console.WriteLine("end :" + item.Key);
            }

            var hhp_Path = Path.Combine(this.WorkTmpDir, "chm.hhp");
            ZlpIOHelper.WriteAllText(hhp_Path, hhp_tpl.RazorRender(new ChmHHP(filePath, this.WorkTmpDir)), CurrEncoding);

            string res = StartRun(HHCPath, hhp_Path, Encoding.GetEncoding("gbk"));
            ZlpIOHelper.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", "chm.log"), res);
        }

        private string StartRun(string hhcPath, string arguments, Encoding encoding)
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
    }
}