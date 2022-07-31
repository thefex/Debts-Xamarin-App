using System;
using System.Globalization;
using Humanizer;
using Humanizer.Localisation;
using MvvmCross.Converters;

namespace Debts.Converters.AppDomain
{
    public class DurationForNoteValueConverter : MvxValueConverter<TimeSpan?, string>
    {
        protected override string Convert(TimeSpan? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!value.HasValue || Math.Abs(value.Value.TotalSeconds) < double.Epsilon)
                return string.Empty;
            
            var valueSeconds = value.Value;

            try
            {
                return valueSeconds.Humanize(precision: 3, minUnit: TimeUnit.Second, maxUnit: TimeUnit.Hour);   
            } catch (Exception){
                return valueSeconds.Humanize(precision: 3, minUnit: TimeUnit.Second, maxUnit: TimeUnit.Hour, culture: CultureInfo.GetCultureInfo("en-us"));    
            }
        }
    }
}