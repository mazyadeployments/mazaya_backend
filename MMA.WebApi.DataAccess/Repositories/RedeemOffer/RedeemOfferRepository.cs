
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.RedeemOffer;
using MMA.WebApi.Shared.Models.RedeemOffer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.RedeemOffer
{
    public class RedeemOfferRepository : BaseRepository<RedeemOfferModel>, IRedeemOfferRepository
    {
        private readonly IConfiguration _configuration;

        public RedeemOfferRepository(Func<MMADbContext> contexFactory, IConfiguration configuration) : base(contexFactory)
        {
            _configuration = configuration;

        }
        public async Task<byte[]> GenerateQRCode(int offerId, string userId)
        {
            var qrCodeText = _configuration["BaseURL:Url"] + "RedeemOffer/" + offerId + "/" + userId;
            var imgData = myQRCodeGenerator.GenerateQRCodeWithLogo(qrCodeText);
            // var completeQR = createQRCodeWithBackgroun(imgData);
            return imgData;
        }

        public async Task<bool> RedeemQRCode(int offerId, string buyerId, string supplierId)
        {
            var context = ContextFactory();

            var offer = await (from o in context.Offer
                               join c in context.Company on o.CompanyId equals c.Id
                               join cs in context.CompanySuppliers on c.Id equals cs.CompanyId
                               where cs.SupplierId == supplierId && o.Id == offerId
                               select o).FirstOrDefaultAsync();
            if (offer == null)
                return false;
            Models.RedeemOffer retVal = new Models.RedeemOffer()
            {
                OfferId = offerId,
                UserId = buyerId,
                CreatedBy = supplierId,
                UpdatedBy = supplierId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };

            await context.RedeemOffers.AddAsync(retVal);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<ICollection<RedeemedOfferCountModel>> GetRedeemedOfferCounts()
        {
            var retVal = new List<RedeemedOfferCountModel>();
            var context = ContextFactory();

            var groupedRedeemedOffers = (from ro in context.RedeemOffers
                                         join o in context.Offer on ro.OfferId equals o.Id
                                         select new RedeemedOfferCountModel { OfferId = ro.OfferId, Count = 0, OfferTitle = o.Title }).ToList().GroupBy(x => x.OfferId);

            foreach (var item in groupedRedeemedOffers)
            {
                var offer = item.First();
                offer.Count = item.Count();
                retVal.Add(offer);
            }

            return retVal;
        }

        protected override IQueryable<RedeemOfferModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        private byte[] createQRCodeWithBackgroun(byte[] data)
        {

            ImageConverter converter = new ImageConverter();
            Image back = Image.FromFile("images/background_a8.png");

            var ms = new MemoryStream(data);
            Image qr = Image.FromStream(ms);
            Image resizeQR = qr.GetThumbnailImage(157, 157, null, IntPtr.Zero);
            int top = back.Height / 4;
            int left = back.Width / 10;
            Size size = resizeQR.Size;

            Graphics g = Graphics.FromImage(back);
            g.DrawImage(resizeQR, new Point(left, top));

            return (byte[])converter.ConvertTo(back, typeof(byte[])); ;


        }


    }
}
