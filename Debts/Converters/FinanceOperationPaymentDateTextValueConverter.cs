using System;
using System.Globalization;
using Debts.Data;
using Debts.Resources;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class FinanceOperationPaymentDateTextValueConverter : MvxValueConverter<PaymentDetails, string>
    {
        protected override string Convert(PaymentDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!value.PaymentDate.HasValue)
                return TextResources.Converters_FinanceOperationPaymentDateTextValueConverter_PaymentHasNotBeenMadeText;

            return value.PaymentDate.Value.ToString("D");
        }
    }
}