namespace GeneratePdfHttpFunction.Entities
{
    public class PdfRequest
    {
        public string HtmlContent { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string BlobContainer { get; set; }
    }
}
