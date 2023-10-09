using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.ApplicationUsers
{
    /*new
     * public class AdnocUsersDataCarrierModel
      {
          public int PageNumber { get; set; }
          public int TotalPage { get; set; }
          public int PageSize { get; set; }
          public int TotalCount { get; set; }
          public IEnumerable<AdnocAlumniModel> Items { get; set; }
          public bool HasPreviousPage { get; set; }
      }

      public class AdnocAlumniModel
      {
          public string Email { get; set; }
      }
    */
    public class AlumniTokenData
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }

    public class AdnocUsersDataCarrierModel
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
        public IEnumerable<AdnocAlumniModel> Models { get; set; }
    }

    public class AdnocAlumniModel
    {
        public string EmailAddress { get; set; }
    }
}
