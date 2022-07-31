using Debts.Messenging;
using Debts.Services;
using MvvmCross.Localization;
using MvvmCross.ViewModels;

namespace Debts.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class BaseViewModel<TInitParams> : MvxViewModel<TInitParams>
    {
        protected readonly MvxMessageObserversController MessageObserversController;
        
        public virtual bool ShouldStopObservingOnDisappear => true;
        public BaseViewModel()
        {
            MessageObserversController = new MvxMessageObserversController(ServicesLocation.Messenger);
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            MessageObserversController?.StartObserving();
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            
            if (ShouldStopObservingOnDisappear)
                MessageObserversController?.StopObserving();
        }

        public override void Prepare(TInitParams parameter)
        {
            
        }

        public virtual async System.Threading.Tasks.Task Load()
        {
            
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            
            if (viewFinishing)
                MessageObserversController?.StopObserving();
        }

        public IMvxLanguageBinder TextSource => new MvxLanguageBinder("", GetType().Name);
    }
}