using System;
using System.Globalization;
using Humanizer;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class HumanizedDateValueConverter : MvxValueConverter<DateTime, string>
    {
        protected override string Convert(DateTime value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Date == DateTime.UtcNow.Date)
                return "today";

            try
            {
                return new DateTimeOffset(value).Humanize();
            }
            catch (Exception)
            {
                return new DateTimeOffset(value).Humanize(culture: new CultureInfo("en-us"));
            }
        }
    }

    public class HumanizedDateFullValueConverter : MvxValueConverter<DateTime, string>
    {
        protected override string Convert(DateTime value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return new DateTimeOffset(value).Humanize();
            }
            catch (Exception)
            {
                return new DateTimeOffset(value).Humanize(culture: new CultureInfo("en-us"));
            }
        }
    }
}