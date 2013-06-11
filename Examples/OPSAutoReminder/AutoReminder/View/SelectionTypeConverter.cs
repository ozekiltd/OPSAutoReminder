using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AutoReminder.Model;

namespace AutoReminder.View
{
    class SelectionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = parameter as string;

            if (parameterString == null || value.Equals(false))
                return Binding.DoNothing;

            return Enum.Parse(targetType, parameterString);
        }
    }
}
