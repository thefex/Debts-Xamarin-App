using System;
using System.Globalization;
using Debts.Data;
using Debts.ViewModel.Contacts;
using Humanizer;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class PaymentDetailsToTextValueConverter : MvxValueConverter<PaymentDetails, string>
    {
        public PaymentDetailsToTextValueConverter()
        {
                
        }

        protected override string Convert(PaymentDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.PaymentDate.HasValue)
                return value.PaymentDate.Value.ToString("d");

            try
            {
                return new DateTimeOffset(value.DeadlineDate).Humanize();
            }
            catch (Exception)
            {
                return new DateTimeOffset(value.DeadlineDate).Humanize(culture: new CultureInfo("en-us"));
            }
        }
    }
}