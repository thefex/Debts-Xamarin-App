using System;
using System.Collections.Generic;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Debts.Services;
using Debts.ViewModel;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Debts.Droid.Fragments
{
    public abstract class BaseFragment<TViewModel, TInitParams> : MvxFragment<TViewModel>
        where TViewModel : BaseViewModel<TInitParams> where TInitParams : class
    {
        private readonly int _layoutId;

        public BaseFragment(int layoutId)
        {
            _layoutId = layoutId;
        }

        public BaseFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
 


        public sealed override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var bindedView = this.BindingInflate(_layoutId, container, false);

            OnCreateViewInflated(bindedView);

            ViewModel.Load();
//            MessageObservers = new MvxMessageObserversController(ServicesLocation.Messenger)
  //              .AddObservers(GetMessageObservers());
    //        MessageObservers.StartObserving();
            return bindedView;
        }
      /*  protected virtual IEnumerable<IMvxMessageObserver> GetMessageObservers()
        {
            yield break;
        }
 */ 
        protected virtual void OnCreateViewInflated(View inflatedView)
        {
        }
 
    }
}