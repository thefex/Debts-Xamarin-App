using System;
using System.Collections;
using Debts.Data;
using Debts.iOS.Cells.Contacts;
using Debts.iOS.Config;
using Debts.iOS.Core.TableView.Contacts;
using Debts.Model;
using Debts.ViewModel.Contacts;
using MvvmCross.Binding.Extensions;
using UIKit;

namespace Debts.iOS.Utilities.TableView.Grouped
{
    public class ContactListTemplateSelector : MvxGroupedTemplateSelector
    {
        private readonly Func<ContactListViewModel> _viewModel;

        public ContactListTemplateSelector(UITableView tableView, Func<ContactListViewModel> viewModel) : base(tableView)
        {
            _viewModel = viewModel;
        }

        public override Type GetCellType(object forDataContext)
        {
            return typeof(ContactCell);
        }

        public override object GetItemAt(IEnumerable itemsSource, int groupIndex, int itemIndex)
        {
            var group = GetGroupItemAt(itemsSource, groupIndex) as ContactsGroup;
            return group.GroupChilds.ElementAt(itemIndex);
        }

        public override object GetGroupItemAt(IEnumerable itemsSource, int groupIndex) => (itemsSource.ElementAt(groupIndex) as ContactsGroup);

        public override int GetNumberOfItemsInGroup(IEnumerable itemsSource, int section)
        {
            var groupItem = GetGroupItemAt(itemsSource, section);
            return (groupItem as ContactsGroup)?.GroupChilds?.Count() ?? 0;
        }

        protected override Type GetGroupHeaderCellType(object fromDataContext) => typeof(LetterGroupCell);
        
        protected override Type GetGroupFooterCellType(object fromDataContext) => null;
        
        public override UIContextualAction[] GetLeadingSwipeContextualActions(object forDataContext)
        {
            if (forDataContext is SelectableItem<ContactDetails> contactDetails && !string.IsNullOrEmpty(contactDetails.Item.PhoneNumber))
            {
                var callAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
                    "Call",
                    (action, view, handler) =>
                    {
                        _viewModel().Call.Execute(contactDetails);
                        handler.Invoke(true);
                    });

                callAction.BackgroundColor = AppColors.BlueSwipeColor;
                callAction.Image = UIImage.GetSystemImage("phone.fill.arrow.up.right");

                return new UIContextualAction[]
                {
                    callAction
                };
            }
            
            return base.GetLeadingSwipeContextualActions(forDataContext);
        }

        public override UIContextualAction[] GetTrailingSwipeContextualActions(object forDataContext)
        {
            if (forDataContext is SelectableItem<ContactDetails> contactDetails && !string.IsNullOrEmpty(contactDetails.Item.PhoneNumber))
            {
                var messageAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Destructive,
                    "Send message",
                    (action, view, handler) =>
                    {
                        _viewModel().SMS.Execute(contactDetails);
                        handler.Invoke(true);
                    });

                messageAction.Image = UIImage.GetSystemImage("message.fill");
                messageAction.BackgroundColor = AppColors.GreenSwipeColor;
                
                return new UIContextualAction[]
                {
                    messageAction
                };
            }
            
            return base.GetTrailingSwipeContextualActions(forDataContext);
        }
    }
}