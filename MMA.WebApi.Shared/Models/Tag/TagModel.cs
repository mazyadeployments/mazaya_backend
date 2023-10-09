using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Tag
{
    public class TagModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsEditable { get; set; }
        public string Description { get; set; }
        public IEnumerable<int> OffersIds { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }
}
