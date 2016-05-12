using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class Department
    {
        [Key]
        public Int32 DepartmentId { get; set; }
        [Required]
        public Int32 CompanyId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}