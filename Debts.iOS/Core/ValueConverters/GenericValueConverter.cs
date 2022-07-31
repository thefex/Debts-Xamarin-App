using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Debts.iOS.Core.ValueConverters
{
    public class GenericValueConverter<TFrom, TTarget> : MvxValueConverter<TFrom, TTarget>
    {
        private readonly Func<TFrom, TTarget> _converter;

        public GenericValueConverter(Func<TFrom, TTarget> converter)
        {
            _converter = converter;
        }
        protected override TTarget Convert(TFrom value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converter(value);
        }
    }
}