using System;
using System.Collections.Generic;
using Debts.iOS.ViewControllers.Finances;
using Debts.ViewModel;
using MaterialComponents;
using UIKit;

namespace Debts.iOS.ViewControllers.Transition
{
    class FinanceDetailsSubViewTransitionAction : ITransitionSubViewAction
    {
        private readonly Func<FinanceDetailsViewModel> _financeDetailsViewModelProvider;

        public FinanceDetailsSubViewTransitionAction(Func<FinanceDetailsViewModel> financeDetailsViewModelProvider)
        {
            _financeDetailsViewModelProvider = financeDetailsViewModelProvider;
        }
        
        public void OnSubViewAppearedAction(BottomAppBarView appBar)
        {
            appBar.SetFloatingButtonHidden(false, true);
            appBar.FloatingButtonPosition = BottomAppBarFloatingButtonPosition.Trailing;
            appBar.FloatingButton.SetImage(UIImage.FromBundle("check").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            appBar.FloatingButton.SetImageTintColor(UIColor.White, UIControlState.Normal);


            var callImage = UIImage.FromBundle("phone").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            var smsImage = UIImage.FromBundle("sms").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            var shareImage = UIImage.FromBundle("share").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            var addNoteImage = UIImage.FromBundle("note").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            
            var callMenuItem = new UIBarButtonItem(callImage,
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    var viewModel = _financeDetailsViewModelProvider();
                    viewModel?.Call.Execute();
                });

            var smsMenuItem = new UIBarButtonItem(smsImage,
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    var viewModel = _financeDetailsViewModelProvider();
                    viewModel?.Sms.Execute();
                });
                
            var shareMenuItem = new UIBarButtonItem(shareImage,
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    var viewModel = _financeDetailsViewModelProvider();
                    viewModel?.Share.Execute();
                });
            
            var addNoteMenuitem = new UIBarButtonItem(addNoteImage,
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    var viewModel = _financeDetailsViewModelProvider();
 
                    viewModel?.AddNote.Execute();
                });

            List<UIBarButtonItem> leadingMenuItems = new List<UIBarButtonItem>()
            {
                addNoteMenuitem,
                shareMenuItem,
                callMenuItem,
                smsMenuItem
            };
            
         //   var providedViewModel = _financeDetailsViewModelProvider();

         //   if (!providedViewModel.ArePhoneRelatedFeaturesEnabled)
        //    {
        //        leadingMenuItems.Remove(callMenuItem);
        //        leadingMenuItems.Remove(smsMenuItem);
        //    };

            appBar.LeadingBarButtonItems = leadingMenuItems.ToArray();
            appBar.TrailingBarButtonItems = new UIBarButtonItem[] { };
        }
    }
}