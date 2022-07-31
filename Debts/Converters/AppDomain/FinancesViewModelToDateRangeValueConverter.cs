using System;
using System.Globalization;
using Debts.Resources;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Converters;

namespace Debts.Converters.AppDomain
{
    public class FinancesViewModelToDateRangeValueConverter : MvxValueConverter<DateTime?[], string>
    {
        protected override string Convert(DateTime?[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!value[0].HasValue || !value[1].HasValue)
                return string.Empty;

            string returnText = TextResources
                .Converters_AppDomain_FinancesViewModelToDateRangeValueConverter_ForDatePeriod
                .Replace("$DATE$", $"{value[0].Value:d} - {value[1].Value:d}");

            return returnText;
        }
    }
}