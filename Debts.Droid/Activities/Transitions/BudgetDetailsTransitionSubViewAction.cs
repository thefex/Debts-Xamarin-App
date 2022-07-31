using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Debts.Droid.Core.Extensions;

namespace Debts.Droid.Activities.Transitions
{
    public class BudgetDetailsTransitionSubViewAction : ITransitionSubViewAction
    {
        public BudgetDetailsTransitionSubViewAction()
        {
     
        }
        public void OnSubViewAppearedAction(BottomAppBar appBar, FloatingActionButton fab)
        {
            appBar.SlideUp();
            appBar.NavigationIcon = null;
            appBar.FabAlignmentMode = BottomAppBar.FabAlignmentModeCenter;
            appBar.ReplaceMenu(Resource.Menu.empty_menu);

            fab.Post(fab.Hide);
        }
    }
}