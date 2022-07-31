using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using Debts.Data;
using Debts.Model;
using MvvmCross.AdvancedRecyclerView.Swipe.ResultActions;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;
using MvvmCross.AdvancedRecyclerView.ViewHolders;

namespace Debts.Droid.Core.Swipe
{
    public class ContactOperationChildSwipeableTemplate : MvxSwipeableTemplate
    {
        public override int SwipeContainerViewGroupId
        {
            get => Resource.Id.container_of_list_item; 
        }
        public override int UnderSwipeContainerViewGroupId => Resource.Id.underSwipe;

        protected override int SwipeReactionType => SwipeableItemConstants.ReactionCanSwipeBothH;

        public override int GetSwipeReactionType(object dataContext, MvxAdvancedRecyclerViewHolder holder)
        {
            if (dataContext is SelectableItem<ContactDetails> contactDetails && string.IsNullOrEmpty(contactDetails.Item.PhoneNumber))
                return SwipeableItemConstants.ReactionCanNotSwipeAny;
                
            return base.GetSwipeReactionType(dataContext, holder);
        }

        protected override float MaxLeftSwipeAmount => -1f;

        protected override float MaxRightSwipeAmount => 1f;

        public override float GetMaxLeftSwipeAmount(object dataContext, MvxAdvancedRecyclerViewHolder viewHolder)
        {
            if (dataContext is SelectableItem<ContactDetails> contactDetails && string.IsNullOrEmpty(contactDetails.Item.PhoneNumber))
                return 0;
            
            return base.GetMaxLeftSwipeAmount(dataContext, viewHolder);
        }

        public override float GetMaxRightSwipeAmount(object dataContext, MvxAdvancedRecyclerViewHolder viewHolder)
        {
            if (dataContext is SelectableItem<ContactDetails> contactDetails && string.IsNullOrEmpty(contactDetails.Item.PhoneNumber))
                return 0;
            
            return base.GetMaxRightSwipeAmount(dataContext, viewHolder);
        }

        public override int ItemViewSwipeLeftBackgroundResourceId => -1;
        //    Resource.Drawable.contact_bg_swipe_item_state_swiping_toLeft;

        public override int ItemViewSwipeRightBackgroundResourceId => -1;
         //   Resource.Drawable.bg_swipe_item_state_swiping_toRight;

        public override MvxSwipeResultActionFactory SwipeResultActionFactory { get; } = new ContactsSwipeResultActionFactory();
    }
}