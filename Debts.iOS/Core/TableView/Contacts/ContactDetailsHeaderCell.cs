using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities;
using Debts.Resources;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Core.TableView.Contacts
{
    public class ContactDetailsHeaderCell : MvxBaseTableViewCell
    {
        private UIImageView avatarView;

        public ContactDetailsHeaderCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            
            int iconSize = 60;
            UILabel mainTitle = new UILabel()
            {
                Font = UIFont.PreferredTitle2,
                TextColor = AppColors.BlackTextColor,
                Text = TextResources.ContactDetailsViewModel_General
            };

            avatarView = new UIImageView()
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            UILabel contactName = new UILabel();
            UILabel phoneNumberTitle = new UILabel()
            {
                Text = TextResources.ContactDetailsViewModel_PhoneNumber
            };
            UILabel phoneNumberContent = new UILabel()
            {
                TextColor = AppColors.GrayForTextFieldContainer,
                Font = UIFont.PreferredSubheadline
            };
            
            contactName.Font = phoneNumberTitle.Font = mainTitle.Font;
            contactName.TextColor = phoneNumberTitle.TextColor = AppColors.BlackTextColor;
            
            UIView phoneIconView = new UIView()
            {
                Layer = { CornerRadius = iconSize / 2, BorderWidth = 6, BorderColor = UIColor.FromRGB(87, 10, 235).CGColor},
                BackgroundColor = UIColor.Clear
            };
            
            UIImageView phoneImageView = new UIImageView(UIImage.FromBundle("phone").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate))
            {
                ContentMode = UIViewContentMode.Center,
                TintColor = UIColor.FromCGColor(phoneIconView.Layer.BorderColor),
                BackgroundColor = UIColor.FromRGB(234, 234, 234),
                Layer = { CornerRadius = (iconSize-12*2)/2, MasksToBounds = true}
            };
            
            phoneIconView.Add(phoneImageView);
            phoneIconView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            phoneIconView.AddConstraints(
                    phoneImageView.AtTopOf(phoneIconView, 12),
                    phoneImageView.AtLeftOf(phoneIconView, 12),
                    phoneImageView.AtRightOf(phoneIconView, 12),
                    phoneImageView.AtBottomOf(phoneIconView, 12)
                );
            
            ContentView.Add(mainTitle);
            ContentView.Add(avatarView);
            ContentView.Add(contactName);
            ContentView.Add(phoneIconView);
            ContentView.Add(phoneNumberTitle);
            ContentView.Add(phoneNumberContent);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
             
            ContentView.AddConstraints(
                mainTitle.AtTopOf(ContentView, 12),
                    mainTitle.AtLeadingOf(ContentView, 18),
                    mainTitle.AtTrailingOf(ContentView, 12),
                    
                    avatarView.WithSameLeft(mainTitle),
                    avatarView.Below(mainTitle, 12),
                    avatarView.Width().EqualTo(iconSize),
                    avatarView.Height().EqualTo(iconSize),
                    
                    contactName.ToRightOf(avatarView, 12),
                    contactName.AtRightOf(ContentView, 12),
                    contactName.WithSameCenterY(avatarView),
                    
                    phoneIconView.WithSameLeft(avatarView),
                    phoneIconView.Below(avatarView, 12),
                    phoneIconView.Width().EqualTo(iconSize),
                    phoneIconView.Height().EqualTo(iconSize),
                    phoneIconView.AtBottomOf(ContentView, 12),
                    
                    phoneNumberTitle.WithSameTop(phoneIconView).Plus(6),
                    phoneNumberTitle.ToRightOf(phoneIconView, 12),
                    phoneNumberTitle.AtRightOf(ContentView, 12),
                    
                    phoneNumberContent.Below(phoneNumberTitle, 3),
                    phoneNumberContent.WithSameLeft(phoneNumberTitle),
                    phoneNumberContent.WithSameRight(phoneNumberTitle)
                );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ContactDetailsHeaderCell, ContactDetailsHeaderSection>();

                set.Bind(contactName)
                    .To(x => x.DataContext.Details.FullName);
                
                set.Bind(this)
                    .For(x => x.Contact)
                    .To(x => x.DataContext.Details);

                set.Bind(phoneNumberContent)
                    .To(x => x.DataContext.Details.PhoneNumber)
                    .WithConversion(new GenericValueConverter<string, string>(x =>
                    {
                        if (!string.IsNullOrEmpty(x))
                            return x;

                        return TextResources.ContactDetailsViewModel_UserHasNoPhoneNumber;
                    }));
                
                set.Apply();
            });
        }
        
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