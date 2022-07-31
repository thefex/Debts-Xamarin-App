using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities;
using Debts.Resources;
using Debts.ViewModel;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Core.TableView.Sections
{
    public class FinanceDetailsHeaderCell : MvxBaseTableViewCell
    {
        public FinanceDetailsHeaderCell(IntPtr ptr) : base(ptr)
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
                Text = TextResources.FinanceDetailsViewModel_General
            };

            avatarView = new UIImageView()
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            UILabel contactName = new UILabel();
            UILabel secondaryTitle = new UILabel()
            {
                Text = TextResources.FinanceDetailsViewModel_Title
            };
            UILabel priceTitle = new UILabel()
            {
                Text = TextResources.FinanceDetailsViewModel_Price,
                Font = UIFont.PreferredTitle3
            };

            avatarView.UserInteractionEnabled = true;
            avatarView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                (ParentViewModel as FinanceDetailsViewModel)?.TransferToContact.Execute();
            }));

            contactName.UserInteractionEnabled = true;
            contactName.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                (ParentViewModel as FinanceDetailsViewModel)?.TransferToContact.Execute();
            }));
            
            contactName.Font = secondaryTitle.Font = priceTitle.Font;
            contactName.TextColor = secondaryTitle.TextColor = priceTitle.TextColor = mainTitle.TextColor;
            
            UILabel operationTitle = new UILabel();
            UILabel operationPrice = new UILabel();

            operationTitle.TextColor = operationPrice.TextColor = AppColors.GrayForTextFieldContainer;
            operationTitle.Font = operationPrice.Font = UIFont.PreferredSubheadline;

            UIImageView emotionImageView = new UIImageView()
            {
                ContentMode = UIViewContentMode.Center
            };

            UIView titleIconView = new UIView()
            {
                Layer = { CornerRadius = iconSize / 2, BorderWidth = 6, BorderColor = UIColor.FromRGB(87, 10, 235).CGColor},
                BackgroundColor = UIColor.Clear
            };
            
            UIImageView titleImageView = new UIImageView(UIImage.FromBundle("note").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate))
            {
                ContentMode = UIViewContentMode.Center,
                TintColor = UIColor.FromCGColor(titleIconView.Layer.BorderColor),
                BackgroundColor = UIColor.FromRGB(234, 234, 234),
                Layer = { CornerRadius = (iconSize-12*2)/2, MasksToBounds = true}
            };
            
            titleIconView.Add(titleImageView);
            titleIconView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            titleIconView.AddConstraints(
                    titleImageView.AtTopOf(titleIconView, 12),
                    titleImageView.AtLeftOf(titleIconView, 12),
                    titleImageView.AtRightOf(titleIconView, 12),
                    titleImageView.AtBottomOf(titleIconView, 12)
                );
            
            UIView priceIconView = new UIView()
            {
                Layer = { CornerRadius = iconSize / 2, BorderWidth = 6, BorderColor = UIColor.FromRGB(0, 0, 183).CGColor},
                BackgroundColor = UIColor.Clear
            };
            
            UIImageView priceImageView = new UIImageView(UIImage.FromBundle("cash").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate))
            {
                ContentMode = UIViewContentMode.Center,
                TintColor = UIColor.FromCGColor(priceIconView.Layer.BorderColor),
                BackgroundColor = UIColor.FromRGB(234, 234, 234),
                Layer = { CornerRadius = (iconSize-12*2)/2, MasksToBounds = true}
            };
            
            priceIconView.Add(priceImageView);
            priceIconView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            priceIconView.AddConstraints(
                    priceImageView.AtTopOf(priceIconView, 12),
                    priceImageView.AtLeftOf(priceIconView, 12),
                    priceImageView.AtRightOf(priceIconView, 12),
                    priceImageView.AtBottomOf(priceIconView, 12)
                );
            
            ContentView.Add(mainTitle);
            ContentView.Add(avatarView);
            ContentView.Add(contactName);
            ContentView.Add(titleIconView);
            ContentView.Add(secondaryTitle);
            ContentView.Add(operationTitle);
            ContentView.Add(priceIconView);
            ContentView.Add(priceTitle);
            ContentView.Add(operationPrice);
            ContentView.Add(emotionImageView);
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
                    
                    titleIconView.WithSameLeft(avatarView),
                    titleIconView.Below(avatarView, 12),
                    titleIconView.Width().EqualTo(iconSize),
                    titleIconView.Height().EqualTo(iconSize),
                    
                    secondaryTitle.WithSameTop(titleIconView).Plus(6),
                    secondaryTitle.ToRightOf(titleIconView, 12),
                    secondaryTitle.AtRightOf(ContentView, 12),
                    
                    operationTitle.Below(secondaryTitle, 3),
                    operationTitle.WithSameLeft(secondaryTitle),
                    operationTitle.WithSameRight(secondaryTitle),
                    
                    priceIconView.WithSameLeft(titleIconView),
                    priceIconView.Below(titleIconView, 12),
                    priceIconView.Width().EqualTo(iconSize),
                    priceIconView.Height().EqualTo(iconSize),
                    priceIconView.AtBottomOf(ContentView, 12),
                    
                    priceTitle.ToRightOf(priceIconView, 12),
                    priceTitle.WithSameTop(priceIconView).Plus(6),
                    priceTitle.AtRightOf(ContentView, 12),
                    
                    operationPrice.Below(priceTitle, 3),
                    operationPrice.WithSameLeft(priceTitle),
                    operationPrice.AtRightOf(priceTitle),
                    
                    emotionImageView.AtRightOf(ContentView, 12),
                    emotionImageView.Width().EqualTo(iconSize),
                    emotionImageView.Height().EqualTo(iconSize),
                    emotionImageView.WithSameTop(priceIconView),
                    emotionImageView.WithSameBottom(priceIconView)
                );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FinanceDetailsHeaderCell, FinanceDetailsHeaderSection>();

                set.Bind(contactName)
                    .To(x => x.DataContext.Details.RelatedTo.FullName);

                set.Bind(operationTitle)
                    .To(x => x.DataContext.Details.Title);

                set.Bind(operationPrice)
                    .To(x => x.DataContext.Details.PaymentDetails.FormattedAmount);

                set.Bind(emotionImageView)
                    .For(x => x.Image)
                    .To(x => x.DataContext.Details.PaymentDetails)
                    .WithConversion(new PaymentDateToImageValueConverter());

                set.Bind(emotionImageView)
                    .For(x => x.TintColor)
                    .To(x => x.DataContext.Details.PaymentDetails)
                    .WithConversion(new PaymentDateToTintColorValueConverter());

                set.Bind(this)
                    .For(x => x.Contact)
                    .To(x => x.DataContext.Details.RelatedTo);
                
                set.Apply();
            });
        }
        
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