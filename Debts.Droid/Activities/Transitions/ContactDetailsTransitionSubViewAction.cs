using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Fragments;

namespace Debts.Droid.Activities.Transitions
{
    public class ContactDetailsTransitionSubViewAction : ITransitionSubViewAction
    {
        private readonly ContactDetailsFragment _fragment;

        public ContactDetailsTransitionSubViewAction(ContactDetailsFragment fragment)
        {
            _fragment = fragment;
        }
        public void OnSubViewAppearedAction(BottomAppBar appBar, FloatingActionButton fab)
        {
            appBar.SlideUp();
            appBar.NavigationIcon = null;
            appBar.FabAlignmentMode = BottomAppBar.FabAlignmentModeEnd;
            appBar.ReplaceMenu(Resource.Menu.contact_details);

            appBar.Post(() =>
            {
                var callMenuitem = appBar.Menu.FindItem(Resource.Id.action_contact_details_call);
                var smsMenuItem = appBar.Menu.FindItem(Resource.Id.action_contact_details_sms);

                bool isVisible = _fragment.ViewModel.ArePhoneRelatedFeaturesEnabled;

                callMenuitem.SetVisible(isVisible);
                smsMenuItem.SetVisible(isVisible);
            });
            
            fab.Hide(); 
        }
    }
}