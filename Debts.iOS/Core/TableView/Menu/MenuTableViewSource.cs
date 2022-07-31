using System;
using System.Collections.Generic;
using Debts.iOS.TableView.Menu;
using Debts.iOS.ViewModels;
using Foundation;
using MvvmCross.Binding.Extensions;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace Debts.iOS.Cells.Menu
{
    public class MenuTableViewSource : MvxTableViewSource
    {
        readonly NSString _menuGroupId = new NSString("menugroup");
        readonly NSString _menuItemId = new NSString("menuitem"); 
        public MenuTableViewSource(UITableView tableView) : base(tableView)
        {
            tableView.RegisterClassForCellReuse(typeof(MenuGroupItemCell), _menuGroupId);
            tableView.RegisterClassForCellReuse(typeof(MenuItemCell), _menuItemId);

            tableView.EstimatedRowHeight = 70;
            tableView.EstimatedSectionHeaderHeight = 35;
            tableView.SectionHeaderHeight = UITableView.AutomaticDimension;
            tableView.RowHeight = UITableView.AutomaticDimension;
        }

        public MenuTableViewSource(IntPtr handle) : base(handle)
        {
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            return tableView.DequeueReusableCell(_menuItemId);
        }
        
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var cell = tableView.DequeueReusableCell(_menuGroupId) as MvxTableViewCell;
            cell.DataContext = ((ItemsSource as IEnumerable<MenuViewModel.MenuGroup>).ElementAt((int)section));
            return cell.ContentView;
        }
        

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return ((ItemsSource as IEnumerable<MenuViewModel.MenuGroup>).ElementAt(indexPath.Section) as
                MenuViewModel.MenuGroup).Items.ElementAt(indexPath.Row) as MenuViewModel.MenuItem;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (ItemsSource == null)
                return 0;
            // If the section is not colapsed return the rows in that section otherwise return 0
            return ((ItemsSource as IEnumerable<MenuViewModel.MenuGroup>).ElementAt((int)section) as MenuViewModel.MenuGroup).Items.Count;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            if (ItemsSource == null)
                return 0;

            return ItemsSource.Count();
        }
    } 
}