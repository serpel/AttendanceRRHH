using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class ExtraHour
    {
        [Key]
        public Int32 ExtraHourId { get; set; }
        [Required]
        public string Name { get; set; }
        public Int32? CompanyId { get; set; }

        public virtual ICollection<ExtraHourDetail> ExtraHourDetail { get; set; }
    }
}