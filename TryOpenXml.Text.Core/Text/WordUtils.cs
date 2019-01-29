using System;
using System.Collections.Generic;
using TryOpenXml.Dtos;

namespace TryOpenXml.Text
{
    /// <summary>
    /// Word处理工具类
    /// </summary>
    public static class WordUtils
    {
        private static string asposeBookmark_prefix = "AsposeBookmark";
        private static string asposeBookmarkLog = "asposeBookmarkLog";
        private static string asposeBookmarkOverview = "asposeBookmarkOverview";

        /// <summary>
        /// 引用Microsoft.Office.Interop.Word.dll导出word数据库字典文档
        /// 注意：使用时需要安装微软office办公软件，如office 2010
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="tables"></param>
        [Obsolete("注意：使用此方法导出word数据库字典文档本机需安装office办公软件，如office 2010", false)]
        public static void ExportWordByMicrosoftOfficeInteropWord(string databaseName, List<TableDto> tables)
        {
            string docTitle = "数据库名：" + databaseName;
            object template = System.Reflection.Missing.Value;
            object oEndOfDoc = @"\endofdoc"; // \endofdoc是预定义的bookmark

            // TODO 依赖冲突，所以用了全类名
            Microsoft.Office.Interop.Word._Application application = new Microsoft.Office.Interop.Word.Application();
            application.Visible = false;
            Microsoft.Office.Interop.Word._Document document = application.Documents.Add(ref template, ref template, ref template, ref template);
            application.ActiveWindow.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdOutlineView;
            application.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekPrimaryHeader;
            application.ActiveWindow.ActivePane.Selection.InsertAfter("DBCHM https://gitee.com/lztkdr/DBCHM");
            application.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            application.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;

            Microsoft.Office.Interop.Word.Paragraph paragraph = document.Content.Paragraphs.Add(ref template);
            paragraph.Range.Text = docTitle;
            paragraph.Range.Font.Bold = 1;
            paragraph.Range.Font.Name = "宋体";
            paragraph.Range.Font.Size = 12f;
            paragraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            paragraph.Format.SpaceAfter = 5f;
            paragraph.OutlineLevel = Microsoft.Office.Interop.Word.WdOutlineLevel.wdOutlineLevel1;
            paragraph.Range.InsertParagraphAfter();

            // TODO 遍历数据库表集合
            foreach (var table in tables)
            {
                string docTableName = table.TableName + " " + (!string.IsNullOrWhiteSpace(table.Comment) ? table.Comment : "");
                // TODO 一级标题
                object oRng = document.Bookmarks[oEndOfDoc].Range;
                Microsoft.Office.Interop.Word.Paragraph paragraph2 = document.Content.Paragraphs.Add(ref oRng);
                paragraph2.Range.Text = docTableName;
                paragraph2.Range.Font.Bold = 1;
                paragraph2.Range.Font.Name = "宋体";
                paragraph2.Range.Font.Size = 10f;
                paragraph2.OutlineLevel = Microsoft.Office.Interop.Word.WdOutlineLevel.wdOutlineLevel2;
                paragraph2.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                paragraph2.Format.SpaceBefore = 15f;
                paragraph2.Format.SpaceAfter = 5f;
                paragraph2.Range.InsertParagraphAfter();

                // TODO 遍历数据库表字段集合
                // TODO 创建表格
                Microsoft.Office.Interop.Word.Range range = document.Bookmarks[oEndOfDoc].Range;
                Microsoft.Office.Interop.Word.Table table2 = document.Tables.Add(range, table.Columns.Count + 1, 10, ref template, ref template);
                table2.Range.Font.Name = "宋体";
                table2.Range.Font.Bold = 0;
                table2.Range.Font.Size = 9f;
                table2.Borders.Enable = 1;
                table2.Rows.Height = 10f;
                table2.AllowAutoFit = true;
                table2.Cell(1, 1).Range.Text = "序号";
                table2.Cell(1, 2).Range.Text = "列名";
                table2.Cell(1, 3).Range.Text = "数据类型";
                table2.Cell(1, 4).Range.Text = "长度";
                table2.Cell(1, 5).Range.Text = "小数位";
                table2.Cell(1, 6).Range.Text = "主键";
                table2.Cell(1, 7).Range.Text = "自增";
                table2.Cell(1, 8).Range.Text = "允许空";
                table2.Cell(1, 9).Range.Text = "默认值";
                table2.Cell(1, 10).Range.Text = "列说明";
                // TODO 分别设置word文档中表格的列宽

                int j = 0;
                foreach (var column in table.Columns)
                {
                    table2.Cell(j + 2, 1).Range.Text = column.ColumnOrder;
                    table2.Cell(j + 2, 2).Range.Text = column.ColumnName;
                    table2.Cell(j + 2, 3).Range.Text = column.ColumnTypeName;
                    table2.Cell(j + 2, 4).Range.Text = column.Length;
                    table2.Cell(j + 2, 5).Range.Text = column.Scale;
                    table2.Cell(j + 2, 6).Range.Text = column.IsPK;
                    table2.Cell(j + 2, 7).Range.Text = column.IsIdentity;
                    table2.Cell(j + 2, 8).Range.Text = column.CanNull;
                    table2.Cell(j + 2, 9).Range.Text = column.DefaultVal;
                    table2.Cell(j + 2, 10).Range.Text = column.Comment;
                    j++;
                }
            }

            application.Visible = true;
            document.Activate();
        }

