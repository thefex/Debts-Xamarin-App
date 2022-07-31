using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SpotlightXamarin.IOS
{
    public class SpotlightContainerView:UIView
    {
        public UIView Container { get; set; }
        public UILabel TitleLabel { get; set; }
        public UILabel DescriptionLabel { get; set; }

        public SpotlightContainerView(CGRect frame) : base(frame)
        {
            Container = new UIView();

            TitleLabel = new UILabel()
            {
                TextColor = UIColor.White,
                Font = UIFont.BoldSystemFontOfSize(24f),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            DescriptionLabel = new UILabel()
            {
                TextColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(18f),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };


            Container.Add(TitleLabel);
            Container.Add(DescriptionLabel);
            Add(Container);
        }

        public void UpdateView()
        {
            CGSize maxSize = new CGSize(Frame.Width - 100, nfloat.MaxValue);

            var titleSize = new NSString(TitleLabel.Text).StringSize(TitleLabel.Font, maxSize, TitleLabel.LineBreakMode);
            var descriptionSize = new NSString(DescriptionLabel.Text).StringSize(DescriptionLabel.Font, maxSize, DescriptionLabel.LineBreakMode);


            TitleLabel.Frame = new CGRect(50, 0, titleSize.Width, titleSize.Height);
            DescriptionLabel.Frame = new CGRect(50, titleSize.Height + 16, descriptionSize.Width, descriptionSize.Height);

            var containerWidth = (titleSize.Width > descriptionSize.Width) ? titleSize.Width : descriptionSize.Width;
            var containerHeight = titleSize.Height + descriptionSize.Height + 16;

            Container.Frame = new CGRect(0, 0, containerWidth, containerHeight);

            Frame = new CGRect(0, 0, containerWidth, containerHeight);
        }
    }
}
