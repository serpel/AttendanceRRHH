using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class ExtraHourDetail
    {
        [Key]
        public Int32 ExtraHourDetailId { get; set; }
        [Required]
        public Int32 ExtraHourId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public decimal StartHour { get; set; }
        [Required]
        public decimal EndHour { get; set; }

        public virtual ExtraHour ExtraHour { get; set; }
    }
}