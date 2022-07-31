using System;
using CoreGraphics;
using UIKit;

namespace Debts.iOS.Core.VIews
{
    public class SlideViewKeyboardVisibilityManager : KeyboardVisibilityManager
    {
        private readonly Func<UIView> _view;
        private readonly float _offset;
        private CGRect? orginalViewFrame;

        public SlideViewKeyboardVisibilityManager(Func<UIView> view, float offset = -90)
        {
            _offset = offset;
            _view = view;
        }
        
        protected override void OnKeyboardHidden(KeyboardData data)
        {
            var viewToSlide = _view();
            if (!orginalViewFrame.HasValue)
                orginalViewFrame = viewToSlide.Frame;

            UIView.Animate(0.3f, () =>
            {
                var destinationFrame = new CGRect(orginalViewFrame.Value.X, orginalViewFrame.Value.Y, viewToSlide.Frame.Width,
                    viewToSlide.Frame.Height);
                viewToSlide.Frame = destinationFrame;
            });
        }

        protected override void OnKeyboardShown(KeyboardData data)
        {
            var viewToSlide = _view();

            if (!orginalViewFrame.HasValue)
                orginalViewFrame = viewToSlide.Frame;

            UIView.Animate(0.3f, () =>
            {
                var destinationFrame = new CGRect(orginalViewFrame.Value.X, orginalViewFrame.Value.Y+_offset, viewToSlide.Frame.Width,
                    viewToSlide.Frame.Height);
                viewToSlide.Frame = destinationFrame;
            });
        }
    }
}