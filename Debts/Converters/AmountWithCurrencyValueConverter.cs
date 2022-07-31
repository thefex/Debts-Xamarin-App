using System;
using System.Globalization;
using Debts.Data;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class AmountWithCurrencyValueConverter : MvxValueConverter<PaymentDetails, string>
    {
        protected override string Convert(PaymentDetails value, Type targetType, object parameter, CultureInfo culture) 
            => value.Amount + " " + value.Currency;
    }
}