using System;
using System.Drawing;
using Foundation;
using UIKit;

namespace Debts.iOS.Core.VIews
{
    public abstract class KeyboardVisibilityManager : IDisposable
    {
        private readonly NSObject _keyboardShowObserver;
        private readonly NSObject _keyboardHideObserver;

        public KeyboardVisibilityManager()
        {
            _keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyboardShown);
            _keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyboardHidden);
        }

        protected void KeyboardHidden(NSNotification notification)
        {
            OnKeyboardHidden(CreateKeyboardToogleEventArgs(notification));
        }

        protected abstract void OnKeyboardHidden(KeyboardData data);

        protected void KeyboardShown(NSNotification notification)
        {
            OnKeyboardShown(CreateKeyboardToogleEventArgs(notification));
        }

        protected abstract void OnKeyboardShown(KeyboardData data);

        protected class KeyboardData
        {
            public double AnimationDuration { get; set; }
            public Rectangle Size { get; set; }
        }

        private static KeyboardData CreateKeyboardToogleEventArgs(NSNotification notification)
        {
            var endFrame = UIKeyboard.FrameEndFromNotification(notification);
            var nsKeyboardBounds = (NSValue)notification.UserInfo.ObjectForKey(UIKeyboard.FrameEndUserInfoKey);
            var nsKeyboardAnimationDuration =
                (NSNumber)notification.UserInfo.ObjectForKey(UIKeyboard.AnimationDurationUserInfoKey);
            var keyboardBounds = nsKeyboardBounds.RectangleFValue;
            
            var keyboardAnimationDuration = nsKeyboardAnimationDuration.DoubleValue;
            return new KeyboardData()
            {
                Size = new Rectangle(0, 0, (int) keyboardBounds.Width, (int) keyboardBounds.Height),
                AnimationDuration = keyboardAnimationDuration
            };
        }

        public void Dispose()
        {
            if (_keyboardHideObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
            }
            if (_keyboardShowObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
            }
        }
    }
}