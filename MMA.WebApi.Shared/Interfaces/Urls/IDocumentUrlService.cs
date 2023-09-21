using System;

namespace MMA.WebApi.Shared.Interfaces.Urls
{
    public interface IDocumentUrlService
    {
        string GetDocumentUrl(Guid? docoumentId, string extension);
        string GetHost();
    }
}