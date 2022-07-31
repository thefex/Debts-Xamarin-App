using System;
using System.Globalization;
using Debts.Data;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class PremiumStateToPremiumIconValueConverter : MvxValueConverter<PremiumState, string>
    {
        protected override string Convert(PremiumState value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == PremiumState.Trial || value == PremiumState.Limited)
                return "star_outline";

            if (value == PremiumState.OneDayExtendedTrial)
                return "star_half";

            if (value == PremiumState.PremiumSubscription)
                return "star";

            if (value == PremiumState.PremiumOwnership)
                return "crown";
            
            throw new InvalidOperationException(value.ToString());
        }
    }
}