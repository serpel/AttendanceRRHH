using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class AttendanceRecord
    {
        [Key]
        public Int32 AttendanceRecodId { get; set; }
        [Required]
        public Int32 DeviceId { get; set; }
        [Required]
        public Int32 EmployeeId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime InsertedAt { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Device Device { get; set; }
    }
}