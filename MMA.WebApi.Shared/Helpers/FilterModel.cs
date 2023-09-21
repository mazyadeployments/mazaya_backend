using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Location;
using MMA.WebApi.Shared.Models.Membership;
using System;
using System.Collections.Generic;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Helpers
{
    public class FilterModel
    {
        public string Keyword { get; set; }
        public IEnumerable<int> Categories { get; set; }
        public IEnumerable<int> Collections { get; set; }
        public List<string> Ratings { get; set; }
        public List<DefaultLocationModel> Locations { get; set; }
        public List<DefaultAreaModel> Areas { get; set; }
        public IEnumerable<int> Tags { get; set; }
        public IEnumerable<string> Status { get; set; }
        public IEnumerable<string> Type { get; set; }
        public int DiscountFrom { get; set; }
        public int DiscountTo { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public string Special { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public IEnumerable<MembershipModel> Memberships { get; set; }
        public IEnumerable<int> Roadshows { get; set; }
        public IEnumerable<int> Companies { get; set; }
        public bool IsFavorite { get; set; } = false;
        public IEnumerable<RoadshowStatus> StatusEnum { get; set; }

        public FilterModel()
        {
            Categories = new List<int>();
            Collections = new List<int>();
            Tags = new List<int>();
            Locations = new List<DefaultLocationModel>();
            Areas = new List<DefaultAreaModel>();
            Status = new List<string>();
            Type = new List<string>();
            StatusEnum = new List<RoadshowStatus>();
            DiscountFrom = 0;
            DiscountTo = 100;
            PriceFrom = 0;
            PriceTo = 100000;
            Ratings = new List<string>();
            Keyword = string.Empty;
            Special = string.Empty;
            DateFrom = DateTime.MinValue;
            DateTo = DateTime.MaxValue;
            Roadshows = new List<int>();
            Companies = new List<int>();
        }
    }

    public class OneHubFilterModel
    {
        public string Keyword { get; set; }
        public IEnumerable<int> Categories { get; set; }
        public IEnumerable<int> Collections { get; set; }
        public List<string> Ratings { get; set; }
        public List<DefaultLocationModel> Locations { get; set; }
        public List<DefaultAreaModel> Areas { get; set; }
        public IEnumerable<int> Tags { get; set; }
        public IEnumerable<string> Status { get; set; }
        public IEnumerable<string> Type { get; set; }
        public IEnumerable<OneHubDiscountModel> DiscountList { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public string Special { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public IEnumerable<MembershipModel> Memberships { get; set; }
        public IEnumerable<int> Roadshows { get; set; }
        public IEnumerable<int> Companies { get; set; }
        public IEnumerable<RoadshowInviteStatus> StatusEnum { get; set; }
        public bool IsFavorite { get; set; } = false;

        public OneHubFilterModel()
        {
            Categories = new List<int>();
            Collections = new List<int>();
            Tags = new List<int>();
            Locations = new List<DefaultLocationModel>();
            Areas = new List<DefaultAreaModel>();
            Status = new List<string>();
            Type = new List<string>();
            StatusEnum = new List<RoadshowInviteStatus>();
            DiscountList = new List<OneHubDiscountModel>();
            PriceFrom = 0;
            PriceTo = 100000;
            Ratings = new List<string>();
            Keyword = string.Empty;
            Special = string.Empty;
            DateFrom = DateTime.MinValue;
            DateTo = DateTime.MaxValue;
            Roadshows = new List<int>();
            Companies = new List<int>();
        }
    }

    public class OneHubDiscountModel
    {
        public int DiscountFrom { get; set; } = 0;
        public int DiscountTo { get; set; } = 20;
    }
}
