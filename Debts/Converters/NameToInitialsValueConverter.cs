using System;
using System.Globalization;
using Debts.Data;
using Debts.Extensions;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class NameToInitialsValueConverter : MvxValueConverter<ContactDetails, string>
    {
        public NameToInitialsValueConverter()
        {
        }

        protected override string Convert(ContactDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "?";

            return value.ToString().GetInitials();
        } 
    }
}