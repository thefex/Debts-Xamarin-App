using System;
using Debts.Commands;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Debts.ViewModel.Statistics
{
    public class FilterStatisticsByDateViewModel : BaseViewModel<string>
    {
        public const string StartDatePickerTag = "StatisticsStartDate";
        public const string EndDatePickerTag = "StatisticsEndDate";
        
        private readonly StatisticsViewModel _statisticsViewModel;

        public FilterStatisticsByDateViewModel(StatisticsViewModel statisticsViewModel)
        {
            _statisticsViewModel = statisticsViewModel;
            

            StartDate = _statisticsViewModel.StartDate ?? new DateTime(2020, 1, 1);;
            EndDate = _statisticsViewModel.EndDate;
        }
 
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public MvxCommand SelectStartDate => new MvxExceptionGuardedCommand((() =>
        {
            ServicesLocation.Messenger.Publish(new PickDateMvxMessage(this) {Tag = StartDatePickerTag});
        }));
        
        public MvxCommand SelectEndDate => new MvxExceptionGuardedCommand((() =>
        {
            ServicesLocation.Messenger.Publish(new PickDateMvxMessage(this) {Tag = EndDatePickerTag });
        }));
        
        public MvxCommand Filter => new MvxExceptionGuardedCommand(() =>
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                ServicesLocation.Messenger.Publish(new MessageDialogMvxMessage(TextResources.Dialog_Error_Title, TextResources.Dialog_Error_DateFilterStatistics_Content, this));
                return;
            }

            if (EndDate < StartDate)
            {
                ServicesLocation.Messenger.Publish(new MessageDialogMvxMessage(TextResources.Dialog_Error_Title, TextResources.Dialog_Error_EndDateFilterStatistics_Content, this));
                return;
            }

            _statisticsViewModel.StartDate = StartDate;
            _statisticsViewModel.EndDate = EndDate;

            _statisticsViewModel.Filter.Execute();
            ServicesLocation.NavigationService.Close(this);
        });
        
     
    }
}