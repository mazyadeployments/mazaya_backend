using System;
namespace MMA.WebApi.DataAccess.Models
{
    public class MembershipPictureData
    {
        public Guid Id { get; set; }
        public Guid DocumentIdHorizontalPicture { get; set; }
        public Guid DocumentIdHorizontalBackPicture { get; set; }
        public Guid DocumentIdVerticalPicture { get; set; }
        public Guid DocumentIdVerticalBackPicture { get; set; }
        public int MembershipType { get; set; }
    }
}
