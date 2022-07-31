using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Services;
using Debts.ViewModel;
using Debts.ViewModel.Finances;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.AddFinance
{
    [MvxFragmentPresentation]
    public class AddFinanceOperationSelectDeadline : MvxFragment, DatePickerDialog.IOnDateSetListener
    {
        private MvxMessageObserversController _observersController;
        public AddFinanceOperationSelectDeadline()
        {
        }

        protected AddFinanceOperationSelectDeadline(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
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
                    datePickerDialog.Show();
                }));
            
            _observersController.StartObserving();
            
            var inflatedView = this.BindingInflate(Resource.Layout.fragment_add_finance_operation_deadline, container, false);
            return inflatedView;
        }

        public override void OnResume()
        {
            base.OnResume();
            _observersController?.StartObserving();
        }

        public override void OnPause()
        {
            base.OnPause();
            _observersController?.StopObserving();
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            var viewModel = ViewModel as AddFinanceOperationViewModel;
            var date = new DateTime(year, month + 1, dayOfMonth);
            viewModel.Deadline = date;
            viewModel.Next.Execute();
        }
    }
}