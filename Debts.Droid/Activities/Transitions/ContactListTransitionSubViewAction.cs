using Android.Graphics.Drawables;
using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Debts.Droid.Core.Extensions;

namespace Debts.Droid.Activities.Transitions
{
    public class ContactListTransitionSubViewAction : ITransitionSubViewAction
    {
        private readonly Drawable _navigationIcon;

        public ContactListTransitionSubViewAction(Drawable navigationIcon)
        {
            _navigationIcon = navigationIcon;
        }

        public void OnSubViewAppearedAction(BottomAppBar appBar, FloatingActionButton fab)
        {
            appBar.SlideUp();
            appBar.NavigationIcon = _navigationIcon;
            appBar.FabAlignmentMode = BottomAppBar.FabAlignmentModeCenter;
            appBar.ReplaceMenu(Resource.Menu.contact_list_menu);

            fab.Post(() => { 
                fab.Hide();
                fab.SetImageResource(Resource.Drawable.plus);
                fab.Show();
            });
        }
    }
}