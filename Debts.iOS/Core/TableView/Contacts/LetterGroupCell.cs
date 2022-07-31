using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Utilities;
using Debts.Model;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Core.TableView.Contacts
{
    public class LetterGroupCell : MvxBaseTableViewCell
    {
        public LetterGroupCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        void Initialize()
        {
            ContentView.BackgroundColor = AppColors.GrayBackground;
            ContentView.UserInteractionEnabled = false;
            SelectionStyle = UITableViewCellSelectionStyle.None;
        
            UILabel label = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(22, UIFontWeight.Regular),
                TextColor = AppColors.StrongGray
            };
            
            ContentView.Add(label);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            ContentView.AddConstraints(
                label.AtTopOf(ContentView, 0),
                label.AtLeftOf(ContentView, 12),
                label.AtRightOf(ContentView, 12),
                label.AtBottomOf(ContentView, 12)
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<LetterGroupCell, ContactsGroup>();

                set.Bind(label)
                    .To(x => x.GroupingLetter);
                
                set.Apply();
            });
        }
    }
}