using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class UserDomain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DomainName { get; set; }
        public string KeyValue { get; set; }
        public string Domains { get; set; }
        public string SequencerName { get; set; }
    }
}
