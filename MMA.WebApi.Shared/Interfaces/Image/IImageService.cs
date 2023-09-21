using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Image
{
    public interface IImageService
    {
        Task<string> GetuserImage(string userId);
        byte[] ImageToByteArray(System.Drawing.Image image);

    }
}
