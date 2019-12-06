using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TryOpenXml.Dtos;

namespace TryOpenXml.Text
{
    public static class MarkDownUtils
    {
        public static string MarkDown<T>(this IEnumerable<T> objs, params string[] excludePropNames)
        {
            if (objs == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            var minus = 0;
            var type = typeof(T);
            var props = type.GetProperties();
            var lstTmp = new List<string>();

            sb.Append(" | ");
            foreach (var prop in props)
            {
                if (excludePropNames != null && excludePropNames.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
                {
                    minus++;
                    continue;
                }
                var headName = ((prop.GetCustomAttributes(typeof(DisplayAttribute), false)?.FirstOrDefault() as DisplayAttribute)?.Name) ?? prop.Name;
                lstTmp.Add(headName);
            }
            sb.Append(string.Join(" | ", lstTmp));
            sb.Append(" | ");
            sb.AppendLine();

            lstTmp = new List<string>();
            sb.Append(" | ");
            for (int j = 0; j < props.Length - minus; j++)
            {
                lstTmp.Add(":---:");
            }
            sb.Append(string.Join(" | ", lstTmp));
            sb.Append(" | ");

            foreach (var obj in objs)
            {
                if (obj == null)
                {
                    continue;
                }
                sb.AppendLine();
                sb.Append(" | ");
                lstTmp = new List<string>();
                foreach (var prop in props)
                {
                    if (excludePropNames != null && excludePropNames.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    var value = (prop.GetValue(obj, null) ?? string.Empty).ToString();
                    lstTmp.Add(value);
                }
                sb.Append(string.Join(" | ", lstTmp));
                sb.Append(" | ");
            }
            var md = sb.ToString();
            return md;
        }

        public static string MarkDown(this DataTable data, params string[] excludeColNames)
        {
            if (data == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            var minus = 0;
            var lstTmp = new List<string>();

            sb.Append(" | ");
            foreach (DataColumn dc in data.Columns)
            {
                if (excludeColNames != null && excludeColNames.Contains(dc.ColumnName, StringComparer.OrdinalIgnoreCase))
                {
                    minus++;
                    continue;
                }
                lstTmp.Add(dc.ColumnName);
            }
            sb.Append(string.Join(" | ", lstTmp));
            sb.Append(" | ");
            sb.AppendLine();

            lstTmp = new List<string>();
            sb.Append(" | ");
            for (int j = 0; j < data.Columns.Count - minus; j++)
            {
                lstTmp.Add(":---:");
            }
            sb.Append(string.Join(" | ", lstTmp));
            sb.Append(" | ");

            foreach (DataRow dr in data.Rows)
            {
                sb.AppendLine();
                sb.Append(" | ");
                lstTmp = new List<string>();
                foreach (DataColumn dc in data.Columns)
                {
                    if (excludeColNames != null && excludeColNames.Contains(dc.ColumnName, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    var value = (dr[dc] ?? string.Empty).ToString();
                    lstTmp.Add(value);
                }
                sb.Append(string.Join(" | ", lstTmp));
                sb.Append(" | ");
            }
            var md = sb.ToString();
            return md;
        }


        public static void Export(string fileName, string dbName, List<TableDto> dtos)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# 数据库表目录");
            var dirMD = dtos.MarkDown("Columns");
            dirMD = Regex.Replace(dirMD, @"(.+?\|\s+)([a-zA-Z][a-zA-Z0-9_]+)(\s+\|.+\n?)", $"$1[$2](#$2)$3", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            sb.Append(dirMD);
            sb.AppendLine();

            foreach (var dto in dtos)
            {
                sb.AppendLine();
                sb.AppendLine($"## <a name=\"{dto.TableName}\">{dto.TableName} {dto.Comment}</a>");
                sb.Append(dto.Columns.MarkDown());
                sb.AppendLine();
            }
            var md = sb.ToString();

            File.WriteAllText(fileName, md, Encoding.UTF8);
        }

    }
}
