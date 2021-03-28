using DinkToPdf;
using DinkToPdf.Contracts;
using GeneratePdfHttpFunction.Configuration;
using GeneratePdfHttpFunction.Services;
using GeneratePdfHttpFunction.Services.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Runtime.InteropServices;

[assembly: FunctionsStartup(typeof(GeneratePdfHttpFunction.Startup))]
namespace GeneratePdfHttpFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var wkHtmlToPdfFileName = "libwkhtmltox";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                wkHtmlToPdfFileName += ".so";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                wkHtmlToPdfFileName += ".dylib";

            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), wkHtmlToPdfFileName));
            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            var configuration = builder.GetContext().Configuration;
            var storageSettings = new StorageSettings
            {
                ConnectionString = configuration["ConnectionString"]
            };

            builder.Services.AddSingleton(storageSettings);
            builder.Services.AddSingleton<IStorageService, StorageService>();
        }
    }
}
