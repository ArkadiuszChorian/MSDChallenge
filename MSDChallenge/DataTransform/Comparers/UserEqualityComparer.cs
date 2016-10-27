using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransform.Models;

namespace DataTransform.Comparers
{
    public class UserEqualityComparer : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return StringComparer.InvariantCulture.Equals(x.Id, y.Id);
        }

        public int GetHashCode(User obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
