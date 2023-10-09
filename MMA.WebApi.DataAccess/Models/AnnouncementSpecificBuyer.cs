using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class AnnouncementSpecificBuyer
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; }

        [ForeignKey("AnnouncementId")]
        public virtual Announcement Announcement { get; set; }
        public int BuyerType { get; set; }
    }
}
