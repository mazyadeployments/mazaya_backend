using MMA.WebApi.DataAccess.Models;
using System.Collections.Generic;

namespace MMA.WebApi.DataAccess.Helpers
{
    public class RoadshowEventOffersEqualityHelper : IEqualityComparer<RoadshowEventOffer>
    {
        public int GetHashCode(RoadshowEventOffer reo)
        {
            if (reo == null)
            {
                return 0;
            }

            unchecked
            {
                int hash = 17;
                hash = hash * 23 + reo.RoadshowOfferId.GetHashCode();
                hash = hash * 23 + reo.RoadshowEventId.GetHashCode();
                return hash;
            }
        }

        public bool Equals(RoadshowEventOffer x1, RoadshowEventOffer x2)
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
            return (x1.RoadshowEventId == x2.RoadshowEventId && x1.RoadshowOfferId == x2.RoadshowOfferId);
        }
    }
}
