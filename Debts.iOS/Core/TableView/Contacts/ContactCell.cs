using System;
using System.Globalization;
using Cirrious.FluentLayouts.Touch;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities;
using Debts.Model;
using Debts.ViewModel.Contacts;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace Debts.iOS.Cells.Contacts
{
    public class ContactCell : MvxBaseTableViewCell
    {
        private InkTouchController inkTouchController;
        TextToSearchAttributedTextValueConverter textToSearchAttributedTextValueConverter = new TextToSearchAttributedTextValueConverter();
        
        public ContactCell(IntPtr ptr) : base(ptr) 
        {
            Initialize();    
        }

        void Initialize()
        {
            ContentView.BackgroundColor = UIColor.White;
            SelectionStyle = UITableViewCellSelectionStyle.None;

            avatarView = new UIImageView()
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            
            name = new UILabel()
            {
                Font = UIFont.PreferredTitle3,
                TextColor = AppColors.BlackTextColor
            };
            
            var padding = new UIEdgeInsets(3, 6, 3, 6);
            var cornerRadius = 9;
            UIPaddingLabel debtsAmount = new UIPaddingLabel()
            {
                Padding = padding,
                Layer = { CornerRadius = cornerRadius},
                BackgroundColor = AppColors.DebtColor,
                TextColor = UIColor.White,
                ClipsToBounds = true,
                Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Center
            };
            
            UIPaddingLabel loanAmount = new UIPaddingLabel()
            {
                Padding = padding,
                Layer = { CornerRadius = cornerRadius},
                ClipsToBounds = true,
                BackgroundColor = AppColors.LoanColor,
                TextColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Center
            };
            
            
            ContentView.Add(avatarView);
            ContentView.Add(name);
            ContentView.Add(debtsAmount);
            ContentView.Add(loanAmount);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            inkTouchController = new InkTouchController(ContentView);
            inkTouchController.AddInkView();
            
            ContentView.AddConstraints(
                    avatarView.AtTopOf(ContentView, 12),
                    avatarView.AtLeftOf(ContentView, 12),
                    avatarView.Width().EqualTo(60),
                    avatarView.Height().EqualTo(60),
                    avatarView.AtBottomOf(ContentView, 12),
                    
                    name.WithSameTop(avatarView),
                    name.ToRightOf(avatarView, 12),
                    name.AtRightOf(ContentView, 12),
                    
                    debtsAmount.WithSameLeft(name), 
                    debtsAmount.Below(name, 6),
                    
                    loanAmount.ToRightOf(debtsAmount, 6), 
                    loanAmount.WithSameTop(debtsAmount)
                );

            var converter = new GenericValueConverter<decimal, string>(x => $"${x.ToString(CultureInfo.InvariantCulture)}");
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ContactCell, SelectableItem<ContactDetails>>();

                set.Bind(this)
                    .For(x => x.Contact)
                    .To(x => x.Item);
                
                set.Bind(this)
                    .For(x => x.FullName)
                    .To(x => x.Item.FullName)
                    ;

                set.Bind(debtsAmount)
                    .To(x => x.Item.ActiveDebtsAmount)
                    .WithConversion(converter);
                
                set.Bind(loanAmount)
                    .To(x => x.Item.ActiveLoansAmount)
                    .WithConversion(converter);
                
                set.Apply();
            });
        }

        private string _fullName;
        public string FullName 
        {
            get => _fullName;
            set
            {
                _fullName = value ?? string.Empty;
                textToSearchAttributedTextValueConverter.RecentSearchWord =
                    (base.ParentViewModel as ContactListViewModel).RecentSearchWord;
                name.AttributedText = textToSearchAttributedTextValueConverter.Convert(_fullName, null, null, null) as NSAttributedString;
            }
        }
        
        private UILabel name;
        private UIImageView avatarView;
        
        private ContactDetails _contact;

        public ContactDetails Contact
        {
            get => _contact;
            set
            {
                _contact = value;
                new AvatarGenerator().GenerateAvatar(avatarView, value?.ToString() ?? "?", value?.AvatarUrl, 15, 64, UIColor.White, UIColor.Gray);
            }
        }
    }
}