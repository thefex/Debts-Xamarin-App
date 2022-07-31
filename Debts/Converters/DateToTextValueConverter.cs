using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class DateToTextValueConverter : MvxValueConverter<DateTime, string>
    {
        protected override string Convert(DateTime value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString("D");
        }
    }
}