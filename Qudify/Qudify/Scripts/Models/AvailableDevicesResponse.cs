using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qudify.Scripts.Models
{
    internal class AvailableDevicesResponse
    {
        public SpotifyDevice[] devices { get; set; } = new SpotifyDevice[0];

        public bool HasDevice(string deviceName)
        {
            return devices.Any(device => device.name == deviceName);
        }
    }
}
