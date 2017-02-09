using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace xbca
{
    public enum BatteryTypes : byte
    {
        // Flags for battery type
        [Description("Disconnected")]
        BATTERY_TYPE_DISCONNECTED = 0x00,
        [Description("Wired")]
        BATTERY_TYPE_WIRED = 0x01,
        [Description("Alkaline")]
        BATTERY_TYPE_ALKALINE = 0x02,
        [Description("NiMH")]
        BATTERY_TYPE_NIMH = 0x03,
        [Description("Unknown")]
        BATTERY_TYPE_UNKNOWN = 0xFF,
    };

    public enum BatteryLevel : byte
    {
        // Flags for battery level
        [Description("Empty")]
        BATTERY_LEVEL_EMPTY = 0x00,
        [Description("Low")]
        BATTERY_LEVEL_LOW = 0x01,
        [Description("Medium")]
        BATTERY_LEVEL_MEDIUM = 0x02,
        [Description("Full")]
        BATTERY_LEVEL_FULL = 0x03,
        [Description("Test")]
        BATTERY_LEVEL_TEST = 0x06
    };

    public class Constants
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }

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
