using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class TimeSheet
    {
        [Key]
        public Int32 TimeSheetId { get; set; }
        [Required]
        public Int32 EmployeeId { get; set; }
        public Int32? ShiftTimeId { get; set; }
        public DateTime Date { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? In { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? Out { get; set; }
        public bool IsManualIn { get; set; }
        public bool IsManualOut { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime InsertedAt { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual ShiftTime ShiftTime { get; set; }
    }
}