using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;

namespace Debts.Droid.Core.Swipe
{
    public class ContactGroupSwipeableTemplate : MvxSwipeableTemplate
    {
        public override int SwipeContainerViewGroupId => Resource.Id.container_of_group_letter;
        public override int UnderSwipeContainerViewGroupId => Resource.Id.container_of_group_letter;
        protected override int SwipeReactionType => SwipeableItemConstants.ReactionCanNotSwipeAny;
    }
}