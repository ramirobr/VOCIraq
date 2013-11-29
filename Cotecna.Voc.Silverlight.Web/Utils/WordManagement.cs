using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Cotecna.Voc.Business;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Cotecna.Voc.Silverlight.Web.Resources;

namespace Cotecna.Voc.Silverlight.Web
{
    public static class WordManagement
    {

        /// <summary>
        /// Generate word report
        /// </summary>
        /// <param name="model"></param>
        /// <param name="templatePath"></param>
        /// <param name="signatureArray"></param>
        /// <param name="errorTagValidation"></param>
        /// <returns></returns>
        public static MemoryStream GenerateWordReport(Certificate model, string templatePath, out string errorValidation, byte[] signatureArray = null, string issuerName = null, byte[] signetArray = null, string officeName = null)
        {
            //Put in memory the template file
            byte[] byteArray = File.ReadAllBytes(templatePath);
            errorValidation = string.Empty;
            
            MemoryStream ms = new MemoryStream();
            ms.Write(byteArray, 0, (int)byteArray.Length);

            using (WordprocessingDocument theDoc = WordprocessingDocument.Open(ms, true))
            {
                MainDocumentPart mainPart = theDoc.MainDocumentPart;

                List<string> logs = ValidateReport(model, mainPart);
                if (logs.Count > 0)
                {
                    errorValidation = "The certificate document has an incorrect format: \r\n\t";
                    errorValidation += string.Join("\r\n\t", logs);                    
                    return null;
                }

                var sdtElements = mainPart.Document.Body.Descendants<SdtElement>(); //includes runs, blocks, cells
                var sdtHeaderElements = mainPart.HeaderParts.FirstOrDefault().Header.Descendants<SdtElement>();

                InsertInBlock(sdtHeaderElements, "IssuanceDate", model.IssuanceDate.HasValue ? model.IssuanceDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                //For Coc
                if (model.CertificateStatusId == CertificateStatusEnum.Conform)  
                {
                    InsertInBlock(sdtHeaderElements, "COCNo", model.Sequential);
                    InsertInBlock(sdtElements, "EntryPoint", model.EntryPoint != null ? model.EntryPoint.Name : string.Empty);
                    InsertInBlock(sdtElements, "PrePaid", model.IsInvoiced.GetValueOrDefault() ? ServiceResource.Yes : ServiceResource.No);                    
                }
                    //For Ncr
                else
                    InsertInBlock(sdtHeaderElements, "NCRNo", model.Sequential);

                InsertInBlock(sdtElements, "FOBValue", model.FOBValue.Value.ToString("F2"));

                //Only when it has signature, it is issuer and set issuerName. Else, blank the issuer name
                if (signatureArray != null)
                {
                    InsertImage(theDoc, new MemoryStream(signatureArray),"Signature");
                    InsertInBlock(sdtElements, "IssuerName", issuerName);
                }
                else
                    InsertInBlock(sdtElements, "IssuerName", ServiceResource.AutomaticText);
                
                
                //Only when office has signet
                if (signetArray != null)
                    InsertImage(theDoc, new MemoryStream(signetArray), "Stamp");

                if(!string.IsNullOrEmpty(officeName))
                    InsertInBlock(sdtElements, "OfficeName", officeName);
                else
                    InsertInBlock(sdtElements, "OfficeName", ServiceResource.AutomaticText);

                
                // Save the changes to the table back into the document.
                mainPart.Document.Save();      
          
            }

            return ms;
        }

        /// <summary>
        /// Validates the report.
        /// </summary>
        /// <param name="model">Certificate model.</param>
        /// <param name="sdtElements">Elements of Word documents.</param>
        /// <returns>Validation list</returns>
        public static List<string> ValidateReport(Certificate model, MainDocumentPart mainPart)
        {
            List<string> logs = new List<string>();

            var headerSection = mainPart.HeaderParts.FirstOrDefault();
            if(headerSection==null)
            {
                logs.Add("Header section does not exist.");
                return logs;
            }

            var sdtHeaderElements =headerSection.Header.Descendants<SdtElement>();
            var sdtBodyElements = mainPart.Document.Body.Descendants<SdtElement>();

            ValidateInBlock(sdtHeaderElements, "IssuanceDate", logs);            
            if (model.CertificateStatusId == CertificateStatusEnum.Conform)
            {
                ValidateInBlock(sdtHeaderElements, "COCNo", logs);
                ValidateInBlock(sdtBodyElements, "EntryPoint", logs);
                ValidateInBlock(sdtBodyElements, "PrePaid", logs);
            }
            else
                ValidateInBlock(sdtHeaderElements, "NCRNo", logs);
            ValidateInBlock(sdtBodyElements, "FOBValue", logs);
            ValidateInBlock(sdtBodyElements, "IssuerName", logs);
            ValidateInBlock(sdtBodyElements, "Signature", logs);     
            return logs;
        }

        /// <summary>
        /// Generate release note report
        /// </summary>
        /// <param name="release">Model</param>
        /// <param name="templatePath">Path of template</param>
        /// <param name="errorValidation">Returns errors</param>
        /// <returns></returns>
        public static MemoryStream GenerateReleaseNoteReport(ReleaseNote release, string templatePath, out string errorValidation)
        {
            //Put in memory the template file
            byte[] byteArray = File.ReadAllBytes(templatePath);
            errorValidation = string.Empty;

            MemoryStream ms = new MemoryStream();
            ms.Write(byteArray, 0, (int)byteArray.Length);

            using (WordprocessingDocument theDoc = WordprocessingDocument.Open(ms, true))
            {
                MainDocumentPart mainPart = theDoc.MainDocumentPart;

                List<string> logs = ValidateReleaseNoteReport(release, mainPart);
                if (logs.Count > 0)
                {
                    errorValidation = "The release template has an incorrect format: \r\n\t";
                    errorValidation += string.Join("\r\n\t", logs);
                    return null;
                }

                var sdtElements = mainPart.Document.Body.Descendants<SdtElement>(); //includes runs, blocks, cells
                string certificateNumber = release.Certificate.Sequential;
                InsertInBlock(sdtElements, "EntryPoint", release.Certificate.EntryPoint.Name);
                InsertInBlock(sdtElements, "ImporterName", release.ImporterName);
                InsertInBlock(sdtElements, "CertificateNumber", certificateNumber);
                var checks = mainPart.Document.Body.Descendants<FieldChar>().Where(x => x.FieldCharType == FieldCharValues.Begin);
                bool visuallyCheck = release.VisuallyCheck.HasValue ? release.VisuallyCheck.Value : false;
                bool isClosed = release.Certificate.WorkflowStatusId == WorkflowStatusEnum.Closed ? true : false;
                bool isPartial = release.PartialComplete.GetValueOrDefault();
                bool isCompleted = !isPartial;
                foreach (var item in checks)
                {
                    if (((FormFieldName)item.FormFieldData.FirstChild).Val == "VisuallyCheck" || ((FormFieldName)item.FormFieldData.FirstChild).Val == "VisuallyCheckArabic")
                    {
                        ((DefaultCheckBoxFormFieldState)item.FormFieldData.GetFirstChild<CheckBox>().LastChild).Val = visuallyCheck == true ? new OnOffValue(true) : new OnOffValue(false);
                    }
                    else
                    {
                        if (((FormFieldName)item.FormFieldData.FirstChild).Val == "Complete")
                        {
                            ((DefaultCheckBoxFormFieldState)item.FormFieldData.GetFirstChild<CheckBox>().LastChild).Val = isCompleted == true ? new OnOffValue(true) : new OnOffValue(false);
                        }
                        else if (((FormFieldName)item.FormFieldData.FirstChild).Val == "Partial")
                        {
                            ((DefaultCheckBoxFormFieldState)item.FormFieldData.GetFirstChild<CheckBox>().LastChild).Val = isPartial == true ? new OnOffValue(true) : new OnOffValue(false);
                        }
                    }
                }
                InsertInBlock(sdtElements, "ContainersDetails", release.ContainersDetails);
                InsertInBlock(sdtElements, "ImportDocumentDetails", release.ImportDocumentDetails);
                InsertInBlock(sdtElements, "NumberLineItems", release.NumberLineItems.GetValueOrDefault().ToString());

                InsertInBlock(sdtElements, "ShipmentType", release.ShipmentType.GetValueOrDefault().ToString());
                InsertInBlock(sdtElements, "ShipmentTypeArabic", release.ShipmentType.GetValueOrDefault().ToString());

                InsertInBlock(sdtElements, "Unit", release.Unit);
                InsertInBlock(sdtElements, "ReceivedQty", release.ReceivedQuantity.GetValueOrDefault().ToString());
                InsertInBlock(sdtElements, "RemanQty", release.RemainingQuantity.GetValueOrDefault().ToString());

                decimal totalqty = release.ReceivedQuantity.GetValueOrDefault() + release.RemainingQuantity.GetValueOrDefault();
                InsertInBlock(sdtElements, "TotalQty", totalqty.ToString());

                InsertInBlock(sdtElements, "Comments", release.Comments);
                InsertInBlock(sdtElements, "CertificateNumberPartial", string.Format("{0}/{1:000}", certificateNumber, release.PartialNumber));
                InsertInBlock(sdtElements, "IssuanceDate", release.IssuanceDate.ToString("dd/MM/yyyy"));

                // Save the changes to the table back into the document.
                mainPart.Document.Save();

            }

            return ms;
        }

        /// <summary>
        /// Validate if the document is ok before fill all fields
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="mainPart">Document</param>
        /// <returns>List of errors</returns>
        public static List<string> ValidateReleaseNoteReport(ReleaseNote model, MainDocumentPart mainPart)
        {
            List<string> logs = new List<string>();

            var sdtBodyElements = mainPart.Document.Body.Descendants<SdtElement>();
            ValidateInBlock(sdtBodyElements, "EntryPoint", logs);
            ValidateInBlock(sdtBodyElements, "ImporterName", logs);
            ValidateInBlock(sdtBodyElements, "CertificateNumber", logs);            
            ValidateInBlock(sdtBodyElements, "ContainersDetails", logs);
            ValidateInBlock(sdtBodyElements, "ImportDocumentDetails", logs);
            ValidateInBlock(sdtBodyElements, "ShipmentType", logs);
            ValidateInBlock(sdtBodyElements, "ShipmentTypeArabic", logs);
            ValidateInBlock(sdtBodyElements, "Unit", logs);
            ValidateInBlock(sdtBodyElements, "ReceivedQty", logs);
            ValidateInBlock(sdtBodyElements, "RemanQty", logs);
            ValidateInBlock(sdtBodyElements, "TotalQty", logs);
            ValidateInBlock(sdtBodyElements, "Comments", logs);
            ValidateInBlock(sdtBodyElements, "CertificateNumberPartial", logs);
            ValidateInBlock(sdtBodyElements, "IssuanceDate", logs);
            
            return logs;
        }


        /// <summary>
        /// Validates the in block.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="logs">The logs.</param>
        private static void ValidateInBlock(IEnumerable<SdtElement> blocks, string tagName, List<string> logs)
        {
            if (logs == null)
                logs = new List<string>();

            SdtElement auxBlock = blocks.Where(r => r.SdtProperties.GetFirstChild<Tag>().Val == tagName).FirstOrDefault();
            if (auxBlock == null)
            {
                logs.Add(string.Format("Not exist tag: {0}", tagName));
            }        
        }


        /// <summary>
        /// Inserts a value in a block given its tag name
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        public static void InsertInBlock(IEnumerable<SdtElement> blocks, string tagName, string value)
        {
            SdtElement auxBlock = blocks.Where(r => r.SdtProperties.GetFirstChild<Tag>().Val == tagName).FirstOrDefault();
            if (auxBlock != null)
            {
                var paragraph = auxBlock.Descendants<Paragraph>().FirstOrDefault();
                if (paragraph != null)
                {
                    paragraph.RemoveAllChildren<Run>();
                    paragraph.Append(new Run(new Text(value)));
                }
                else
                {
                    var run = auxBlock.Descendants<SdtContentRun>().FirstOrDefault();
                    if (run != null)
                    {
                        run.RemoveAllChildren<Run>();
                        run.Append(new Run(new Text(value)));
                    }
                    else
                    {
                        throw new Exception("Word document has wrong format in tag: " + tagName);
                    }
                }
            }
        }


        /// <summary>
        /// Insert Photos in a "Signature" predefined block tag
        /// </summary>        
        /// <param name="wordDoc">Word doc built dinamically using the template</param>
        /// <param name="signature">Signature image</param>
        /// <param name="imageName">Image Name</param>
        public static void InsertImage(WordprocessingDocument wordDoc, MemoryStream signature, string imageName)
        {
            MainDocumentPart mainPart = wordDoc.MainDocumentPart;
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
            //Set the content of the image part
            imagePart.FeedData(signature);
            //Place the imagePart in Signature element
            AddImage(wordDoc, mainPart.GetIdOfPart(imagePart), 1, imageName);
        }

        /// <summary>
        /// Add images to the body of a document. An AppendedImages tag must exist
        /// </summary>
        /// <param name="wordDoc">Word document reference</param>
        /// <param name="relationshipId">Image Part Id</param>
        /// <param name="id">Image counter</param>
        /// <param name="imageName">Image Name</param>
        private static void AddImage(WordprocessingDocument wordDoc, string relationshipId, int id, string imageName)
        {
            long x = 1200000L;
            long y = 600000L;

            //Define the reference of the image.
            Drawing element = BuildImage(id, relationshipId, x, y);

            var blocks = wordDoc.MainDocumentPart.Document.Body.Descendants<SdtBlock>();


            SdtElement signatureBlock = blocks.Where(r => r.SdtProperties.GetFirstChild<Tag>().Val == imageName).FirstOrDefault();
            if (signatureBlock != null)
            {
                Text signatureText = signatureBlock.Descendants<Text>().FirstOrDefault();
                if (signatureText != null)
                    signatureText.Text = "";
                //Add the signature to the body
                var signatureParagraph = signatureBlock.Descendants<Paragraph>().FirstOrDefault();
                if (signatureParagraph != null)
                {
                    signatureParagraph.Append(new Run(element));
                }
            }
        }

        /// <summary>
        /// Define the reference to the image in the word document
        /// </summary>
        /// <param name="id">Image counter</param>
        /// <param name="relationshipId">Image Part id</param>
        /// <param name="x">Size x</param>
        /// <param name="y">Size y</param>
        /// <returns></returns>
        private static Drawing BuildImage(int id, string relationshipId, long x, long y)
        {
            return new Drawing(
                new DW.Inline(
                    new DW.Extent() { Cx = x, Cy = y },
                    new DW.EffectExtent()
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new DW.DocProperties()
                    {
                        Id = (UInt32Value)1U,
                        Name = "Picture " + id.ToString()
                    },
                    new DW.NonVisualGraphicFrameDrawingProperties(
                        new A.GraphicFrameLocks() { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties()
                                    {
                                        Id = (UInt32Value)0U,
                                        Name = "image" + id + ".jpg"
                                    },
                                    new PIC.NonVisualPictureDrawingProperties()),
                                new PIC.BlipFill(
                                    new A.Blip(
                                        new A.BlipExtensionList(
                                            new A.BlipExtension()
                                            {
                                                Uri =
                                                    "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                            })
                                        )
                                    {
                                        Embed = relationshipId,
                                        CompressionState =
                                            A.BlipCompressionValues.Print
                                    },
                                    new A.Stretch(
                                        new A.FillRectangle())),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset() { X = 0L, Y = 0L },
                                        new A.Extents() { Cx = x, Cy = y }),
                                    new A.PresetGeometry(
                                        new A.AdjustValueList()
                                        ) { Preset = A.ShapeTypeValues.Rectangle }))
                            ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                    )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U
                    //EditId = "50D07946"         //This sentence works only for Office 2010                
                });
        }

        /// <summary>
        /// Save word Report as pdf
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="certificatePath"></param>
        public static void SaveWordReportAsPdf(MemoryStream ms, string certificatePath)
        {
            try
            {
                ms.Position = 0;
                //****By requirements definition. The reports originally made in Word are saved as Pdf
                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense("Aspose.Words.lic");
                Aspose.Words.Document d = new Aspose.Words.Document(ms);
                ms.Close();
                using (System.IO.MemoryStream asposeStream = new System.IO.MemoryStream())
                {
                    d.Save(asposeStream, Aspose.Words.SaveFormat.Pdf);
                    asposeStream.Position = 0;
                    byte[] bytes = new byte[asposeStream.Length];
                    asposeStream.Read(bytes, 0, System.Convert.ToInt32(asposeStream.Length));
                    //****End requirement definition
                    using (System.IO.FileStream fs = System.IO.File.Create(certificatePath.Replace(".docx", ".pdf")))
                        asposeStream.WriteTo(fs);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}