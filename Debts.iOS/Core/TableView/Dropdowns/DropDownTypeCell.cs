using System;
using Cirrious.FluentLayouts.Touch;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace Debts.iOS.Cells.Dropdowns
{
    public class DropDownTypeCell : DropDownCell
    {
        private UILabel textLabel;

        public DropDownTypeCell() => Initialize();

        public DropDownTypeCell(IntPtr ptr) : base(ptr)
        {    
            Initialize();
        }

        public string Type
        {
            get => textLabel.Text;
            set => textLabel.Text = value;
        }

        void Initialize()
        {
            ContentView.BackgroundColor = UIColor.White;
            textLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(15, UIFontWeight.Regular),
                TextColor = UIColor.Gray,
                BackgroundColor = UIColor.White
            };
            
            ContentView.Add(textLabel);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            ContentView.AddConstraints(
                    textLabel.AtLeftOf(ContentView, 12),
                    textLabel.AtRightOf(ContentView, 12),
                    textLabel.AtTopOf(ContentView, 3),
                    textLabel.AtBottomOf(ContentView, 3)
            );
        }
    }
}