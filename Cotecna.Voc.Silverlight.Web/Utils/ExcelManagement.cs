using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Web.Resources;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Cotecna.Voc.Silverlight.Web
{
    public static class ExcelManagement
    {
        /// <summary>
        /// This method is use to generate excel reports
        /// </summary>
        /// <typeparam name="T">Data source type</typeparam>
        /// <param name="dataSource">Data Source</param>
        /// <param name="titleList">Title of the report</param>
        /// <param name="classType">Class type</param>
        /// <param name="logoPath">Paht of logo</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GenerateReport<T>(List<T> dataSource, string titleList, Type classType, string logoPath, Dictionary<string,object> parameters = null)
        {
            MemoryStream ms = new MemoryStream();
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart;
                Workbook workbook;

                CreateWorkbook(document, out workbookPart, out workbook);

                WorksheetPart worksheetPart;
                Worksheet worksheet;
                SheetData sheetData1;
                Sheets sheets;
                string[] excelColumnNamesTitle;
                CreateWorksheet<T>(classType, user, workbookPart, out worksheetPart, out worksheet, out sheetData1, out sheets, out excelColumnNamesTitle);


                MergeCells mergeCells;
                Drawing drawing;
                MergeCellsAndColumns(titleList, classType, logoPath, user, worksheetPart, worksheet, sheetData1, excelColumnNamesTitle, out mergeCells, out drawing);

                if (parameters != null)
                {
                    string minDate, maxDate;
                    if (classType.Name.Equals("Certificate"))
                    {
                        minDate = parameters["issuanceDateFrom"].ToString();
                        maxDate = parameters["issuanceDateTo"].ToString();
                        
                        Row currentRowDateTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)5);
                        //write both dates
                        UpdateStringCellValue("C5", ServiceResource.IssuanceDateFrom + ": " + minDate, currentRowDateTitle, 7);
                        UpdateStringCellValue("D5", ServiceResource.IssuanceDateTo + ": " + maxDate, currentRowDateTitle, 7);
                        
                        Row currentRowOfficeTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)6);
                        UpdateStringCellValue("C6", ServiceResource.Office + ": " + parameters["officeName"].ToString(), currentRowOfficeTitle, 7);
                    }
                    else if (classType.Name.Equals("SecurityPaper"))
                    {
                        minDate = parameters["modificationDateFrom"].ToString();
                        maxDate = parameters["modificationDateTo"].ToString();

                        Row currentRowDateTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)5);
                        //write both dates
                        UpdateStringCellValue("C5", ServiceResource.ModificationDateFrom + ": " + minDate, currentRowDateTitle, 7);
                        UpdateStringCellValue("D5", ServiceResource.ModificationDateTo + ": " + maxDate, currentRowDateTitle, 7);
                    }
                }

                sheetData1 = FinalizeWorkbook<T>(dataSource, classType, user, worksheetPart, worksheet, sheetData1, mergeCells, drawing);

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = titleList + " List", SheetId = (UInt32Value)1, Id = workbookPart.GetIdOfPart(worksheetPart) };
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

        /// <summary>
        /// Generate release note report
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <param name="logoPath">Path of logo</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GenerateReleaseNotesReport(List<Certificate> dataSource, string logoPath, Dictionary<string, object> parameters)
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
                string[] excelColumnNamesTitle = new string[14];
                for (int n = 0; n < 14; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 7; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < 14; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                    }
                    sheetData1.Append(rowTitle);
                }

                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the business application name
                UpdateStringCellValue("A2", ServiceResource.ReleaseNotes, currentRowTitle, 5);

                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "A2:G4";
                mergeCells.Append(mergeCell);

                Drawing drawing = AddLogo(logoPath, worksheetPart);

                string minDate = parameters["issuanceDateFrom"].ToString();
                string maxDate = parameters["issuanceDateTo"].ToString();

                
                //set date range
                Row currentRowDateTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)5);
                //write both dates
                UpdateStringCellValue("C5", ServiceResource.IssuanceDateFrom + ": " + minDate, currentRowDateTitle, 7);
                UpdateStringCellValue("D5", ServiceResource.IssuanceDateTo + ": " + maxDate, currentRowDateTitle, 7);
                

                //set office name
                Row currentRowOfficeTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)6);
                UpdateStringCellValue("C6", ServiceResource.Office + ": " + parameters["officeName"].ToString(), currentRowOfficeTitle, 7);

                Columns columns = new Columns();
                columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 32));
                columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 28));
                columns.Append(CreateColumnData((UInt32Value)(uint)3, (UInt32Value)(uint)3, 29));
                columns.Append(CreateColumnData((UInt32Value)(uint)4, (UInt32Value)(uint)4, 26));
                columns.Append(CreateColumnData((UInt32Value)(uint)5, (UInt32Value)(uint)5, 22));
                columns.Append(CreateColumnData((UInt32Value)(uint)6, (UInt32Value)(uint)6, 21));
                columns.Append(CreateColumnData((UInt32Value)(uint)7, (UInt32Value)(uint)7, 37));
                columns.Append(CreateColumnData((UInt32Value)(uint)8, (UInt32Value)(uint)8, 28));
                columns.Append(CreateColumnData((UInt32Value)(uint)9, (UInt32Value)(uint)9, 22));
                columns.Append(CreateColumnData((UInt32Value)(uint)10, (UInt32Value)(uint)10, 15));
                columns.Append(CreateColumnData((UInt32Value)(uint)11, (UInt32Value)(uint)11, 18));
                columns.Append(CreateColumnData((UInt32Value)(uint)12, (UInt32Value)(uint)12, 15));
                columns.Append(CreateColumnData((UInt32Value)(uint)13, (UInt32Value)(uint)13, 17));
                columns.Append(CreateColumnData((UInt32Value)(uint)14, (UInt32Value)(uint)14, 15));
                worksheet.Append(columns);

                int rowIndex = 9;

                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex, ServiceResource.OfficeName, rowData, 2);
                AppendTextCell("B" + rowIndex, ServiceResource.CertificateNumber, rowData, 2);
                AppendTextCell("C" + rowIndex, ServiceResource.IssuanceDate, rowData, 2);
                AppendTextCell("D" + rowIndex, ServiceResource.EntryPoint, rowData, 2);
                AppendTextCell("E" + rowIndex, ServiceResource.Invoiced, rowData, 2);
                AppendTextCell("F" + rowIndex, ServiceResource.RNNumber, rowData, 2);
                AppendTextCell("G" + rowIndex, ServiceResource.Goods, rowData, 2);
                AppendTextCell("H" + rowIndex, ServiceResource.DocumentaryCheckResults, rowData, 2);
                AppendTextCell("I" + rowIndex, ServiceResource.PhysicalCheckResults, rowData, 2);
                AppendTextCell("J" + rowIndex, ServiceResource.OverallResults, rowData, 2);
                AppendTextCell("K" + rowIndex, ServiceResource.Date, rowData, 2);
                AppendTextCell("L" + rowIndex, ServiceResource.NetWeight, rowData, 2);
                AppendTextCell("M" + rowIndex, ServiceResource.NumberTrucks, rowData, 2);
                AppendTextCell("N" + rowIndex, ServiceResource.PaidFees, rowData, 2);
                sheetData1.Append(rowData);

                rowIndex = 10;

                //build the data
                foreach (var item in dataSource)
                {
                    if (item.ReleaseNotes != null && item.ReleaseNotes.Count > 0)
                    {
                        foreach (var releaseNote in item.ReleaseNotes)
                        {
                            rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };

                            AppendTextCell("A" + rowIndex.ToString(), item.Office == null ? "" : item.Office.OfficeName, rowData, 1);
                            AppendTextCell("B" + rowIndex.ToString(), string.IsNullOrEmpty(item.Sequential) ? "" : item.Sequential, rowData, 1);
                            AppendTextCell("C" + rowIndex.ToString(), item.IssuanceDate.HasValue ? item.IssuanceDate.Value.ToString("dd/MM/yyyy") : "", rowData, 1);
                            AppendTextCell("D" + rowIndex.ToString(), item.EntryPoint != null ? item.EntryPoint.Name : "", rowData, 1);
                            AppendTextCell("E" + rowIndex.ToString(), item.IsInvoiced.GetValueOrDefault() ? ServiceResource.Yes : ServiceResource.No, rowData, 1);
                            AppendTextCell("F" + rowIndex, string.Concat(item.Sequential,"/",releaseNote.PartialNumber.GetValueOrDefault().ToString("000")), rowData, 1);
                            AppendTextCell("G" + rowIndex, releaseNote.Goods, rowData, 1);
                            AppendTextCell("H" + rowIndex, releaseNote.DocumentaryCheckResultId.HasValue? releaseNote.DocumentaryCheckResultId.GetValueOrDefault().ToString() : string.Empty, rowData, 1);
                            AppendTextCell("I" + rowIndex, releaseNote.PhysicalCheckResultId.HasValue ? releaseNote.PhysicalCheckResultId.Value.ToString() : string.Empty, rowData, 1);
                            AppendTextCell("J" + rowIndex, releaseNote.OverallResultId.HasValue ? releaseNote.OverallResultId.Value.ToString() : string.Empty, rowData, 1);
                            AppendTextCell("K" + rowIndex, releaseNote.IssuanceDate.ToString("dd/MM/yyyy"), rowData, 1);
                            AppendNumberCell("L" + rowIndex, releaseNote.NetWeight.GetValueOrDefault().ToString(), rowData, 1);
                            AppendNumberCell("M" + rowIndex, releaseNote.NumberOfContainers != null ? releaseNote.NumberOfContainers.GetValueOrDefault().ToString() : string.Empty, rowData, 1);
                            AppendNumberCell("N" + rowIndex, releaseNote.PaidFees != null ? releaseNote.PaidFees.GetValueOrDefault().ToString() : string.Empty, rowData, 1);

                            sheetData1.Append(rowData);
                            rowIndex++;
                        }
                    }
                    else
                    {
                        rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };

                        AppendTextCell("A" + rowIndex.ToString(), item.Office == null ? "" : item.Office.OfficeName, rowData, 1);
                        AppendTextCell("B" + rowIndex.ToString(), string.IsNullOrEmpty(item.Sequential) ? "" : item.Sequential, rowData, 1);
                        AppendTextCell("C" + rowIndex.ToString(), item.IssuanceDate.HasValue ? item.IssuanceDate.Value.ToString("dd/MM/yyyy") : "", rowData, 1);
                        AppendTextCell("D" + rowIndex.ToString(), item.EntryPoint != null ? item.EntryPoint.Name : "", rowData, 1);
                        AppendTextCell("E" + rowIndex.ToString(), item.IsInvoiced.GetValueOrDefault() ? ServiceResource.Yes : ServiceResource.No, rowData, 1);
                        AppendTextCell("F" + rowIndex, string.Empty, rowData, 1);
                        AppendTextCell("G" + rowIndex, string.Empty, rowData, 1);
                        AppendTextCell("H" + rowIndex, string.Empty, rowData, 1);
                        AppendTextCell("I" + rowIndex, string.Empty, rowData, 1);
                        AppendTextCell("J" + rowIndex, string.Empty, rowData, 1);
                        AppendTextCell("K" + rowIndex, string.Empty, rowData, 1);
                        AppendTextCell("L" + rowIndex, string.Empty, rowData, 1);
                        AppendTextCell("M" + rowIndex, string.Empty, rowData, 1);
                        AppendTextCell("N" + rowIndex, string.Empty, rowData, 1);
                        sheetData1.Append(rowData);
                        rowIndex++;
                    }
                }
                //add the information of the current sheet
                worksheet.Append(sheetData1);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = ServiceResource.ReleaseNotes, SheetId = (UInt32Value)1, Id = workbookPart.GetIdOfPart(worksheetPart) };
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

        private static SheetData FinalizeWorkbook<T>(List<T> dataSource, Type classType, VocUser user, WorksheetPart worksheetPart, Worksheet worksheet, SheetData sheetData1, MergeCells mergeCells, Drawing drawing)
        {

            int rowIndex = 0;
            if (classType.Name.Equals("Certificate"))
                rowIndex = 9;
            else
                rowIndex = 8;

            Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
            GetColumnsByTypeClass(ref rowData, classType, rowIndex, user);
            sheetData1.Append(rowData);

            rowIndex++;
            BuildData<T>(dataSource, classType, rowIndex, ref sheetData1, user);

            //add the information of the current sheet
            worksheet.Append(sheetData1);
            //add merged cells
            worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
            worksheet.Append(drawing);
            worksheetPart.Worksheet = worksheet;
            worksheetPart.Worksheet.Save();
            return sheetData1;
        }

        private static void MergeCellsAndColumns(string titleList, Type classType, string logoPath, VocUser user, WorksheetPart worksheetPart, Worksheet worksheet, SheetData sheetData1, string[] excelColumnNamesTitle, out MergeCells mergeCells, out Drawing drawing)
        {
            mergeCells = new MergeCells();

            Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
            //add the business application name
            UpdateStringCellValue("A2", titleList + " List", currentRowTitle, 5);

            //merge all cells in the title
            MergeCell mergeCell = new MergeCell();
            mergeCell.Reference = "A2:" + GetExcelColumnName(excelColumnNamesTitle.Length - 1) + "4";
            mergeCells.Append(mergeCell);
            drawing = AddLogo(logoPath, worksheetPart);

            Columns columns = new Columns();
            columns = SetColumns(columns, classType, user);

            worksheet.Append(columns);
        }

        private static void CreateWorksheet<T>(Type classType, VocUser user, WorkbookPart workbookPart, out WorksheetPart worksheetPart, out Worksheet worksheet, out SheetData sheetData1, out Sheets sheets, out string[] excelColumnNamesTitle)
        {
            //add the new workseet
            worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

            worksheet = new Worksheet();
            sheetData1 = new SheetData();

            sheets = new Sheets();

            //get the number of columns in the report
            Row rowTitle;

            var fields = typeof(T).GetProperties();

            //get the string name of the columns
            excelColumnNamesTitle = new string[GetColumnsNumber(classType, user)];
            for (int n = 0; n < excelColumnNamesTitle.Length; n++)
                excelColumnNamesTitle[n] = GetExcelColumnName(n);

            int lastRow = 6;
            if (classType.Name.Equals("Certificate"))
                lastRow = 7;
            
            //build the title
            for (int i = 1; i <= lastRow; i++)
            {
                rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                for (int cellval = 0; cellval < excelColumnNamesTitle.Length; cellval++)
                {
                    AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                }
                sheetData1.Append(rowTitle);
            }

            
        }

        private static void CreateWorkbook(SpreadsheetDocument document, out WorkbookPart workbookPart, out Workbook workbook)
        {
            //create the new workbook
            workbookPart = document.AddWorkbookPart();
            workbook = new Workbook();
            workbookPart.Workbook = workbook;

            //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
            WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

            //get and save the stylesheet
            Stylesheet stylesheet = VocStyleSheet();
            workbookStylesPart.Stylesheet = stylesheet;
            workbookStylesPart.Stylesheet.Save();
        }

        /// <summary>
        /// Build the data 
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="dataSource">Data source</param>
        /// <param name="classType">Type of class</param>
        /// <param name="rowIndex"></param>
        /// <param name="sheetData1">Data sheet</param>
        /// <param name="user">Logged user</param>
        private static void BuildData<T>(List<T> dataSource, Type classType, int rowIndex, ref SheetData sheetData1,VocUser user)
        {

            if (classType.Name.Equals("Certificate"))
            {
                BuildCertificateReport<T>(dataSource, classType, rowIndex, sheetData1, user);
            }
            else if(classType.Name.Equals("Document"))
            {
                BuildDocumentReport<T>(dataSource, classType, rowIndex, sheetData1);
            }
            else if (classType.Name.Equals("UserProfile"))
            {
                BuildUserProfileReport<T>(dataSource, classType, rowIndex, sheetData1);
            }
            else if (classType.Name.Equals("SecurityPaper"))
            {
                BuildSecurityPaperReport<T>(dataSource, classType, rowIndex, sheetData1);
            }
            
        }

        /// <summary>
        /// Build security papers report
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="classType"></param>
        /// <param name="rowIndex"></param>
        /// <param name="sheetData1"></param>
        private static void BuildSecurityPaperReport<T>(List<T> dataSource, Type classType, int rowIndex, SheetData sheetData1)
        {
            ////build the data
            foreach (var item in dataSource)
            {
                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex.ToString(), classType.GetProperty("SecurityPaperNumber").GetValue(item, null).ToString(), rowData, 1);
                AppendTextCell("B" + rowIndex.ToString(), classType.GetProperty("Status").GetValue(item, null).ToString(), rowData, 1);

                string entrypoint = string.Empty;
                if (classType.GetProperty("EntryPoint").GetValue(item, null) != null)
                {
                    var v = classType.GetProperty("EntryPoint").GetValue(item, null);
                    entrypoint = typeof(EntryPoint).GetProperty("Name").GetValue(v, null).ToString();
                }
                AppendTextCell("C" + rowIndex.ToString(), entrypoint, rowData, 1);

                AppendTextCell("D" + rowIndex.ToString(), classType.GetProperty("FileReference").GetValue(item, null) == null ? "" : classType.GetProperty("FileReference").GetValue(item, null).ToString(), rowData, 1);
                AppendTextCell("E" + rowIndex.ToString(), classType.GetProperty("Comment").GetValue(item, null) == null ? "" : classType.GetProperty("Comment").GetValue(item, null).ToString(), rowData, 1);
                AppendTextCell("F" + rowIndex.ToString(), classType.GetProperty("ModificationBy").GetValue(item, null) == null ? "" : classType.GetProperty("ModificationBy").GetValue(item, null).ToString(), rowData, 1);
                AppendTextCell("G" + rowIndex.ToString(), classType.GetProperty("ModificationDate").GetValue(item, null) == null ? "" : string.Format("{0:dd/MM/yyyy}", DateTime.Parse(classType.GetProperty("ModificationDate").GetValue(item, null).ToString())), rowData, 1);
                sheetData1.Append(rowData);
                rowIndex++;
            }
        }

        /// <summary>
        /// Build user report
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="classType"></param>
        /// <param name="rowIndex"></param>
        /// <param name="sheetData1"></param>
        private static void BuildUserProfileReport<T>(List<T> dataSource, Type classType, int rowIndex, SheetData sheetData1)
        {
            VocEntities ctx = new VocEntities();
            UsersContext us = new UsersContext();

            ////build the data
            foreach (var item in dataSource)
            {
                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };

                AppendTextCell("A" + rowIndex.ToString(), classType.GetProperty("FirstName").GetValue(item, null) == null ? "" : classType.GetProperty("FirstName").GetValue(item, null).ToString(), rowData, 1);
                AppendTextCell("B" + rowIndex.ToString(), classType.GetProperty("LastName").GetValue(item, null) == null ? "" : classType.GetProperty("LastName").GetValue(item, null).ToString(), rowData, 1);

                AppendTextCell("C" + rowIndex.ToString(), classType.GetProperty("UserName").GetValue(item, null) == null ? "" : classType.GetProperty("UserName").GetValue(item, null).ToString(), rowData, 1);

                int userId = (int)classType.GetProperty("UserId").GetValue(item, null);
                UserInRole userInRole = us.UserInRoles.Where(x => x.UserId == userId).FirstOrDefault();
                Role role = us.Roles.Where(x => x.RoleId == userInRole.RoleId).FirstOrDefault();
                AppendTextCell("D" + rowIndex.ToString(), role.RoleName, rowData, 1);


                if (classType.GetProperty("OfficeId").GetValue(item, null) == null)
                    AppendTextCell("E" + rowIndex.ToString(), "", rowData, 1);
                else
                {
                    int oficceId = (int)classType.GetProperty("OfficeId").GetValue(item, null);
                    Office office = ctx.Offices.Where(x => x.OfficeId == oficceId).FirstOrDefault();
                    AppendTextCell("E" + rowIndex.ToString(), office.OfficeName, rowData, 1);
                }



                if (classType.GetProperty("EntryPointId").GetValue(item, null) == null)
                    AppendTextCell("F" + rowIndex.ToString(), "", rowData, 1);
                else
                {
                    int entryPointId = (int)classType.GetProperty("EntryPointId").GetValue(item, null);
                    EntryPoint entryPoint = ctx.EntryPoints.Where(x => x.EntryPointId == entryPointId).FirstOrDefault();
                    AppendTextCell("F" + rowIndex.ToString(), entryPoint.Name, rowData, 1);
                }

                string active = string.Empty;
                if (classType.GetProperty("IsActive").GetValue(item, null) == null)
                    active = "No";
                else
                    active = (bool)classType.GetProperty("IsActive").GetValue(item, null) ? "Yes" : "No";
                AppendTextCell("G" + rowIndex.ToString(), active, rowData, 1);

                AppendTextCell("H" + rowIndex.ToString(), classType.GetProperty("Email").GetValue(item, null) == null ? "" : classType.GetProperty("Email").GetValue(item, null).ToString(), rowData, 1);

                sheetData1.Append(rowData);
                rowIndex++;
            }
        }


        private static void BuildDocumentReport<T>(List<T> dataSource, Type classType, int rowIndex, SheetData sheetData1)
        {
            ////build the data
            foreach (var item in dataSource)
            {
                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex.ToString(), classType.GetProperty("Filename").GetValue(item, null).ToString(), rowData, 1);
                AppendTextCell("B" + rowIndex.ToString(), classType.GetProperty("Description").GetValue(item, null) == null ? "" : classType.GetProperty("Description").GetValue(item, null).ToString(), rowData, 1);

                string category = string.Empty;
                if (classType.GetProperty("IsSupporting").GetValue(item, null) == null)
                    category = "";
                else
                    category = (bool)classType.GetProperty("IsSupporting").GetValue(item, null) ? ServiceResource.SupportingDocument : ServiceResource.Certificate;

                AppendTextCell("C" + rowIndex.ToString(), category, rowData, 1);

                sheetData1.Append(rowData);
                rowIndex++;
            }
        }

        /// <summary>
        /// Build certificate report
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="classType"></param>
        /// <param name="rowIndex"></param>
        /// <param name="sheetData1"></param>
        /// <param name="user"></param>
        private static void BuildCertificateReport<T>(List<T> dataSource, Type classType, int rowIndex, SheetData sheetData1, VocUser user)
        {
            ////build the data
            foreach (var item in dataSource)
            {
                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };

                string office = string.Empty;
                if (classType.GetProperty("Office").GetValue(item, null) != null)
                {
                    var v = classType.GetProperty("Office").GetValue(item, null);
                    office = typeof(Office).GetProperty("OfficeName").GetValue(v, null).ToString();
                }
                AppendTextCell("A" + rowIndex.ToString(), office, rowData, 1);
                AppendTextCell("B" + rowIndex.ToString(), classType.GetProperty("ComdivNumber").GetValue(item, null) == null ? "" : classType.GetProperty("ComdivNumber").GetValue(item, null).ToString(), rowData, 1);
                AppendTextCell("C" + rowIndex.ToString(), classType.GetProperty("Sequential").GetValue(item, null) == null ? "" : classType.GetProperty("Sequential").GetValue(item, null).ToString(), rowData, 1);
                AppendTextCell("D" + rowIndex.ToString(), classType.GetProperty("IssuanceDate").GetValue(item, null) == null ? "" : string.Format("{0:dd/MM/yyyy}", DateTime.Parse(classType.GetProperty("IssuanceDate").GetValue(item, null).ToString())), rowData, 1);
                AppendTextCell("E" + rowIndex.ToString(), classType.GetProperty("CertificateStatusId").GetValue(item, null) == null ? "" : classType.GetProperty("CertificateStatusId").GetValue(item, null).ToString(), rowData, 1);
                AppendTextCell("F" + rowIndex.ToString(), classType.GetProperty("WorkflowStatusId").GetValue(item, null) == null ? "" : classType.GetProperty("WorkflowStatusId").GetValue(item, null).ToString(), rowData, 1);

                string entrypoint = string.Empty;
                if (classType.GetProperty("EntryPoint").GetValue(item, null) != null)
                {
                    var v = classType.GetProperty("EntryPoint").GetValue(item, null);
                    entrypoint = typeof(EntryPoint).GetProperty("Name").GetValue(v, null).ToString();
                }
                AppendTextCell("G" + rowIndex.ToString(), entrypoint, rowData, 1);
                AppendNumberCell("H" + rowIndex.ToString(), classType.GetProperty("FOBValue").GetValue(item, null) == null ? "" : classType.GetProperty("FOBValue").GetValue(item, null).ToString(), rowData, 1);

                AppendTextCell("I" + rowIndex.ToString(), classType.GetProperty("IsInvoiced").GetValue(item, null) == null ? "" : (bool)classType.GetProperty("IsInvoiced").GetValue(item, null) ? "Yes" : "No", rowData, 1);
                if (!(user.IsInRole(UserRoleEnum.BorderAgent) || user.IsInRole(UserRoleEnum.LOAdmin)))
                    AppendTextCell("J" + rowIndex.ToString(), classType.GetProperty("IsPublished").GetValue(item, null) == null ? "" : (bool)classType.GetProperty("IsPublished").GetValue(item, null) ? "Yes" : "No", rowData, 1);


                sheetData1.Append(rowData);
                rowIndex++;
            }
        }

        /// <summary>
        /// Set the width of the columns acording the class type
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="classType"></param>
        /// <returns></returns>
        private static Columns SetColumns(Columns columns, Type classType,VocUser user)
        {
            if (classType.Name.Equals("Certificate"))
            {
                SetColumnsCertificateReport(columns, user);
            }
            else if (classType.Name.Equals("Document"))
            {
                SetColumnsDocumentReport(columns);
            }
            else if (classType.Name.Equals("UserProfile"))
            {
                SetColumnsUserProfileReport(columns);
            }
            else if (classType.Name.Equals("SecurityPaper"))
            {
                SetColumnsSecurityPaperReport(columns);
            }
            return columns;
        }

        /// <summary>
        /// set the width of columns in security papers report
        /// </summary>
        /// <param name="columns">Columns array</param>
        private static void SetColumnsSecurityPaperReport(Columns columns)
        {
            columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 20));
            columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 15));
            columns.Append(CreateColumnData((UInt32Value)(uint)3, (UInt32Value)(uint)3, 33));
            columns.Append(CreateColumnData((UInt32Value)(uint)4, (UInt32Value)(uint)4, 30));
            columns.Append(CreateColumnData((UInt32Value)(uint)5, (UInt32Value)(uint)5, 42));
            columns.Append(CreateColumnData((UInt32Value)(uint)6, (UInt32Value)(uint)6, 28));
            columns.Append(CreateColumnData((UInt32Value)(uint)7, (UInt32Value)(uint)7, 20));
        }

        /// <summary>
        /// set the width of columns in user profile report
        /// </summary>
        /// <param name="columns">Columns array</param>
        private static void SetColumnsUserProfileReport(Columns columns)
        {
            columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 20));
            columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 20));
            columns.Append(CreateColumnData((UInt32Value)(uint)3, (UInt32Value)(uint)3, 35));
            columns.Append(CreateColumnData((UInt32Value)(uint)4, (UInt32Value)(uint)4, 20));
            columns.Append(CreateColumnData((UInt32Value)(uint)5, (UInt32Value)(uint)5, 20));
            columns.Append(CreateColumnData((UInt32Value)(uint)6, (UInt32Value)(uint)6, 15));
            columns.Append(CreateColumnData((UInt32Value)(uint)7, (UInt32Value)(uint)7, 10));
            columns.Append(CreateColumnData((UInt32Value)(uint)8, (UInt32Value)(uint)8, 40));
        }

        /// <summary>
        /// set the width of columns in documents report
        /// </summary>
        /// <param name="columns">Columns array</param>
        private static void SetColumnsDocumentReport(Columns columns)
        {
            columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 45));
            columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 45));
            columns.Append(CreateColumnData((UInt32Value)(uint)3, (UInt32Value)(uint)3, 30));
        }

        /// <summary>
        /// set the width of columns in certificates list report
        /// </summary>
        /// <param name="columns">Columns array</param>
        private static void SetColumnsCertificateReport(Columns columns, VocUser user)
        {
            columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 25));
            columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 25));
            columns.Append(CreateColumnData((UInt32Value)(uint)3, (UInt32Value)(uint)3, 29));
            columns.Append(CreateColumnData((UInt32Value)(uint)4, (UInt32Value)(uint)4, 26));
            columns.Append(CreateColumnData((UInt32Value)(uint)5, (UInt32Value)(uint)5, 20));
            columns.Append(CreateColumnData((UInt32Value)(uint)6, (UInt32Value)(uint)6, 20));
            columns.Append(CreateColumnData((UInt32Value)(uint)7, (UInt32Value)(uint)7, 20));
            columns.Append(CreateColumnData((UInt32Value)(uint)8, (UInt32Value)(uint)8, 13));
            columns.Append(CreateColumnData((UInt32Value)(uint)9, (UInt32Value)(uint)9, 22));
            if (!(user.IsInRole(UserRoleEnum.BorderAgent) || user.IsInRole(UserRoleEnum.LOAdmin)))
                columns.Append(CreateColumnData((UInt32Value)(uint)10, (UInt32Value)(uint)10, 12));
        }

        /// <summary>
        /// Return the grid header acording to the class type
        /// </summary>
        /// <param name="rowData"></param>
        /// <param name="classType"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private static Row GetColumnsByTypeClass(ref Row rowData, Type classType, int rowIndex,VocUser user)
        {
            if (classType.Name.Equals("Certificate"))
            {
                GetColumnsCertificateReport(rowData, rowIndex, user);
            }
            else if (classType.Name.Equals("Document"))
            {
                GetColumnsDocumentReport(rowData, rowIndex);
            }
            else if (classType.Name.Equals("UserProfile"))
            {
                GetColumnsUserProfileReport(rowData, rowIndex);
            }
            else if (classType.Name.Equals("SecurityPaper"))
            {
                GetColumnsSecurityPapersReport(rowData, rowIndex);
            }

            return rowData;
        }

        /// <summary>
        /// Set the title of the columns in security papers report
        /// </summary>
        /// <param name="rowData">Row to add</param>
        /// <param name="rowIndex">Row index</param>
        private static void GetColumnsSecurityPapersReport(Row rowData, int rowIndex)
        {
            AppendTextCell("A" + rowIndex.ToString(), ServiceResource.Number, rowData, 2);
            AppendTextCell("B" + rowIndex.ToString(), ServiceResource.Status, rowData, 2);
            AppendTextCell("C" + rowIndex.ToString(), ServiceResource.RemittedTo, rowData, 2);
            AppendTextCell("D" + rowIndex.ToString(), ServiceResource.FileReference, rowData, 2);
            AppendTextCell("E" + rowIndex.ToString(), ServiceResource.Comment, rowData, 2);
            AppendTextCell("F" + rowIndex.ToString(), ServiceResource.ModificationBy, rowData, 2);
            AppendTextCell("G" + rowIndex.ToString(), ServiceResource.ModificationDate, rowData, 2);
        }

        /// <summary>
        /// Set the title of the columns in user profile report
        /// </summary>
        /// <param name="rowData">Row to add</param>
        /// <param name="rowIndex">Row index</param>
        private static void GetColumnsUserProfileReport(Row rowData, int rowIndex)
        {
            AppendTextCell("A" + rowIndex.ToString(), ServiceResource.FirstName, rowData, 2);
            AppendTextCell("B" + rowIndex.ToString(), ServiceResource.LastName, rowData, 2);
            AppendTextCell("C" + rowIndex.ToString(), ServiceResource.UserLogin, rowData, 2);
            AppendTextCell("D" + rowIndex.ToString(), ServiceResource.Role, rowData, 2);
            AppendTextCell("E" + rowIndex.ToString(), ServiceResource.Office, rowData, 2);
            AppendTextCell("F" + rowIndex.ToString(), ServiceResource.EntryPoint, rowData, 2);
            AppendTextCell("G" + rowIndex.ToString(), ServiceResource.Active, rowData, 2);
            AppendTextCell("H" + rowIndex.ToString(), ServiceResource.Email, rowData, 2);
        }

        /// <summary>
        /// Set the title of the columns in documents report
        /// </summary>
        /// <param name="rowData">Row to add</param>
        /// <param name="rowIndex">Row index</param>
        private static void GetColumnsDocumentReport(Row rowData, int rowIndex)
        {
            AppendTextCell("A" + rowIndex.ToString(), ServiceResource.Name, rowData, 2);
            AppendTextCell("B" + rowIndex.ToString(), ServiceResource.Description, rowData, 2);
            AppendTextCell("C" + rowIndex.ToString(), ServiceResource.Category, rowData, 2);
        }

        /// <summary>
        /// Set the title of the columns in certificates list report
        /// </summary>
        /// <param name="rowData">Row to add</param>
        /// <param name="rowIndex">Row index</param>
        private static void GetColumnsCertificateReport(Row rowData, int rowIndex, VocUser user)
        {
            AppendTextCell("A" + rowIndex.ToString(), ServiceResource.Office, rowData, 2);
            AppendTextCell("B" + rowIndex.ToString(), ServiceResource.ComdivNumber, rowData, 2);
            AppendTextCell("C" + rowIndex.ToString(), ServiceResource.CertificateNumber, rowData, 2);
            AppendTextCell("D" + rowIndex.ToString(), ServiceResource.IssuanceDate, rowData, 2);
            AppendTextCell("E" + rowIndex.ToString(), ServiceResource.CertificateStatus, rowData, 2);
            AppendTextCell("F" + rowIndex.ToString(), ServiceResource.WorkflowStatus, rowData, 2);
            AppendTextCell("G" + rowIndex.ToString(), ServiceResource.EntryPoint, rowData, 2);
            AppendTextCell("H" + rowIndex.ToString(), ServiceResource.FOBValue, rowData, 2);
            AppendTextCell("I" + rowIndex.ToString(), ServiceResource.Invoiced, rowData, 2);
            if (!(user.IsInRole(UserRoleEnum.BorderAgent) || user.IsInRole(UserRoleEnum.LOAdmin)))
                AppendTextCell("J" + rowIndex.ToString(), ServiceResource.Published, rowData, 2);
        }
        
        /// <summary>
        /// Return the number of columns acording the class type 
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        private static int GetColumnsNumber(Type classType, VocUser user)
        {
            if (classType.Name.Equals("Certificate") && !(user.IsInRole(UserRoleEnum.BorderAgent) || user.IsInRole(UserRoleEnum.LOAdmin)))
                return 10;
            else if (classType.Name.Equals("Certificate") && (user.IsInRole(UserRoleEnum.BorderAgent) || user.IsInRole(UserRoleEnum.LOAdmin)))
                return 9;
            else if (classType.Name.Equals("Document"))
                return 3;
            else if (classType.Name.Equals("UserProfile"))
                return 8;
            else if (classType.Name.Equals("SecurityPaper"))
                return 7;
            else
                return 6;
        }

        #region VocStyleSheet
        /// <summary>
        /// Create an stylesheet to use in excel files
        /// </summary>
        /// <returns></returns>
        private static Stylesheet VocStyleSheet()
        {
            Stylesheet styleSheet = new Stylesheet();

            Fonts fonts = StyleSheetFonts();

            Fills fills = StyleSheetFills();

            Borders borders = StyleSheetBorders();

            CellFormats cellFormats = StyleSheetCellFormats();

            styleSheet.Append(fonts);
            styleSheet.Append(fills);
            styleSheet.Append(borders);
            styleSheet.Append(cellFormats);

            return styleSheet;
        }

        private static CellFormats StyleSheetCellFormats()
        {
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
            return cellFormats;
        }

        private static Borders StyleSheetBorders()
        {
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
            return borders;
        }

        private static Fills StyleSheetFills()
        {
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
            return fills;
        }

        private static Fonts StyleSheetFonts()
        {
            Fonts fonts = new Fonts();
            //0-normal fonts
            DocumentFormat.OpenXml.Spreadsheet.Font myFont = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                FontSize = new FontSize() { Val = 11 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                FontName = new FontName() { Val = "Calibri" }
            };
            fonts.Append(myFont);

            //1-font bold
            myFont = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                Bold = new Bold(),
                FontSize = new FontSize() { Val = 11 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                FontName = new FontName() { Val = "Calibri" }
            };
            fonts.Append(myFont);

            //2-font title
            myFont = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                Bold = new Bold(),
                FontSize = new FontSize() { Val = 20 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "FFFFFF" } },
                FontName = new FontName() { Val = "Verdana" }
            };
            fonts.Append(myFont);

            //3-font bold
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
            return fonts;
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

            DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties nvpp;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill blipFill;
            DrawingDefinitionPartOne(dp, imgp, out nvpp, out blipFill);

            DocumentFormat.OpenXml.Drawing.Extents extents;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture picture;
            DrawingDefinitionPartTwo(sImagePath, nvpp, blipFill, out extents, out picture);

            AbsoluteAnchor anchor = DrawingPosition(extents);


            anchor.Append(picture);
            anchor.Append(new ClientData());
            WorksheetDrawing wsd = new WorksheetDrawing();
            wsd.Append(anchor);
            Drawing drawing = new Drawing();
            drawing.Id = dp.GetIdOfPart(imgp);

            wsd.Save(dp);
            return drawing;
        }

        private static AbsoluteAnchor DrawingPosition(DocumentFormat.OpenXml.Drawing.Extents extents)
        {
            DocumentFormat.OpenXml.Drawing.Spreadsheet.Position pos = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Position();
            pos.X = 18 * 914400 / 72;
            pos.Y = 28 * 914400 / 72;

            Extent ext = new Extent();
            ext.Cx = extents.Cx;
            ext.Cy = extents.Cy;
            AbsoluteAnchor anchor = new AbsoluteAnchor();
            anchor.Position = pos;
            anchor.Extent = ext;
            return anchor;
        }

        private static void DrawingDefinitionPartTwo(string sImagePath, DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties nvpp, DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill blipFill, out DocumentFormat.OpenXml.Drawing.Extents extents, out DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture picture)
        {
            DocumentFormat.OpenXml.Drawing.Transform2D t2d = new DocumentFormat.OpenXml.Drawing.Transform2D();
            DocumentFormat.OpenXml.Drawing.Offset offset = new DocumentFormat.OpenXml.Drawing.Offset();
            offset.X = 0;
            offset.Y = 0;
            t2d.Offset = offset;
            Bitmap bm = new Bitmap(sImagePath);
            extents = new DocumentFormat.OpenXml.Drawing.Extents();
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

            picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture();
            picture.NonVisualPictureProperties = nvpp;
            picture.BlipFill = blipFill;
            picture.ShapeProperties = sp;
        }

        private static void DrawingDefinitionPartOne(DrawingsPart dp, ImagePart imgp, out DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties nvpp, out DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill blipFill)
        {
            DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties nvdp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties();
            nvdp.Id = 1025;
            nvdp.Name = "Picture 1";
            nvdp.Description = "logo";
            DocumentFormat.OpenXml.Drawing.PictureLocks picLocks = new DocumentFormat.OpenXml.Drawing.PictureLocks();
            picLocks.NoChangeAspect = true;
            picLocks.NoChangeArrowheads = true;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureDrawingProperties nvpdp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureDrawingProperties();
            nvpdp.PictureLocks = picLocks;
            nvpp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties();
            nvpp.NonVisualDrawingProperties = nvdp;
            nvpp.NonVisualPictureDrawingProperties = nvpdp;

            DocumentFormat.OpenXml.Drawing.Stretch stretch = new DocumentFormat.OpenXml.Drawing.Stretch();
            stretch.FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle();

            blipFill = new DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill();
            DocumentFormat.OpenXml.Drawing.Blip blip = new DocumentFormat.OpenXml.Drawing.Blip();
            blip.Embed = dp.GetIdOfPart(imgp);
            blip.CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print;
            blipFill.Blip = blip;
            blipFill.SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle();
            blipFill.Append(stretch);
        }
        #endregion

    }
}