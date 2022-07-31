using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using MvvmCross.AdvancedRecyclerView.Swipe.ResultActions;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;

namespace Debts.Droid.Core.Swipe
{
    public class BudgetChildSwipeableTemplate : MvxSwipeableTemplate
    {
        public override int SwipeContainerViewGroupId
        {
            get => Resource.Id.container_of_list_item; 
        }
        public override int UnderSwipeContainerViewGroupId => Resource.Id.underSwipe;

        protected override int SwipeReactionType => SwipeableItemConstants.ReactionCanSwipeLeft;
  
        protected override float MaxLeftSwipeAmount => -1f;

        protected override float MaxRightSwipeAmount => 0;
 
        public override int ItemViewSwipeLeftBackgroundResourceId => -1;

        public override int ItemViewSwipeRightBackgroundResourceId => -1;

        public override MvxSwipeResultActionFactory SwipeResultActionFactory => new SwipeResultActionFactory();
    }
}