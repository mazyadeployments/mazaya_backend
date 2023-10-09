using System;

namespace MMA.WebApi.DataAccess.Models
{
    public class LogBannerClick
    {
        public long Id { get; set; }
        public int OfferId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
