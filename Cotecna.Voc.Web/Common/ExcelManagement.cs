using Cotecna.Voc.Web.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Cotecna.Voc.Web.Common
{
    public static class ExcelManagement
    {
        #region GenerateCertificateReport
        /// <summary>
        /// Generate an excel file with the information of certificates
        /// </summary>
        /// <param name="dataSource">The list of certificates</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GenerateCertificateReport(CertificateListModel model, string logoPath)
        {
            MemoryStream ms = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                //create the new workbook
                WorkbookPart workbookPart = document.AddWorkbookPart();
                Workbook workbook = new Workbook();
                workbookPart.Workbook = workbook;

                //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

                //get and save the stylesheet
                Stylesheet stylesheet = VocStyleSheet();
                workbookStylesPart.Stylesheet = stylesheet;
                workbookStylesPart.Stylesheet.Save();

                //add the new workseet
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Worksheet worksheet = new Worksheet();
                SheetData sheetData1 = new SheetData();

                Sheets sheets = new Sheets();

                //get the number of columns in the report
                Row rowTitle;

                //get the string name of the columns
                string[] excelColumnNamesTitle = new string[4];
                for (int n = 0; n < 4; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 6; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < 4; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                    }
                    sheetData1.Append(rowTitle);
                }
                List<CertificateDocument> dataSource = model.Certificates.Collection;
                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the title
                UpdateStringCellValue("A2", Resources.Common.CertificateList, currentRowTitle, 5);

                //set min date and max date in header
                Row currentRowDateTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)5);
                string minDate, maxDate;
                //get dates
                if (string.IsNullOrEmpty(model.IssuanceDateFrom) || string.IsNullOrEmpty(model.IssuanceDateTo))
                {
                    minDate = dataSource.Select(x => x.Certificate.IssuanceDate.GetValueOrDefault()).Min().ToString("dd/MM/yyyy");
                    maxDate = dataSource.Select(x => x.Certificate.IssuanceDate.GetValueOrDefault()).Max().ToString("dd/MM/yyyy");
                }
                else
                {
                    minDate = model.IssuanceDateFrom;
                    maxDate = model.IssuanceDateTo;
                }
                
                //write both dates
                UpdateStringCellValue("B5", Resources.Common.IssuanceDateFrom + ": "+minDate, currentRowDateTitle, 7);
                UpdateStringCellValue("C5", Resources.Common.IssuanceDateTo + ": " + maxDate, currentRowDateTitle, 7);
                
                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "A2:D4";
                mergeCells.Append(mergeCell);

                Drawing drawing = AddLogo(logoPath, worksheetPart);

                Columns columns = new Columns();
                columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 32));
                columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 30));
                columns.Append(CreateColumnData((UInt32Value)(uint)3, (UInt32Value)(uint)3, 33));
                columns.Append(CreateColumnData((UInt32Value)(uint)4, (UInt32Value)(uint)4, 45));
                worksheet.Append(columns);
                
                int rowIndex = 8;

                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex, Resources.Common.CertificateNumber, rowData, 2);
                AppendTextCell("B" + rowIndex, Resources.Common.IssuanceDate, rowData, 2);
                AppendTextCell("C" + rowIndex, Resources.Common.CertificateStatus, rowData, 2);
                AppendTextCell("D" + rowIndex, Resources.Common.EntryPoint, rowData, 2);
                sheetData1.Append(rowData);

                rowIndex = 9;

                //build the data
                foreach (var item in dataSource)
                {

                    rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };

                    AppendTextCell("A" + rowIndex.ToString(), item.Certificate.Sequential, rowData, 1);
                    AppendTextCell("B" + rowIndex.ToString(), item.Certificate.IssuanceDate.HasValue ? item.Certificate.IssuanceDate.Value.ToString("dd/MM/yyyy") : "", rowData, 1);
                    AppendTextCell("C" + rowIndex.ToString(), item.Certificate.CertificateStatusId.ToString(), rowData, 1);
                    AppendTextCell("D" + rowIndex.ToString(), item.Certificate.EntryPoint != null ? item.Certificate.EntryPoint.Name : "", rowData, 1);

                    sheetData1.Append(rowData);
                    rowIndex++;
                }
                //add the information of the current sheet
                worksheet.Append(sheetData1);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = Resources.Common.CertificateList, SheetId = (UInt32Value)1, Id = workbookPart.GetIdOfPart(worksheetPart) };
                sheets.Append(sheet);
                //add the new sheet to the report
                workbook.Append(sheets);
                //save all report
                workbook.Save();
                //close the stream.
                document.Close();
            }
            return ms;
        }

        
        #endregion

        #region GenerateUserReport
        /// <summary>
        /// Generate an excel file with the list of users
        /// </summary>
        /// <param name="dataSource">The list of users</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GenerateUserReport(List<UserModel> dataSource, string logoPath)
        {
            MemoryStream ms = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                //create the new workbook
                WorkbookPart workbookPart = document.AddWorkbookPart();
                Workbook workbook = new Workbook();
                workbookPart.Workbook = workbook;

                //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

                //get and save the stylesheet
                Stylesheet stylesheet = VocStyleSheet();
                workbookStylesPart.Stylesheet = stylesheet;
                workbookStylesPart.Stylesheet.Save();

                //add the new workseet
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Worksheet worksheet = new Worksheet();
                SheetData sheetData1 = new SheetData();

                Sheets sheets = new Sheets();

                //get the number of columns in the report
                Row rowTitle;

                //get the string name of the columns
                string[] excelColumnNamesTitle = new string[4];
                for (int n = 0; n < 4; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 6; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < 4; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                    }
                    sheetData1.Append(rowTitle);
                }

                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the business application name
                UpdateStringCellValue("A2", Resources.Common.UserList, currentRowTitle, 5);

                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "A2:D4";
                mergeCells.Append(mergeCell);

                Drawing drawing = AddLogo(logoPath, worksheetPart);

                Columns columns = new Columns();
                columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 45));
                columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 42));
                columns.Append(CreateColumnData((UInt32Value)(uint)3, (UInt32Value)(uint)3, 10));
                columns.Append(CreateColumnData((UInt32Value)(uint)4, (UInt32Value)(uint)4, 32));
                worksheet.Append(columns);

                int rowIndex = 8;

                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex, Resources.Common.Email, rowData, 2);
                AppendTextCell("B" + rowIndex, Resources.Common.FullName, rowData, 2);
                AppendTextCell("C" + rowIndex, Resources.Common.Active, rowData, 2);
                AppendTextCell("D" + rowIndex, Resources.Common.Role, rowData, 2);
                sheetData1.Append(rowData);

                rowIndex = 9;

                //build the data
                foreach (var item in dataSource)
                {

                    rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };

                    AppendTextCell("A" + rowIndex.ToString(), item.Email, rowData, 1);
                    AppendTextCell("B" + rowIndex.ToString(), item.FullName, rowData, 1);
                    AppendTextCell("C" + rowIndex.ToString(), item.IsActive, rowData, 1);
                    AppendTextCell("D" + rowIndex.ToString(), item.Role, rowData, 1);

                    sheetData1.Append(rowData);
                    rowIndex++;
                }

                //add the information of the current sheet
                worksheet.Append(sheetData1);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = Resources.Common.UserList, SheetId = (UInt32Value)1, Id = workbookPart.GetIdOfPart(worksheetPart) };
                sheets.Append(sheet);
                //add the new sheet to the report
                workbook.Append(sheets);
                //save all report
                workbook.Save();
                //close the stream.
                document.Close();
            }
            return ms;
        }
        #endregion

        #region VocStyleSheet
        /// <summary>
        /// Create an stylesheet to use in excel files
        /// </summary>
        /// <returns></returns>
        private static Stylesheet VocStyleSheet()
        {
            Stylesheet styleSheet = new Stylesheet();

            Fonts fonts = new Fonts();
            // 0 - normal fonts
            DocumentFormat.OpenXml.Spreadsheet.Font myFont = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                FontSize = new FontSize() { Val = 11 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                FontName = new FontName() { Val = "Calibri" }
            };
            fonts.Append(myFont);

            //1 - font bold
            myFont = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                Bold = new Bold(),
                FontSize = new FontSize() { Val = 11 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                FontName = new FontName() { Val = "Calibri" }
            };
            fonts.Append(myFont);

            //2 - font title
            myFont = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                Bold = new Bold(),
                FontSize = new FontSize() { Val = 20 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "FFFFFF" } },
                FontName = new FontName() { Val = "Verdana" }
            };
            fonts.Append(myFont);

            //3 - font bold
            myFont = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                Bold = new Bold(),
                FontSize = new FontSize() { Val = 16 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                FontName = new FontName() { Val = "Calibri" }
            };
            fonts.Append(myFont);

            //4 - small font white
            myFont = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                FontSize = new FontSize() { Val = 11 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "FFFFFF" } },
                FontName = new FontName() { Val = "Calibri" }
            };
            fonts.Append(myFont);


            Fills fills = new Fills();
            //default fill
            Fill fill = new Fill()
            {
                PatternFill = new PatternFill() { PatternType = PatternValues.None }
            };
            fills.Append(fill);
            //default fill
            fill = new Fill()
            {
                PatternFill = new PatternFill() { PatternType = PatternValues.Gray125 }
            };
            fills.Append(fill);
            //title fill
            fill = new Fill()
            {
                PatternFill = new PatternFill()
                {
                    ForegroundColor = new ForegroundColor()
                    {
                        Rgb = new HexBinaryValue() { Value = "499EB1" }
                    },
                    PatternType = PatternValues.Solid
                }
            };
            fills.Append(fill);

            Borders borders = new Borders();
            //normal borders
            Border border = new Border()
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);

            //borders applied
            border = new Border()
            {
                LeftBorder = new LeftBorder() { Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }, Style = BorderStyleValues.Thin },
                RightBorder = new RightBorder() { Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }, Style = BorderStyleValues.Thin },
                TopBorder = new TopBorder() { Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }, Style = BorderStyleValues.Thin },
                BottomBorder = new BottomBorder() { Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }, Style = BorderStyleValues.Thin },
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);

            CellFormats cellFormats = new CellFormats();
            //0- normal
            CellFormat cellFormat = new CellFormat()
            {
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //1 - border
            cellFormat = new CellFormat()
            {
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                Alignment = new Alignment() { WrapText = true },
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //2 - border and bold
            cellFormat = new CellFormat()
            {
                FontId = 1,
                FillId = 0,
                BorderId = 1,
                Alignment = new Alignment() { WrapText = true },
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //3 -title
            cellFormat = new CellFormat()
            {
                FontId = 0,
                FillId = 2,
                BorderId = 0,
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //4 - title
            cellFormat = new CellFormat()
            {
                FontId = 2,
                FillId = 2,
                BorderId = 0,
                Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center },
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //5 - title
            cellFormat = new CellFormat()
            {
                FontId = 2,
                FillId = 2,
                BorderId = 0,
                Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.CenterContinuous, Vertical = VerticalAlignmentValues.Center },
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //6 - section title
            cellFormat = new CellFormat()
            {
                FontId = 3,
                FillId = 0,
                BorderId = 1,
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //7 - section title filter values
            cellFormat = new CellFormat()
            {
                FontId = 4,
                FillId = 2,
                BorderId = 0,
                Alignment = new Alignment() { WrapText = false, Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center },
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            styleSheet.Append(fonts);
            styleSheet.Append(fills);
            styleSheet.Append(borders);
            styleSheet.Append(cellFormats);

            return styleSheet;
        }
        #endregion

        #region GetExcelColumnName
        /// <summary>
        /// Get an excel column
        /// </summary>
        /// <param name="columnIndex">index</param>
        /// <returns></returns>
        private static string GetExcelColumnName(int columnIndex)
        {
            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            //
            //  eg  GetExcelColumnName(0) should return "A"
            //      GetExcelColumnName(1) should return "B"
            //      GetExcelColumnName(25) should return "Z"
            //      GetExcelColumnName(26) should return "AA"
            //      GetExcelColumnName(27) should return "AB"
            //      ..etc..
            //
            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            char firstChar = (char)('A' + (columnIndex / 26) - 1);
            char secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }
        #endregion

        #region AppendTextCell
        /// <summary>
        /// Append text in a cell
        /// </summary>
        /// <param name="cellReference">Reference</param>
        /// <param name="cellStringValue">Value</param>
        /// <param name="excelRow">Excel row</param>
        /// <param name="styleIndex">Style</param>
        private static void AppendTextCell(string cellReference, string cellStringValue, Row excelRow, UInt32Value styleIndex)
        {
            //  Add a new Excel Cell to our Row 
            Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.StyleIndex = styleIndex;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }
        #endregion

        #region AppendNumberCell
        /// <summary>
        /// Append text in a cell
        /// </summary>
        /// <param name="cellReference">Reference</param>
        /// <param name="cellStringValue">Value</param>
        /// <param name="excelRow">Excel row</param>
        /// <param name="styleIndex">Style</param>
        private static void AppendNumberCell(string cellReference, string cellStringValue, Row excelRow, UInt32Value styleIndex)
        {
            //  Add a new Excel Cell to our Row 
            Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.Number };
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.StyleIndex = styleIndex;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }
        #endregion

        #region UpdateStringCellValue
        /// <summary>
        /// Update the value of a existing cell
        /// </summary>
        /// <param name="cellReference">Address of the cell</param>
        /// <param name="cellStringValue">Value to update</param>
        /// <param name="excelRow">Row of the cell</param>
        private static void UpdateStringCellValue(string cellReference, string cellStringValue, Row excelRow, UInt32Value styleIndex = null)
        {
            Cell currentCell = excelRow.Elements<Cell>().First(cell => cell.CellReference.Value == cellReference);
            currentCell.CellValue = new CellValue(cellStringValue);
            currentCell.DataType = new EnumValue<CellValues>(CellValues.String);
            if (styleIndex != null)
                currentCell.StyleIndex = styleIndex;
        }
        #endregion

        #region UpdateNumberCellValue
        /// <summary>
        /// Update the value of a existing cell
        /// </summary>
        /// <param name="cellReference">Address of the cell</param>
        /// <param name="cellStringValue">Value to update</param>
        /// <param name="excelRow">Row of the cell</param>
        private static void UpdateNumberCellValue(string cellReference, string cellStringValue, Row excelRow, UInt32Value styleIndex = null)
        {
            Cell currentCell = excelRow.Elements<Cell>().First(cell => cell.CellReference.Value == cellReference);
            currentCell.CellValue = new CellValue(cellStringValue);
            currentCell.DataType = new EnumValue<CellValues>(CellValues.Number);
            if (styleIndex != null)
                currentCell.StyleIndex = styleIndex;
        }
        #endregion

        #region CreateColumnData
        /// <summary>
        /// Create a column and set the height of the column
        /// </summary>
        /// <param name="StartColumnIndex">Initial index</param>
        /// <param name="EndColumnIndex">End index</param>
        /// <param name="ColumnWidth">Column width</param>
        /// <returns></returns>
        private static Column CreateColumnData(UInt32 StartColumnIndex, UInt32 EndColumnIndex, double ColumnWidth)
        {
            Column column;
            column = new Column();
            column.Min = StartColumnIndex;
            column.Max = EndColumnIndex;
            column.Width = ColumnWidth;
            column.CustomWidth = true;
            return column;
        }
        #endregion

        #region AddLogo
        /// <summary>
        /// Add the logo of the system
        /// </summary>
        /// <param name="logoPath">Path of the logo</param>
        /// <param name="worksheetPart">Worksheet Part</param>
        /// <returns>Drawing</returns>
        private static Drawing AddLogo(string logoPath, WorksheetPart worksheetPart)
        {
            string sImagePath = logoPath;
            DrawingsPart dp = worksheetPart.AddNewPart<DrawingsPart>();
            ImagePart imgp = dp.AddImagePart(ImagePartType.Png, worksheetPart.GetIdOfPart(dp));
            using (FileStream fs = new FileStream(sImagePath, FileMode.Open, FileAccess.Read))
            {
                imgp.FeedData(fs);
            }

            DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties nvdp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties();
            nvdp.Id = 1025;
            nvdp.Name = "Picture 1";
            nvdp.Description = "logo";
            DocumentFormat.OpenXml.Drawing.PictureLocks picLocks = new DocumentFormat.OpenXml.Drawing.PictureLocks();
            picLocks.NoChangeAspect = true;
            picLocks.NoChangeArrowheads = true;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureDrawingProperties nvpdp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureDrawingProperties();
            nvpdp.PictureLocks = picLocks;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties nvpp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties();
            nvpp.NonVisualDrawingProperties = nvdp;
            nvpp.NonVisualPictureDrawingProperties = nvpdp;

            DocumentFormat.OpenXml.Drawing.Stretch stretch = new DocumentFormat.OpenXml.Drawing.Stretch();
            stretch.FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle();

            DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill blipFill = new DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill();
            DocumentFormat.OpenXml.Drawing.Blip blip = new DocumentFormat.OpenXml.Drawing.Blip();
            blip.Embed = dp.GetIdOfPart(imgp);
            blip.CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print;
            blipFill.Blip = blip;
            blipFill.SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle();
            blipFill.Append(stretch);

            DocumentFormat.OpenXml.Drawing.Transform2D t2d = new DocumentFormat.OpenXml.Drawing.Transform2D();
            DocumentFormat.OpenXml.Drawing.Offset offset = new DocumentFormat.OpenXml.Drawing.Offset();
            offset.X = 0;
            offset.Y = 0;
            t2d.Offset = offset;
            Bitmap bm = new Bitmap(sImagePath);
            DocumentFormat.OpenXml.Drawing.Extents extents = new DocumentFormat.OpenXml.Drawing.Extents();
            extents.Cx = (long)bm.Width * (long)((float)914400 / bm.HorizontalResolution);
            extents.Cy = (long)bm.Height * (long)((float)914400 / bm.VerticalResolution);
            bm.Dispose();
            t2d.Extents = extents;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.ShapeProperties sp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.ShapeProperties();
            sp.BlackWhiteMode = DocumentFormat.OpenXml.Drawing.BlackWhiteModeValues.Auto;
            sp.Transform2D = t2d;
            DocumentFormat.OpenXml.Drawing.PresetGeometry prstGeom = new DocumentFormat.OpenXml.Drawing.PresetGeometry();
            prstGeom.Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle;
            prstGeom.AdjustValueList = new DocumentFormat.OpenXml.Drawing.AdjustValueList();
            sp.Append(prstGeom);
            sp.Append(new DocumentFormat.OpenXml.Drawing.NoFill());

            DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture();
            picture.NonVisualPictureProperties = nvpp;
            picture.BlipFill = blipFill;
            picture.ShapeProperties = sp;

            DocumentFormat.OpenXml.Drawing.Spreadsheet.Position pos = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Position();
            pos.X = 18 * 914400  / 72;
            pos.Y = 28 * 914400 / 72;

            Extent ext = new Extent();
            ext.Cx = extents.Cx;
            ext.Cy = extents.Cy;
            AbsoluteAnchor anchor = new AbsoluteAnchor();
            anchor.Position = pos;
            anchor.Extent = ext;
            

            anchor.Append(picture);
            anchor.Append(new ClientData());
            WorksheetDrawing wsd = new WorksheetDrawing();
            wsd.Append(anchor);
            Drawing drawing = new Drawing();
            drawing.Id = dp.GetIdOfPart(imgp);

            wsd.Save(dp);
            return drawing;
        }
        #endregion
    }
}