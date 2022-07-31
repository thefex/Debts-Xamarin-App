using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Sections;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.Resources;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Core.TableView.Contacts
{
    public class FinanceOperationHeaderLikeCell : MvxBaseTableViewCell
    {
        public FinanceOperationHeaderLikeCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            
            headerTitle = new UILabel()
            {
                Font = UIFont.PreferredTitle2,
                TextColor = AppColors.BlackTextColor,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
       
            ContentView.Add(headerTitle);  
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            ContentView.AddConstraints(
                headerTitle.AtLeftOf(ContentView, 18),
                headerTitle.AtTopOf(ContentView, 12),
                headerTitle.AtRightOf(ContentView, 12),
                headerTitle.AtBottomOf(ContentView, 12)
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FinanceOperationHeaderLikeCell, HeaderFinanceOperationSection>();

                set.Bind(this)
                    .For(x => x.ItemsCount)
                    .To(x => x.OperationsSection.ItemsCount);
                
                set.Apply();
            });
        }

        private int _itemsCount;
        private UILabel headerTitle;

        public int ItemsCount 
        {
            get => _itemsCount;
            set
            {
                _itemsCount = value;
                headerTitle.Text = (DataContext as HeaderFinanceOperationSection).OperationsSection.Title + " (" + value + ")";
            }
                
        }
    } 
}