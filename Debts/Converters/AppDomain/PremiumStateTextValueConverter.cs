using System;
using System.Globalization;
using Debts.Data;
using Debts.ViewModel;
using MvvmCross.Converters;

namespace Debts.Converters.AppDomain
{
    public class PremiumStateTextValueConverter : MvxValueConverter<MainViewModel, string>
    {
        protected override string Convert(MainViewModel value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.PremiumState == PremiumState.PremiumOwnership )
                return "Premium User" + Environment.NewLine + "Royal Premium Version";

            if (value.PremiumState == PremiumState.PremiumSubscription)
                return "Premium Subscription" + Environment.NewLine + "active monthly premium subscription";
            
            if (value.PremiumState == PremiumState.Trial)
                return "Free Trial" + Environment.NewLine + "valid for " + value.AmountOfDaysOfValidPremiumSubscription + " " + (value.AmountOfDaysOfValidPremiumSubscription > 1 ? "days" : "day");

            if (value.PremiumState == PremiumState.OneDayExtendedTrial)
                return "One-Day reward" + Environment.NewLine + "access to Premium features";

            if (value.PremiumState == PremiumState.Limited)
                return "Limited" + Environment.NewLine + "tap here for Premium!";
            
            throw new InvalidOperationException(value.PremiumState.ToString());
        }
    } 
}