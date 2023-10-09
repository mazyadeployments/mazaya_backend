using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class Membership
    {
        public Guid Id { get; set; }
        public string NameEng { get; set; }
        public string NameAr { get; set; }
        public string Description { get; set; }
        public Guid PictureDataId { get; set; }

        [ForeignKey("PictureDataId")]
        public virtual MembershipPictureData PictureData { get; set; }
    }
}
