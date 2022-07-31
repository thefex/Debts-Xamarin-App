using System;
using UIKit;

namespace Debts.iOS.Utilities.TableView
{
    public class DefaultMvxTemplateSelector : MvxTemplateSelector
    {
        private readonly Type _cellType;

        public DefaultMvxTemplateSelector(UITableView tableView, Type cellType) : base(tableView)
        {
            _cellType = cellType;
        }

        public override Type GetCellType(object forDataContext)
        {
            return _cellType;
        }
    }
}