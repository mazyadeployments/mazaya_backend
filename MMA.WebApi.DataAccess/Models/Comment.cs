using System;
using System.ComponentModel.DataAnnotations;

namespace MMA.WebApi.DataAccess.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }

        [MaxLength(1000)]
        public string CreatedBy { get; set; }
        public int OfferId { get; set; }
    }
}
