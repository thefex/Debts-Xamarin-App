using System;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Services;
using Debts.Services.AppGrowth;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Debts.ViewModel.AppGrowth
{
    public class RateAppViewModel : MvxViewModel
    {
        private readonly RateAppService _rateAppService;

        public RateAppViewModel(RateAppService rateAppService)
        {
            _rateAppService = rateAppService;
        }

        public override Task Initialize()
        {
            IsNeverAskAgainAvailable = _rateAppService.IsNeverShowAgainOptionAvailable;
            return base.Initialize();
        }

        public bool IsNeverAskAgainAvailable { get; set; }
        
        public bool IsNeverShowAgainChecked { get; set; } 
        
        public MvxCommand Skip => new MvxExceptionGuardedCommand(() =>
        {
            if (IsNeverShowAgainChecked)
                _rateAppService.DisableShowingAppRatePrompt();
            ServicesLocation.NavigationService.Close(this);
        });
        
        public MvxCommand Rate => new MvxExceptionGuardedCommand(() =>
        {
            _rateAppService.RateApp();
            ServicesLocation.NavigationService.Close(this);
        });

        public string RateText => "It seems that you have been using Debts app for a while." + Environment.NewLine + Environment.NewLine +
                                  "The more suggestions we get - the better product we can build!" +
                                  Environment.NewLine +
                                  Environment.NewLine +
                                  "Rate us and share your feedback!";

    }
}