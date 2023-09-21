using MMA.WebApi.Shared.Helpers;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowInvitesQueryModel
    {
        public IEnumerable<RoadshowEventModel> Events { get; set; } = new List<RoadshowEventModel>();
        public IEnumerable<int> CompanyIds { get; set; } = new List<int>();
        public IEnumerable<int> InviteIds { get; set; } = new List<int>();
        public QueryModel QueryModel { get; set; }
        public bool SendInvite { get; set; }
        public bool KeepEvents { get; set; }
    }
}
