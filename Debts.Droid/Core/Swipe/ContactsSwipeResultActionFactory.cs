using System;
using Android.Runtime;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable.Action;
using Debts.Data;
using Debts.Model;
using MvvmCross.AdvancedRecyclerView.Swipe;
using MvvmCross.AdvancedRecyclerView.Swipe.ResultActions;
using MvvmCross.AdvancedRecyclerView.Swipe.ResultActions.ItemManager;
using MvvmCross.AdvancedRecyclerView.ViewHolders;

namespace Debts.Droid.Core.Swipe
{
    public class ContactsSwipeResultActionFactory : MvxSwipeResultActionFactory
    {
        public override SwipeResultAction GetSwipeLeftResultAction(IMvxSwipeResultActionItemManager itemProvider)
        {
            return new MvxSwipeToDirectionResultAction2(itemProvider, SwipeDirection.FromLeft);
        }

        public override SwipeResultAction GetSwipeRightResultAction(IMvxSwipeResultActionItemManager itemProvider)
        {
            return new MvxSwipeToDirectionResultAction2(itemProvider, SwipeDirection.FromRight);
        }

        public override SwipeResultAction GetUnpinSwipeResultAction(IMvxSwipeResultActionItemManager itemProvider)
        {
            return new MvxSwipeUnpinResultAction(itemProvider);
        }
 
    }
    
    public class MvxSwipeToDirectionResultAction2 : SwipeResultActionMoveToSwipedDirection
    {
        private IMvxSwipeResultActionItemManager _itemProvider;
        private readonly SwipeDirection _swipeDirection;

        public MvxSwipeToDirectionResultAction2(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MvxSwipeToDirectionResultAction2(IMvxSwipeResultActionItemManager itemProvider, SwipeDirection swipeDirection)
        {
            _itemProvider = itemProvider;
            _swipeDirection = swipeDirection;
        }

        protected override void OnPerformAction()
        {
            base.OnPerformAction();

            var stateController = _itemProvider.GetAttachedPinnedStateControllerProviderWithItem().FromSwipeDirection(_swipeDirection);

            var item = _itemProvider.GetItem();
            if (!stateController.IsPinned(item))
            {
                stateController.SetPinnedState(item, true);
                var viewHolder = _itemProvider.GetViewHolder(); 
                viewHolder.SwipeItemHorizontalSlideAmount = 0;
            }
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            _itemProvider = null;
        }
    }
}