        /// <summary>
        /// 引用Aspose.Words.dll导出word数据库字典文档
        /// 注意：不依赖微软office办公软件
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="tables"></param>
        public static void ExportWordByAsposeWords(string fileName, string databaseName, List<TableDto> tables)
        {
            Aspose.Words.Document doc = new Aspose.Words.Document();

            // TODO document properties
            doc.BuiltInDocumentProperties.Subject = "设计文档";
            doc.BuiltInDocumentProperties.ContentType = "数据库字典";
            doc.BuiltInDocumentProperties.Title = "数据库字典文档";
            doc.BuiltInDocumentProperties.Author = doc.BuiltInDocumentProperties.LastSavedBy = doc.BuiltInDocumentProperties.Manager = "trycache";
            doc.BuiltInDocumentProperties.Company = "51Try.Top";
            doc.BuiltInDocumentProperties.Version = doc.BuiltInDocumentProperties.RevisionNumber = 1;
            doc.BuiltInDocumentProperties.ContentStatus = "初稿";
            doc.BuiltInDocumentProperties.NameOfApplication = "DBCHM";
            doc.BuiltInDocumentProperties.LastSavedTime = doc.BuiltInDocumentProperties.CreatedTime = System.DateTime.Now;

            // TODO header and footer setting
            Aspose.Words.HeaderFooter header = new Aspose.Words.HeaderFooter(doc, Aspose.Words.HeaderFooterType.HeaderPrimary);
            doc.FirstSection.HeadersFooters.Add(header);
            // Add a paragraph with text to the header.
            header.AppendParagraph("51Try.Top数据库字典文档").ParagraphFormat.Alignment =
                Aspose.Words.ParagraphAlignment.Right;
            Aspose.Words.HeaderFooter footer = new Aspose.Words.HeaderFooter(doc, Aspose.Words.HeaderFooterType.FooterPrimary);
            doc.FirstSection.HeadersFooters.Add(footer);
            // Add a paragraph with text to the footer.
            footer.AppendParagraph("DBCHM-51Try.Top  https://gitee.com/lztkdr/DBCHM").ParagraphFormat.Alignment = 
                Aspose.Words.ParagraphAlignment.Center;

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            // TODO 创建文档标题书签
            CreateBookmark(builder, Aspose.Words.ParagraphAlignment.Center, Aspose.Words.OutlineLevel.Level1, 25, 
                asposeBookmark_prefix + "0", "数据库字典文档");
            builder.ParagraphFormat.OutlineLevel = Aspose.Words.OutlineLevel.BodyText;
            builder.Writeln("—— " + databaseName);

            // TODO 换行
            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);

            // TODO 数据库字典文档修订日志表
            CreateBookmark(builder, Aspose.Words.ParagraphAlignment.Center, Aspose.Words.OutlineLevel.Level2, 16,
                asposeBookmarkLog, AppConst.LOG_CHAPTER_NAME);
            CreateLogTable(builder);
            builder.InsertBreak(Aspose.Words.BreakType.PageBreak);

