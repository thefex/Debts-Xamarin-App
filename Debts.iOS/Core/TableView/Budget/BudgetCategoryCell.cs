using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities;
using Debts.ViewModel.Budget;
using Foundation;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Plugin.Color.Platforms.Ios;
using UIKit;

namespace Debts.iOS.Cells.Budget
{
    public class BudgetCategoryCell : MvxBaseTableViewCell
    {
        private UIImageView categoryImageView;
        private UILabel titleView;
        private InkTouchController inkTouchController;
        private const int categoryImageSize = 48;
        private TextToSearchAttributedTextValueConverter _textToSearchAttributedTextValueConverter = new TextToSearchAttributedTextValueConverter();

        public BudgetCategoryCell(IntPtr handle) : base(handle)
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
            
            titleView = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(19, UIFontWeight.Regular),
                TextColor = AppColors.BlackTextColor
            };
             
            ContentView.Add(categoryImageView);
            ContentView.Add(titleView); 
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            inkTouchController = new InkTouchController(ContentView);
            inkTouchController.AddInkView();

            ContentView.AddConstraints(
                    categoryImageView.AtTopOf(ContentView, 12),
                    categoryImageView.AtLeftOf(ContentView, 12),
                    categoryImageView.Width().EqualTo(categoryImageSize),
                    categoryImageView.Height().EqualTo(categoryImageSize),
                    categoryImageView.AtBottomOf(ContentView, 12),
                    
                    titleView.ToRightOf(categoryImageView, 12),
                    titleView.AtRightOf(ContentView, 12),
                    titleView.WithSameCenterY(categoryImageView)
            );
            
 
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<BudgetCategoryCell, BudgetCategory>();

                set.Bind(this)
                    .For(x => x.Name)
                    .To(x => x.Name);

                set.Bind(this)
                    .For(x => x.BudgetAssetName)
                    .To(x => x.AssetName);

                set.Bind(this)
                    .For(x => x.ColorHex)
                    .To(x => x.ColorHex);
                
                set.Apply();
            });
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

        private string _name;
        public string Name 
        {
            get => _name;
            set
            {
                _name = value ?? string.Empty;
                _textToSearchAttributedTextValueConverter .RecentSearchWord =
                    (base.ParentViewModel as PickBudgetCategoryViewModel).RecentSearchWord;
                titleView.AttributedText = _textToSearchAttributedTextValueConverter.Convert(_name, null, null, null) as NSAttributedString;
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