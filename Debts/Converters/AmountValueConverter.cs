using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class AmountValueConverter : MvxValueConverter<decimal, string>
    {
        protected override string Convert(decimal value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value > 999)
                return "$999+";

            return "$" + value;
        }
    } 
}