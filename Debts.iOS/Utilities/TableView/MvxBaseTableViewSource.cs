using System;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using UIKit;

namespace Debts.iOS.Utilities
{
    public class MvxBaseTableViewSource : MvxTableViewSource
    {
        private readonly IMvxView _parentView; 

        public MvxBaseTableViewSource(UITableView tableView, IMvxView parentView) : base(tableView)
        {
            _parentView = parentView;
            tableView.RowHeight = UITableView.AutomaticDimension;
            tableView.EstimatedRowHeight = 60;
        }

        public MvxBaseTableViewSource(IntPtr handle) : base(handle)
        {
        }

        public IMvxViewModel ParentDataContext => _parentView.ViewModel; 
          
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

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 0;
        }
 

        public MvxTemplateSelector TemplateSelector { get; set; }

        public Func<UIViewController> ParentViewControllerProvider { get; set; }
    }
}