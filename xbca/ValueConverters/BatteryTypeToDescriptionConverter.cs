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
    /// Represents the converter that converts battery type value to its description.
    /// </summary>
    [ValueConversion(typeof(byte), typeof(string))]
    public class BatteryTypeToDescriptionConverter : IValueConverter
    {
        /// <summary>
        /// Converts battery type value to description string.
        /// </summary>
        /// <param name="value">Battery type value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture information.</param>
        /// <returns>description string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte? type = value as byte?;

            if (type != null)
            {
                return Constants.GetEnumDescription((Constants.BatteryTypes)type);
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
