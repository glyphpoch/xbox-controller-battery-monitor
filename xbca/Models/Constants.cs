namespace Xbca.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Varius XInput constants.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Battery type values.
        /// </summary>
        public enum BatteryTypes : byte
        {
            /// <summary>
            /// Battery disconnected.
            /// </summary>
            [Description("Disconnected")]
            BATTERY_TYPE_DISCONNECTED = 0x00,

            /// <summary>
            /// Wired controller.
            /// </summary>
            [Description("Wired")]
            BATTERY_TYPE_WIRED = 0x01,

            /// <summary>
            /// Alkaline batteries.
            /// </summary>
            [Description("Alkaline")]
            BATTERY_TYPE_ALKALINE = 0x02,

            /// <summary>
            /// NiMH batteries - rechargable.
            /// </summary>
            [Description("NiMH")]
            BATTERY_TYPE_NIMH = 0x03,

            /// <summary>
            /// No batteries or alkaline.
            /// </summary>
            [Description("No battery/Alkaline")]
            BATTERY_TYPE_NOBATTERY = 0xEF,

            /// <summary>
            /// Unknown battery type.
            /// </summary>
            [Description("Unknown")]
            BATTERY_TYPE_UNKNOWN = 0xFF,
        }

        /// <summary>
        /// Battery levels.
        /// </summary>
        public enum BatteryLevel : byte
        {
            /// <summary>
            /// Battery empty.
            /// </summary>
            [Description("Empty")]
            BATTERY_LEVEL_EMPTY = 0x00,

            /// <summary>
            /// Battery low.
            /// </summary>
            [Description("Low")]
            BATTERY_LEVEL_LOW = 0x01,

            /// <summary>
            /// Battery medium.
            /// </summary>
            [Description("Medium")]
            BATTERY_LEVEL_MEDIUM = 0x02,

            /// <summary>
            /// Battery full.
            /// </summary>
            [Description("Full")]
            BATTERY_LEVEL_FULL = 0x03,

            /// <summary>
            /// Test/debug battery status.
            /// </summary>
            [Description("Test")]
            BATTERY_LEVEL_TEST = 0x06,
        }

        /// <summary>
        /// Battery charge thresholds.
        /// </summary>
        public enum BatteryChargeThreshold : int
        {
            /// <summary>
            /// Empty threshold.
            /// </summary>
            [Description("Empty")]
            BATTERY_THRESHOLD_EMPTY = 0,

            /// <summary>
            /// Low charge level threshold.
            /// </summary>
            [Description("Low")]
            BATTERY_THRESHOLD_LOW = 30, // API returns 400 mW

            /// <summary>
            /// Medium charge level threshold.
            /// </summary>
            [Description("Medium")]
            BATTERY_THRESHOLD_MEDIUM = 60, // API returns 700 mW

            /// <summary>
            /// Full charge level threshold.
            /// </summary>
            [Description("Full")]
            BATTERY_THRESHOLD_FULL = 90, // API returns 1000 mW
        }

        /// <summary>
        /// Gets enums description.
        /// </summary>
        /// <param name="value">enums value.</param>
        /// <returns>enums description.</returns>
        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
