using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views.InputMethods;
using Android.Widget;
using Debts.Droid.Core.Extensions.Wrappers;

namespace Debts.Droid.Core.Extensions
{
    public static class ActivityExtensions
    {
        public static Activity HideKeyboard(this Activity activity)
        {
            var windowToken = activity?.CurrentFocus?.WindowToken;
            if (windowToken != null)
            {
                activity.GetSystemService(Context.InputMethodService)
                    .JavaCast<InputMethodManager>()
                    .HideSoftInputFromWindow(windowToken, HideSoftInputFlags.None);
                activity.CurrentFocus.ClearFocus();
            }

            return activity;
        }
        public static Activity ShowQuestionDialog(this Activity activity, string title, string content, Action onOk, Action onCancel, string cancelText, string okText)
        {
            bool hasCancelOrOkBeenCalled = false;
            
            new Android.Support.V7.App.AlertDialog.Builder(activity)
                .SetIcon(Resource.Mipmap.app_icon)
                .SetTitle(title)
                .SetMessage(content)
                .SetPositiveButton(okText, (e, a) =>
                {
                    hasCancelOrOkBeenCalled = true;
                    onOk?.Invoke(); 
                })
                .SetNegativeButton(cancelText, (e, a) =>
                {
                    hasCancelOrOkBeenCalled = true;
                    onCancel?.Invoke();
                }) 
                .SetOnDismissListener(new DismissActionListenerForDialogWrapper(() =>
                {
                    if (!hasCancelOrOkBeenCalled)
                        onCancel?.Invoke();
                }))
                .Show();

            return activity;
        }
        
        public static Activity ShowMessageDialog(this Activity activity, string title, string content, Action onDismiss = null)
        {
            
            new Android.Support.V7.App.AlertDialog.Builder(activity)
                .SetIcon(Resource.Mipmap.app_icon)
                .SetTitle(title)
                .SetMessage(content)
                .SetPositiveButton("ok", (sender, args) => {})
                .SetOnDismissListener(new DismissActionListenerForDialogWrapper(() =>
                {
                    onDismiss?.Invoke();
                }))
                .Show();

            return activity;
        }
        
        public static Activity ShowQuestionDialog(this Activity activity, string title, string content, string checkBoxMessage, Action<bool> onOk, Action<bool> onCancel)
        {
            bool hasCancelOrOkBeenCalled = false;

            var view = activity.LayoutInflater.Inflate(Resource.Layout.dialog_checkbox, null);
            var checkBox = view.FindViewById<CheckBox>(Resource.Id.checkbox);
            checkBox.Text = checkBoxMessage;

            new Android.Support.V7.App.AlertDialog.Builder(activity)
                .SetIcon(Resource.Mipmap.app_icon)
                .SetTitle(title)
                .SetMessage(content)
                .SetView(view)
                .SetPositiveButton("ok", (e, a) =>
                {
                    hasCancelOrOkBeenCalled = true;
                    onOk?.Invoke(checkBox.Checked); 
                })
                .SetNegativeButton("no, thanks", (e, a) =>
                {
                    hasCancelOrOkBeenCalled = true;
                    onCancel?.Invoke(checkBox.Checked);
                }) 
                .SetOnDismissListener(new DismissActionListenerForDialogWrapper(() =>
                {
                    if (!hasCancelOrOkBeenCalled)
                        onCancel?.Invoke(checkBox.Checked);
                }))
                .Show();

            return activity;
        }
        
    }
}