using System.Collections.Generic;
using TryOpenXml.Dtos;

namespace TryOpenXml.Text
{
    /// <summary>
    /// Excel处理工具类
    /// </summary>
    public static class ExcelUtils
    {

        /// <summary>
        /// 引用EPPlus.dll导出excel数据库字典文档
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="databaseName"></param>
        /// <param name="tables"></param>
        public static void ExportExcelByEpplus(string fileName, string databaseName, List<TableDto> tables)
        {
            System.IO.FileInfo xlsFileInfo = new System.IO.FileInfo(fileName);
            if (xlsFileInfo.Exists)
            {
                // TODO 注意此处：存在Excel文档即删除再创建一个
                xlsFileInfo.Delete();
                xlsFileInfo = new System.IO.FileInfo(fileName);
            }
            // TODO 创建并添加Excel文档信息
            using (OfficeOpenXml.ExcelPackage epck = new OfficeOpenXml.ExcelPackage(xlsFileInfo))
            {
                // TODO 创建overview sheet
                CreateLogSheet(epck, "修订日志", tables);

                // TODO 创建overview sheet
                CreateOverviewSheet(epck, "数据库表名", tables);

                // TODO 创建tables sheet
                CreateTableSheet(epck, "数据库表结构", tables);

                epck.Save(); // 保存excel
                epck.Dispose();
            }
        }

