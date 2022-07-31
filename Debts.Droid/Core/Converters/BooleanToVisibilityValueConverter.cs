using System;
using System.Globalization;
using Android.Views;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class BooleanToVisibilityValueConverter : MvxValueConverter<bool, ViewStates>
    {
        protected override ViewStates Convert(bool value, Type targetType, object parameter, CultureInfo culture) => value ? ViewStates.Visible : ViewStates.Gone;
    }
}