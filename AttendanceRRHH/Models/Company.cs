using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class Company
    {
        [Key]
        public Int32 CompanyId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(150)]
        public string Address { get; set; }
        public string LogoUrl { get; set; }
        public Int32? CountryId { get; set; }
        public Int32? CityId { get; set; }
        public bool IsActive { get; set; }
        public string EmailSendCronExpression { get; set; }

        public virtual Country Country { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }

        public Company()
        {
            IsActive = true;
            EmailSendCronExpression = "30 8 * * *";
        }
    }
}