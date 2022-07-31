using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable.Action;
using MvvmCross.AdvancedRecyclerView.Swipe;
using MvvmCross.AdvancedRecyclerView.Swipe.ResultActions;
using MvvmCross.AdvancedRecyclerView.Swipe.ResultActions.ItemManager;

namespace Debts.Droid.Core.Swipe
{
    public class SwipeResultActionFactory : MvxSwipeResultActionFactory
    {
        public override SwipeResultAction GetSwipeLeftResultAction(IMvxSwipeResultActionItemManager itemProvider)
        {
            return new MvxSwipeToDirectionResultAction(itemProvider, SwipeDirection.FromLeft);
        }

        public override SwipeResultAction GetSwipeRightResultAction(IMvxSwipeResultActionItemManager itemProvider)
        {
            return new MvxSwipeToDirectionResultAction(itemProvider, SwipeDirection.FromRight);
        }

        public override SwipeResultAction GetUnpinSwipeResultAction(IMvxSwipeResultActionItemManager itemProvider)
        {
            return new MvxSwipeUnpinResultAction(itemProvider);
        }
    }
}