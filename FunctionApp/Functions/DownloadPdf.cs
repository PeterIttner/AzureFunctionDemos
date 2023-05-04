using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace DemoFunctions.Functions
{

    public class DownloadPdfFunction
    {
        private readonly ILogger<UploadPdfFunction> _logger;

        public DownloadPdfFunction(ILogger<UploadPdfFunction> log)
        {
            _logger = log;
        }

        [FunctionName("DownloadPdf")]
        [OpenApiOperation(operationId: "DonwloadPdf")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/pdf", bodyType: typeof(Stream), Description = "The generated pdf")]
        public async Task<IActionResult> DownloadPdf(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {
            _logger.LogInformation("Download request.");

            try
            {
                var file = PdfGenerator.Program.Main((s) => _logger.LogInformation(s), context.FunctionAppDirectory);
                using (var stream = new MemoryStream())
                {
                    var fileBytes = File.ReadAllBytes(file);
                    return new FileContentResult(fileBytes, "application/pdf") { FileDownloadName = "demo.pdf" };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred");
                throw;
            }

        }
    }
}

