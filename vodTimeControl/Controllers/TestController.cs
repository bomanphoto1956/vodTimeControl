using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vodTimeControl.Models;
using RazorPDF;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using System.Web.Hosting;

namespace vodTimeControl.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult test2()
        {
            return View();
        }

        public ActionResult Pdf()
        {
            CUser cu = new CUser();
            cu.userName = "Test";
            cu.userRoleName = "TestUser";
            ViewBag.Title = "Testar PDF generering";
            var pdfResult = new PdfResult(cu, "Pdf");

            return pdfResult;

        }

        public ActionResult Pdf2()
        {
            return View();
        }

        public ActionResult Pdf3()
        {

            Document document = new Document();

            MemoryStream stream = new MemoryStream();

            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                pdfWriter.CloseStream = false;

                document.Open();
                document.Add(new Paragraph("Hello World"));
            }
            catch (DocumentException de)
            {
                Console.Error.WriteLine(de.Message);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.Message);
            }

            document.Close();

            stream.Flush(); //Always catches me out
            stream.Position = 0; //Not sure if this is required

            return File(stream, "application/pdf", @"d:\DownloadName.pdf");


        }

        public ActionResult pdf4()
        {
            //string fileName = Server.MapPath("pdf") + "\\" + "First PDF document.pdf";
            string fileName = @"C:\Users\kjbo\source\repos\vodTimeControl\vodTimeControl\pdf\second PDF.pdf";
            //string host = HostingEnvironment.
            string path = Server.MapPath("~/pdf");
            FileStream fs = new FileStream(fileName, FileMode.Create);
            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.

            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.AddAuthor("Micke Blomquist");
            document.AddCreator("Sample application using iTextSharp");
            document.AddKeywords("PDF tutorial education");
            document.AddSubject("Document subject - Describing the steps creating a PDF document");
            document.AddTitle("The document title - PDF creation using iTextSharp");
            // Open the document to enable you to write to the document
            document.Open();
            // Add a simple and wellknown phrase to the document in a flow layout manner
            document.Add(new Paragraph("Hello World!"));
            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            return File(fileName, "application/pdf");            
        }

    }
}
