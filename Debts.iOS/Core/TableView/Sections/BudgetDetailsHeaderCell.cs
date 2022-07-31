using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities;
using Debts.Resources;
using Debts.ViewModel;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Plugin.Color.Platforms.Ios;
using UIKit;

namespace Debts.iOS.Core.TableView.Sections
{
    public class BudgetDetailsHeaderCell : MvxBaseTableViewCell
    {
        public BudgetDetailsHeaderCell(IntPtr ptr) : base(ptr)
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

            categoryImageView = new UIImageView()
            {
                ContentMode = UIViewContentMode.Center,
                TintColor = UIColor.White,
                Layer = { CornerRadius = iconSize/2f }
            };

            UILabel categoryName = new UILabel();
            UILabel title = new UILabel()
            {
                Text = TextResources.FinanceDetailsViewModel_Title
            };
            UILabel priceTitle = new UILabel()
            {
                Text = TextResources.FinanceDetailsViewModel_Price,
                Font = UIFont.PreferredTitle3
            };
 
            
            categoryName.Font = title.Font = priceTitle.Font;
            categoryName.TextColor = title.TextColor = priceTitle.TextColor = mainTitle.TextColor;
            
            UILabel operationTitle = new UILabel();
            UILabel operationPrice = new UILabel();

            operationTitle.TextColor = operationPrice.TextColor = AppColors.GrayForTextFieldContainer;
            operationTitle.Font = operationPrice.Font = UIFont.PreferredSubheadline;
  
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
            
            var operationTypeIconView = BuildIconView(AppColors.Primary, "cash");
            
