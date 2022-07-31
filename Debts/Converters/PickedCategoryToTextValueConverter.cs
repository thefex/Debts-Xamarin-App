using System;
using System.Globalization;
using Debts.Data;
using Debts.Resources;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class PickedCategoryToTextValueConverter : MvxValueConverter<BudgetCategory, string>
    {
        protected override string Convert(BudgetCategory value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Name ?? TextResources.Converters_PickedBudgetCategoryToTextValueConverter_TapToPickBudgetCategoryText;
        }
    }
}