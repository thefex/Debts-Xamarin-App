using System;
using System.Globalization;
using Debts.Data;
using Debts.Model;
using Humanizer;
using MvvmCross.Converters;

namespace Debts.iOS.Core.ValueConverters
{
    public class FinanceGroupKeyValueConverter : MvxValueConverter<FinancesOperationGroup, string>
    {
        protected override string Convert(FinancesOperationGroup value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GroupType is FinanceGroupType groupType)
            {
                if (groupType == FinanceGroupType.DeadlinePassed)
                    return "Payment deadline exceed";

                if (groupType == FinanceGroupType.Paid)
                    return "Paid-off";
            }

            var date = (DateTime) value.ForDate;
            if (date.Date == DateTime.UtcNow.Date)
                return "today";

            try
            {
                return new DateTimeOffset(date).Humanize();
            }
            catch (Exception)
            {
                return new DateTimeOffset(date).Humanize(culture: new CultureInfo("en-us"));
            }
        }
    } 
}