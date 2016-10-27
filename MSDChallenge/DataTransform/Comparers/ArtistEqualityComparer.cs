using System;
using System.Collections.Generic;
using DataTransform.Models;

namespace DataTransform.Comparers
{
    public class ArtistEqualityComparer : IEqualityComparer<Artist>
    {
        public bool Equals(Artist x, Artist y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return StringComparer.InvariantCulture.Equals(x.Id, y.Id);
        }

        public int GetHashCode(Artist obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
