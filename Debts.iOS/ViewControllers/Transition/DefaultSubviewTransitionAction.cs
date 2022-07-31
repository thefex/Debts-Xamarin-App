using Debts.iOS.ViewControllers.Menu;
using MaterialComponents;
using UIKit;

namespace Debts.iOS.ViewControllers.Transition
{
    class DefaultSubviewTransitionAction : ITransitionSubViewAction
    {
        public void OnSubViewAppearedAction(BottomAppBarView appBar)
        {
            appBar.SetFloatingButtonHidden(true, true);
            
            var menuItem = new UIBarButtonItem(UIImage.FromBundle("menu_hamburger"), UIBarButtonItemStyle.Plain, (e, a) =>
            {
                MenuViewController.Show();
            });
            
            
            appBar.LeadingBarButtonItems = new UIBarButtonItem[] { menuItem };
            appBar.TrailingBarButtonItems = new UIBarButtonItem[]
            { 
            };  
        }
    }
}