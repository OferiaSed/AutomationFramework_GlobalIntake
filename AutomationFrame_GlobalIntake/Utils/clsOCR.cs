using GemBox.Pdf;
using Tesseract;
using System.Configuration;
using System.IO;
using AutomationFramework;
using System;

namespace MyUtils
{
    class clsOCR
    {
        public static string fnGetOCRText(string pdfPath)
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            Pix objPix;
            using (PdfDocument document = PdfDocument.Load(pdfPath)) 
            {
                string strTempPngLocation = ConfigurationManager.AppSettings["TesseractTempPngPath"];
                document.Save(strTempPngLocation);
                objPix = Pix.LoadFromFile(strTempPngLocation);
                File.Delete(strTempPngLocation);
            }

            using (var engine = new TesseractEngine(ConfigurationManager.AppSettings["TesseractLangPath"], "eng", EngineMode.Default))
            {
                Page ocrPage = engine.Process(objPix, PageSegMode.AutoOnly);
                return ocrPage.GetText();
            }
        }
    }
}
