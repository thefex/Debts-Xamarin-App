using System;
using System.Globalization;
using Debts.Data;
using Debts.Services.AppGrowth;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class FinanceOperationBubbleValueConverter : MvxValueConverter<FinanceOperation, string>
    {
        protected override string Convert(FinanceOperation value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.IsDebt ? "bubble_debt" : "bubble_loan";
        }
    }
}