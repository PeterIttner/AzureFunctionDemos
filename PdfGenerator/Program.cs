namespace PdfGenerator
{
    public class Program
    {
        private static readonly string BlobStorageConnectionString = "TODO-REPLACE-ME";
        public static void Main()
        {
            Main((s) => Console.WriteLine(s), Directory.GetCurrentDirectory());

        }
        public static string Main(Action<string> doLog, string templateDirectory)
        {
            MyLogger.Action = doLog;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");

            var outputDirectory = PdfGenerator.EnsureEmptyDirectory();
            var pdfFile = PdfGenerator.GeneratePdf(templateDirectory, outputDirectory);

            PdfGenerator.UploadToStorage(BlobStorageConnectionString, pdfFile, $"Generated-{timestamp}.pdf");
            return pdfFile;
        }
    }
}