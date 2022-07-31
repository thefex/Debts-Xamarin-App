using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Fragments;

namespace Debts.Droid.Activities.Transitions
{
    public class FinanceDetailsTransitionSubViewAction : ITransitionSubViewAction
    {
        private readonly FinanceDetailsFragment _financeDetailsFragment;

        public FinanceDetailsTransitionSubViewAction(FinanceDetailsFragment financeDetailsFragment)
        {
            _financeDetailsFragment = financeDetailsFragment;
        }
        
        public void OnSubViewAppearedAction(BottomAppBar appBar, FloatingActionButton fab)
        {
            appBar.SlideUp();
            appBar.NavigationIcon = null;
            appBar.FabAlignmentMode = BottomAppBar.FabAlignmentModeEnd;

            appBar.Post(() =>
            {
                appBar.ReplaceMenu(Resource.Menu.finance_details);

                appBar.Post(() =>
                {
                    var callMenuitem = appBar.Menu.FindItem(Resource.Id.action_finance_details_call);
                    var smsMenuItem = appBar.Menu.FindItem(Resource.Id.action_finance_details_sms);

                    var hasPhoneNumber = _financeDetailsFragment.ViewModel.ArePhoneRelatedFeaturesEnabled;
                    callMenuitem.SetVisible(hasPhoneNumber);
                    smsMenuItem.SetVisible(hasPhoneNumber);
                });
                
            });
            if (_financeDetailsFragment.ViewModel.IsPaid)
                fab.Post(fab.Hide);
            else
            {
                fab.Hide();

                fab.Post(() =>
                {
                    fab.SetImageResource(Resource.Drawable.check);
                    fab.Show();
                });
            }
        }
    }
}