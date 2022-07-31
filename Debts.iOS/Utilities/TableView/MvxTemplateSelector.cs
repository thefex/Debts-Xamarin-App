using System;
using System.Collections.Generic;
using UIKit;

namespace Debts.iOS.Utilities
{
    public abstract class MvxTemplateSelector<TItem> : MvxTemplateSelector where TItem : class
    {
        protected MvxTemplateSelector(UITableView tableView) : base(tableView)
        {
        }

        public override Type GetCellType(object forDataContext)
        {
            return GetCellTypeForDataContext(forDataContext as TItem);
        }

        public abstract Type GetCellTypeForDataContext(TItem dataContext);
    }

    public abstract class MvxTemplateSelector
    {
        protected readonly UITableView TableView;
        private readonly HashSet<Type> _registeredCellTypes;

        public MvxTemplateSelector(UITableView tableView)
        {
            TableView = tableView;
            _registeredCellTypes = new HashSet<Type>();
        }

        public UITableViewCell DequeueReusableCell(object forDataContext)
        {
            var cellType = GetCellType(forDataContext);

            if (!_registeredCellTypes.Contains(cellType))
            {
                TableView.RegisterClassForCellReuse(cellType, cellType.ToString());
                _registeredCellTypes.Add(cellType);
            }

            return TableView.DequeueReusableCell(cellType.ToString());
        }

        public virtual UIContextualAction[] GetLeadingSwipeContextualActions(object forDataContext)
        {
            return new UIContextualAction[] { };
        }
        
        public virtual UIContextualAction[] GetTrailingSwipeContextualActions(object forDataContext)
        {
            return new UIContextualAction[] { };
        }
        
        public abstract Type GetCellType(object forDataContext);
    }
}