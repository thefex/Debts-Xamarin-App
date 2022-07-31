using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;

namespace Debts.Droid.Activities.Transitions
{
    public interface ITransitionSubViewAction
    {
        void OnSubViewAppearedAction(BottomAppBar appBar, FloatingActionButton fab);
    }
}