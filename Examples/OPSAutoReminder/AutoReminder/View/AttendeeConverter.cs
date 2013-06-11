using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using AutoReminder.Model;

namespace AutoReminder.View
{
    class AttendeeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sb = new StringBuilder();

            foreach (var attendee in (IEnumerable<Attendee>)value)
            {
                sb.Append(string.Format("{0} ({1})", attendee.FullName, attendee.EmailAddresses.FirstOrDefault()));
                sb.Append("; ");
            }

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
