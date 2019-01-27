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
        public static void ExportPdfByITextSharp(string fileName, string baseFontPath, string databaseName, List<TableDto> tables)
        {
            // TODO 创建并添加文档信息
            iTextSharp.text.Document pdfDocument = new iTextSharp.text.Document();
            pdfDocument.AddTitle(fileName);

            iTextSharp.text.pdf.PdfWriter pdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDocument, 
                new System.IO.FileStream(fileName, System.IO.FileMode.Create));
            pdfDocument.Open(); // 打开文档

            // TODO 遍历数据库表集合
            int chapterNum = 1; // PDF书签章节序号
            
            // TODO 字体设置，处理iTextSharp中文不识别显示问题
            iTextSharp.text.pdf.BaseFont chinese = iTextSharp.text.pdf.BaseFont.CreateFont(baseFontPath, iTextSharp.text.pdf.BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font pdfFont = new iTextSharp.text.Font(chinese, 12, iTextSharp.text.Font.NORMAL);

            // TODO 标题
            iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("数据库字典文档\n\n", new iTextSharp.text.Font(chinese, 30, iTextSharp.text.Font.BOLD));
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfDocument.Add(title);
            iTextSharp.text.Paragraph subTitle = new iTextSharp.text.Paragraph(" —— " + databaseName, new iTextSharp.text.Font(chinese, 20, iTextSharp.text.Font.NORMAL));
            subTitle.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            pdfDocument.Add(subTitle);

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
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("序号", pdfFont)));
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("列名", pdfFont)));
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("数据类型", pdfFont)));
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("长度", pdfFont)));
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("小数位", pdfFont)));
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("主键", pdfFont)));
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("自增", pdfFont)));
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("允许空", pdfFont)));
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("默认值", pdfFont)));
                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("列说明", pdfFont)));
                // TODO 添加数据行,循环数据库表字段
                foreach (var column in table.Columns)
                {
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.ColumnOrder, pdfFont)));
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.ColumnName, pdfFont)));
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.ColumnTypeName, pdfFont)));
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.Length, pdfFont)));
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.Scale, pdfFont)));
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.IsPK, pdfFont)));
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.IsIdentity, pdfFont)));
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.CanNull, pdfFont)));
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.DefaultVal, pdfFont)));
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(column.Comment, pdfFont)));
                }
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

    }
}
