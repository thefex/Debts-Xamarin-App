using System;
using System.Collections;
using System.Collections.Generic;
using MvvmCross.Binding.Extensions;
using UIKit;

namespace Debts.iOS.Utilities
{
    public abstract class MvxGroupedTemplateSelector : MvxTemplateSelector
    {
        private readonly HashSet<Type> _registeredGroupHeaderTypes = new HashSet<Type>();
        private readonly HashSet<Type> _registeredGroupFooterTypes = new HashSet<Type>();

        protected MvxGroupedTemplateSelector(UITableView tableView) : base(tableView)
        {
        }

        public abstract object GetItemAt(IEnumerable itemsSource, int groupIndex, int itemIndex);

        public abstract object GetGroupItemAt(IEnumerable itemsSource, int groupIndex);

        public virtual int GetGroupCount(IEnumerable itemsSource) => itemsSource?.Count() ?? 0;

        public abstract int GetNumberOfItemsInGroup(IEnumerable itemsSource, int section);

        public UIView GetHeaderViewForGroup(IEnumerable itemsSource, int groupIndex)
        {
            var groupItemObject = GetGroupItemAt(itemsSource, groupIndex);
            var groupHeaderCellType = GetGroupHeaderCellType(groupItemObject);

            if (groupHeaderCellType == null)
                return null;
            
            if (!_registeredGroupHeaderTypes.Contains(groupHeaderCellType))
            {
                TableView.RegisterClassForCellReuse(groupHeaderCellType, groupHeaderCellType.ToString());
                _registeredGroupHeaderTypes.Add(groupHeaderCellType);
            }

            var cell = TableView.DequeueReusableCell(groupHeaderCellType.ToString()) as MvxBaseTableViewCell;
            cell.DataContext = groupItemObject;

            return cell.ContentView;
        }
         
        protected abstract Type GetGroupHeaderCellType(object fromDataContext);

        public virtual string GetSectionHeaderTitle(object fromDataContext) => null;

        public UIView GetFooterViewForGroup(IEnumerable itemsSource, int groupIndex)
        {
            var groupItemObject = GetGroupItemAt(itemsSource, groupIndex);
            var groupFooterCellType = GetGroupFooterCellType(groupItemObject);

            if (groupFooterCellType == null)
                return null;
            
            if (!_registeredGroupFooterTypes.Contains(groupFooterCellType))
            {
                TableView.RegisterClassForCellReuse(groupFooterCellType, groupFooterCellType.ToString());
                _registeredGroupFooterTypes.Add(groupFooterCellType);
            }

            var cell = TableView.DequeueReusableCell(groupFooterCellType.ToString()) as MvxBaseTableViewCell;
            cell.DataContext = groupItemObject;

            return cell.ContentView;
        }

        protected abstract Type GetGroupFooterCellType(object fromDataContext);

        public virtual string GetSectionFooterTitle(object fromDataContext) => null;
    }
}