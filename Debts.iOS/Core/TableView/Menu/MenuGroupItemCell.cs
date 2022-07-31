using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace Debts.iOS.TableView.Menu
{
    public sealed class MenuGroupItemCell : MvxTableViewCell
    {
        public MenuGroupItemCell(IntPtr ptr) : base(ptr)
        {
            InitializeCell();    
        }

        void InitializeCell()
        {
            ContentView.BackgroundColor = UIColor.White;
            SelectionStyle = UITableViewCellSelectionStyle.None;
            
            UILabel nameLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(109,109,109)
            }; 
            ContentView.Add(nameLabel);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            ContentView.AddConstraints(
                nameLabel.AtTopOf(ContentView, 6),
                nameLabel.AtLeftOf(ContentView, 28),
                nameLabel.AtRightOf(ContentView, 36),
                nameLabel.AtBottomOf(ContentView, 14)
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<MenuGroupItemCell, MenuViewModel.MenuGroup>();
                
                set.Bind(nameLabel)
                    .To(x => x.GroupName);
                
                set.Apply();
            });
        }
    }
}