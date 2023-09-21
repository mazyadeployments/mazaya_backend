using System;
using System.ComponentModel.DataAnnotations;

namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowComment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }

        [MaxLength(1000)]
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public int RoadshowId { get; set; }
    }
}
