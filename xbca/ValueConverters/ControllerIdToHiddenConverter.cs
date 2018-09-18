namespace Xbca.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Represents the converter that converts controller id value to a bool indicating whether the controller is going to be displayed or not.
    /// </summary>
    [ValueConversion(typeof(int), typeof(bool))]
    public class ControllerIdToHiddenConverter : IValueConverter
    {
        /// <summary>
        /// Converts controller id value to a bool indicating whether the controller is going to be displayed or not.
        /// </summary>
        /// <param name="value">Controller id.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture information.</param>
        /// <returns><c>true</c>if the controller is present; otherwise, <c>false</c>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? id = value as int?;

            if (id != null && id >= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Unused.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture information.</param>
        /// <returns>-1.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return -1;
        }
    }
}
