using AttendanceRRHH.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceRRHH.Helpers
{
    public class UserCompanyComparer : IEqualityComparer<UserCompany>
    {
        public bool Equals(UserCompany x, UserCompany y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(UserCompany obj)
        {
            throw new NotImplementedException();
        }
    }
}