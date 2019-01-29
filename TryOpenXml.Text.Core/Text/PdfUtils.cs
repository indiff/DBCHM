using System.Collections.Generic;
using TryOpenXml.Dtos;

namespace TryOpenXml.Text
{
    /// <summary>
    /// Pdf处理工具类
    /// </summary>
    public static class PdfUtils
    {

        /// <summary>
        /// 引用iTextSharp.dll导出pdf数据库字典文档
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="tables"></param>
        public static void ExportPdfByITextSharp(string fileName, string fontPath, string databaseName, List<TableDto> tables)
        {
            // TODO 创建并添加文档信息
            iTextSharp.text.Document pdfDocument = new iTextSharp.text.Document();
            pdfDocument.AddTitle(fileName);

            iTextSharp.text.pdf.PdfWriter pdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDocument, 
                new System.IO.FileStream(fileName, System.IO.FileMode.Create));
            //pdfDocument.AddHeader("DBCHM-51Try.Top", "https://gitee.com/lztkdr/DBCHM");
            pdfDocument.Open(); // 打开文档

            // TODO 遍历数据库表集合
            int chapterNum = 1; // PDF书签章节序号

            // TODO 标题
            iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("数据库字典文档\n\n", BaseFont(fontPath, 30, iTextSharp.text.Font.BOLD));
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfDocument.Add(title);
            iTextSharp.text.Paragraph subTitle = new iTextSharp.text.Paragraph(" —— " + databaseName, BaseFont(fontPath, 20, iTextSharp.text.Font.NORMAL));
            subTitle.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfDocument.Add(subTitle);

            // TODO PDF换页
            pdfDocument.NewPage();

            // TODO 字体设置，处理iTextSharp中文不识别显示问题
            iTextSharp.text.Font pdfFont = BaseFont(fontPath, 12, iTextSharp.text.Font.NORMAL);

            // TODO overview table
            CreateOverviewTable(pdfDocument, pdfFont, tables);

            // TODO PDF换页
            pdfDocument.NewPage();

            // TODO table structure
            foreach (var table in tables)
            {
                string docTableName = table.TableName + " " + (!string.IsNullOrWhiteSpace(table.Comment) ? table.Comment : "");
                // TODO 创建添加书签章节
                iTextSharp.text.Chapter chapter = new iTextSharp.text.Chapter(new iTextSharp.text.Paragraph(docTableName, pdfFont), chapterNum);
                pdfDocument.Add(chapter);
                pdfDocument.Add(new iTextSharp.text.Paragraph("\n", pdfFont)); // 换行

                // TODO 遍历数据库表
                // TODO 创建表格
                iTextSharp.text.pdf.PdfPTable pdfTable = new iTextSharp.text.pdf.PdfPTable(10);
                // TODO 添加列标题
                pdfTable.AddCell(CreatePdfPCell("序号", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("列名", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("数据类型", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("长度", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("小数位", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("主键", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("自增", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("允许空", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("默认值", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("列说明", pdfFont));
                // TODO 添加数据行,循环数据库表字段
                foreach (var column in table.Columns)
                {
                    pdfTable.AddCell(CreatePdfPCell(column.ColumnOrder, pdfFont));
                    pdfTable.AddCell(CreatePdfPCell(column.ColumnName, pdfFont));
                    pdfTable.AddCell(CreatePdfPCell(column.ColumnTypeName, pdfFont));
                    pdfTable.AddCell(CreatePdfPCell(column.Length, pdfFont));
                    pdfTable.AddCell(CreatePdfPCell(column.Scale, pdfFont));
                    pdfTable.AddCell(CreatePdfPCell(column.IsPK, pdfFont));
                    pdfTable.AddCell(CreatePdfPCell(column.IsIdentity, pdfFont));
                    pdfTable.AddCell(CreatePdfPCell(column.CanNull, pdfFont));
                    pdfTable.AddCell(CreatePdfPCell(column.DefaultVal, pdfFont));
                    pdfTable.AddCell(CreatePdfPCell(column.Comment, pdfFont));
                }

                // TODO 设置表格居中
                pdfTable.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                pdfTable.TotalWidth = 520F;
                pdfTable.LockedWidth = true;
                pdfTable.SetWidths(new float[] { 50F, 60F, 60F, 50F, 50F, 50F, 50F, 50F, 50F, 50F });

                // TODO 添加表格
                pdfDocument.Add(pdfTable);

                // TODO PDF换页
                pdfDocument.NewPage();

                // TODO PDF书签章节序号自增
                chapterNum++;
            }

            // TODO 关闭释放PDF文档资源
            pdfDocument.Close();
        }

        /// <summary>
        /// create overview table
        /// </summary>
        /// <param name="pdfDocument"></param>
        /// <param name="pdfFont"></param>
        /// <param name="tables"></param>
        private static void CreateOverviewTable(iTextSharp.text.Document pdfDocument, iTextSharp.text.Font pdfFont, List<TableDto> tables)
        {
            iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("数据库表名", pdfFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfDocument.Add(title);
            pdfDocument.Add(new iTextSharp.text.Paragraph("\n", pdfFont)); // 换行

            // TODO 创建表格
            iTextSharp.text.pdf.PdfPTable pdfTable = new iTextSharp.text.pdf.PdfPTable(3);

            // TODO 添加列标题
            pdfTable.AddCell(CreatePdfPCell("序号", pdfFont));
            pdfTable.AddCell(CreatePdfPCell("表名", pdfFont));
            pdfTable.AddCell(CreatePdfPCell("注释/说明", pdfFont));
            foreach (var table in tables)
            {
                // TODO 添加数据行,循环数据库表字段
                pdfTable.AddCell(CreatePdfPCell(table.TableOrder, pdfFont));
                pdfTable.AddCell(CreatePdfPCell(table.TableName, pdfFont));
                pdfTable.AddCell(CreatePdfPCell((!string.IsNullOrWhiteSpace(table.Comment) ? table.Comment : ""), pdfFont));
            }

            // TODO 设置表格居中
            pdfTable.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfTable.TotalWidth = 330F;
            pdfTable.LockedWidth = true;
            pdfTable.SetWidths(new float[] { 60F, 120F, 150F });

            // TODO 添加表格
            pdfDocument.Add(pdfTable);
        }

        private static iTextSharp.text.pdf.PdfPCell CreatePdfPCell(string text, iTextSharp.text.Font pdfFont)
        {
            iTextSharp.text.Phrase phrase = new iTextSharp.text.Phrase(text, pdfFont);
            iTextSharp.text.pdf.PdfPCell pdfPCell = new iTextSharp.text.pdf.PdfPCell(phrase);

            // TODO 单元格垂直居中显示
            pdfPCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;

            pdfPCell.MinimumHeight = 30;

            return pdfPCell;
        }

        /// <summary>
        /// iTextSharp字体设置
        /// </summary>
        /// <param name="fontPath"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontStyle"></param>
        private static iTextSharp.text.Font BaseFont(string fontPath, float fontSize, int fontStyle)
        {
            iTextSharp.text.pdf.BaseFont chinese = iTextSharp.text.pdf.BaseFont.CreateFont(fontPath, iTextSharp.text.pdf.BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font pdfFont = new iTextSharp.text.Font(chinese, fontSize, fontStyle);
            return pdfFont;
        }

    }
}
