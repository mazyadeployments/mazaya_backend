using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Membership
{
    public class MobileUserInfo
    {
        public IEnumerable<MembershipModel> Memberships { get; set; }
        public int MyCardCount { get; set; }
        public int MyFamilyCardCount { get; set; }
    }
}
