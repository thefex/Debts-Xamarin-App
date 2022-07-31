using System;
using System.Globalization;
using Android.Graphics;
using Debts.Data;
using Debts.Model;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class InitialsDarkColorProviderValueConverter : MvxValueConverter<ContactDetails>
    {
        protected override object Convert(ContactDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.ParseColor("#a4a4a4");
        }
    }
}