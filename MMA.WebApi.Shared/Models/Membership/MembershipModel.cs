using System;

namespace MMA.WebApi.Shared.Models.Membership
{
    public class MembershipModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string NameEng { get; set; }
        public string NameAr { get; set; }
        public string Description { get; set; }
        public Guid PictureDataId { get; set; }
        public MembershipPictureDataModel PictureData { get; set; }
    }
}
