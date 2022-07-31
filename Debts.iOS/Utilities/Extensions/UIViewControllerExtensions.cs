using System;
using UIKit;

namespace Debts.iOS.Utilities.Extensions
{
    public static class UIViewControllerExtensions
    {
        public static void AddCloseKeyboardOnTapHandlers(this UIViewController uiViewController,
            Func<UIView, bool> skipView = null, bool shouldSkipViewControllerView = true)
        {
            skipView = skipView ?? (view => false);
            uiViewController.View.ForEachView(view =>
            {
                if ((view == uiViewController.View && shouldSkipViewControllerView) ||
                    view is UIButton ||
                    view is UITextField ||
                    view is UIScrollView ||
                    skipView(view))
                    return;

                view.AddGestureRecognizer(new UITapGestureRecognizer(() => uiViewController.View.Window.EndEditing(true))
                    {CancelsTouchesInView = false});
            });
        }
    }
}