            UILabel operationTypeTitle = new UILabel()
            {
                Text = TextResources.FinanceDetailsViewModel_OperationType,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            UILabel operationTypeContent = new UILabel()
            {
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            
            var createdDateIconView = BuildIconView(AppColors.AppInfoOrange, "filter_date");
            
            UILabel createdDateTitle = new UILabel()
            {
                Text = TextResources.FinanceDetailsViewModel_CreatedDate,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            UILabel createdDateContent = new UILabel()
            { 
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            UILabel createdDateAgo = new UILabel()
            { 
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            
            operationTypeTitle.Font = createdDateTitle.Font = UIFont.PreferredTitle3;
            
            operationTypeTitle.TextColor = createdDateTitle.TextColor = AppColors.BlackTextColor;
            
            operationTypeContent.TextColor = createdDateContent.TextColor = createdDateAgo.TextColor = AppColors.GrayForTextFieldContainer;
            
            operationTypeContent.Font = createdDateContent.Font = createdDateAgo.Font = UIFont.PreferredSubheadline;
            
            ContentView.Add(mainTitle);
            ContentView.Add(categoryImageView);
            ContentView.Add(categoryName);
            ContentView.Add(titleIconView);
            ContentView.Add(title);
            ContentView.Add(operationTitle);
            ContentView.Add(priceIconView);
            ContentView.Add(priceTitle);
            ContentView.Add(operationPrice);
            ContentView.Add(operationTypeIconView);
            ContentView.Add(operationTypeTitle);
            ContentView.Add(operationTypeContent);
            ContentView.Add(createdDateIconView);
            ContentView.Add(createdDateTitle);
            ContentView.Add(createdDateContent);
            ContentView.Add(createdDateAgo);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            ContentView.AddConstraints(
                mainTitle.AtTopOf(ContentView, 12),
                mainTitle.AtLeadingOf(ContentView, 18),
                mainTitle.AtTrailingOf(ContentView, 12),
                    
                categoryImageView.WithSameLeft(mainTitle),
                categoryImageView.Below(mainTitle, 12),
                categoryImageView.Width().EqualTo(iconSize),
                categoryImageView.Height().EqualTo(iconSize),
                    
                categoryName.ToRightOf(categoryImageView, 12),
                categoryName.AtRightOf(ContentView, 12),
                categoryName.WithSameCenterY(categoryImageView),
                    
                titleIconView.WithSameLeft(categoryImageView),
                titleIconView.Below(categoryImageView, 12),
                titleIconView.Width().EqualTo(iconSize),
                titleIconView.Height().EqualTo(iconSize),
                    
                title.WithSameTop(titleIconView).Plus(6),
                title.ToRightOf(titleIconView, 12),
                title.AtRightOf(ContentView, 12),
                    
                operationTitle.Below(title, 3),
                operationTitle.WithSameLeft(title),
                operationTitle.WithSameRight(title),
                    
                priceIconView.WithSameLeft(titleIconView),
                priceIconView.Below(titleIconView, 12),
                priceIconView.Width().EqualTo(iconSize),
                priceIconView.Height().EqualTo(iconSize),
                    
                priceTitle.ToRightOf(priceIconView, 12),
                priceTitle.WithSameTop(priceIconView).Plus(6),
                priceTitle.AtRightOf(ContentView, 12),
                    
                operationPrice.Below(priceTitle, 3),
                operationPrice.WithSameLeft(priceTitle),
                operationPrice.AtRightOf(priceTitle),
                
                operationTypeIconView.Below(priceIconView, 12),
                operationTypeIconView.WithSameLeft(mainTitle),
                operationTypeIconView.Width().EqualTo(60),
                operationTypeIconView.Height().EqualTo(60),
                operationTypeTitle.ToRightOf(operationTypeIconView, 12),
                operationTypeTitle.WithSameTop(operationTypeIconView).Plus(6),
                operationTypeTitle.AtRightOf(ContentView, 12),
                operationTypeContent.Below(operationTypeTitle, 6),
                operationTypeContent.WithSameLeft(operationTypeTitle),
                operationTypeContent.WithSameRight(operationTypeTitle),
                    
                createdDateIconView.Below(operationTypeIconView, 20),
                createdDateIconView.WithSameLeft(mainTitle),
                createdDateIconView.Width().EqualTo(60),
                createdDateIconView.Height().EqualTo(60),
                createdDateTitle.ToRightOf(createdDateIconView, 12),
                createdDateTitle.WithSameTop(createdDateIconView).Plus(-10),
                createdDateTitle.AtRightOf(ContentView, 12),
                createdDateContent.Below(createdDateTitle, 6),
                createdDateContent.WithSameLeft(createdDateTitle),
                createdDateContent.WithSameRight(createdDateTitle),
                createdDateAgo.Below(createdDateContent, 6),
                createdDateAgo.WithSameLeading(createdDateContent),
                createdDateAgo.WithSameTrailing(createdDateContent),
                
                createdDateIconView.AtBottomOf(ContentView, 12)
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<BudgetDetailsHeaderCell, BudgetDetailsHeaderSection>();

                set.Bind(categoryName)
                    .To(x => x.DataContext.Item.Category.Name);

                set.Bind(operationTitle)
                    .To(x => x.DataContext.Item.Title);

                set.Bind(operationPrice)
                    .To(x => x.DataContext.Item.FormattedAmount);
                 
                set.Bind(this)
                    .For(x => x.ColorHex)
                    .To(x => x.DataContext.Item.Category.ColorHex);

                set.Bind(this)
                    .For(x => x.BudgetAssetName)
                    .To(x => x.DataContext.Item.Category.AssetName);
                
                set.Bind(operationTypeContent)
                    .To(x => x.DataContext.Item.Type);

                set.Bind(createdDateContent)
                    .To(x => x.DataContext.Item.CreatedAt)
                    .WithConversion(new DateToTextValueConverter());

                set.Bind(createdDateAgo)
                    .To(x => x.DataContext.Item.CreatedAt)
                    .WithConversion(new HumanizedDateValueConverter());

                set.Apply();
            });
        }
        
        private UIImageView categoryImageView;
        private string _colorHex;

        public string ColorHex
        {
            get => _colorHex;
            set
            {
                _colorHex = value;
                categoryImageView.BackgroundColor = MvvmCross.Plugin.Color.MvxHexParser.ColorFromHexString(value).ToNativeColor();
            }
        }

        string _budgetAssetName;
        public string BudgetAssetName
        {
            get => _budgetAssetName;
            set
            {
                _budgetAssetName = value;
                categoryImageView.Image = UIImage.FromBundle(_budgetAssetName).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }
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
    }
}