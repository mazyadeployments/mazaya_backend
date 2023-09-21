using System;

namespace MMA.WebApi.Shared.Models.Units
{
    public class UnitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateOn { get; set; }
        public string UpdateBy { get; set; }
    }
}
