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
            pdfDocument.Open(); // 打开文档

            // TODO 标题
            iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("数据库字典文档\n\n", BaseFont(fontPath, 30, iTextSharp.text.Font.BOLD));
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfDocument.Add(title);
            iTextSharp.text.Paragraph subTitle = new iTextSharp.text.Paragraph(" —— " + databaseName, BaseFont(fontPath, 20, iTextSharp.text.Font.NORMAL));
            subTitle.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfDocument.Add(subTitle);

            // TODO PDF换页
            pdfDocument.NewPage();

            // TODO 创建添加书签章节
            int chapterNum = 1;
            // TODO 全局字体设置，处理iTextSharp中文不识别显示问题
            iTextSharp.text.Font pdfFont = BaseFont(fontPath, 12, iTextSharp.text.Font.NORMAL);

            // TODO log table
            iTextSharp.text.Chapter chapter1 = new iTextSharp.text.Chapter(new iTextSharp.text.Paragraph(AppConst.LOG_CHAPTER_NAME, pdfFont), chapterNum);
            pdfDocument.Add(chapter1);
            pdfDocument.Add(new iTextSharp.text.Paragraph("\n", pdfFont)); // 换行
            CreateLogTable(pdfDocument, pdfFont, tables);
            // TODO PDF换页
            pdfDocument.NewPage();

            // TODO overview table
            iTextSharp.text.Chapter chapter2 = new iTextSharp.text.Chapter(new iTextSharp.text.Paragraph(AppConst.TABLE_CHAPTER_NAME, pdfFont), (++chapterNum));
            pdfDocument.Add(chapter2);
            pdfDocument.Add(new iTextSharp.text.Paragraph("\n", pdfFont)); // 换行
            CreateOverviewTable(pdfDocument, pdfFont, tables);
            // TODO PDF换页
            pdfDocument.NewPage();

            // TODO table structure
            // TODO 添加书签章节
            iTextSharp.text.Chapter chapter3 = new iTextSharp.text.Chapter(new iTextSharp.text.Paragraph(AppConst.TABLE_STRUCTURE_CHAPTER_NAME, pdfFont), (++chapterNum));
            chapter3.BookmarkOpen = true;
            pdfDocument.Add(chapter3);
            pdfDocument.Add(new iTextSharp.text.Paragraph("\n", pdfFont)); // 换行

            foreach (var table in tables)
            {
                string docTableName = table.TableName + " " + (!string.IsNullOrWhiteSpace(table.Comment) ? table.Comment : "");
                // TODO 添加书签章节
                iTextSharp.text.Section selection = chapter3.AddSection(20f, new iTextSharp.text.Paragraph(docTableName, pdfFont), chapterNum);
                pdfDocument.Add(selection);
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
            }

            // TODO 关闭释放PDF文档资源
            pdfDocument.Close();
        }

        /// <summary>
        /// create log table
        /// </summary>
        /// <param name="pdfDocument"></param>
        /// <param name="pdfFont"></param>
        /// <param name="tables"></param>
        private static void CreateLogTable(iTextSharp.text.Document pdfDocument, iTextSharp.text.Font pdfFont, List<TableDto> tables)
        {
            // TODO 创建表格
            iTextSharp.text.pdf.PdfPTable pdfTable = new iTextSharp.text.pdf.PdfPTable(5);

            // TODO 添加列标题
            pdfTable.AddCell(CreatePdfPCell("版本号", pdfFont));
            pdfTable.AddCell(CreatePdfPCell("修订日期", pdfFont));
            pdfTable.AddCell(CreatePdfPCell("修订内容", pdfFont));
            pdfTable.AddCell(CreatePdfPCell("修订人", pdfFont));
            pdfTable.AddCell(CreatePdfPCell("审核人", pdfFont));
            for (var i = 0; i < 16; i++)
            {
                // TODO 添加数据行,循环数据库表字段
                pdfTable.AddCell(CreatePdfPCell("", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("", pdfFont));
                pdfTable.AddCell(CreatePdfPCell("", pdfFont));
            }

            // TODO 设置表格居中
            pdfTable.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfTable.TotalWidth = 540F;
            pdfTable.LockedWidth = true;
            pdfTable.SetWidths(new float[] { 80F, 100F, 200F, 80F, 80F });

            // TODO 添加表格
            pdfDocument.Add(pdfTable);
        }

        /// <summary>
        /// create overview table
        /// </summary>
        /// <param name="pdfDocument"></param>
        /// <param name="pdfFont"></param>
        /// <param name="tables"></param>
        private static void CreateOverviewTable(iTextSharp.text.Document pdfDocument, iTextSharp.text.Font pdfFont, List<TableDto> tables)
        {
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

        /// <summary>
        /// 创建pdf表格单元格
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pdfFont"></param>
        /// <returns></returns>
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
