using System;
using System.Globalization;
using Debts.Data;
using Debts.iOS.Config;
using MvvmCross.Converters;
using UIKit;

namespace Debts.iOS.Core.ValueConverters
{
    public class FinanceOperationTypeToBackgroundColor : MvxValueConverter<FinanceOperationType, UIColor>
    {
        protected override UIColor Convert(FinanceOperationType value, Type targetType, object parameter, CultureInfo culture) 
            => value == FinanceOperationType.Debt ? AppColors.AppRed : AppColors.LoanColor;
    }
}