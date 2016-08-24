using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRRHH.Models
{
    public enum DeviceStatus { Unavailable = 0, Available = 1, Unknown = 2 }
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

        [Required]
        public string SyncTimeCronExpression { get; set; }
        [Required]
        public string TransferCronExpression { get; set; }
        public DeviceStatus DeviceStatus { get; set; }
        public DateTime SyncDate { get; set; }

        public virtual DeviceType DeviceType { get; set; }
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; }

        public Device()
        {
            SyncDate = DateTime.Now;
            IsActive = true;
            Port = 4370;
            DeviceStatus = DeviceStatus.Unknown;
            SyncTimeCronExpression = "0 8 * * *";
            TransferCronExpression = "30 8 * * *";
        }
    }
}