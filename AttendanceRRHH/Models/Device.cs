using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class Device
    {
        [Key]
        public Int32 DeviceId { get; set; }
        [Required]
        public Int32 DeviceTypeId { get; set; }
        [Required]
        [StringLength(150)]
        public string Description { get; set; }
        [StringLength(150)]
        public string Location { get; set; }
        [Required]
        public string IP { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public bool IsSSR { get; set; }
        public bool OpenDoors { get; set; }
        public bool IsActive { get; set; }

        public virtual DeviceType DeviceType { get; set; }
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; }
    }
}