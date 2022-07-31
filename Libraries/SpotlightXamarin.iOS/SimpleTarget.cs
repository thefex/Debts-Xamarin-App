using System;
using CoreGraphics;
using UIKit;

namespace SpotlightXamarin.IOS
{
    public class SimpleTarget : Target
    {
        public SimpleTarget(CGPoint point, nfloat radius, UIView view)
        {
            Point = point;
            Radius = radius;
            View = view;
        }
    }

    public class SimpleTargetBuilder : AbstractBuilder<SimpleTargetBuilder, SimpleTarget>
    {
        enum ContainerPosition
        {
            Above = 0,
            Below = 1
        }

        private string Title;
        private string Description;
        private SpotlightContainerView SpotlightContainerView;

        public SimpleTargetBuilder(UIViewController controller) : base(controller)
        {
        }

        public override SimpleTarget Build()
        {
            if (ParentController == null)
            {
                throw new Exception("context is null");
            }

            SpotlightContainerView = new SpotlightContainerView(ParentController.View.Bounds);

            SpotlightContainerView.TitleLabel.Text = Title;
            SpotlightContainerView.DescriptionLabel.Text = Description;

            SpotlightContainerView.UpdateView();

            CGPoint point = new CGPoint(StartX, StartY);
            CalculatePosition(point, Radius, SpotlightContainerView);

            return new SimpleTarget(point, Radius, SpotlightContainerView);
        }

        protected override SimpleTargetBuilder Self()
        {
            return this;
        }

        public SimpleTargetBuilder SetTitle(string title)
        {
            Title = title;
            return this;
        }

        public SimpleTargetBuilder SetDescription(string description)
        {
            Description = description;
            return this;
        }

        private void CalculatePosition(CGPoint point, nfloat radius, UIView spotlightView)
        {
            CGRect screenSize = UIScreen.MainScreen.Bounds;

            nfloat aboveArea = point.Y / screenSize.Height;
            nfloat belowArea = (screenSize.Height - point.Y) / screenSize.Height;

            ContainerPosition containerPosition;

            if (aboveArea > belowArea)
            {
                containerPosition = ContainerPosition.Above;
            }
            else
            {
                containerPosition = ContainerPosition.Below;
            }

            UIView layout = (spotlightView as SpotlightContainerView).Container;

            switch (containerPosition)
            {
                case ContainerPosition.Above:
                    layout.Frame = new CGRect(layout.Bounds.X, point.Y - radius - 50 - layout.Bounds.Height, layout.Bounds.Width, layout.Bounds.Height);
                    break;
                case ContainerPosition.Below:
                    layout.Frame = new CGRect(layout.Bounds.X, point.Y + radius + 50, layout.Bounds.Width, layout.Bounds.Height);
                    break;
            }

            spotlightView.LayoutSubviews();
        }
    }
}
