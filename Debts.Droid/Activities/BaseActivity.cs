using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Messenging.Observers;
using Debts.Droid.Services;
using Debts.Messenging;
using Debts.Services;
using Debts.Services.Phone;
using Debts.ViewModel;
using MvvmCross;
using MvvmCross.Platforms.Android.Core;
using Plugin.Permissions;

namespace Debts.Droid.Activities
{
    public abstract class BaseApplicationMvxActivity<TViewModel, TInitParams> :
        MvvmCross.Droid.Support.V7.AppCompat.MvxAppCompatActivity<TViewModel> where TViewModel : BaseViewModel<TInitParams> where TInitParams : class
    {
        protected MvxMessageObserversController MessageObservers;

        public BaseApplicationMvxActivity()
        {
        }

        public BaseApplicationMvxActivity(IntPtr ptr, JniHandleOwnership ownership) : base(ptr, ownership)
        {
        }

        public abstract int LayoutId { get; }

        public virtual int LayoutRootId => Resource.Id.LayoutRoot;

        protected sealed override async void OnCreate(Bundle bundle)
        {
            var setup = MvxAndroidSetupSingleton.EnsureSingletonAvailable(ApplicationContext);
            setup.EnsureInitialized();
            base.OnCreate(bundle);

            SetContentView(LayoutId);
  
            MessageObservers = 
                new MvxMessageObserversController(ServicesLocation.Messenger)
                    .AddObservers(GetMessageObservers());
            MessageObservers?.StartObserving();
            
            OnCreateView(bundle); 
        }

        protected override void OnResume()
        {
            base.OnResume();
            MessageObservers?.StartObserving();
        }

        protected override void OnPause()
        {
            base.OnPause();
            MessageObservers?.StopObserving();
        }

        protected virtual int SnackBarLayoutId => Resource.Id.LayoutRoot;
        
        protected virtual IEnumerable<IMvxMessageObserver> GetMessageObservers()
        {
            yield return new QuestionDialogMessageObserver(this);
            yield return new CheckBoxQuestionDialogMessageObserver(this);
            yield return new MessageDialogMessageObserver(this);
        }
        
        protected virtual void OnCreateView(Bundle bundle)
        {
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            if (requestCode == PhoneCallPermission.ReadPhoneStatePermission)
            {
                var phoneCallPermission = (Mvx.IoCProvider.Resolve<IPhoneCallPermission>() as PhoneCallPermission);
                if (grantResults.Contains(Permission.Granted))
                    phoneCallPermission.OnPermissionGranted();
                else
                    phoneCallPermission.OnPermissionDenied();
            }
            else
            {
                PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            View currentFocusView = CurrentFocus;

            if (ev.Action == MotionEventActions.Up || ev.Action == MotionEventActions.Move)
            {
                bool shouldCloseKeyboard = true;

                if (currentFocusView is EditText)
                {
                    int[] locationCoords = new int[2];
                    currentFocusView.GetLocationOnScreen(locationCoords);

                    float x = ev.RawX + currentFocusView.Left - locationCoords[0];
                    float y = ev.RawY + currentFocusView.Top - locationCoords[1];

                    shouldCloseKeyboard = (x < currentFocusView.Left || x > currentFocusView.Right ||
                                           y < currentFocusView.Top || y > currentFocusView.Bottom);
                }

                if (shouldCloseKeyboard)
                    this.HideKeyboard();

                if (currentFocusView is EditText)
                    currentFocusView.ClearFocus();
            }

            return base.DispatchTouchEvent(ev);
        }

    }
}