using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class BooleanNegationValueConverter : MvxValueConverter<bool>
    {
        protected override object Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}