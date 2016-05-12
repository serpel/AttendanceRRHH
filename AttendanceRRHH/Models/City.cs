using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class City
    {
        [Key]
        public Int32 CityId { get; set; }
        [Required]
        public Int32 CountryId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}