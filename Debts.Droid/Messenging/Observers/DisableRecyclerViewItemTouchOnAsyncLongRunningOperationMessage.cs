using System;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Debts.Messenging;

namespace Debts.Droid.Messenging.Observers
{
    public class DisableRecyclerViewItemTouchOnAsyncLongRunningOperationMessage : LongRunningAsyncOperationMessageObserver
    {
        private readonly Func<RecyclerView> _view;
        private OnItemTouchDisableListener _onItemTouchDisableListener;

        public DisableRecyclerViewItemTouchOnAsyncLongRunningOperationMessage(Func<RecyclerView> view)
        {
            _view = view;
        }

        protected override void OnAsyncOperationStarted()
        {
            base.OnAsyncOperationStarted();

            var view = _view();

            _onItemTouchDisableListener = new OnItemTouchDisableListener();
            view.AddOnItemTouchListener(_onItemTouchDisableListener);
        }

        protected override void OnAsyncOperationFinished()
        {
            base.OnAsyncOperationFinished();

            if (_onItemTouchDisableListener != null)
            {
                var view = _view();
                view.RemoveOnItemTouchListener(_onItemTouchDisableListener);
                _onItemTouchDisableListener = null;
            }
            
        }

        public class OnItemTouchDisableListener : Java.Lang.Object, RecyclerView.IOnItemTouchListener
        {
            public OnItemTouchDisableListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            public OnItemTouchDisableListener()
            {
            }

            public bool OnInterceptTouchEvent(RecyclerView rv, MotionEvent e)
            {
                return true;
            }

            public void OnRequestDisallowInterceptTouchEvent(bool disallowIntercept)
            {
                
            }

            public void OnTouchEvent(RecyclerView rv, MotionEvent e)
            {
                
            }
        }
    }
}