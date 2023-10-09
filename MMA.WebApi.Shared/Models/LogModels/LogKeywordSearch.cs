using System;

namespace MMA.WebApi.Shared.Models.LogModels

{
    public class LogKeywordSearch
    {
        public long Id { get; set; }
        public string Keyword { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

    }
}