            // TODO 创建数据库字典文档数据库概况一览表
            CreateBookmark(builder, Aspose.Words.ParagraphAlignment.Center, Aspose.Words.OutlineLevel.Level2, 16,
                asposeBookmarkOverview, AppConst.TABLE_CHAPTER_NAME);
            CreateOverviewTable(builder, tables);
            builder.InsertBreak(Aspose.Words.BreakType.PageBreak);

            // TODO 创建书签
            CreateBookmark(builder, Aspose.Words.ParagraphAlignment.Left, Aspose.Words.OutlineLevel.Level2, 16,
                asposeBookmark_prefix + 0, AppConst.TABLE_STRUCTURE_CHAPTER_NAME);

            int i = 0; // 计数器
            // TODO 遍历数据库表集合
            foreach (var table in tables)
            {
                string bookmarkName = table.TableName + " " + (!string.IsNullOrWhiteSpace(table.Comment) ? table.Comment : "");

                // TODO 创建书签
                CreateBookmark(builder, Aspose.Words.ParagraphAlignment.Left, Aspose.Words.OutlineLevel.Level3, 16,
                    asposeBookmark_prefix + i, table.TableOrder + "、" + bookmarkName);

                // TODO 遍历数据库表字段集合
                // TODO 创建表格
                Aspose.Words.Tables.Table asposeTable = builder.StartTable();

                // 清除段落样式
                builder.ParagraphFormat.ClearFormatting();

                #region 表格列设置，列标题，列宽，字体等
                // Make the header row.
                builder.InsertCell();
                // Set the left indent for the table. Table wide formatting must be applied after 
                // at least one row is present in the table.
                asposeTable.Alignment = Aspose.Words.Tables.TableAlignment.Center;

                // Set height and define the height rule for the header row.
                builder.RowFormat.Height = 40.0;
                builder.RowFormat.HeightRule = Aspose.Words.HeightRule.AtLeast;
                // Some special features for the header row.
                builder.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(198, 217, 241);
                builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
                builder.Font.Size = 14;
                builder.Font.Name = "Arial";
                builder.Font.Bold = true;
                //builder.CellFormat.Width = 100.0;
                builder.Write("序号");

                // We don't need to specify the width of this cell because it's inherited from the previous cell.
                builder.InsertCell();
                builder.Write("列名");

                builder.InsertCell();
                builder.Write("数据类型");

                builder.InsertCell();
                builder.Write("长度");

                builder.InsertCell();
                builder.Write("小数位");

                builder.InsertCell();
                builder.Write("主键");

                builder.InsertCell();
                builder.Write("允许空");

                builder.InsertCell();
                builder.Write("默认值");

                builder.InsertCell();
                builder.Write("列说明");
                builder.EndRow();
                #endregion

                foreach (var column in table.Columns)
                {
                    #region 遍历表格数据行写入
                    // Set features for the other rows and cells.
                    builder.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.White;
                    builder.CellFormat.Width = 100.0;
                    builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                    // Reset height and define a different height rule for table body
                    builder.RowFormat.Height = 60.0;
                    builder.InsertCell();
                    // Reset font formatting.
                    builder.Font.Size = 12;
                    builder.Font.Bold = false;
                    builder.Write(column.ColumnOrder); // 序号

                    builder.InsertCell();
                    builder.Write(column.ColumnName); // 列名

                    builder.InsertCell();
                    builder.Write(column.ColumnTypeName); // 数据类型

                    builder.InsertCell();
                    builder.Write(column.Length); // 长度

                    builder.InsertCell();
                    builder.Write(column.Scale); // 小数位

                    builder.InsertCell();
                    builder.Write(column.IsPK); // 主键

                    builder.InsertCell();
                    builder.Write(column.IsIdentity); // 自增

                    builder.InsertCell();
                    builder.Write(column.CanNull); // 是否为空

                    builder.InsertCell();
                    builder.Write(column.DefaultVal); // 默认值

                    builder.Write(column.Comment); // 列说明

                    builder.EndRow();
                    #endregion
                }

                // TODO 表格创建完成，结束
                asposeTable.AutoFit(Aspose.Words.Tables.AutoFitBehavior.AutoFitToContents);
                builder.EndTable();

                i++;

                // TODO page breaks
                if (i < tables.Count)
                {
                    builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
                }
            }

            // TODO 添加水印
            //InsertWatermarkText(doc, "DBCHM-51Try.Top");

            doc.Save(fileName);
        }

