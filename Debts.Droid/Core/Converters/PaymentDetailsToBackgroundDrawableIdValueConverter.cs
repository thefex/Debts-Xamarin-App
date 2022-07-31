using System;
using System.Globalization;
using Debts.Data;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class PaymentDetailsToBackgroundDrawableIdValueConverter : MvxValueConverter<PaymentDetails, string>
    {
        protected override string Convert(PaymentDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.PaymentDate.HasValue)
                return "round_text_success";

            if (value.DeadlineDate <= DateTime.UtcNow)
                return "round_text_warning";

            return "round_text_info";
        }
    }
}