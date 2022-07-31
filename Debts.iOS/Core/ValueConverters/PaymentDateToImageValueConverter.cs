using System;
using System.Globalization;
using Debts.Data;
using MvvmCross.Converters;
using UIKit;

namespace Debts.iOS.Core.ValueConverters
{
    public class PaymentDateToImageValueConverter : MvxValueConverter<PaymentDetails, UIImage>
    {
        protected override UIImage Convert(PaymentDetails value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.PaymentDate.HasValue)
                return UIImage.FromBundle("face_happy").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

            if (DateTime.UtcNow > value.DeadlineDate)
                return UIImage.FromBundle("face_sad").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

            return UIImage.FromBundle("face_neutral").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        }
    }
}