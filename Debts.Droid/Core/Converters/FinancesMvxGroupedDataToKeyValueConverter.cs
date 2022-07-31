using System;
using System.Globalization;
using Debts.Model;
using Humanizer;
using MvvmCross.AdvancedRecyclerView.Data;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class FinancesMvxGroupedDataToKeyValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as MvxGroupedData;
        
            if (val.Key is FinanceGroupType groupType)
            {
                if (groupType == FinanceGroupType.DeadlinePassed)
                    return "Payment deadline exceed";

                if (groupType == FinanceGroupType.Paid)
                    return "Paid-off";
            }

            var date = (DateTime) val.Key;
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}