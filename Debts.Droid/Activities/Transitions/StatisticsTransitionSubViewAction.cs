using Android.Graphics.Drawables;
using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Debts.Droid.Core.Extensions;

namespace Debts.Droid.Activities.Transitions
{
    public class StatisticsTransitionSubViewAction : ITransitionSubViewAction
    {
        private readonly Drawable _navigationIcon;

        public StatisticsTransitionSubViewAction(Drawable navigationIcon)
        {
            _navigationIcon = navigationIcon;
        }

        public void OnSubViewAppearedAction(BottomAppBar appBar, FloatingActionButton fab)
        {
            appBar.SlideUp();
            appBar.NavigationIcon = _navigationIcon;
            appBar.FabAlignmentMode = BottomAppBar.FabAlignmentModeCenter;
            appBar.ReplaceMenu(Resource.Menu.statistics_menu);

            fab.Post(() => { 
                fab.Hide(); 
            });
        }
    }
}