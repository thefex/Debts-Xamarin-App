using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Debts.iOS.Core.ValueConverters
{
    public class BooleanToMethodProviderValueConverter<TTarget> : MvxValueConverter<bool, TTarget>
    {
        private readonly Func<TTarget> _onFalse;
        private readonly Func<TTarget> _onTrue;

        public BooleanToMethodProviderValueConverter(Func<TTarget> onFalse, Func<TTarget> onTrue)
        {
            _onFalse = onFalse;
            _onTrue = onTrue;
        }

        protected override TTarget Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ? _onTrue() : _onFalse();
        }
    }
}