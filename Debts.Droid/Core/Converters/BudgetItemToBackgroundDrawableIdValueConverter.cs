using System;
using System.Globalization;
using Debts.Data;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class BudgetItemToBackgroundDrawableIdValueConverter : MvxValueConverter<BudgetItem, string>
    {
        protected override string Convert(BudgetItem value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.IsExpense ? "round_text_warning" : "round_text_info";
        }
    }
}