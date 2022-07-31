using System;
using Android.Support.V4.View.Animation;
using Android.Views;
using Debts.Messenging;

namespace Debts.Droid.Messenging.Observers
{
    class ReplaceViewOnLongOperationMessageObserver : LongRunningAsyncOperationMessageObserver
    {
        public Func<View> ProgressViewProvider { get; set; }
        public Func<View> ViewToReplace { get; set; }

        private readonly bool _animate;

        public ReplaceViewOnLongOperationMessageObserver(Func<View> progressViewProvider, Func<View> viewToReplace, bool animate = true)
        {
            _animate = animate;
            ProgressViewProvider = progressViewProvider;
            ViewToReplace = viewToReplace;
        }

        public bool IsProgressRunning { get; private set; }

        protected override void OnAsyncOperationStarted()
        {
            base.OnAsyncOperationStarted();
            IsProgressRunning = true;
            StartReplaceViewAnimation(ViewToReplace(), ProgressViewProvider());
        }

        protected override void OnAsyncOperationFinished()
        {
            base.OnAsyncOperationFinished();
            IsProgressRunning = false;
            StartReplaceViewAnimation(ProgressViewProvider(), ViewToReplace());
        }

        private void StartReplaceViewAnimation(View viewToHide, View viewToShow)
        {
            if (viewToHide == null || viewToShow == null)
                return;
            if (_animate)
            {
                viewToHide.Animate().Cancel();
                viewToShow.Animate().Cancel();

                viewToShow.Animate().Alpha(1)
                    .SetInterpolator(new FastOutLinearInInterpolator());
                viewToHide.Animate().Alpha(0)
                    .SetDuration(250L)
                    .SetInterpolator(new FastOutLinearInInterpolator());
            }
            else
            {
                viewToShow.Alpha = 1;
                viewToHide.Alpha = 0;
            }
        }
    }
}