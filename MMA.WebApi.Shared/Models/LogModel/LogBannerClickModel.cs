using System;

namespace MMA.WebApi.Shared.Models.LogModel
{
    public class LogBannerClickModel
    {
        public long Id { get; set; }
        public int OfferId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
