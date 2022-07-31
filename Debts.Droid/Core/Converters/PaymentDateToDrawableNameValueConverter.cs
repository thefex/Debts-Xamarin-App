using System;
using System.Globalization;
using Debts.Data;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class PaymentDateToDrawableNameValueConverter : MvxValueConverter<PaymentDetails, string>
    {
        protected override string Convert(PaymentDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.PaymentDate.HasValue)
                return "finance_success";

            if (DateTime.UtcNow > value.DeadlineDate)
                return "finance_alert";

            return "finance_neutral";
        }
    }
}