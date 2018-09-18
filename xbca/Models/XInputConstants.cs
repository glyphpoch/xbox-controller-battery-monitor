namespace Xbca.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the XInput constants.
    /// </summary>
    public class XInputConstants
    {
        /// <summary>
        /// Max amount of devices.
        /// </summary>
        public const int XUSER_MAX_COUNT = 4;

        /// <summary>
        /// Device type for gamepad.
        /// </summary>
        public const int XINPUT_DEVTYPE_GAMEPAD = 0x01;

        /// <summary>
        /// Device subtypes available in XINPUT_CAPABILITIES.
        /// </summary>
        public const int XINPUT_DEVSUBTYPE_GAMEPAD = 0x01;

        /// <summary>
        /// Flags for XINPUT_CAPABILITIES.
        /// </summary>
        public enum CapabilityFlags
        {
            XINPUT_CAPS_VOICE_SUPPORTED = 0x0004,
            //For Windows 8 and above only
            XINPUT_CAPS_FFB_SUPPORTED = 0x0001,
            XINPUT_CAPS_WIRELESS = 0x0002,
            XINPUT_CAPS_PMD_SUPPORTED = 0x0008,
            XINPUT_CAPS_NO_NAVIGATION = 0x0010,
        }

        /// <summary>
        /// Flags to pass to XInputGetCapabilities.
        /// </summary>
        public const int XINPUT_FLAG_GAMEPAD = 0x00000001;
    }
}
