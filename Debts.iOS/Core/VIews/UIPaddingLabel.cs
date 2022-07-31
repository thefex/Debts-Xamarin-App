using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Debts.iOS.Core.VIews
{
    public class UIPaddingLabel : UILabel
    {
        public UIPaddingLabel()
        {
        }

        public UIPaddingLabel(NSCoder coder) : base(coder)
        {
        }

        protected UIPaddingLabel(NSObjectFlag t) : base(t)
        {
        }

        protected internal UIPaddingLabel(IntPtr handle) : base(handle)
        {
        }

        public UIEdgeInsets Padding { get; set; } = UIEdgeInsets.Zero;
 
        public override void DrawText(CGRect rect)
        {
            rect = Padding.InsetRect(rect);
            base.DrawText(rect);
        }

        public override CGSize IntrinsicContentSize
        {
            get
            {
                var intrinsicContentSize = base.IntrinsicContentSize;
                intrinsicContentSize.Width += Padding.Left + Padding.Right;

                var text = Text ?? string.Empty;
                var newHeight = text.StringSize(Font, new CGSize(intrinsicContentSize.Width, nfloat.MaxValue), LineBreakMode).Height;
                intrinsicContentSize.Height = (int)Math.Ceiling(newHeight) + Padding.Top + Padding.Bottom;
                return intrinsicContentSize;
            }
        }
        
    }
}