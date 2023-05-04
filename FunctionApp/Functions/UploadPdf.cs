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

    public class UploadPdfFunction
    {
        private readonly ILogger<UploadPdfFunction> _logger;

        public UploadPdfFunction(ILogger<UploadPdfFunction> log)
        {
            _logger = log;
        }

        [FunctionName("UploadPdf")]
        [OpenApiOperation(operationId: "UploadPdf")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> UploadPdf(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                PdfGenerator.Program.Main((s) => _logger.LogInformation(s), context.FunctionAppDirectory);
                return new OkObjectResult("OK");
            }
            catch (Exception e)
            {
                throw;
                _logger.LogError(e, "An error occurred");
            }
        }
    }
}

