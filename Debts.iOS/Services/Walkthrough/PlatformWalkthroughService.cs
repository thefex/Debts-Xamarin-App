using CoreGraphics;
using Debts.iOS.ViewControllers.Base;
using UIKit;

namespace Debts.Droid.Services.Walkthrough
{
    public abstract class PlatformWalkthroughService
    {
        protected CGPoint GetRelativeCenterPosition(UIView view, UIViewController viewController)
        {
            var center = view.ConvertPointToView(viewController.View.Frame.Location, viewController.View);
            center.X += view.Frame.Width / 2f;
            center.Y += view.Frame.Height / 2f;
            
            return center;
        }

        protected virtual MainViewController GetMainViewController() => UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
    }
}