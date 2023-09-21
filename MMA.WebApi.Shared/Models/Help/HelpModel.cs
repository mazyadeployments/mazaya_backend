using System;

namespace MMA.WebApi.Shared.Models.Help
{
    public class HelpModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string HelpText { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
