using System;
using System.Collections.Generic;
using Debts.iOS.Config;
using Debts.iOS.Core.VIews;
using Debts.Messenging;
using Debts.Services;
using Debts.ViewModel;
using Foundation;
using MvvmCross.Platforms.Ios.Views;

namespace Debts.iOS.ViewControllers.Base
{
   public class BaseViewController<TViewModel, TInitParams> : MvxViewController<TViewModel>
        where TViewModel : BaseViewModel<TInitParams>  
    {
        protected MvxMessageObserversController MessageObservers;
        public BaseViewController()
        {
        }

        public BaseViewController(IntPtr handle) : base(handle)
        {
        }

        protected BaseViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        } 
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = AppColors.GrayBackground;
            MessageObservers =
                new MvxMessageObserversController(ServicesLocation.Messenger)
                    .AddObservers(GetMessageObservers());
 
            ViewModel.Load();  
            
            
        }
        protected virtual IEnumerable<IMvxMessageObserver> GetMessageObservers()
        {
            yield break;
        }
  
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            MessageObservers?.StartObserving();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            MessageObservers?.StopObserving();
        }
    }
}