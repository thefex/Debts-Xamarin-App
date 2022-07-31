using System;
using System.Globalization;
using Debts.Data;
using Debts.iOS.Config;
using MvvmCross.Converters;
using UIKit;

namespace Debts.iOS.Core.ValueConverters
{
    public class PaymentDateToTintColorValueConverter : MvxValueConverter<PaymentDetails, UIColor>
    {
        protected override UIColor Convert(PaymentDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.PaymentDate.HasValue)
                return AppColors.SuccessColor;

            if (DateTime.UtcNow > value.DeadlineDate)
                return AppColors.AppRed;

            return AppColors.AppInfoOrange;
        }
    }
}