using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xbca.Models
{
    public class Constants
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

        public enum BatteryChargeThreshold : int
        {
            // Battery Charge Thresholds
            [Description("Empty")]
            BATTERY_THRESHOLD_EMPTY = 0,
            [Description("Low")] // API returns 400 mW
            BATTERY_THRESHOLD_LOW = 30,
            [Description("Medium")]
            BATTERY_THRESHOLD_MEDIUM = 60, // API returns 700 mW
            [Description("Full")]
            BATTERY_THRESHOLD_FULL = 90 // API returns 1000 mW
        }

        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
