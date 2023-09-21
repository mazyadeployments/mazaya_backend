using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Image
{
    public interface IImageRepository
    {
        Task<string> GetuserImage(string userId);
    }
}
