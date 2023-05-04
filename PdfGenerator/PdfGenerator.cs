using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfGenerator
{
    public class PdfGenerator
    {
        public static string EnsureEmptyDirectory()
        {
            return MyLogger.LogDuration("EnsureEmptyDirectory", () =>
            {
                MyLogger.LogInfo("Ensuring an empty local directory for pdfs");
                try
                {
                    var baseDir = Path.GetTempPath();
                    var directory = baseDir + "PDFs\\";

                    if (Directory.Exists(directory))
                    {
                        Directory.Delete(directory, true);
                    }

                    Directory.CreateDirectory(directory);
                    return directory;
                }
                catch (Exception ex)
                {
                    MyLogger.LogErrror(ex.Message);
                    throw;
                }
            });
        }

        public static string GeneratePdf(string rootDirectory, string destinationDirectory)
        {
            MyLogger.LogInfo("Generating a pdf in a local directory");
            try
            {
                return MyLogger.LogDuration("GeneratePdf", () =>
                {

                    PdfFontResolver.Apply(rootDirectory);

                    var outputDocument = new PdfDocument();

                    var template = Path.Combine(rootDirectory, "Templates", "BigTemplate.pdf");
                    MyLogger.LogInfo($"Using {template} as PDF-Template");
                    var templateDoc = PdfReader.Open(template, PdfDocumentOpenMode.Import);
                    outputDocument.Info.Title = "Generated PDF";

                    foreach (var templatePage in templateDoc.Pages)
                    {
                        var page = outputDocument.AddPage(templatePage);

                        MyLogger.LogInfo("Drawing text into PDF");
                        var gfx = XGraphics.FromPdfPage(page);
                        var font = new XFont("Arial", 20, XFontStyle.Regular);
                        gfx.DrawString("Hello, World!",
                            font,
                            XBrushes.Black,
                            new XRect(0, 0, page.Width, page.Height),
                            XStringFormats.Center);

                        MyLogger.LogInfo("Drawing dynamically loaded bmp into PDF");
                        var imagePath = Path.Combine(rootDirectory, "Images", "tacho.png");
                        using (var imageStream = File.Open(imagePath, FileMode.Open))
                        {
                            gfx.DrawImage(XImage.FromStream(imageStream), new XRect(10, 10, 150, 150));
                        }

                        MyLogger.LogInfo("Drawing random lines into PDF");
                        for (int i = 0; i < 20; i++)
                        {
                            var x1 = new Random().Next(0, Convert.ToInt32(page.Width.Point));
                            var y1 = new Random().Next(0, Convert.ToInt32(page.Width.Point));
                            var x2 = new Random().Next(0, Convert.ToInt32(page.Width.Point));
                            var y2 = new Random().Next(0, Convert.ToInt32(page.Width.Point));

                            var p1 = new XPoint(x1, y1);
                            var p2 = new XPoint(x2, y2);
                            gfx.DrawLine(XPens.DarkRed, p1, p2);
                        }
                    }

                    MyLogger.LogInfo("Saving PDF in a local directory");
                    var filename = Path.Combine(destinationDirectory, "HelloWorld.pdf");
                    outputDocument.Save(filename);
                    return filename;
                });
            }
            catch (Exception ex)
            {
                MyLogger.LogErrror(ex.Message);
                throw;
            }
        }

        public static void UploadToStorage(string blobConnectionString, string sourcePath, string destinationBlob)
        {
            MyLogger.LogInfo("Uploading file to Azure Blob Storage");
            try
            {
                MyLogger.LogDuration("Upload", () =>
                {
                    var containerName = "pdfs";

                    var container = new BlobContainerClient(blobConnectionString, containerName);
                    container.CreateIfNotExists(PublicAccessType.BlobContainer);

                    var blob = new BlobClient(blobConnectionString, containerName, destinationBlob);
                    blob.DeleteIfExists();
                    using (var stream = File.Open(sourcePath, FileMode.Open))
                    {
                        blob.Upload(stream);
                    }
                    MyLogger.LogInfo($"Download via: {blob.Uri}");
                });
            }
            catch (Exception ex)
            {
                MyLogger.LogErrror(ex.Message);
                throw;
            }
        }


    }
}
