using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Location;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Filters
{
    public class FilterData
    {
        public IEnumerable<Category.CategoryModel> Category { get; set; }
        public IEnumerable<Collection.CollectionModel> Collections { get; set; }
        public IEnumerable<Tag.TagModel> Tags { get; set; }
        public IEnumerable<DefaultLocationModel> Locations { get; set; }
        public IEnumerable<DefaultAreaModel> Areas { get; set; }
        public IEnumerable<Membership.MembershipModel> Memberships { get; set; }
    }
}
