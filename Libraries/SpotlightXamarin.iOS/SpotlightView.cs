using System;
using System.Timers;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SpotlightXamarin.IOS
{
    public class SpotlightView : UIView
    {
        public delegate void OnTargetClosedHandler();
        public OnTargetClosedHandler OnTargetClosed;

        public delegate void OnTargetClickHandler();
        public OnTargetClickHandler OnTargetClicked;

        private CGPoint Point = new CGPoint();
        private UIViewController ParentController;
        private nfloat Radius = 0;
        private bool IsRunningAnimation = false;

        public SpotlightView(UIViewController parentController, CGRect frame) : base(frame)
        {
            ParentController = parentController;
            BackgroundColor = UIColor.Clear;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            var gctx = UIGraphics.GetCurrentContext();

            gctx.SetFillColor(UIColor.Black.ColorWithAlpha(0.8f).CGColor);
            gctx.FillRect(rect);

            if (Radius>=0)
            {
                gctx.SetBlendMode(CGBlendMode.Clear);
                UIColor.Clear.SetColor();

                var path = new CGPath();
                path.AddArc(Point.X, Point.Y, Radius, 0, (nfloat)(Math.PI * 2), false);
                gctx.AddPath(path);
                gctx.DrawPath(CGPathDrawingMode.Fill);
            }
        }

        public void TurnUp(nfloat x, nfloat y, nfloat radius, long duration)
        {
            Point = new CGPoint(x, y);

            var animValue = (radius / duration) * 16.6667f;

            Timer timer = new Timer(16.6667);
            timer.Start();
            timer.Elapsed += (sender, e) =>
            {
                if (Radius <= radius)
                {
                    Radius += animValue;
                    IsRunningAnimation = true;

                    ParentController.InvokeOnMainThread(() =>
                    {
                        SetNeedsDisplay();
                    });
                }
                else
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };

            timer.Disposed += (sender, e) => 
            {
                IsRunningAnimation = false;  
            };
        }

        public void TurnDown(nfloat radius, long duration)
        {
            var animValue = (radius / duration) * 16.6667f;

            Timer timer = new Timer(16.6667f);
            timer.Start();
            timer.Elapsed += (sender, e) =>
            {
                if (Radius >= 0)
                {
                    Radius -= animValue;
                    IsRunningAnimation = true;

                    ParentController.InvokeOnMainThread(() =>
                    {
                        SetNeedsDisplay();
                    });
                }
                else
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };

            timer.Disposed += (sender, e) => 
            {
                IsRunningAnimation = false;

                ParentController.InvokeOnMainThread(() =>
                {
                    OnTargetClosed?.Invoke();
                });
            };
        }

        public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            if (!IsRunningAnimation && Radius > 0)
            {
                OnTargetClicked?.Invoke();
            }
        }
    }
}
