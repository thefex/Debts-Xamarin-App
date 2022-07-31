using System;
using System.Threading.Tasks;
using Android.Views.Animations;
using Debts.Droid.Activities;
using Debts.Droid.Core.Extensions.Wrappers;
using Debts.Droid.Fragments.Contacts;
using Debts.Services;
using Debts.ViewModel.AppGrowth;
using Debts.ViewModel.Contacts;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.Presenter
{
    public class EmptyFragmentAnimationSubPresenter : MvxSubpresenter
    {
        public EmptyFragmentAnimationSubPresenter(MvxAppPresenter appPresenter) : base(appPresenter)
        {
        }

        public override bool ShouldHandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            return false;
        }

        public override bool ShouldHandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            return viewModel is PickContactViewModel ||
                   viewModel is GoPremiumViewModel ||
                   viewModel is RateAppViewModel;
        }

        public override Task<bool> HandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            var view = (Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity)
                .SupportFragmentManager
                .FindFragmentById(Resource.Id.pick_contacts_presenter)
                .View;
                
            view
                .Post(() =>
                {
                    view.Animate()
                        .TranslationY(view.Height)
                        .SetDuration(275)
                        .SetInterpolator(new AccelerateInterpolator(2f))
                        .SetListener(new AnimatorActionListenerWrapper(x =>
                        {
                            
                        }, x =>
                        {
                            base.HandleCloseFragmentRequest(viewModel, attribute);
                        }, x => { }, x => { }));
                });
                  
            return Task.FromResult(true);
        }
    }
}