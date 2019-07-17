using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TryOpenXml.Dtos;

namespace TryOpenXml.Text
{
    public class HtmlTemplate
    {
        private List<TableDto> Tables { get; set; }

        public HtmlTemplate(string dbName, List<TableDto> tables, string str_version = "1.0")
        {
            this.DBName = dbName;
            this.Tables = tables;
            this.Doc_Version = str_version;
        }

        public string DBName { get; set; }

        public string Doc_Version { get; set; }

        public string Left_Dir { get; set; }

        public string Dir_Detail { get; set; }

        public string Tab_Struct { get; set; }


        public override string ToString()
        {
            var props = this.GetType().GetProperties();
            this.Left_Dir = Read_Left_Dir();
            this.Dir_Detail = Read_Dir_Detail();
            this.Tab_Struct = Read_Table_Struct();

            var html = ReadHtml();

            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            foreach (var prop in props)
            {
                var val = prop.GetValue(this, null).ToString();

                html = html.Replace("{" + prop.Name + "}", val);
            }

            //表比较多时，左侧目录取消 固定
            if (this.Tables.Count > 20)
            {
                html = html.Replace("position: fixed;", "");
            }

            return html;
        }

        private string Read_Table_Struct()
        {
            StringBuilder code = new StringBuilder();
            foreach (var table in Tables)
            {
                code.Append("<div class=\"page-content\" id=\"" + table.TableName + "\">");
                code.Append("<div style=\"padding: 20px 0; border-bottom: 1px dashed #ccc\"><h1 style=\"float:left;\">" + table.Comment + "</h1><h1 id=\"page-title\">" + table.TableName + "</h1></div>");
                code.Append("<div class=\"page-entry\">");

                code.Append("<table class=\"tab-struct\" cellspacing=\"0\" cellpadding=\"5\" border=\"1\" width=\"100%\" bordercolorlight=\"#D7D7E5\" bordercolordark=\"#D3D8E0\">");

                code.Append("<tr bgcolor=\"#F0F0F0\" style=\"font-weight: bold;\">");
                code.Append("<td>序号</td><td>列名</td><td>数据类型</td><td>长度</td><td>小数位数</td><td>主键</td><td>自增</td><td>允许空</td><td>默认值</td><td>列说明</td>");
                code.Append("</tr>");

                var num = 1;
                foreach (var column in table.Columns)
                {
                    code.Append("<tr class=\"other-bg-color\">");
                    code.Append($"<td>{num}</td><td>{column.ColumnName}</td><td>{column.ColumnTypeName}</td><td>{column.Length}</td><td>{column.Scale}</td><td>{column.IsPK}</td><td>{column.IsIdentity}</td><td>{column.CanNull}</td><td>{column.DefaultVal}</td><td>{column.Comment}</td>");
                    code.Append("</tr>");
                    num++;
                }

                code.Append("</table>");
                code.Append("</div>");
                code.Append("</div>");
            }
            return code.ToString();
        }

        private string ReadHtml()
        {
            using (var stmReader= new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("TryOpenXml.template.html")))
            {
                return stmReader.ReadToEnd();
            }
        }

        /// <summary>
        /// html 左侧目录
        /// </summary>
        /// <returns></returns>
        private string Read_Left_Dir()
        {
            return string.Join("", Tables.Select(t => "<a href=\"#" + t.TableName + "\" class=\"sidebar-link\">" + t.TableName + " " + t.Comment + "</a>"));
        }

        /// <summary>
        /// 数据库表目录
        /// </summary>
        /// <returns></returns>
        private string Read_Dir_Detail()
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("<table class=\"tab-struct\" cellspacing=\"0\" cellpadding=\"5\" border=\"1\" width=\"100%\" bordercolorlight=\"#D7D7E5\" bordercolordark=\"#D3D8E0\">");
            code.AppendLine("<tr bgcolor=\"#F0F0F0\" style='font-weight: bold;'><td>序号</td><td>表名</td><td>表说明</td></tr>");
            //构建表头
            int j = 1;
            //构建数据行
            foreach (var table in Tables)
            {
                code.AppendLine("<tr>");
                code.AppendLine($"<td>{j}</td>");
                code.AppendLine($"<td><a href='#{ table.TableName}'>{table.TableName}</a></td>");
                code.AppendLine($"<td>{(!string.IsNullOrWhiteSpace(table.Comment) ? table.Comment : "&nbsp;")}</td>");
                code.AppendLine("</tr>");
                j++;
            }
            code.AppendLine("</table>");
            return code.ToString();
        }
    }
    public class HtmlUtils
    {
        public static void ExportHtml(string fileName, string dbName, List<TableDto> tables)
        {
            tables = tables ?? new List<TableDto>();

            HtmlTemplate template = new HtmlTemplate(dbName, tables);

            File.WriteAllText(fileName, template.ToString(), Encoding.UTF8);
        }
    }
}
