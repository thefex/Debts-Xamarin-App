using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Utilities;
using Debts.Resources;
using FPT.Framework.iOS.UI.DropDown;
using UIKit;

namespace Debts.iOS.Core.TableView.Sections
{
    public abstract class StatisticsCell : MvxBaseTableViewCell
    {
        protected UILabel mainTitle;
        protected UILabel amountTitle;
        protected UILabel amountContent;
        protected UILabel totalTitle;
        protected UILabel totalContent;
        protected UILabel remainingTitle;
        protected UILabel remainingContent;
        protected UILabel collectedTitle;
        protected UILabel collectedContent;

        protected StatisticsCell(IntPtr ptr) : base(ptr)
        {
            Initialize();
        }

        void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            int iconSize = 60;
            mainTitle = new UILabel()
            {
                Font = UIFont.PreferredTitle2,
                TextColor = AppColors.BlackTextColor,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            
            amountContent = new UILabel()
            {
                TextColor = AppColors.GrayForTextFieldContainer,
                Font = UIFont.PreferredSubheadline,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            amountTitle = new UILabel()
            {
                TextColor = AppColors.BlackTextColor,
                Font = UIFont.PreferredTitle2,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            totalContent = new UILabel()
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            totalTitle = new UILabel()
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            remainingContent = new UILabel()
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            remainingTitle = new UILabel()
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            collectedContent = new UILabel()
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            collectedTitle = new UILabel()
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            totalTitle.Font = remainingTitle.Font = collectedTitle.Font = amountTitle.Font;
            totalTitle.TextColor = remainingTitle.TextColor = collectedTitle.TextColor = amountTitle.TextColor;
            
            totalContent.Font = remainingContent.Font = collectedContent.Font = amountContent.Font;
            totalContent.TextColor = remainingContent.TextColor = collectedContent.TextColor = amountContent.TextColor; 
            
            UIView amountIconView = BuildIconView(AppColors.Primary, "statistics");
            UIView totalIconView = BuildIconView(AppColors.AppInfoOrange, "credit_card");
            UIView remainingIconView = BuildIconView(AppColors.AppRed, "cash");
            UIView collectedIconView = BuildIconView(AppColors.SuccessColor, "cash");

            var amountSection = BuildSubsectionView(amountIconView,
                amountTitle,
                amountContent);

            var totalSection = BuildSubsectionView(totalIconView,
                totalTitle,
                totalContent);

            var remainingSection = BuildSubsectionView(remainingIconView,
                remainingTitle,
                remainingContent);

            var collectedSection = BuildSubsectionView(collectedIconView,
                collectedTitle,
                collectedContent); 
            
            ContentView.AddSubviews(
                    mainTitle,
                    amountSection,
                    totalSection,
                    remainingSection,                    
                    collectedSection
                );
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            ContentView.AddConstraints(
                    mainTitle.AtLeftOf(ContentView, 18),
                    mainTitle.AtTopOf(ContentView, 12),
                    mainTitle.AtRightOf(ContentView, 12),
                    
                    amountSection.AtLeftOf(ContentView),
                    amountSection.Below(mainTitle, 12),
                    amountSection.AtRightOf(ContentView),
                    
                    totalSection.AtLeftOf(ContentView),
                    totalSection.Below(amountSection, 12),
                    totalSection.AtRightOf(ContentView),
                    
                    remainingSection.AtLeftOf(ContentView),
                    remainingSection.Below(totalSection, 12),
                    remainingSection.AtRightOf(ContentView),
                    
                    collectedSection.AtLeftOf(ContentView),
                    collectedSection.Below(remainingSection, 12),
                    collectedSection.AtRightOf(ContentView),
                    collectedSection.AtBottomOf(ContentView, 12)
                );
            
            SetupBindings();
        } 

        UIView BuildIconView(UIColor tintColor, string imageName)
        {
            int iconSize = 60;
            UIView iconView = new UIView()
            {
                Layer = { CornerRadius = iconSize / 2, BorderWidth = 6, BorderColor = tintColor.CGColor},
                BackgroundColor = UIColor.Clear
            };
            
            UIImageView imageView = new UIImageView(UIImage.FromBundle(imageName).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate))
            {
                ContentMode = UIViewContentMode.Center,
                TintColor = UIColor.FromCGColor(iconView.Layer.BorderColor),
                BackgroundColor = UIColor.FromRGB(234, 234, 234),
                Layer = { CornerRadius = (iconSize-12*2)/2, MasksToBounds = true}
            };
            
            iconView.Add(imageView);
            iconView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            iconView.AddConstraints(
                imageView.AtTopOf(iconView, 12),
                imageView.AtLeftOf(iconView, 12),
                imageView.AtRightOf(iconView, 12),
                imageView.AtBottomOf(iconView, 12)
            );

            return iconView;
        }

        UIView BuildSubsectionView(UIView iconView, UIView titleView, UIView content)
        {
            var container = new UIView();
            
            container.AddSubviews(iconView, titleView, content);
            container.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            container.AddConstraints(
                    iconView.WithSameCenterY(container),
                    iconView.AtLeftOf(container, 12),
                    iconView.Width().EqualTo(60),
                    iconView.Height().EqualTo(60),
                    
                    titleView.AtTopOf(container, 6),
                    titleView.ToRightOf(iconView, 12),
                    titleView.AtRightOf(container, 12),
                    
                    content.Below(titleView, 3),
                    content.WithSameLeft(titleView),
                    content.WithSameRight(titleView),
                    content.AtBottomOf(container, 6)
                );

            return container;
        }

        protected abstract void SetupBindings();
    }
}