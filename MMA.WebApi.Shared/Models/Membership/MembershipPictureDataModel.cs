using System;

namespace MMA.WebApi.Shared.Models.Membership
{
    public class MembershipPictureDataModel
    {

        public Guid Id { get; set; }
        public Guid DocumentIdHorizontalPicture { get; set; }
        public Guid DocumentIdVerticalPicture { get; set; }
        public int MembershipType { get; set; }

    }
}
