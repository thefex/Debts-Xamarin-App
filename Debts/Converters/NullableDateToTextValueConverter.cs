using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Debts.Converters
{
    public class NullableDateToTextValueConverter : MvxValueConverter<DateTime?, string>
    {
        protected override string Convert(DateTime? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return EmptyValueText;

            return value.Value.ToString("D");
        }

        public string EmptyValueText { get; set; } = string.Empty;
    }
}