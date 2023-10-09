using MMA.WebApi.DataAccess.Models;
using System.Collections.Generic;

namespace MMA.WebApi.DataAccess.Helpers
{
    public class RoadshowEventEqualityHelper : IEqualityComparer<RoadshowEvent>
    {
        public int GetHashCode(RoadshowEvent re)
        {
            if (re == null)
            {
                return 0;
            }
            return re.Id.GetHashCode();
        }

        public bool Equals(RoadshowEvent x1, RoadshowEvent x2)
        {
            if (object.ReferenceEquals(x1, x2))
            {
                return true;
            }
            if (object.ReferenceEquals(x1, null) ||
                object.ReferenceEquals(x2, null))
            {
                return false;
            }
            return x1.Id == x2.Id;
        }
    }
}
