using MMA.WebApi.DataAccess.Models;
using System.Collections.Generic;

namespace MMA.WebApi.DataAccess.Helpers
{
    class DefaultLocationEqualityHelper : IEqualityComparer<DefaultLocation>
    {
        public int GetHashCode(DefaultLocation dl)
        {
            if (dl == null)
            {
                return 0;
            }
            return dl.Id.GetHashCode();
        }

        public bool Equals(DefaultLocation x1, DefaultLocation x2)
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
