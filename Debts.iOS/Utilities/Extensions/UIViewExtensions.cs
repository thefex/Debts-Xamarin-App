using System;
using CoreGraphics;
using UIKit;

namespace Debts.iOS.Utilities.Extensions
{
    public static class UIViewExtensions
    {
        public static void AddShadow(this UIView view)
        {
            view.Layer.ShadowColor = UIColor.Black.CGColor;
            view.Layer.ShadowRadius = 2;
            view.Layer.ShadowOpacity = 0.2f;
            view.Layer.ShadowOffset = new CGSize(2, 2);
            view.Layer.MasksToBounds = false;
        }
        
        public static void ForEachView(this UIView view, Action<UIView> actionToInvoke)
        {
            if (view == null)
                return;

            actionToInvoke(view);

            if (view.Subviews == null)
                return;

            foreach (var subView in view.Subviews)
                ForEachView(subView, actionToInvoke);
        }
    }
}