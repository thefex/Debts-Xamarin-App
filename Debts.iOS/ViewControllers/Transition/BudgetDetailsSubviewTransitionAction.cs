using MaterialComponents;
using UIKit;

namespace Debts.iOS.ViewControllers.Transition
{
    class BudgetDetailsSubviewTransitionAction : ITransitionSubViewAction
    {
        public void OnSubViewAppearedAction(BottomAppBarView appBar)
        {
            appBar.SetFloatingButtonHidden(true, true);

            appBar.LeadingBarButtonItems = new UIBarButtonItem[] { };
            appBar.TrailingBarButtonItems = new UIBarButtonItem[]
            { 
            };  
        }
    }
}