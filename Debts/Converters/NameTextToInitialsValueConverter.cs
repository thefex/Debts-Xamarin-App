using System;
using System.Globalization;
using Debts.Extensions;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class NameTextToInitialsValueConverter : MvxValueConverter<string, string>
    {
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value) ? "?" : value.GetInitials();
        }
    }
}