using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities;
using Debts.ViewModel.Contacts;
using Debts.ViewModel.FinancesViewModel;
using Foundation;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Cells
{
    public class FinanceOperationCell : MvxBaseTableViewCell
    {
        private InkTouchController inkTouchController;
        
        private const int avatarSize = 52;
        TextToSearchAttributedTextValueConverter textToSearchAttributedTextValueConverter = new TextToSearchAttributedTextValueConverter();

        private UIImageView avatarView;
        private UILabel title;
        private UILabel priceLabel;
        
        public FinanceOperationCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        void Initialize()
        {
            ContentView.BackgroundColor = UIColor.White;
            SelectionStyle = UITableViewCellSelectionStyle.Gray;

            avatarView = new UIImageView()
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            
            title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular),
                TextColor = AppColors.BlackTextColor
            };
            
            var padding = new UIEdgeInsets(3, 6, 3, 6);
            var cornerRadius = 9;
            UIPaddingLabel paymentLabel = new UIPaddingLabel()
            {
                Padding = padding,
                Layer = { CornerRadius = cornerRadius, MasksToBounds = true },
                BackgroundColor = AppColors.DebtColor,
                TextColor = UIColor.White,
                ClipsToBounds = true,
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Center
            };

            int priceLabelSize = avatarSize;
            priceLabel = new UILabel()
            {
                Layer = {CornerRadius = priceLabelSize / 2, MasksToBounds = true},
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(15, UIFontWeight.Regular),
                TextColor = UIColor.White
            };
            
        
            ContentView.Add(avatarView);
            ContentView.Add(title);
            ContentView.Add(paymentLabel);
            ContentView.Add(priceLabel);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            inkTouchController = new InkTouchController(ContentView);
            inkTouchController.AddInkView();

            ContentView.AddConstraints(
                    avatarView.AtTopOf(ContentView, 12),
                    avatarView.AtLeftOf(ContentView, 12),
                    avatarView.Width().EqualTo(avatarSize),
                    avatarView.Height().EqualTo(avatarSize),
                    avatarView.AtBottomOf(ContentView, 12),
                    
                    priceLabel.WithSameTop(avatarView),
                    priceLabel.WithSameBottom(avatarView),
                    priceLabel.WithSameWidth(avatarView),
                    priceLabel.WithSameHeight(avatarView),
                    priceLabel.AtRightOf(ContentView, 12),
                    
                    title.WithSameTop(avatarView),
                    title.ToRightOf(avatarView, 12),
                    title.AtRightOf(ContentView, 12+priceLabelSize+12),
                    
                    paymentLabel.WithSameLeft(title),
                    paymentLabel.Below(title, 6)
                );
            
 
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FinanceOperationCell, FinanceOperation>();

                set.Bind(this)
                    .For(x => x.OperationTitle)
                    .To(x => x.Title);

                set.Bind(this)
                    .For(x => x.Contact)
                    .To(x => x.RelatedTo);

                set.Bind(paymentLabel)
                    .To(x => x.PaymentDetails)
                    .WithConversion(new PaymentDetailsToTextValueConverter());

                set.Bind(paymentLabel)
                    .For(x => x.BackgroundColor)
                    .To(x => x.PaymentDetails)
                    .WithConversion(new PaymentDetailsToBackgroundColorValueConverter());

                set.Bind(this)
                    .For(x => x.PriceAmount)
                    .To(x => x.PaymentDetails.Amount);

                set.Bind(priceLabel)
                    .For(x => x.BackgroundColor)
                    .To(x => x.Type)
                    .WithConversion(new FinanceOperationTypeToBackgroundColor());
                
                
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
                    (base.ParentViewModel as BaseFinancesViewModel)?.RecentSearchWord ?? string.Empty;

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
                    (base.ParentViewModel as BaseFinancesViewModel)?.RecentSearchWord ?? string.Empty;
                title.AttributedText = textToSearchAttributedTextValueConverter.Convert(_operationTitle, null, null, null) as NSAttributedString;
            }
        }

        private ContactDetails _contact; 

        public ContactDetails Contact
        {
            get => _contact;
            set
            {
                _contact = value;
                new AvatarGenerator().GenerateAvatar(avatarView, value?.ToString() ?? "?", value?.AvatarUrl, 15, avatarSize, UIColor.White, UIColor.Gray);
            }
        }
    }
}