using System;
using System.Collections.Generic;
using System.Linq;
using Android.Support.Design.Widget;
using Android.Views;
using Debts.Droid.Core.Extensions;
using Debts.Messenging;

namespace Debts.Droid.Messenging.Observers
{
    internal class BlockViewOnAsyncLongOperationMessageObserver : LongRunningAsyncOperationMessageObserver
    {
        private readonly WeakReference<View> _rootView;

        public BlockViewOnAsyncLongOperationMessageObserver(WeakReference<View> rootView)
        {
            _rootView = rootView;
        }
 

        protected override void OnAsyncOperationStarted()
        {
            base.OnAsyncOperationStarted();
            SetViewState(false);
        }

        protected override void OnAsyncOperationFinished()
        {
            base.OnAsyncOperationFinished();
            SetViewState(true);
        }

        private void SetViewState(bool isEnabled)
        {
            View view = null;
            _rootView.TryGetTarget(out view);

            if (view == null)
                return;

            //var tag = view.GetTag(Resource.Id.do_not_block_on_long_operations);
            // if (tag != null && tag.ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
            //     return;
            view.Enabled = isEnabled;
        }


        public static IEnumerable<BlockViewOnAsyncLongOperationMessageObserver> BuildObservers(ViewGroup fromViewGroup)
        {
            var viewsToBlock = new List<View>();

            fromViewGroup.ForEachView<View>(view => {
                //  var tag = view.GetTag(Resource.Id.do_not_block_on_long_operations);
                //   if (tag != null && tag.ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
                //   {
                //       return;
                //  }
                viewsToBlock.Add(view);  
            });

            return
                viewsToBlock
                    .Where(view => view is AppBarLayout == false)
                    .Where(view => view is Android.Support.V7.Widget.Toolbar == false)
                    .Select(
                        view => new BlockViewOnAsyncLongOperationMessageObserver(new WeakReference<View>(view))
                    );
        }
    }
}