using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.OfferLocations;
using MMA.WebApi.Shared.Models.Offer;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repository.Offers
{
    public class OfferLocationRepository : BaseRepository<OfferLocationModel>, IOfferLocationRepository
    {
        public OfferLocationRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public async Task CreateAsync(OfferLocationModel model)
        {
            var context = ContextFactory();
            var offerLocationModel = new OfferLocation();

            PopulateEntityModel(offerLocationModel, model);

            context.Add(offerLocationModel);

            await context.SaveChangesAsync();
        }

        private Expression<Func<OfferLocation, OfferLocationModel>> projectToOfferLocationModel = data =>
           new OfferLocationModel()
           {
               Address = data.Address,
               Country = data.Country,
               Id = data.Id,
               Latitude = data.Latitude,
               Longitude = data.Longitude,
               OfferId = data.OfferId,
               Vicinity = data.Vicinity,
               DefaultAreaId = data.DefaultAreaId
           };

        private void PopulateEntityModel(OfferLocation data, OfferLocationModel model)
        {
            data.Id = model.Id;
            data.Address = model.Address;
            data.Country = model.Country;
            data.Latitude = model.Latitude;
            data.Longitude = model.Longitude;
            data.OfferId = model.OfferId;
            data.Vicinity = model.Vicinity;
            data.DefaultAreaId = model.DefaultAreaId;
        }
        protected override IQueryable<OfferLocationModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        public IQueryable<OfferLocationModel> Get()
        {
            throw new NotImplementedException();
        }
    }
}
