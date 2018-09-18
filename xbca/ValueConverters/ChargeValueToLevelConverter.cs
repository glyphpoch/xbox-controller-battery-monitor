namespace Xbca.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using Xbca.Models;

    /// <summary>
    /// Represents the converter that converts battery charge value to description string.
    /// </summary>
    [ValueConversion(typeof(byte), typeof(string))]
    public class ChargeValueToLevelConverter : IValueConverter
    {
        /// <summary>
        /// Converts battery charge value to description string.
        /// </summary>
        /// <param name="value">Battery charge value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture information.</param>
        /// <returns>description string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte? charge = value as byte?;

            if (charge != null)
            {
                return Constants.GetEnumDescription((Constants.BatteryLevel)charge);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Unused.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture information.</param>
        /// <returns>null.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
