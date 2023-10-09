using System;

namespace MMA.WebApi.Shared.Models.Membership
{
    public class MembershipEcardModel
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string MemberId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ECardSequence { get; set; }
        public int MembershipType { get; set; }
        public Guid? MembershipId { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsMembershipCard { get; set; }
        public DateTime ValidTo { get; set; }
        public string Status { get; set; }
        public string MembershipNameEng { get; set; }
        public string MembershipNameAr { get; set; }
        public string BackgroundPortraitUrl { get; set; }
        public string BackgroundLandscapeUrl { get; set; }
        public string backgroundLandscapeBackUrl { get; set; }
        public string backgroundPortraitBackUrl { get; set; }
        public string supportEngText { get; set; }
        public string supportArText { get; set; }

    }
}