        /// <summary>
        /// 创建数据库字典文档修订日志表
        /// </summary>
        /// <param name="builder"></param>
        private static void CreateLogTable(Aspose.Words.DocumentBuilder builder)
        {
            // 清除段落样式
            builder.ParagraphFormat.ClearFormatting();

            // TODO 创建表格
            Aspose.Words.Tables.Table logTable = builder.StartTable();

            #region 表格列设置，列标题，列宽，字体等
            // Make the header row.
            builder.InsertCell();
            // Set the left indent for the table. Table wide formatting must be applied after 
            // at least one row is present in the table.
            logTable.Alignment = Aspose.Words.Tables.TableAlignment.Center;
            logTable.AllowAutoFit = true;
            // Set height and define the height rule for the header row.
            builder.RowFormat.Height = 40.0;
            builder.RowFormat.HeightRule = Aspose.Words.HeightRule.AtLeast;
            // Some special features for the header row.
            builder.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(198, 217, 241);
            builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
            builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
            builder.Font.Size = 14;
            builder.Font.Name = "Arial";
            builder.Font.Bold = true;
            builder.CellFormat.Width = 100.0;
            builder.Write("版本号");

            // We don't need to specify the width of this cell because it's inherited from the previous cell.
            builder.InsertCell();
            builder.Write("修订日期");

            builder.InsertCell();
            builder.Write("修订内容");

            builder.InsertCell();
            builder.Write("修订人");

            builder.InsertCell();
            builder.Write("审核人");

            builder.EndRow();
            #endregion

            for (var i = 0; i < 5; i++)
            {
                #region 遍历表格数据行写入
                // Set features for the other rows and cells.
                builder.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.White;
                builder.CellFormat.Width = 100.0;
                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                // Reset height and define a different height rule for table body
                builder.RowFormat.Height = 40.0;
                builder.InsertCell();
                // Reset font formatting.
                builder.Font.Size = 12;
                builder.Font.Bold = false;
                builder.Write(""); // 版本号

                builder.InsertCell();
                builder.Write(""); // 修订日期

                builder.InsertCell();
                builder.Write(""); // 修订内容

                builder.InsertCell();
                builder.Write(""); // 修订人

                builder.InsertCell();
                builder.Write(""); // 审核人

                builder.EndRow();
                #endregion
            }
            // TODO 表格创建完成，结束
            builder.EndTable();
        }

        /// <summary>
        /// 创建数据库字典文档数据库概况一览表
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tables"></param>
        private static void CreateOverviewTable(Aspose.Words.DocumentBuilder builder, List<TableDto> tables)
        {
            // 清除段落样式
            builder.ParagraphFormat.ClearFormatting();

            // TODO 创建表格
            Aspose.Words.Tables.Table overviewTable = builder.StartTable();

            #region 表格列设置，列标题，列宽，字体等
            // Make the header row.
            builder.InsertCell();
            // Set the left indent for the table. Table wide formatting must be applied after 
            // at least one row is present in the table.
            overviewTable.Alignment = Aspose.Words.Tables.TableAlignment.Center;
            overviewTable.AllowAutoFit = true;
            // Set height and define the height rule for the header row.
            builder.RowFormat.Height = 40.0;
            builder.RowFormat.HeightRule = Aspose.Words.HeightRule.AtLeast;
            // Some special features for the header row.
            builder.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(198, 217, 241);
            builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
            builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
            builder.Font.Size = 14;
            builder.Font.Name = "Arial";
            builder.Font.Bold = true;
            builder.CellFormat.Width = 100.0;
            builder.Write("序号");

            builder.InsertCell();
            builder.Write("表名");

            builder.InsertCell();
            builder.Write("注释/说明");

            builder.EndRow();
            #endregion

            // TODO 遍历数据库表集合
            foreach (var table in tables)
            {
                #region 遍历表格数据行写入
                // Set features for the other rows and cells.
                builder.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.White;
                builder.CellFormat.Width = 100.0;
                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                // Reset height and define a different height rule for table body
                builder.RowFormat.Height = 40.0;
                builder.InsertCell();
                // Reset font formatting.
                builder.Font.Size = 12;
                builder.Font.Bold = false;
                builder.Write(table.TableOrder); // 序号

                builder.InsertCell();
                builder.Write(table.TableName); // 表名

                builder.InsertCell();
                builder.Write((!string.IsNullOrWhiteSpace(table.Comment) ? table.Comment : "")); // 说明
                #endregion

                builder.EndRow();
            }
            // TODO 表格创建完成，结束
            builder.EndTable();
        }

