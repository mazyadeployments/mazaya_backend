using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.Collection;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Models.Tag;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Mobile
{
    public class SynchronizationDataModel
    {
        public IEnumerable<OfferModelMobile> Offers { get; set; }
        public IEnumerable<CategoryModel> Categories { get; set; }
        public IEnumerable<CollectionModel> Collections { get; set; }
        public IEnumerable<TagModel> Tags { get; set; }
        //public IEnumerable<RoadshowLocationModel> RoadshowsLocations { get; set; }
        public IEnumerable<RoadshowOfferMobileModel> RoadshowsOffers { get; set; }

        /// <summary>
        /// DateTime value which is less than UpdatedOn values in 'Offers', 'Collections', 'Categories'
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        public IEnumerable<int> OffersIds { get; set; }
        public IEnumerable<int> CollectionsIds { get; set; }
        public IEnumerable<int> CategoriesIds { get; set; }
        public IEnumerable<int> TagIds { get; set; }
        public IEnumerable<int> RoadshowsOfferIds { get; set; }
        //public string CurrentAndroidVersion
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["CurrentAndroidMobileVersion"];
        //    }
        //}

        //public string CurrentiOSVersion
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["CurrentiOSMobileVersion"];
        //    }
        //}
    }
}
