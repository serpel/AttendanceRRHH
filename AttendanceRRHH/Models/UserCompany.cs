using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class UserCompany
    {
        [Key]
        public Int32 UserCompanyId { get; set; }

        [Required]
        [Display(Name = "User")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public Int32 CompanyId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Company Company { get; set; }
    }
}