using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using xbca.Models;

namespace xbca.ValueConverters
{
    [ValueConversion(typeof(byte), typeof(string))]
    class ChargeValueToLevelConverter : IValueConverter
    {
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
