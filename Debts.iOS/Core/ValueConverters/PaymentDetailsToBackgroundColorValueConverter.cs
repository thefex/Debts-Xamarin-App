using System;
using System.Globalization;
using Debts.Data;
using Debts.iOS.Config;
using MvvmCross.Converters;
using UIKit;

namespace Debts.iOS.Core.ValueConverters
{
    public class PaymentDetailsToBackgroundColorValueConverter : MvxValueConverter<PaymentDetails, UIColor>
    {
        protected override UIColor Convert(PaymentDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.PaymentDate.HasValue)
                return AppColors.SuccessColor;

            if (value.DeadlineDate <= DateTime.UtcNow)
                return AppColors.AppRed;

            return AppColors.AppInfoOrange; 
        }
    }
}