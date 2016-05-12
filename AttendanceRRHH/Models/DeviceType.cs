using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public class DeviceType
    {
        [Key]
        public Int32 DeviceTypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(150)]
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Device> Devices { get; set; }
    }
}