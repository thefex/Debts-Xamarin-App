using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.Data.Queries;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.Model;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Cells
{
    public class BudgetGroupCell : MvxBaseTableViewCell
    {
        public BudgetGroupCell(IntPtr handle) : base(handle)
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
                var set = this.CreateBindingSet<BudgetGroupCell, BudgetItemGroup>();

                set.Bind(dateGroupLabel)
                    .To(x => x.ForDate)
                    .WithConversion(new HumanizedDateValueConverter());
                
                set.Apply();
            });
        }
    }
}