        /// <summary>
        /// 创建书签
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="alignment"></param>
        /// <param name="outlineLevel"></param>
        /// <param name="fontSize"></param>
        /// <param name="bookmarkName"></param>
        /// <param name="bookmarkText"></param>
        private static void CreateBookmark(Aspose.Words.DocumentBuilder builder, Aspose.Words.ParagraphAlignment alignment,
            Aspose.Words.OutlineLevel outlineLevel, double fontSize, string bookmarkName, string bookmarkText)
        {
            // 清除段落样式
            builder.ParagraphFormat.ClearFormatting();

            // TODO 创建书签
            builder.StartBookmark(bookmarkName);
            builder.ParagraphFormat.Alignment = alignment;
            builder.ParagraphFormat.OutlineLevel = outlineLevel;
            builder.ParagraphFormat.SpaceBefore = builder.ParagraphFormat.SpaceAfter = 15;
            builder.Font.Size = fontSize;
            builder.Font.Name = "Arial";
            builder.Font.Bold = true;
            builder.Writeln(bookmarkText);
            builder.EndBookmark(bookmarkName);
        }

        /// <summary>
        /// Inserts a watermark into a document.
        /// </summary>
        /// <param name="doc">The input document.</param>
        /// <param name="watermarkText">Text of the watermark.</param>
        public static void InsertWatermarkText(Aspose.Words.Document doc, string watermarkText)
        {
            // Create a watermark shape. This will be a WordArt shape. 
            // You are free to try other shape types as watermarks.
            Aspose.Words.Drawing.Shape watermark = new Aspose.Words.Drawing.Shape(doc, Aspose.Words.Drawing.ShapeType.TextPlainText);
            // Set up the text of the watermark.
            watermark.TextPath.Text = watermarkText;
            watermark.TextPath.FontFamily = "Arial";
            watermark.Width = 500;
            watermark.Height = 100;
            // Text will be directed from the bottom-left to the top-right corner.
            watermark.Rotation = -40;
            // Remove the following two lines if you need a solid black text.
            watermark.Fill.Color = System.Drawing.Color.Gray; // Try LightGray to get more Word-style watermark
            watermark.StrokeColor = System.Drawing.Color.Gray; // Try LightGray to get more Word-style watermark
            // Place the watermark in the page center.
            watermark.RelativeHorizontalPosition = Aspose.Words.Drawing.RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = Aspose.Words.Drawing.RelativeVerticalPosition.Page;
            watermark.WrapType = Aspose.Words.Drawing.WrapType.None;
            watermark.VerticalAlignment = Aspose.Words.Drawing.VerticalAlignment.Center;
            watermark.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;
            // Create a new paragraph and append the watermark to this paragraph.
            Aspose.Words.Paragraph watermarkPara = new Aspose.Words.Paragraph(doc);
            watermarkPara.AppendChild(watermark);
            // Insert the watermark into all headers of each document section.
            foreach (Aspose.Words.Section sect in doc.Sections)
            {
                // There could be up to three different headers in each section, since we want
                // the watermark to appear on all pages, insert into all headers.
                InsertWatermarkIntoHeader(watermarkPara, sect, Aspose.Words.HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader(watermarkPara, sect, Aspose.Words.HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader(watermarkPara, sect, Aspose.Words.HeaderFooterType.HeaderEven);
            }
        }

        /// <summary>
        /// Inserts a watermark into a document header.
        /// </summary>
        /// <param name="watermarkPara"></param>
        /// <param name="sect"></param>
        /// <param name="headerType"></param>
        public static void InsertWatermarkIntoHeader(Aspose.Words.Paragraph watermarkPara, Aspose.Words.Section sect, Aspose.Words.HeaderFooterType headerType)
        {
            Aspose.Words.HeaderFooter header = sect.HeadersFooters[headerType];
            if (null == header)
            {
                // There is no header of the specified type in the current section, create it.
                header = new Aspose.Words.HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }
            // Insert a clone of the watermark into the header.
            header.AppendChild(watermarkPara.Clone(true));
        }

    }
}
