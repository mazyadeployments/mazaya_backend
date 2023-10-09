using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.Image;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Image
{
    public interface IImageUtilsService
    {
        Task<List<ImageModel>> PrepareImagesForUpload(List<ImageModel> images);
        Task CreateImages(List<ImageModel> croppedImages, Declares.ImageForType imageForType);
        byte[] ProcessImage(byte[] data);
        byte[] ProcessImageForOffer(byte[] data, ImageBackground backgroundType = ImageBackground.Blur, string htmlBackgroundColor = "#DCE2E8", int? overrideRatio = null, bool blur = true, int? maxSize = null);
        byte[] ProcessImageForCategory(byte[] data, ImageBackground backgroundType = ImageBackground.Blur, string htmlBackgroundColor = "#DCE2E8", int? overrideRatio = null, bool blur = true, int? maxSize = null);
        byte[] ProcessImageForCollection(byte[] data, ImageBackground backgroundType = ImageBackground.Blur, string htmlBackgroundColor = "#DCE2E8", int? overrideRatio = null, bool blur = true, int? maxSize = null);
        byte[] CropImage(byte[] data, int cropX, int cropY, int cropWidth, int cropHeight);
        byte[] Resize(byte[] source, int width, int height);
    }
}
