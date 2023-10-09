using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Interfaces.Document;
using System.ComponentModel;

namespace MMA.Documents.Domain.Helpers
{
    public class DocumentProviderFactory
    {
        public enum Operator
        {
            [Description("filesystem")]
            filesys = 0,
            [Description("azureblobstorage")]
            azureblobstorage = 1,
            [Description("database")]
            database = 2
        }

        public static DocumentProvider GetDocumentProvider(Operator name, IDocumentRepository documentRepository, IConfiguration _configurationservice)
        {

            int i = (int)name;

            switch (i)
            {
                case 0:
                    return new DocumentFileSystem(documentRepository, _configurationservice);

                case 1:
                    return new DocumetAzureStorage(_configurationservice);

                case 2:
                    return new DocumentDatabase(documentRepository);

                default:
                    return new DocumentFileSystem(documentRepository, _configurationservice);
            }

        }

    }
}
