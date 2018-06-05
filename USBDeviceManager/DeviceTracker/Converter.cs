using System;
using System.Windows.Data;
using DeviceTracker.Common;

namespace DeviceTracker
{
    public class EnumToBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            var check = value.Equals(DeviceImportOption.Always);

            return check;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           return null;
        }
    }
}
