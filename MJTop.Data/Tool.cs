using MJTop.Data.SPI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTop.Data
{
    public class Tool
    {
        private DB Db = null;
        private DBType DbType = DBType.SqlServer;
        private IDBInfo Info = null;
        private NameValueCollection TableComments = null;
        private NameValueCollection ExcludeTableCols = null;
        /// <summary>
        /// 处理 数据库数据类型 对应的 C#代码的数据类型 （不同数据库 对 默认值的写法不同）
        /// </summary>
        private Dictionary<string, string> Dict_CSharpType = new Dictionary<string, string>();
        /// <summary>
        /// 处理 默认值 对应的 C#代码 值生成方式（不同数据库 对 默认值的写法不同）
        /// </summary>
        private Dictionary<string, string> Dict_Data_DefValue = new Dictionary<string, string>();
        internal Tool(DB db, IDBInfo info)
        {
            this.Db = db;
            this.Info = info;
            this.TableComments = info.TableComments;
            this.DbType = db.DBType;
            this.ExcludeTableCols = db.GetInsertExcludeColumns();
            switch (DbType)
            {
                case DBType.SqlServer:
                    this.Dict_CSharpType = Global.Dict_SqlServer_CSharpType;
                    this.Dict_Data_DefValue = Global.Dict_SqlServer_DefValue;
                    break;
                case DBType.MySql:
                    this.Dict_CSharpType = Global.Dict_MySql_CSharpType;
                    this.Dict_Data_DefValue = Global.Dict_MySql_DefValue;
                    break;
                case DBType.Oracle:
                case DBType.OracleDDTek:
                    this.Dict_CSharpType = Global.Dict_Oracle_CSharpType;
                    this.Dict_Data_DefValue = Global.Dict_Oracle_DefValue;
                    break;
                case DBType.PostgreSql:
                    this.Dict_CSharpType = Global.Dict_PostgreSql_CSharpType;
                    this.Dict_Data_DefValue = Global.Dict_PostgreSql_DefValue;
                    break;
                case DBType.SQLite:
                    this.Dict_CSharpType = Global.Dict_Sqlite_CSharpType;
                    this.Dict_Data_DefValue = Global.Dict_Sqlite_DefValue;
                    break;
                case DBType.DB2DDTek:
                    this.Dict_CSharpType = Global.Dict_DB2_CSharpType;
                    this.Dict_Data_DefValue = Global.Dict_DB2_DefValue;
                    break;
                default:
                    throw new Exception("未指定Charp类型字典！");
            }
        }

        /// <summary>
        /// 生成实体类
        /// 注：当设置 InsertExcludeColumns 时，排除列 不会在成 实体中 的属性
        /// </summary>
        /// <param name="codeDir">存放生成实体类的文件夹</param>
        public void GenerateEntityCode(string codeDir)
        {
            GenerateEntityCode(codeDir, null);
        }

        /// <summary>
        /// 生成实体类
        /// 注：当设置 InsertExcludeColumns 时，排除列 不会在成 实体中 的属性
        /// </summary>
        /// <param name="codeDir">存放生成实体类的文件夹</param>
        /// <param name="strNamespace">命名空间</param>
        public void GenerateEntityCode(string codeDir, string strNamespace)
        {
            GenerateEntityCode(codeDir, strNamespace, false);
        }

        /// <summary>
        /// 生成实体类
        /// 注：当设置 InsertExcludeColumns 时，排除列 不会在成 实体中 的属性
        /// </summary>
        /// <param name="codeDir">存放生成实体类的文件夹</param>
        /// <param name="strNamespace">命名空间</param>
        /// <param name="AutoNameSpaceByTableNamePrefix">是否根据表名前缀，自动附加生成命名空间</param>
        public void GenerateEntityCode(string codeDir, string strNamespace, bool AutoNameSpaceByTableNamePrefix)
        {
            if (string.IsNullOrWhiteSpace(codeDir))
            {
                throw new ArgumentNullException("CodeDir");
            }

            if (!Directory.Exists(codeDir))
            {
                Directory.CreateDirectory(codeDir);
            }

            foreach (var item in Info.TableColumnInfoDict)
            {
                int index = Info.TableNames.FindIndex(t => t.Equals(item.Key, StringComparison.OrdinalIgnoreCase));
                string rowTableName = Info.TableNames[index];
                string filePath = Path.Combine(codeDir, rowTableName + ".cs");

                string newCodeDir = codeDir, newNameSpace = strNamespace;
                if (AutoNameSpaceByTableNamePrefix && rowTableName.Contains("_"))
                {
                    string prefix = rowTableName.Split('_')[0].ToUpper();

                    newCodeDir = Path.Combine(codeDir, prefix);

                    if (!Directory.Exists(newCodeDir))
                    {
                        Directory.CreateDirectory(newCodeDir);
                    }
                    filePath = Path.Combine(newCodeDir, rowTableName + ".cs");

                    newNameSpace = strNamespace + "." + prefix;
                }
                Generate(newNameSpace, rowTableName, item.Value, filePath);
            }
        }


        /// <summary>
        /// 单表生成实体类
        /// </summary>
        /// <param name="strNamespace"></param>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="filePath"></param>
        private void Generate(string strNamespace, string tableName, List<ColumnInfo> columns, string filePath)
        {
            StringPlus plus = new StringPlus();
            plus.AppendLine("using System;");
            plus.AppendLine("using System.Collections.Generic;");
            plus.AppendLine("using System.Linq;");
            plus.AppendLine("using System.Text;");
            plus.AppendLine();

            if (!string.IsNullOrWhiteSpace(strNamespace))
            {
                plus.AppendLine("namespace " + strNamespace);
                plus.AppendLine("{");
            }           
            
            if (!string.IsNullOrWhiteSpace(TableComments[tableName]))
            {
                plus.AppendSpaceLine(1, "/// <summary>");

                string comment = TableComments[tableName];
                string[] strs = comment.Split(new string[] { "\r\n" }, StringSplitOptions.None);               
                foreach (var str in strs)
                {
                    plus.AppendSpaceLine(1, "/// " + str.Trim());
                }

                plus.AppendSpaceLine(1, "/// </summary>");
            }

            plus.AppendSpaceLine(1, "[Serializable]");
            plus.AppendSpaceLine(1, "public partial class " + tableName);
            plus.AppendSpaceLine(1, "{");

            plus.AppendSpaceLine(2, "public " + tableName + "()");
            plus.AppendSpaceLine(2, "{");
            foreach (var colInfo in columns)
            {
                AppendDefaultVal(plus, 3, colInfo);
            }
            plus.AppendSpaceLine(2, "}");

            plus.AppendLine();

            foreach (var colInfo in columns)
            {
                if (ExcludeTableCols[tableName] != null 
                    && ExcludeTableCols.ContainsValue(colInfo.ColumnName))
                {
                    continue;
                }
                AppendColumn(plus, 2, colInfo);
            }

            plus.AppendSpaceLine(1, "}");

            if (!string.IsNullOrWhiteSpace(strNamespace))
            {
                plus.AppendLine("}");
            }

            File.WriteAllText(filePath, plus.Value, Encoding.UTF8);
        }


        /// <summary>
        /// 处理单个 属性 列
        /// </summary>
        /// <param name="plus"></param>
        /// <param name="SpaceNum"></param>
        /// <param name="colInfo"></param>
        private void AppendColumn(StringPlus plus, int SpaceNum, ColumnInfo colInfo)
        {
            if (!string.IsNullOrWhiteSpace(colInfo.DeText))
            {
                plus.AppendSpaceLine(SpaceNum, "/// <summary>");
                
                string[] strs = colInfo.DeText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                foreach (var str in strs)
                {
                    plus.AppendSpaceLine(SpaceNum, "/// " + str.Trim());
                }

                plus.AppendSpaceLine(SpaceNum, "/// </summary>");
            }

            Dict_CSharpType.TryGetValue(colInfo.TypeName, out string strType);
            strType = strType ?? "object";

            //可以为空,并且默认值也为空
            if (strType != "string" && strType != "object" && colInfo.CanNull 
                && string.IsNullOrWhiteSpace(colInfo.DefaultVal))
            {
                strType += "?";
            }
            plus.AppendSpaceLine(SpaceNum, "public " + strType + " " + colInfo.ColumnName + " { get;set; }");
            plus.AppendLine();
        }

        /// <summary>
        /// 默认值处理
        /// </summary>
        /// <param name="plus"></param>
        /// <param name="SpaceNum"></param>
        /// <param name="colInfo"></param>
        private void AppendDefaultVal(StringPlus plus, int SpaceNum, ColumnInfo colInfo)
        {
            if (!string.IsNullOrWhiteSpace(colInfo.DefaultVal))
            {
                Dict_CSharpType.TryGetValue(colInfo.TypeName, out string strType);
                strType = strType ?? "object";

                //SqlServer中列的 默认值去除括号
                string defValue = colInfo.DefaultVal.Trim('(', ')');

                string newValue;
                Dict_Data_DefValue.TryGetValue(defValue, out newValue);
                newValue = newValue ?? defValue;

                //string类型的默认值 加上 双引号
                if (strType == "string")
                {
                    newValue = "\"" + newValue + "\"";
                }
                else if(strType == "decimal")
                {
                    newValue = newValue + "m";
                }
                else if (strType == "float")
                {
                    newValue = newValue + "f";
                }

                plus.AppendSpaceLine(SpaceNum, "this." + colInfo.ColumnName + " = " + newValue + ";");
            }
        }
    }
}
