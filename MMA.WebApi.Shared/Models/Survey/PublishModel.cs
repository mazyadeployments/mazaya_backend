using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Survey
{
    public class PublishModel
    {
        public ICollection<string> UsersId { get; set; }
        public ICollection<string> Types { get; set; }//id type
        public ICollection<string> Roles { get; set; }
        public bool AllUsers { get; set; }
    }
}
