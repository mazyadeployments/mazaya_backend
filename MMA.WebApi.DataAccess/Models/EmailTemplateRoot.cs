using MMA.WebApi.Shared.Interfaces.GenericData;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class EmailTemplateRoot : IChangeable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string MailTemplate { get; set; }
        public string MailBodyFooter { get; set; }
        public string MailApplicationLogin { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
