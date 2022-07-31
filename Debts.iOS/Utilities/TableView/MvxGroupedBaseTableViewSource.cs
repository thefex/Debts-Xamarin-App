using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Debts.Model;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using UIKit;

namespace Debts.iOS.Utilities
{
    public class MvxGroupedBaseTableViewSource : MvxTableViewSource
    {
        private readonly IMvxView _parentView;

        public MvxGroupedBaseTableViewSource(UITableView tableView, IMvxView parentView) : base(tableView)
        {
            _parentView = parentView;
            tableView.EstimatedRowHeight = 70;
            tableView.RowHeight = UITableView.AutomaticDimension;
            tableView.EstimatedSectionFooterHeight = tableView.EstimatedSectionHeaderHeight = 35;
            tableView.SectionFooterHeight = tableView.SectionHeaderHeight = UITableView.AutomaticDimension;
        }

        public MvxGroupedBaseTableViewSource(IntPtr handle) : base(handle)
        {
        }

        public IMvxViewModel ParentDataContext => _parentView.ViewModel;

        public override IEnumerable ItemsSource
        {
            get => base.ItemsSource;
            set
            {
                base.ItemsSource = value;
                
                if (value is IEnumerable<IGroupedData> groupedDatas)
                {
                    foreach (var item in groupedDatas)
                    {
                        if (item.GroupChilds is INotifyCollectionChanged collectionChanged)
                        {
                            collectionChanged.CollectionChanged += (e, a) => ReloadTableData();
                        }
                    }
                }
            }
        }
        


        void SetupCollectionChanged()
        {
            
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            if (TemplateSelector == null)
                throw new InvalidOperationException("Template selector is not set!");
            
            var cell = TemplateSelector.DequeueReusableCell(item);

            if (cell is MvxBaseTableViewCell tableViewCell)
            {
                tableViewCell.ParentViewModel = ParentDataContext;
                tableViewCell.TableView = TableView;
                tableViewCell.TableViewSource = this;
                tableViewCell.ParentViewControllerProvider = ParentViewControllerProvider;
                tableViewCell.IndexPath = indexPath;
            }

            return cell;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            var groupItem = TemplateSelector.GetGroupItemAt(ItemsSource, (int) section);
            return TemplateSelector.GetSectionHeaderTitle(groupItem);
        }

        public override string TitleForFooter(UITableView tableView, nint section)
        {
            var groupItem = TemplateSelector.GetGroupItemAt(ItemsSource, (int) section);
            return TemplateSelector.GetSectionFooterTitle(groupItem);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section) =>
            TemplateSelector.GetHeaderViewForGroup(ItemsSource, (int) section);

        public override UIView GetViewForFooter(UITableView tableView, nint section) =>
            TemplateSelector.GetFooterViewForGroup(ItemsSource, (int) section);

        protected override object GetItemAt(NSIndexPath indexPath) => TemplateSelector.GetItemAt(ItemsSource, indexPath.Section, indexPath.Row);

        public override nint RowsInSection(UITableView tableview, nint section)
        { 
            var rowsInSection = TemplateSelector.GetNumberOfItemsInGroup(ItemsSource, (int)section); 
            return rowsInSection;
        } 

        public override nint NumberOfSections(UITableView tableView)
        {
            var groupCount = TemplateSelector.GetGroupCount(ItemsSource);
            return groupCount;
        }

        public override UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
        {
            var leadingSwipeContextualActions = TemplateSelector.GetLeadingSwipeContextualActions(GetItemAt(indexPath));
            return UISwipeActionsConfiguration.FromActions(leadingSwipeContextualActions);
        }

        public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
        {
            var trailingSwipeContextualActions = TemplateSelector.GetTrailingSwipeContextualActions(GetItemAt(indexPath));
            return UISwipeActionsConfiguration.FromActions(trailingSwipeContextualActions);
        }

        public MvxGroupedTemplateSelector TemplateSelector { get; set; }

        public Func<UIViewController> ParentViewControllerProvider { get; set; }
        
        protected override void CollectionChangedOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            base.CollectionChangedOnCollectionChanged(sender, args);

            if (args.Action == NotifyCollectionChangedAction.Reset && sender is IEnumerable<IGroupedData> collection)
            {
                foreach (var item in collection)
                {
                    if (item.GroupChilds is INotifyCollectionChanged notifyCollectionChanged)
                    {
                        notifyCollectionChanged.CollectionChanged += (e, a) => ReloadTableData();
                    }
                }
                ReloadTableData();
            }

            foreach (var addedItems in args?.NewItems?.Cast<IGroupedData>() ?? Enumerable.Empty<IGroupedData>())
            {
                if (addedItems.GroupChilds is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged += (e, a) =>
                    {
                        ReloadTableData();
                    };
                }
            }
        }
    } 
}