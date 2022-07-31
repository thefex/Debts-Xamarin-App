using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Debts.Messenging.Messages;

namespace Debts.Droid.Core.ViewControllers
{
    public class CustomSnackController
    {
        private ViewGroup _snackbarContainer;
        public void Initialize(ViewGroup snackbarContainer)
        {
            _snackbarContainer = snackbarContainer;
            snackbarContainer.Post(() =>
            {
                snackbarContainer.TranslationY = -snackbarContainer.Height;
                snackbarContainer.Alpha = 0;
            });
        }

        public async Task Show(string text, ToastMvxMessage.ToastStyle style, ICommand dismissCommand, bool needExtraMargin = false, string actionText = null, ICommand actionCommand = null)
        {
            var layoutParams = _snackbarContainer.LayoutParameters as ViewGroup.MarginLayoutParams;
            int dpValue = needExtraMargin ? 64 : 12;
            dpValue = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, dpValue, _snackbarContainer.Context.Resources.DisplayMetrics);

            int leftRightDpValue = needExtraMargin ? 24 : 12;
            leftRightDpValue = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, leftRightDpValue, _snackbarContainer.Context.Resources.DisplayMetrics);

            
            layoutParams.SetMargins(leftRightDpValue, dpValue, leftRightDpValue, 0);
            _snackbarContainer.LayoutParameters = layoutParams;
            
            _snackbarContainer.SetBackgroundResource(
                    style == ToastMvxMessage.ToastStyle.Info ?
                        Resource.Drawable.material_v2_snackbar_info_background : Resource.Drawable.material_v2_snackbar_error_background
                );
            
            var snackBarTextView = _snackbarContainer.FindViewById<TextView>(Resource.Id.custom_snackbar_text);
            snackBarTextView.Text = text;

            var snackBarActionButton =
                _snackbarContainer.FindViewById<MaterialButton>(Resource.Id.custom_snackbar_action);

            if (string.IsNullOrEmpty(actionText))
            {
                snackBarActionButton.Visibility = ViewStates.Gone;
            }
            else
            {
                snackBarActionButton.Visibility = ViewStates.Visible;
                snackBarActionButton.Text = actionText;
                snackBarActionButton.SetOnClickListener(new ClickListener(() =>
                {
                    snackBarActionButton.Visibility = ViewStates.Gone;
                    _snackbarContainer.Animate()
                        .Alpha(0)
                        .SetDuration(375)
                        .TranslationY(-_snackbarContainer.Height)
                        .SetInterpolator(new AccelerateInterpolator(2f))
                        .Start();
                    
                    actionCommand?.Execute(null);
                }));
            }

            IsShown = true;
            
            _snackbarContainer.Animate()
                .Alpha(1)
                .SetDuration(375)
                .TranslationY(0)
                .SetInterpolator(new AccelerateInterpolator(2f))
                .Start();

            await Task.Delay(375 + 1500);

            _snackbarContainer.Animate()
                .Alpha(0)
                .SetDuration(375)
                .TranslationY(-_snackbarContainer.Height)
                .SetInterpolator(new AccelerateInterpolator(2f))
                .Start();

            IsShown = false;
        }
        
        public bool IsShown { get; set; }
        
        class ClickListener : Java.Lang.Object, View.IOnClickListener 
        {
            private readonly Action _actionListener;

            public ClickListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            internal ClickListener(Action actionListener)
            {
                _actionListener = actionListener;
            }

            public void OnClick(View v)
            {
                _actionListener();
            }
        }
    }
}