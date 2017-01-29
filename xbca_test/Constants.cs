using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xbca_test
{
    [Flags]
    public enum ButtonFlags : int
    {
        XINPUT_GAMEPAD_DPAD_UP = 0x0001,
        XINPUT_GAMEPAD_DPAD_DOWN = 0x0002,
        XINPUT_GAMEPAD_DPAD_LEFT = 0x0004,
        XINPUT_GAMEPAD_DPAD_RIGHT = 0x0008,
        XINPUT_GAMEPAD_START = 0x0010,
        XINPUT_GAMEPAD_BACK = 0x0020,
        XINPUT_GAMEPAD_LEFT_THUMB = 0x0040,
        XINPUT_GAMEPAD_RIGHT_THUMB = 0x0080,
        XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100,
        XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200,
        XINPUT_GAMEPAD_A = 0x1000,
        XINPUT_GAMEPAD_B = 0x2000,
        XINPUT_GAMEPAD_X = 0x4000,
        XINPUT_GAMEPAD_Y = 0x8000,
    };

    [Flags]
    public enum ControllerSubtypes
    {
        XINPUT_DEVSUBTYPE_UNKNOWN = 0x00,
        XINPUT_DEVSUBTYPE_WHEEL = 0x02,
        XINPUT_DEVSUBTYPE_ARCADE_STICK = 0x03,
        XINPUT_DEVSUBTYPE_FLIGHT_STICK = 0x04,
        XINPUT_DEVSUBTYPE_DANCE_PAD = 0x05,
        XINPUT_DEVSUBTYPE_GUITAR = 0x06,
        XINPUT_DEVSUBTYPE_GUITAR_ALTERNATE = 0x07,
        XINPUT_DEVSUBTYPE_DRUM_KIT = 0x08,
        XINPUT_DEVSUBTYPE_GUITAR_BASS = 0x0B,
        XINPUT_DEVSUBTYPE_ARCADE_PAD = 0x13
    };

    public enum BatteryTypes : byte
    {
        //
        // Flags for battery status level
        //
        [Description("Disconnected")]
        BATTERY_TYPE_DISCONNECTED = 0x00,    // This device is not connected
        [Description("Wired")]
        BATTERY_TYPE_WIRED = 0x01,    // Wired device, no battery
        [Description("Alkaline")]
        BATTERY_TYPE_ALKALINE = 0x02,    // Alkaline battery source
        [Description("NiMH")]
        BATTERY_TYPE_NIMH = 0x03,    // Nickel Metal Hydride battery source
        [Description("Unknown")]
        BATTERY_TYPE_UNKNOWN = 0xFF,    // Cannot determine the battery type
    };


    // These are only valid for wireless, connected devices, with known battery types
    // The amount of use time remaining depends on the type of device.
    public enum BatteryLevel : byte
    {
        [Description("Empty")]
        BATTERY_LEVEL_EMPTY = 0x00,
        [Description("Low")]
        BATTERY_LEVEL_LOW = 0x01,
        [Description("Medium")]
        BATTERY_LEVEL_MEDIUM = 0x02,
        [Description("Full")]
        BATTERY_LEVEL_FULL = 0x03
    };

    public enum BatteryDeviceType : byte
    {
        BATTERY_DEVTYPE_GAMEPAD = 0x00,
        BATTERY_DEVTYPE_HEADSET = 0x01,
    }

    public class XInputConstants
    {
        public const int XINPUT_DEVTYPE_GAMEPAD = 0x01;

        //
        // Device subtypes available in XINPUT_CAPABILITIES
        //
        public const int XINPUT_DEVSUBTYPE_GAMEPAD = 0x01;

        //
        // Flags for XINPUT_CAPABILITIES
        //
        public enum CapabilityFlags
        {
            XINPUT_CAPS_VOICE_SUPPORTED = 0x0004,
            //For Windows 8 only
            XINPUT_CAPS_FFB_SUPPORTED = 0x0001,
            XINPUT_CAPS_WIRELESS = 0x0002,
            XINPUT_CAPS_PMD_SUPPORTED = 0x0008,
            XINPUT_CAPS_NO_NAVIGATION = 0x0010,
        };
        //
        // Constants for gamepad buttons
        //

        //
        // Gamepad thresholds
        //
        public const int XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE = 7849;
        public const int XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE = 8689;
        public const int XINPUT_GAMEPAD_TRIGGER_THRESHOLD = 30;

        //
        // Flags to pass to XInputGetCapabilities
        //
        public const int XINPUT_FLAG_GAMEPAD = 0x00000001;


    }
}
