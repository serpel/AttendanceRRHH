using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRRHH.BLL
{
    public class DeviceFactory
    {
        public IDevice CreateIntance(string ip, int port, string type, string description, string location, bool isSSR)
        {
            IDevice result;

            //you should create a new class inherited from IDevice (This simulate several brands of biometrics)
            switch (type)
            {
                case "ZK":
                    result = new ZKDevice()
                    {
                        Ip = ip,
                        Port = port,
                        Type = type,
                        Description = description,
                        Location = location,
                        IsSSR = isSSR,
                        Status = Status.Unknown
                    };
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }
    }
}
