using System;
using System.Globalization;
using System.Net.Mime;
using Debts.Data;
using Debts.Resources;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class PickedContactToTextValueConverter : MvxValueConverter<ContactDetails, string>
    {
        protected override string Convert(ContactDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? TextResources.Converters_PickedContactToTextValueConverter_TapToPickContactText;
        }
    }
}