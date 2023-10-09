using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class MailStorageDocument
    {
        public int Id { get; set; }
        public int MailStorageId { get; set; }

        [ForeignKey("DocumentId")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
