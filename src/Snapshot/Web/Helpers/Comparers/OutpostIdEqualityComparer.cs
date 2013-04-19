using Domain;
using System.Collections.Generic;

namespace Web.Helpers.Comparers
{
    public class OutpostIdEqualityComparer: IEqualityComparer<Outpost>
    {
        public bool Equals(Outpost x, Outpost y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Outpost obj)
        {
            return obj.GetHashCode();
        }
    }
}