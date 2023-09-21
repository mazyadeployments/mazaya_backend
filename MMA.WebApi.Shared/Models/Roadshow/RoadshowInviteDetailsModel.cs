using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.Companies;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowInviteDetailsModel
    {
        public int Id { get; set; }
        public int RoadshowId { get; set; }
        public IEnumerable<RoadshowInviteEventModel> RoadshowEvents { get; set; } = new List<RoadshowInviteEventModel>();
        public virtual ICollection<RoadshowCommentModel> RoadshowComments { get; set; } = new List<RoadshowCommentModel>();
        public CompanyModel Company { get; set; }
        public int CompanyId { get; set; }
        public Declares.RoadshowInviteStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
