using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enum;

namespace SmartHome.Dto.Device
{
    public class DevicePin
    {
        public int PinNumber { get; set; }
        public DevicePinPurpose Purpose { get; set; }
    }
}
