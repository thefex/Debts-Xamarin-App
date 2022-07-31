using System;
using System.Globalization;
using Debts.Data;
using Debts.iOS.ViewModels;
using MvvmCross.Converters;

namespace Debts.iOS.Core.ValueConverters
{
    public class MenuViewModelToPremiumStateTextValueConverter : MvxValueConverter<MenuViewModel, string>
    {
        protected override string Convert(MenuViewModel value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.PremiumState == PremiumState.PremiumOwnership )
                return "Premium User" + Environment.NewLine + "Royal Premium Version";

            if (value.PremiumState == PremiumState.PremiumSubscription)
                return "Premium Subscription" + Environment.NewLine + "active monthly premium subscription";
            
            if (value.PremiumState == PremiumState.Trial)
                return "Regular Access" + Environment.NewLine + "without premium subscription";

            if (value.PremiumState == PremiumState.OneDayExtendedTrial)
                return "One-Day reward" + Environment.NewLine + "access to Premium features";

            if (value.PremiumState == PremiumState.Limited)
                return "Limited" + Environment.NewLine + "tap here for Premium!";
            
            throw new InvalidOperationException(value.PremiumState.ToString());
        }
        
    }
}