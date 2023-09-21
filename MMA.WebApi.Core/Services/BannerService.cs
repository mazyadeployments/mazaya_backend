using Microsoft.AspNetCore.Http;
using MMA.WebApi.Shared.Interfaces.Banner;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Models.Banner;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Core.Services
{
    public class BannerService : IBannerService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IBannerRepository _bannerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BannerService(IOfferRepository offerRepository, IBannerRepository bannerRepository, IHttpContextAccessor httpContextAccessor)
        {
            _offerRepository = offerRepository;
            _bannerRepository = bannerRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateOrUpdateBanner(List<int> offerIds, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            _offerRepository.CreateOrUpdateBanner(offerIds, auditVisitor);
        }

        public IEnumerable<BannerViewModel> GetBanners(int limit)
        {
            var bannerViewImages = _bannerRepository.GetBannerViewModel().ToList();
            foreach (var bannerViewImage in bannerViewImages)
            {
                ProcessImages(bannerViewImage);
                bannerViewImage.Images = null;
            }

            return bannerViewImages;
        }

        private static void ProcessImages(BannerViewModel bannerViewModel)
        {
            bannerViewModel.ImageUrls = new List<ImageUrlsModel>();

            foreach (var image in bannerViewModel.Images)
            {
                if (image.OriginalImageId == Guid.Empty)
                {
                    image.OriginalImageId = new Guid(image.Id);
                }
            }

            var grouping = bannerViewModel.Images.GroupBy(x => x.OriginalImageId);
            foreach (var imageGroping in grouping)
            {
                ImageUrlsModel imageUrlsModel = new ImageUrlsModel();
                foreach (var imageModel in imageGroping)
                {
                    if (imageModel.Type == OfferDocumentType.Thumbnail)
                    {
                        imageUrlsModel.Thumbnail = imageModel.Id;
                        bannerViewModel.MainImage = imageModel.Id;
                    }
                    else if (imageModel.Type == OfferDocumentType.Large)
                    {
                        imageUrlsModel.Large = imageModel.Id;
                    }
                    else
                    {
                        imageUrlsModel.Original = imageModel.Id;
                    }
                }
                bannerViewModel.ImageUrls.Add(imageUrlsModel);
            }
        }
    }
}