        /// <summary>
        /// 创建修订日志sheet
        /// </summary>
        /// <param name="epck"></param>
        /// <param name="sheetName"></param>
        /// <param name="tables"></param>
        private static void CreateLogSheet(OfficeOpenXml.ExcelPackage epck, string sheetName, List<TableDto> tables)
        {
            OfficeOpenXml.ExcelWorksheet overviewTbWorksheet = epck.Workbook.Worksheets.Add(sheetName);

            int row = 1;

            overviewTbWorksheet.Cells[row, 1, row, 5].Merge = true;
            //overviewTbWorksheet.Cells[row, 1].Value = "总表数量";
            //overviewTbWorksheet.Cells[row, 2].Value = tables.Count + "";
            //overviewTbWorksheet.Cells[row, 4].Value = "密码等级";
            //overviewTbWorksheet.Cells[row, 5].Value = "秘密";

            row++; // 行号+1

            overviewTbWorksheet.Cells[row, 1].Value = "版本号";
            overviewTbWorksheet.Cells[row, 2].Value = "修订日期";
            overviewTbWorksheet.Cells[row, 3].Value = "修订内容";
            overviewTbWorksheet.Cells[row, 4].Value = "修订人";
            overviewTbWorksheet.Cells[row, 5].Value = "审核人";
            overviewTbWorksheet.Cells[row, 1, row, 5].Style.Font.Bold = true;
            overviewTbWorksheet.Cells[row, 1, row, 5].Style.Font.Size = 10;
            overviewTbWorksheet.Row(1).Height = 20; // 行高

            // TODO 循环日志记录
            row++; // 行号+1
            for (var i = 0; i < 16; i++)
            {
                // TODO 添加列标题
                overviewTbWorksheet.Cells[row, 1].Value = "";
                overviewTbWorksheet.Cells[row, 2].Value = "";
                overviewTbWorksheet.Cells[row, 3].Value = "";
                overviewTbWorksheet.Cells[row, 4].Value = "";
                overviewTbWorksheet.Cells[row, 5].Value = "";

                overviewTbWorksheet.Row(row).Height = 20; // 行高

                row++; // 行号+1
            }
            // TODO 水平居中
            overviewTbWorksheet.Cells[1, 1, row - 1, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            // TODO 垂直居中
            overviewTbWorksheet.Cells[1, 1, row - 1, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            // TODO 上下左右边框线
            overviewTbWorksheet.Cells[1, 1, row - 1, 5].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            overviewTbWorksheet.Cells[1, 1, row - 1, 5].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            overviewTbWorksheet.Cells[1, 1, row - 1, 5].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            overviewTbWorksheet.Cells[1, 1, row - 1, 5].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            overviewTbWorksheet.Column(1).Width = 25;
            overviewTbWorksheet.Column(2).Width = 25;
            overviewTbWorksheet.Column(3).Width = 50;
            overviewTbWorksheet.Column(4).Width = 25;
            overviewTbWorksheet.Column(5).Width = 25;
        }

        /// <summary>
        /// 创建表名sheet
        /// </summary>
        /// <param name="epck"></param>
        /// <param name="sheetName"></param>
        /// <param name="tables"></param>
        private static void CreateOverviewSheet(OfficeOpenXml.ExcelPackage epck, string sheetName, List<TableDto> tables)
        {
            OfficeOpenXml.ExcelWorksheet overviewTbWorksheet = epck.Workbook.Worksheets.Add(sheetName);

            int row = 1;
            overviewTbWorksheet.Cells[row, 1].Value = "序号";
            overviewTbWorksheet.Cells[row, 2].Value = "表名";
            overviewTbWorksheet.Cells[row, 3].Value = "注释/说明";
            overviewTbWorksheet.Cells[row, 1, row, 3].Style.Font.Bold = true;
            overviewTbWorksheet.Cells[row, 1, row, 3].Style.Font.Size = 16;
            overviewTbWorksheet.Row(1).Height = 30; // 行高

            // TODO 循环数据库表名
            row++;
            foreach (var table in tables)
            {
                // TODO 数据库名称
                // TODO 添加列标题
                overviewTbWorksheet.Cells[row, 1].Value = table.TableOrder;
                overviewTbWorksheet.Cells[row, 2].Value = table.TableName;
                overviewTbWorksheet.Cells[row, 3].Value = (!string.IsNullOrWhiteSpace(table.Comment) ? table.Comment : "");

                overviewTbWorksheet.Row(row).Height = 30; // 行高

                row++; // 行号+1
            }
            // TODO 水平居中
            overviewTbWorksheet.Cells[1, 1, row - 1, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            // TODO 垂直居中
            overviewTbWorksheet.Cells[1, 1, row - 1, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            // TODO 上下左右边框线
            overviewTbWorksheet.Cells[1, 1, row - 1, 3].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            overviewTbWorksheet.Cells[1, 1, row - 1, 3].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            overviewTbWorksheet.Cells[1, 1, row - 1, 3].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            overviewTbWorksheet.Cells[1, 1, row - 1, 3].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            overviewTbWorksheet.Column(1).Width = 10;
            overviewTbWorksheet.Column(2).Width = 50;
            overviewTbWorksheet.Column(3).Width = 50;
        }

        /// <summary>
        /// 创建表结构sheet
        /// </summary>
        /// <param name="epck"></param>
        /// <param name="sheetName"></param>
        /// <param name="tables"></param>
        private static void CreateTableSheet(OfficeOpenXml.ExcelPackage epck, string sheetName, List<TableDto> tables)
        {
            OfficeOpenXml.ExcelWorksheet tbWorksheet = epck.Workbook.Worksheets.Add(sheetName);
            int rowNum = 1, fromRow = 0, count = 0; // 行号计数器
                                                    // TODO 循环数据库表名
            foreach (var table in tables)
            {
                // TODO 数据库名称
                tbWorksheet.Cells[rowNum, 1, rowNum, 10].Merge = true;
                tbWorksheet.Cells[rowNum, 1].Value = table.TableName + " " + (!string.IsNullOrWhiteSpace(table.Comment) ? table.Comment : "");
                tbWorksheet.Cells[rowNum, 1, rowNum, 10].Style.Font.Bold = true;
                tbWorksheet.Cells[rowNum, 1, rowNum, 10].Style.Font.Size = 16;
                tbWorksheet.Cells[rowNum, 1, rowNum, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                tbWorksheet.Cells[rowNum, 1, rowNum, 10].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                // TODO 注意：保存起始行号
                fromRow = rowNum;

                rowNum++; // 行号+1

                // TODO 添加列标题
                tbWorksheet.Cells[rowNum, 1].Value = "序号";
                tbWorksheet.Cells[rowNum, 2].Value = "列名";
                tbWorksheet.Cells[rowNum, 3].Value = "数据类型";
                tbWorksheet.Cells[rowNum, 4].Value = "长度";
                tbWorksheet.Cells[rowNum, 5].Value = "小数位";
                tbWorksheet.Cells[rowNum, 6].Value = "主键";
                tbWorksheet.Cells[rowNum, 7].Value = "自增";
                tbWorksheet.Cells[rowNum, 8].Value = "允许空";
                tbWorksheet.Cells[rowNum, 9].Value = "默认值";
                tbWorksheet.Cells[rowNum, 10].Value = "列说明";

                rowNum++; // 行号+1

                // TODO 添加数据行,遍历数据库表字段
                foreach (var column in table.Columns)
                {
                    tbWorksheet.Cells[rowNum, 1].Value = column.ColumnOrder;
                    tbWorksheet.Cells[rowNum, 2].Value = column.ColumnName;
                    tbWorksheet.Cells[rowNum, 3].Value = column.ColumnTypeName;
                    tbWorksheet.Cells[rowNum, 4].Value = column.Length;
                    tbWorksheet.Cells[rowNum, 5].Value = column.Scale;
                    tbWorksheet.Cells[rowNum, 6].Value = column.IsPK;
                    tbWorksheet.Cells[rowNum, 7].Value = column.IsIdentity;
                    tbWorksheet.Cells[rowNum, 8].Value = column.CanNull;
                    tbWorksheet.Cells[rowNum, 9].Value = column.DefaultVal;
                    tbWorksheet.Cells[rowNum, 10].Value = column.Comment;

                    rowNum++; // 行号+1
                }

                // tbWorksheet.Cells[int FromRow, int FromCol, int ToRow, int ToCol]
                // TODO 列标题字体为粗体
                tbWorksheet.Cells[fromRow, 1, rowNum - 1, 10].Style.Font.Bold = true;
                // TODO 水平居中
                tbWorksheet.Cells[fromRow, 1, rowNum - 1, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                // TODO 垂直居中
                tbWorksheet.Cells[fromRow, 1, rowNum - 1, 10].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                // TODO 上下左右边框线
                tbWorksheet.Cells[fromRow, 1, rowNum - 1, 10].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                tbWorksheet.Cells[fromRow, 1, rowNum - 1, 10].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                tbWorksheet.Cells[fromRow, 1, rowNum - 1, 10].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                tbWorksheet.Cells[fromRow, 1, rowNum - 1, 10].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                // TODO 处理空白行，分割用
                if (count < tables.Count - 1)
                {
                    //tbWorksheet.Cells[rowNum, 1, rowNum, 10].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    //tbWorksheet.Cells[rowNum, 1, rowNum, 10].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    tbWorksheet.Cells[rowNum, 1, rowNum, 10].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    tbWorksheet.Cells[rowNum, 1, rowNum, 10].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    tbWorksheet.Cells[rowNum, 1, rowNum, 10].Merge = true;
                    tbWorksheet.Cells[rowNum, 1, rowNum, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    tbWorksheet.Cells[rowNum, 1, rowNum, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DodgerBlue);
                }

                rowNum++; // 行号+1

                count++; // 计数器+1
            }

            // TODO 设置表格样式
            tbWorksheet.Cells.Style.WrapText = true; // 自动换行
            tbWorksheet.Cells.Style.ShrinkToFit = true; // 单元格自动适应大小
        }

    }
}
