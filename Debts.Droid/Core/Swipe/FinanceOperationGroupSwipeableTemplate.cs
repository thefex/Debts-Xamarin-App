using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;

namespace Debts.Droid.Core.Swipe
{
    public class FinanceOperationGroupSwipeableTemplate : MvxSwipeableTemplate
    {
        public override int SwipeContainerViewGroupId => Resource.Id.text_header;
        public override int UnderSwipeContainerViewGroupId => Resource.Id.text_header;
        protected override int SwipeReactionType => SwipeableItemConstants.ReactionCanNotSwipeAny;
    }
}