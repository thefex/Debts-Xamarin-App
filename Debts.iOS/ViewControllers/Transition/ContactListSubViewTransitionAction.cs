using System;
using Debts.iOS.ViewControllers.Menu;
using Debts.ViewModel.Contacts;
using MaterialComponents;
using UIKit;

namespace Debts.iOS.ViewControllers.Transition
{
    class ContactListSubViewTransitionAction : ITransitionSubViewAction
    {
        private Func<ContactListViewModel> _viewModel;

        public ContactListSubViewTransitionAction(Func<ContactListViewModel> viewModel)
        {
            _viewModel = viewModel;
        }

        public void OnSubViewAppearedAction(BottomAppBarView appBar)
        {
            appBar.SetFloatingButtonHidden(false, true);
            appBar.FloatingButtonPosition = BottomAppBarFloatingButtonPosition.Center;
            appBar.FloatingButton.SetImage(UIImage.FromBundle("plus"), UIControlState.Normal);
            appBar.FloatingButton.SetImageTintColor(UIColor.White, UIControlState.Normal);
            
            var menuItem = new UIBarButtonItem(UIImage.FromBundle("menu_hamburger"), UIBarButtonItemStyle.Plain, (e, a) =>
            {
                MenuViewController.Show();
            });

            var importContacts = new UIBarButtonItem(UIImage.FromBundle("import"),
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    _viewModel().ImportContacts.Execute();
                });

            
            appBar.LeadingBarButtonItems = new UIBarButtonItem[] { menuItem };
            appBar.TrailingBarButtonItems = new UIBarButtonItem[]
            {
                importContacts
            };
        }
    }
}