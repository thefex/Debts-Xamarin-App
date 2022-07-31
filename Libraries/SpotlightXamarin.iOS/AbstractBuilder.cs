using System;
using CoreGraphics;
using UIKit;

namespace SpotlightXamarin.IOS
{
    public abstract class AbstractBuilder<T, S>
    {
        protected UIViewController ParentController;
        protected nfloat StartX = 0f;
        protected nfloat StartY = 0f;
        protected nfloat Radius = 100f;

        protected abstract T Self();
        public abstract S Build();

        protected AbstractBuilder(UIViewController controller)
        {
            ParentController = controller;
        }

        public T SetPoint(nfloat x, nfloat y)
        {
            StartX = x;
            StartY = y;

            return Self();
        }

        public T SetPoint(CGPoint point)
        {
            return SetPoint(point.X, point.Y);
        }

        public T SetPoint(UIView view)
        {
            nfloat x = view.Frame.X + view.Frame.Width / 2;
            nfloat y = view.Frame.Y + view.Frame.Height / 2;

            return SetPoint(x, y);
        }

        public T SetRadius(float radius)
        {
            if (radius <= 0)
            {
                throw new Exception("Spotlight: Radius must be greater than 0");
            }

            Radius = radius;
            return Self();
        }
    }
}
