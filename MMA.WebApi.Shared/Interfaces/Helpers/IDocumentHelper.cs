using Microsoft.AspNetCore.Http;

namespace MMA.WebApi.Shared.Interfaces.Helpers
{
    public interface IDocumentHelper
    {
        byte[] GetBytes(IFormFile file);
        string GetBytesForMagicNumberCheck(byte[] bytes);
    }
}
