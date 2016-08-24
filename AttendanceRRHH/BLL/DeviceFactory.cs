using AttendanceRRHH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRRHH.BLL
{
    public class DeviceFactory
    {
        public IDevice CreateIntance(Device device)
        {
            IDevice result;

            //you should create a new class inherited from IDevice (This simulate several brands of biometrics)
            switch (device.DeviceType.Name)
            {
                case "ZK":
                    result = new ZKDevice();
                    result.Device = device;
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        public IDevice CreateIntance(Device device, ApplicationDbContext context)
        {
            IDevice result;

            //you should create a new class inherited from IDevice (This simulate several brands of biometrics)
            switch (device.DeviceType.Name)
            {
                case "ZK":
                    result = new ZKDevice(device.DeviceId, context);
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }
    }
}
