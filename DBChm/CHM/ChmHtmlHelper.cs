using MJTop.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace DBCHM.CHM
{
    public static class StringExt
    {
        public static string FormatString(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
    }
}




namespace DBCHM.CHM
{
    /// <summary>
    /// 常用功能类
    /// </summary>
    public class ChmHtmlHelper
    {


        public static void CreateDirHtml(string tabDirName, NameValueCollection dict_tabs, string indexHtmlpath)
        {
            var code = new StringBuilder();
            code.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            code.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            code.AppendLine("<head>");
            code.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html;charset=gbk\" />");
            code.AppendLine("    <title>{0}</title>".FormatString(tabDirName));
            code.AppendLine("    <style type=\"text/css\">");
            code.AppendLine("        *");
            code.AppendLine("        {");
            code.AppendLine("            font-family:'Microsoft YaHei';");
            code.AppendLine("        }");
            code.AppendLine("        body");
            code.AppendLine("        {");
            code.AppendLine("            font-size: 9pt;");
            code.AppendLine("            font-family:'lucida console';");
            code.AppendLine("        }");
            code.AppendLine("        .styledb");
            code.AppendLine("        {");
            code.AppendLine("            font-size: 14px;");
            code.AppendLine("        }");
            code.AppendLine("        .styletab");
            code.AppendLine("        {");
            code.AppendLine("            font-size: 18px;");
            code.AppendLine("            padding-top: 10px;");
            code.AppendLine("        }");
            code.AppendLine("        a");
            code.AppendLine("        {");
            code.AppendLine("            color: #015FB6;");
            code.AppendLine("        }");
            code.AppendLine("        a:link, a:visited, a:active");
            code.AppendLine("        {");
            code.AppendLine("            color: #015FB6;");
            code.AppendLine("            text-decoration: none;");
            code.AppendLine("        }");
            code.AppendLine("        a:hover");
            code.AppendLine("        {");
            code.AppendLine("            color: #E33E06;");
            code.AppendLine("        }");

            code.AppendLine("        .other-bg-color");
            code.AppendLine("        {");
            code.AppendLine("            background-color: #FCFCFC;");
            code.AppendLine("        }");
            code.AppendLine("        .check-bg-color");
            code.AppendLine("        {");
            code.AppendLine("            background-color: #E6E6FA;");
            code.AppendLine("        }");
            code.AppendLine("    </style>");

            code.AppendLine("<script type=\"text/javascript\">");
            code.AppendLine("     window.onload = function () {");
            code.AppendLine("     var trs = document.getElementById('tab-struct').getElementsByTagName('tr');");
            code.AppendLine("     for (var i = 0; i < trs.length; i++) {");
            code.AppendLine("       if(i==0){continue;}");
            code.AppendLine("       trs[i].onclick = function () {");
            code.AppendLine("         for (var j = 0; j < trs.length; j++) {");
            code.AppendLine("                  if(j==0){continue;}");
            code.AppendLine("                  trs[j].className = 'other-bg-color';");
            code.AppendLine("             }");
            code.AppendLine("          this.className = 'check-bg-color';");
            code.AppendLine("        }");
            code.AppendLine("     }");
            code.AppendLine("}");
            code.AppendLine("</script>");

            code.AppendLine("</head>");
            code.AppendLine("<body>");
            code.AppendLine("    <div style=\"text-align: center\">");
            code.AppendLine("        <div>");
            code.AppendLine("            <table border=\"0\" cellpadding=\"5\" cellspacing=\"0\" width=\"90%\">");
            code.AppendLine("                <tr>");
            code.AppendLine("                    <td bgcolor=\"#FBFBFB\">");
            code.AppendLine("                        <table id='tab-struct' cellspacing=\"0\" cellpadding=\"5\" border=\"1\" width=\"100%\" bordercolorlight=\"#D7D7E5\" bordercolordark=\"#D3D8E0\">");
            code.AppendLine("                        <caption>");
            code.AppendLine("        <div class=\"styletab\">{0}</div>".FormatString("<b>" + tabDirName + "</b>"));
            code.AppendLine("                        </caption>");
            code.AppendLine("                          <tr bgcolor=\"#F0F0F0\" style='font-weight: bold;'><td>序号</td><td>表名</td><td>表说明</td></tr>");


            //构建表头
            int j = 1;          
            //构建数据行
            foreach (var tableName in dict_tabs.AllKeys)
            {
                code.AppendLine("            <tr>");
                code.AppendLine("            <td>{0}</td>".FormatString(j));
                code.AppendLine("            <td>{0}</td>".FormatString("<a href=\"表结构\\" + tableName + " " + FilterIllegalDir(dict_tabs[tableName]) + ".html\">" + tableName + "</a>"));
                code.AppendLine("            <td>{0}</td>".FormatString(!string.IsNullOrWhiteSpace(dict_tabs[tableName]) ? dict_tabs[tableName] : "&nbsp;"));
                code.AppendLine("            </tr>");
                j++;
            }
            code.AppendLine("                        </table>");
            code.AppendLine("                    </td>");
            code.AppendLine("                </tr>");
            code.AppendLine("            </table>");
            code.AppendLine("        </div>");
            code.AppendLine("    </div>");
            code.AppendLine("</body>");
            code.AppendLine("</html>");
            File.WriteAllText(indexHtmlpath, code.ToString(), Encoding.GetEncoding("gbk"));
        }


        /// <summary>
        /// 处理非法字符路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FilterIllegalDir(string path)
        {
            if (path.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                path = string.Join(" ", path.Split(Path.GetInvalidFileNameChars()));
            }
            return path;
        }


        public static void CreateHtml(Dictionary<string, TableInfo> dictTabs, string tabsdir)
        {
            
            foreach (var tab in dictTabs)
            {
                string tableName = tab.Key;
                TableInfo tabInfo = tab.Value;

                string tabPath = tabsdir + "\\" + tabInfo.TableName + " " + FilterIllegalDir(tabInfo.TabComment) + ".html";

                var code = new StringBuilder();
                code.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                code.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                code.AppendLine("<head>");
                code.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html;charset=gbk\" />");
                code.AppendLine("    <title>{0}</title>".FormatString(tabInfo.TableName));
                code.AppendLine("    <style type=\"text/css\">");
                code.AppendLine("        *");
                code.AppendLine("        {");
                code.AppendLine("            font-family:'Microsoft YaHei';");
                code.AppendLine("        }");
                code.AppendLine("        body");
                code.AppendLine("        {");
                code.AppendLine("            font-size: 9pt;");
                code.AppendLine("            font-family:'lucida console';");
                code.AppendLine("        }");
                code.AppendLine("        .styledb");
                code.AppendLine("        {");
                code.AppendLine("            font-size: 16px;");
                code.AppendLine("        }");
                code.AppendLine("        .styletab");
                code.AppendLine("        {");
                code.AppendLine("            font-size: 18px;");
                code.AppendLine("            padding-top: 15px;");
                code.AppendLine("            font-weight: bold;");
                code.AppendLine("        }");
                code.AppendLine("        a");
                code.AppendLine("        {");
                code.AppendLine("            color: #015FB6;");
                code.AppendLine("        }");
                code.AppendLine("        a:link, a:visited, a:active");
                code.AppendLine("        {");
                code.AppendLine("            color: #015FB6;");
                code.AppendLine("            text-decoration: none;");
                code.AppendLine("        }");
                code.AppendLine("        a:hover");
                code.AppendLine("        {");
                code.AppendLine("            color: #E33E06;");
                code.AppendLine("        }");

                code.AppendLine("        .other-bg-color");
                code.AppendLine("        {");
                code.AppendLine("            background-color: #FCFCFC;");
                code.AppendLine("        }");
                code.AppendLine("        .check-bg-color");
                code.AppendLine("        {");
                code.AppendLine("            background-color: #E6E6FA;");
                code.AppendLine("        }");
                code.AppendLine("    </style>");

                code.AppendLine("<script type=\"text/javascript\">");
                code.AppendLine("     window.onload = function () {");
                code.AppendLine("     var trs = document.getElementById('tab-struct').getElementsByTagName('tr');");
                code.AppendLine("     for (var i = 0; i < trs.length; i++) {");
                code.AppendLine("       if(i==0){continue;}");
                code.AppendLine("       trs[i].onclick = function () {");
                code.AppendLine("         for (var j = 0; j < trs.length; j++) {");
                code.AppendLine("                  if(j==0){continue;}");
                code.AppendLine("                  trs[j].className = 'other-bg-color';");
                code.AppendLine("             }");
                code.AppendLine("          this.className = 'check-bg-color';");
                code.AppendLine("        }");
                code.AppendLine("     }");
                code.AppendLine("}");
                code.AppendLine("</script>");

                code.AppendLine("</head>");
                code.AppendLine("<body>");
                code.AppendLine("    <div style=\"text-align: center\">");
                code.AppendLine("        <div>");
                code.AppendLine("            <table border=\"0\" cellpadding=\"5\" cellspacing=\"0\" width=\"90%\">");
                code.AppendLine("                <tr>");
                code.AppendLine("                    <td bgcolor=\"#FBFBFB\">");
                code.AppendLine("                        <table id='tab-struct' cellspacing=\"0\" cellpadding=\"5\" border=\"1\" width=\"100%\" bordercolorlight=\"#D7D7E5\" bordercolordark=\"#D3D8E0\">");
                code.AppendLine("                        <caption>");
                code.AppendLine("        <div class=\"styletab\">{0}</div>{1}{2}".FormatString(
                    tabInfo.TableName,
                    "<span style='float: left; margin-top: 6px;font-size:16px;'>" + tabInfo.TabComment + "</span>",
                    "<a href='../数据库表目录.html' style='float: right; margin-top: 6px;font-size:16px;'>返回目录</a>"));
                code.AppendLine("                        </caption>");


                code.Append("<tr bgcolor=\"#F0F0F0\" style='font-weight: bold;'>");
                code.Append("<td>序号</td><td>列名</td><td>数据类型</td><td>长度</td><td>小数位数</td><td>主键</td><td>自增</td><td>允许空</td><td>默认值</td><td>列说明</td>");
                code.AppendLine("</tr>");

                int j = 1;
                foreach (var col in tabInfo.Colnumns)
                {
                    code.Append("<tr>");
                    code.Append("<td>{0}</td>".FormatString(col.Colorder));
                    code.Append("<td>{0}</td>".FormatString(col.ColumnName));
                    code.Append("<td>{0}</td>".FormatString(col.TypeName));
                    code.Append("<td>{0}</td>".FormatString((col.Length.HasValue ? col.Length.Value.ToString() : "&nbsp;")));
                    code.Append("<td>{0}</td>".FormatString((col.Scale.HasValue ? col.Scale.Value.ToString() : "&nbsp;")));
                    code.Append("<td>{0}</td>".FormatString((col.IsPK ? "√" : "&nbsp;")));
                    code.Append("<td>{0}</td>".FormatString((col.IsIdentity ? "√" : "&nbsp;")));
                    code.Append("<td>{0}</td>".FormatString((col.CanNull ? "√" : "&nbsp;")));
                    code.Append("<td>{0}</td>".FormatString((!string.IsNullOrWhiteSpace(col.DefaultVal) ? col.DefaultVal : "&nbsp;")));
                    code.Append("<td>{0}</td>".FormatString((!string.IsNullOrWhiteSpace(col.DeText) ? col.DeText : "&nbsp;")));
                    code.AppendLine("</tr>");
                    j++;
                }
                code.AppendLine("                        </table>");
                code.AppendLine("                    </td>");
                code.AppendLine("                </tr>");
                code.AppendLine("            </table>");
                code.AppendLine("        </div>");
                code.AppendLine("    </div>");
                code.AppendLine("</body>");
                code.AppendLine("</html>");
                File.WriteAllText(tabPath, code.ToString(), Encoding.GetEncoding("gbk"));
                j++;
            }
        }


       
    }
}
