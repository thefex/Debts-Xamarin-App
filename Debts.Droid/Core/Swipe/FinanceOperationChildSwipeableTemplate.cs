using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using Debts.Data;
using MvvmCross.AdvancedRecyclerView.Swipe.ResultActions;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;
using MvvmCross.AdvancedRecyclerView.ViewHolders;

namespace Debts.Droid.Core.Swipe
{
    public class FinanceOperationChildSwipeableTemplate : MvxSwipeableTemplate
    {
        public override int SwipeContainerViewGroupId
        {
            get => Resource.Id.container_of_list_item; 
        }
        public override int UnderSwipeContainerViewGroupId => Resource.Id.underSwipe;

        protected override int SwipeReactionType => SwipeableItemConstants.ReactionCanSwipeBothH;

        public override int GetSwipeReactionType(object dataContext, MvxAdvancedRecyclerViewHolder holder)
        {
            if (dataContext is FinanceOperation operation && operation.IsPaid)
                return SwipeableItemConstants.ReactionCanSwipeLeft;
        
            return base.GetSwipeReactionType(dataContext, holder);
        }

        protected override float MaxLeftSwipeAmount => -1f;

        protected override float MaxRightSwipeAmount => 1f;

        public override float GetMaxRightSwipeAmount(object dataContext, MvxAdvancedRecyclerViewHolder viewHolder)
        {
            if (dataContext is FinanceOperation financeOperation && financeOperation.IsPaid)
                return 0;
        
            return base.GetMaxRightSwipeAmount(dataContext, viewHolder);
        }

        public override int ItemViewSwipeLeftBackgroundResourceId => -1;

        public override int ItemViewSwipeRightBackgroundResourceId => -1;

        public override MvxSwipeResultActionFactory SwipeResultActionFactory => new SwipeResultActionFactory();
    }
}