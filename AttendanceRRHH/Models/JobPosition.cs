using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class JobPosition
    {
        [Key]
        public Int32 JobPositionId { get; set; }
        [StringLength(100)]
        public string JobTitle { get; set; }
        public Int32? CompanyId { get; set; }
        public bool IsActive { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}