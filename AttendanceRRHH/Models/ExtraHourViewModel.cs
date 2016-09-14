using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceRRHH.Models
{
    public class ExtraHourViewModel
    {
        public Int32? CompanyId { get; set; }
        public string Name { get; set; }

        public List<ExtraHourDetail> ExtraDetails { get; set; }

    }
}