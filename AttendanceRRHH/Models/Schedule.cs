using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class Schedule
    {
        [Key]
        public Int32 ScheduleId { get; set; }
        public Int32? ShiftId { get; set; }
        public Int32? EmployeeId { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime InsertedAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        public virtual Shift Shift { get; set; }
        public virtual Employee Employee { get; set; }
    }
}