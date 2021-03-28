using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using DinkToPdf;
using Azure.Storage.Blobs.Models;
using GeneratePdfHttpFunction.Entities;
using DinkToPdf.Contracts;
using GeneratePdfHttpFunction.Services.Interfaces;

namespace GeneratePdfHttpFunction
{
    public class FunctionGeneratePdf
    {
        private readonly IConverter _converter;
        private readonly IStorageService _storageService;

        public FunctionGeneratePdf(IConverter converter, IStorageService storageService)
        {
            _converter = converter;
            _storageService = storageService;
        }

        [FunctionName("GeneratePdf")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<PdfRequest>(requestBody);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = request.Title,
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                WebSettings = { DefaultEncoding = "utf-8" },
                HtmlContent = request.HtmlContent
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            string fileName = await SavePdf(request, pdf);

            return new OkObjectResult(new PdfReply() { FileName = fileName });
        }

        private async Task<string> SavePdf(PdfRequest request, HtmlToPdfDocument pdf)
        {
            string fileName = Path.ChangeExtension(request.FileName, ".pdf");

            using (var pdfStream = new MemoryStream(_converter.Convert(pdf)))
            {
                var httpHeader = new BlobHttpHeaders()
                {
                    ContentType = "application/pdf",
                    ContentEncoding = "utf-8",
                };

                await _storageService.UploadFromStream(request.BlobContainer, fileName, pdfStream, httpHeader);
            }

            return fileName;
        }
    }
}
