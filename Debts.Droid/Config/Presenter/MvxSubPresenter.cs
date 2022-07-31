using System;
using System.Threading.Tasks;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.Presenter
{
    public abstract class MvxSubpresenter
    {
        private readonly MvxAppPresenter _appPresenter;

        public MvxSubpresenter(MvxAppPresenter appPresenter)
        {
            _appPresenter = appPresenter;
        }
         
        public virtual Task<bool> HandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request) 
            => _appPresenter.InvokeShowFragment(view, attribute, request);

        public abstract bool ShouldHandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request);

        public virtual Task<bool> HandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute) 
            => _appPresenter.InvokeCloseFragment(viewModel, attribute);

        public abstract bool ShouldHandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute);
    }
}