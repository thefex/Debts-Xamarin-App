using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.Bindings;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Utilities
{
    public abstract class MvxBaseTableViewCell : MvxTableViewCell
    {
        public IMvxViewModel ParentViewModel { get; set; }

        public UITableView TableView { get; set; }

        public MvxTableViewSource TableViewSource { get; set; }
        
        public Func<UIViewController> ParentViewControllerProvider { get; set; }
        
        public NSIndexPath IndexPath { get; set; }

        protected MvxBaseTableViewCell()
        {
        }

        protected MvxBaseTableViewCell(string bindingText) : base(bindingText)
        {
        }

        protected MvxBaseTableViewCell(IEnumerable<MvxBindingDescription> bindingDescriptions) : base(bindingDescriptions)
        {
        }

        protected MvxBaseTableViewCell(string bindingText, CGRect frame) : base(bindingText, frame)
        {
        }

        protected MvxBaseTableViewCell(IEnumerable<MvxBindingDescription> bindingDescriptions, CGRect frame) : base(bindingDescriptions, frame)
        {
        }

        protected MvxBaseTableViewCell(IntPtr handle) : base(handle)
        {
        }

        protected MvxBaseTableViewCell(string bindingText, IntPtr handle) : base(bindingText, handle)
        {
        }

        protected MvxBaseTableViewCell(IEnumerable<MvxBindingDescription> bindingDescriptions, IntPtr handle) : base(bindingDescriptions, handle)
        {
        }

        protected MvxBaseTableViewCell(string bindingText, UITableViewCellStyle cellStyle, NSString cellIdentifier, UITableViewCellAccessory tableViewCellAccessory = UITableViewCellAccessory.None) : base(bindingText, cellStyle, cellIdentifier, tableViewCellAccessory)
        {
        }

        protected MvxBaseTableViewCell(IEnumerable<MvxBindingDescription> bindingDescriptions, UITableViewCellStyle cellStyle, NSString cellIdentifier, UITableViewCellAccessory tableViewCellAccessory = UITableViewCellAccessory.None) : base(bindingDescriptions, cellStyle, cellIdentifier, tableViewCellAccessory)
        {
        }
    }
}