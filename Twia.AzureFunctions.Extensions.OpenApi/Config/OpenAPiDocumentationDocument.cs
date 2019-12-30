namespace Twia.AzureFunctions.Extensions.OpenApi.Config
{
    public class OpenAPiDocumentationDocument
    {
        public string Name { get; set; } = "v1";

        public string Title { get; set; } = "Open API Documentation";

        public string Version { get; set; } = "v1";

        public string Description { get; set; }
    }
}