using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class ShiftTime
    {
        [Key]
        public Int32 ShiftTimeId { get; set; }
        [Required]
        public Int32 ShiftId { get; set; }
        [Required]
        public int DayNumber { get; set; }
        [Required]
        public bool IsLaborDay { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
        [Required]
        public bool HasLunchTime { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan LunchStartTime { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan LunchEndTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime InsertedAt { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public virtual Shift Shift { get; set; }

        public ShiftTime()
        {
            IsActive = true;
            InsertedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}