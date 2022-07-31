using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.Messenging.Messages;
using UIKit;

namespace Debts.iOS.Core.VIews
{
    public class ToastViewBuilder 
    {
        private readonly Func<UIView> _rootViewProvider;

        public ToastViewBuilder(Func<UIView> rootViewProvider)
        {
            _rootViewProvider = rootViewProvider;
        }

        private const int ToastViewTag = 14154;
        private const double ToastDurationInMs = (3000-325-325)/1000.0;
        private const double ToastShowHideAnimationDurationInMs = 325/1000.0;
        
        public void Show(string content, ToastMvxMessage.ToastStyle style, Action dismissAction)
        {
            var rootVc = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (rootVc.PresentedViewController != null)
                rootVc = rootVc.PresentedViewController;
            
            var rootView = rootVc?.View;

            if (rootView == null || rootView.Subviews.Any(x => x.Tag == ToastViewTag))
                return;
                
            
            UIView toastView = new UIView()
            {
                Layer = {CornerRadius = 4},
                BackgroundColor = style == ToastMvxMessage.ToastStyle.Info ? AppColors.Primary : AppColors.AppRed,
                ClipsToBounds = true,
                Tag = ToastViewTag,
            };
                    
            UILabel toastContentLabel = new UILabel()
            {
                TextColor = UIColor.White,
                Text = content,
                Font = UIFont.SystemFontOfSize(14),
                Lines =  0
            };
            
            toastView.Add(toastContentLabel);
            
            var frameWhenToastNotVisible = toastView.Frame;
            frameWhenToastNotVisible.Width = rootView.Frame.Width - 36;
            frameWhenToastNotVisible.Height = 48;
            frameWhenToastNotVisible.X = 18;
            frameWhenToastNotVisible.Y = -frameWhenToastNotVisible.Height;

            toastView.Frame = frameWhenToastNotVisible;

            var frameWhenToastVisible = frameWhenToastNotVisible;
            frameWhenToastVisible.Y = 42;

            toastView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            toastView.AddConstraints(
                toastContentLabel.WithSameCenterY(toastView),
                toastContentLabel.AtLeftOf(toastView, 12),
                toastContentLabel.AtRightOf(toastView, 12)
            );
             

            toastView.Alpha=0;
            rootView.Add(toastView);
            
            
            UIView.Animate(ToastShowHideAnimationDurationInMs, 0, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                toastView.Alpha = 1;
                toastView.Frame = frameWhenToastVisible;
            }, async () =>
            {
                await Task.Delay((int)(ToastDurationInMs*1000));
                
                UIView.Animate(ToastShowHideAnimationDurationInMs, 0, UIViewAnimationOptions.CurveEaseOut, () =>
                {
                    toastView.Alpha = 0;
                    toastView.Frame = frameWhenToastNotVisible;
                }, () =>
                {
                    toastView.RemoveFromSuperview();
                    dismissAction();
                });
            });
        }
    
        
        public void ShowWithAction(string content, ToastMvxMessage.ToastStyle style, string undoText, Action dismissAction, Action undoCommand)
        {
        var rootVc = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (rootVc.PresentedViewController != null)
                rootVc = rootVc.PresentedViewController;
            
            var rootView = rootVc?.View;

            if (rootView == null || rootView.Subviews.Any(x => x.Tag == ToastViewTag))
                return;
                
            
            UIView toastView = new UIView()
            {
                Layer = {CornerRadius = 4},
                BackgroundColor = style == ToastMvxMessage.ToastStyle.Info ? AppColors.Primary : AppColors.AppRed,
                ClipsToBounds = true,
                Tag = ToastViewTag,
            };
                    
            UILabel toastContentLabel = new UILabel()
            {
                TextColor = UIColor.White,
                Text = content,
                Font = UIFont.SystemFontOfSize(14),
                Lines =  0
            };
            
            UIButton button = new UIButton(UIButtonType.Custom);
            button.Font = toastContentLabel.Font;
            button.SetTitle(undoText, UIControlState.Normal);
            button.SetTitleColor(UIColor.White, UIControlState.Normal);
            
            toastView.Add(toastContentLabel);
            toastView.Add(button);
            
            var frameWhenToastNotVisible = toastView.Frame;
            frameWhenToastNotVisible.Width = rootView.Frame.Width - 36;
            frameWhenToastNotVisible.Height = 48;
            frameWhenToastNotVisible.X = 18;
            frameWhenToastNotVisible.Y = -frameWhenToastNotVisible.Height;

            toastView.Frame = frameWhenToastNotVisible;

            var frameWhenToastVisible = frameWhenToastNotVisible;
            frameWhenToastVisible.Y = 42;

            toastView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            toastView.AddConstraints(
                toastContentLabel.WithSameCenterY(toastView),
                toastContentLabel.AtLeftOf(toastView, 12),
                toastContentLabel.ToLeftOf(button, 12),

                button.AtRightOf(toastView, 12),
                button.WithSameCenterY(toastView)
            );
            toastView.Alpha=0;
            rootView.Add(toastView);
            
            
            CancellationTokenSource tcs = new CancellationTokenSource();
            UIView.Animate(ToastShowHideAnimationDurationInMs, 0, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                toastView.Alpha = 1;
                toastView.Frame = frameWhenToastVisible;
            }, async () =>
            {
                bool isCanceled = false;
                try
                {
                    await Task.Delay((int) (ToastDurationInMs * 1000), tcs.Token);
                }
                catch (TaskCanceledException)
                {
                    isCanceled = true;
                }

                UIView.Animate(ToastShowHideAnimationDurationInMs, 0, UIViewAnimationOptions.CurveEaseOut, () =>
                    {
                        toastView.Alpha = 0;
                        toastView.Frame = frameWhenToastNotVisible;
                    }, () =>
                {
                    toastView.RemoveFromSuperview();
                    if (!isCanceled)
                        dismissAction();
                });

            });
            
            button.TouchUpInside += (e, a) =>
            {
                button.UserInteractionEnabled = false;
                tcs.Cancel();
                undoCommand();
            };
            
        }
    }
}