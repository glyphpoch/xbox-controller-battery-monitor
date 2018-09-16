using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xbca.Models
{
    public class XInputConstants
    {
        // Max amount of devices
        public const int XUSER_MAX_COUNT = 4;

        // Device type for gamepad
        public const int XINPUT_DEVTYPE_GAMEPAD = 0x01;

        // Device subtypes available in XINPUT_CAPABILITIES
        public const int XINPUT_DEVSUBTYPE_GAMEPAD = 0x01;

        // Flags for XINPUT_CAPABILITIES
        public enum CapabilityFlags
        {
            XINPUT_CAPS_VOICE_SUPPORTED = 0x0004,
            //For Windows 8 and above only
            XINPUT_CAPS_FFB_SUPPORTED = 0x0001,
            XINPUT_CAPS_WIRELESS = 0x0002,
            XINPUT_CAPS_PMD_SUPPORTED = 0x0008,
            XINPUT_CAPS_NO_NAVIGATION = 0x0010,
        };

        // Flags to pass to XInputGetCapabilities
        public const int XINPUT_FLAG_GAMEPAD = 0x00000001;
    }
}
