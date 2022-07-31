using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Services;
using Debts.ViewModel.Statistics;
using MvvmCross.Droid.Support.Design;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments
{
    [MvxFragmentPresentation]
    public class FilterStatisticsByDateFragment : MvxBottomSheetDialogFragment<FilterStatisticsByDateViewModel>, DatePickerDialog.IOnDateSetListener
    {
        public const string FragmentTag = "FilterStatisticsFragmentByDate";
        private MvxMessageObserversController _observersController;

        public FilterStatisticsByDateFragment()
        {
            
        }

        public FilterStatisticsByDateFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            _observersController = new MvxMessageObserversController(ServicesLocation.Messenger)
                .AddObservers(new InvokeActionMessageObserver<PickDateMvxMessage>(msg =>
                {
                    var datePickerDialog = new DatePickerDialog(Activity, this, 
                        DateTime.UtcNow.Year,
                        DateTime.UtcNow.Month - 1,
                        DateTime.UtcNow.Day
                    );
                    currentDatePickerTag = msg.Tag;
                    datePickerDialog.Show();
                }));
            
            _observersController.StartObserving();

            return this.BindingInflate(Resource.Layout.select_date_range_fragment, container, false);
        }

        private string currentDatePickerTag = string.Empty;

        public override void OnPause()
        {
            base.OnPause();
            _observersController?.StopObserving();
        }

        public override void OnResume()
        {
            base.OnResume();
            _observersController?.StartObserving();
        }

        public override int Theme => Resource.Style.Widget_AppTheme_BottomSheet;
        
        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            var date = new DateTime(year, month + 1, dayOfMonth);

            if (currentDatePickerTag == FilterStatisticsByDateViewModel.StartDatePickerTag)
                ViewModel.StartDate = date;
            else if (currentDatePickerTag == FilterStatisticsByDateViewModel.EndDatePickerTag) 
                ViewModel.EndDate = date;
        }
    }
}