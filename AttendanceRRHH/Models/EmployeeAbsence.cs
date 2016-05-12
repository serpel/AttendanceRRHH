using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceRRHH.Models
{
    public class EmployeeAbsence
    {
        [Key]
        public Int32 EmployeeAbsenceId { get; set; }
        [Required]
        public Int32 AbsenceId { get; set; }
        [Required]
        public Int32 EmployeeId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string Comment { get; set; }

        public virtual Absence Absence { get; set; }
        public virtual Employee Employee { get; set; } 
    }
}