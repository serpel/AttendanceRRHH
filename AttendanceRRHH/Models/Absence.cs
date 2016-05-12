using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceRRHH.Models
{
    public class Absence
    {
        [Key]
        public Int32 AbsenceId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<EmployeeAbsence> EmployeeAbsences { get; set; }
    }
}