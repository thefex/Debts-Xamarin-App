using System;
using System.Globalization;
using Debts.Data;
using Debts.iOS.Config;
using MvvmCross.Converters;
using UIKit;

namespace Debts.iOS.Core.ValueConverters
{
    public class BudgetCategoryTypeToBackgroundColor : MvxValueConverter<BudgetType, UIColor>
    {
        protected override UIColor Convert(BudgetType value, Type targetType, object parameter, CultureInfo culture) 
            => value == BudgetType.Expense ? AppColors.AppRed : AppColors.LoanColor;
    }
}