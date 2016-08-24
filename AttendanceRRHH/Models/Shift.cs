using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class Shift
    {
        [Key]
        public Int32 ShiftId { get; set; }
        //[Required]
        public Int32? CompanyId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(150)]
        public string Description { get; set; }
        [Required]
        public bool IsSpecialShift { get; set; }
        [Required]
        public DateTime InsertedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<ShiftTime> ShiftTimes { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }

        public Shift()
        {
            IsActive = true;
        }
    }
}