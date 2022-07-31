using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.Model;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Cells
{
    public class FinancesOperationGroupCell : MvxBaseTableViewCell
    {
        public FinancesOperationGroupCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            ContentView.UserInteractionEnabled=false;
            UILabel dateGroupLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(164,164,164)
            };
            
            ContentView.Add(dateGroupLabel);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            ContentView.AddConstraints(
                    dateGroupLabel.AtTopOf(ContentView),
                    dateGroupLabel.AtLeftOf(ContentView, 9),
                    dateGroupLabel.AtRightOf(ContentView, 12),
                    dateGroupLabel.AtBottomOf(ContentView, 12)
                );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FinancesOperationGroupCell, FinancesOperationGroup>();

                set.Bind(dateGroupLabel)
                    .To(x => x)
                    .WithConversion(new FinanceGroupKeyValueConverter());
                
                set.Apply();
            });
        }
    }
}