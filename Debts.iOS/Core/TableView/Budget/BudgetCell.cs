using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities;
using Debts.ViewModel.Budget;
using Debts.ViewModel.FinancesViewModel;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Plugin.Color.Platforms.Ios;
using UIKit;

namespace Debts.iOS.Cells.Budget
{
    public class BudgetCell : MvxBaseTableViewCell
    {
       private InkTouchController inkTouchController;
        
        private const int categoryImageSize = 48;
        TextToSearchAttributedTextValueConverter textToSearchAttributedTextValueConverter = new TextToSearchAttributedTextValueConverter();

        private UIImageView categoryImageView;
        private UILabel title;
        private UILabel priceLabel;
        
        public BudgetCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        void Initialize()
        {
            ContentView.BackgroundColor = UIColor.White;
            SelectionStyle = UITableViewCellSelectionStyle.Gray;

            categoryImageView = new UIImageView()
            {
                ContentMode = UIViewContentMode.Center,
                TintColor = UIColor.White,
                Layer = { CornerRadius = categoryImageSize/2f }
            };
            
            title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular),
                TextColor = AppColors.BlackTextColor
            };
             
            int priceLabelSize = categoryImageSize;
            priceLabel = new UILabel()
            {
                Layer = {CornerRadius = priceLabelSize / 2, MasksToBounds = true},
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(15, UIFontWeight.Regular),
                TextColor = UIColor.White
            };
            
            var padding = new UIEdgeInsets(3, 6, 3, 6);
            var cornerRadius = 9;
            UIPaddingLabel categoryLabel = new UIPaddingLabel()
            {
                Padding = padding,
                Layer = { CornerRadius = cornerRadius, MasksToBounds = true },
                BackgroundColor = AppColors.DebtColor,
                TextColor = UIColor.White,
                ClipsToBounds = true,
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Center
            };
            
            ContentView.Add(categoryImageView);
            ContentView.Add(title);
            ContentView.Add(priceLabel);
            ContentView.Add(categoryLabel);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            inkTouchController = new InkTouchController(ContentView);
            inkTouchController.AddInkView();

            ContentView.AddConstraints(
                    categoryImageView.AtTopOf(ContentView, 12),
                    categoryImageView.AtLeftOf(ContentView, 12),
                    categoryImageView.Width().EqualTo(categoryImageSize),
                    categoryImageView.Height().EqualTo(categoryImageSize),
                    categoryImageView.AtBottomOf(ContentView, 12),
                    
                    priceLabel.WithSameTop(categoryImageView),
                    priceLabel.WithSameBottom(categoryImageView),
                    priceLabel.WithSameWidth(categoryImageView),
                    priceLabel.WithSameHeight(categoryImageView),
                    priceLabel.AtRightOf(ContentView, 12),
                    
                    title.WithSameTop(categoryImageView),
                    title.ToRightOf(categoryImageView, 12),
                    title.AtRightOf(ContentView, 12+priceLabelSize+12),
                    
                    categoryLabel.WithSameLeft(title),
                    categoryLabel.Below(title, 6)
            );
            
 
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<BudgetCell, BudgetItem>();

                set.Bind(this)
                    .For(x => x.OperationTitle)
                    .To(x => x.Title);

                set.Bind(categoryLabel)
                    .To(x => x.Category.Name);

                set.Bind(this)
                    .For(x => x.ColorHex)
                    .To(x => x.Category.ColorHex);

                set.Bind(this)
                    .For(x => x.BudgetAssetName)
                    .To(x => x.Category.AssetName);
                
                set.Bind(this)
                    .For(x => x.PriceAmount)
                    .To(x => x.Amount);

                set.Bind(categoryLabel)
                    .For(x => x.BackgroundColor)
                    .To(x => x.Type)
                    .WithConversion(new BudgetCategoryTypeToBackgroundColor());
                
                set.Bind(priceLabel)
                    .For(x => x.BackgroundColor)
                    .To(x => x.Type)
                    .WithConversion(new BudgetCategoryTypeToBackgroundColor());
                
                set.Apply();
            });
        }

        private decimal _priceAmount;
        public decimal PriceAmount
        {
            get => _priceAmount;
            set 
            {
                _priceAmount=value;
                textToSearchAttributedTextValueConverter.RecentSearchWord =
                    (base.ParentViewModel as BudgetListViewModel)?.RecentSearchWord ?? string.Empty;

                string amountText = $"${_priceAmount}";

                if (_priceAmount > 999)
                    amountText = $"$999+";
                
                priceLabel.AttributedText = textToSearchAttributedTextValueConverter.Convert(amountText, null, null, null) as NSAttributedString;
            }
        } 
        private string _operationTitle;
        public string OperationTitle 
        {
            get => _operationTitle;
            set
            {
                _operationTitle = value ?? string.Empty;
                textToSearchAttributedTextValueConverter.RecentSearchWord =
                    (base.ParentViewModel as BudgetListViewModel)?.RecentSearchWord ?? string.Empty;
                title.AttributedText = textToSearchAttributedTextValueConverter.Convert(_operationTitle, null, null, null) as NSAttributedString;
            }
        }
        
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
    }
}