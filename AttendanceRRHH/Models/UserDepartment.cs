using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceRRHH.Models
{
    public class UserDepartment
    {
        [Key]
        public Int32 UserDepartmentId { get; set; }

        [Required]
        [Display(Name = "User")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Department")]
        public Int32 DepartmentId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Department Department { get; set; }
    }
}