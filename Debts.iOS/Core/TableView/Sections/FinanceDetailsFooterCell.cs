using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Utilities;
using Debts.Resources;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Core.TableView.Sections
{
    public class FinanceDetailsFooterCell : MvxBaseTableViewCell
    {
        public FinanceDetailsFooterCell(IntPtr ptr) : base(ptr)
        {
            Initialize();
        }

        void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            UILabel mainTitle = new UILabel()
            {
                Font = UIFont.PreferredTitle2,
                TextColor = AppColors.BlackTextColor,
                Text = TextResources.FinanceDetailsViewModel_Details,
                LineBreakMode = UILineBreakMode.TailTruncation
            };

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

            var paymentDeadlineIconView = BuildIconView(AppColors.AppRed, "filter_date");

            UILabel paymentDeadlineTitle = new UILabel()
            {
                Text = TextResources.DateDialog_PaymentDeadline_Title,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            UILabel paymentDeadlineContent = new UILabel()
            {
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            UILabel paymentDeadlineWhen = new UILabel()
            {
                LineBreakMode = UILineBreakMode.TailTruncation
            };

            var createdAtLocationIconView = BuildIconView(AppColors.SuccessColor, "map_marker");

            UILabel createdAtLocationTitle = new UILabel()
            {
                Text = TextResources.FinanceDetailsViewModel_CreatedAtLocation, 
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            UILabel createdAtLocationContent = new UILabel()
            {
                Text = TextResources.FinanceDetailsViewModel_TapToSeeOnMap, 
                LineBreakMode = UILineBreakMode.TailTruncation
            };

            var paymentDateIconView = BuildIconView(AppColors.Pink, "credit_card");
            
            UILabel paymentDateTitle = new UILabel()
            {
                Text = TextResources.FinanceDetailsViewModel_PaymentDate, 
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            UILabel paymentDateContent = new UILabel()
            { 
                LineBreakMode = UILineBreakMode.TailTruncation
            };

            operationTypeTitle.Font = createdDateTitle.Font =
                paymentDeadlineTitle.Font = createdAtLocationTitle.Font = paymentDateTitle.Font = UIFont.PreferredTitle3;
            
            operationTypeTitle.TextColor = createdDateTitle.TextColor =
                paymentDeadlineTitle.TextColor = createdAtLocationTitle.TextColor = paymentDateTitle.TextColor = AppColors.BlackTextColor;

            operationTypeContent.TextColor = createdDateContent.TextColor = createdDateAgo.TextColor =
                paymentDeadlineContent.TextColor = paymentDeadlineWhen.TextColor = createdAtLocationContent.TextColor =
                    paymentDateContent.TextColor = AppColors.GrayForTextFieldContainer;
            
            operationTypeContent.Font = createdDateContent.Font = createdDateAgo.Font =
                paymentDeadlineContent.Font = paymentDeadlineWhen.Font = createdAtLocationContent.Font =
                    paymentDateContent.Font = UIFont.PreferredSubheadline;
             
            ContentView.AddSubviews(mainTitle,
                operationTypeIconView,
                operationTypeTitle, 
                operationTypeContent,
                createdDateIconView,
                createdDateTitle, 
                createdDateContent, 
                createdDateAgo,
                paymentDeadlineIconView,
                paymentDeadlineTitle,
                paymentDeadlineContent,
                paymentDeadlineWhen,
                createdAtLocationIconView,
                createdAtLocationTitle,
                createdAtLocationContent,
                paymentDateIconView,
                paymentDateTitle,
                paymentDateContent);

            createdAtLocationIconView.UserInteractionEnabled = true;
            createdAtLocationTitle.UserInteractionEnabled = true;
            createdAtLocationContent.UserInteractionEnabled = true;
            createdAtLocationTitle.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                (DataContext as FinanceDetailsFooterSection).DataContext.Map.Execute();
            }));
            createdAtLocationIconView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                (DataContext as FinanceDetailsFooterSection).DataContext.Map.Execute();
            }));
            createdAtLocationContent.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                (DataContext as FinanceDetailsFooterSection).DataContext.Map.Execute();
            }));
            
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            ContentView.AddConstraints(
                    mainTitle.AtLeftOf(ContentView, 18),
                    mainTitle.AtTopOf(ContentView, 12),
                    mainTitle.AtRightOf(ContentView, 12),
                    
                    operationTypeIconView.Below(mainTitle, 12),
                    operationTypeIconView.AtLeftOf(ContentView, 12),
                    operationTypeIconView.Width().EqualTo(60),
                    operationTypeIconView.Height().EqualTo(60),
                    operationTypeTitle.ToRightOf(operationTypeIconView, 12),
                    operationTypeTitle.WithSameTop(operationTypeIconView).Plus(6),
                    operationTypeTitle.AtRightOf(ContentView, 12),
                    operationTypeContent.Below(operationTypeTitle, 6),
                    operationTypeContent.WithSameLeft(operationTypeTitle),
                    operationTypeContent.WithSameRight(operationTypeTitle),
                    
                    createdDateIconView.Below(operationTypeIconView, 20),
                    createdDateIconView.AtLeftOf(ContentView, 12),
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
                    
                    paymentDeadlineIconView.Below(createdDateIconView, 36),
                    paymentDeadlineIconView.AtLeftOf(ContentView, 12),
                    paymentDeadlineIconView.Width().EqualTo(60),
                    paymentDeadlineIconView.Height().EqualTo(60),
                    paymentDeadlineTitle.ToRightOf(paymentDeadlineIconView, 12),
                    paymentDeadlineTitle.WithSameTop(paymentDeadlineIconView).Plus(-10),
                    paymentDeadlineTitle.AtRightOf(ContentView, 12),
                    paymentDeadlineContent.Below(paymentDeadlineTitle, 6),
                    paymentDeadlineContent.WithSameLeft(paymentDeadlineTitle),
                    paymentDeadlineContent.WithSameRight(paymentDeadlineTitle),
                    paymentDeadlineWhen.Below(paymentDeadlineContent, 6),
                    paymentDeadlineWhen.WithSameLeading(paymentDeadlineContent),
                    paymentDeadlineWhen.WithSameTrailing(paymentDeadlineContent),

                    createdAtLocationIconView.Below(paymentDeadlineIconView, 24),
                    createdAtLocationIconView.AtLeftOf(ContentView, 12),
                    createdAtLocationIconView.Width().EqualTo(60),
                    createdAtLocationIconView.Height().EqualTo(60),
                    createdAtLocationTitle.ToRightOf(createdAtLocationIconView, 12),
                    createdAtLocationTitle.WithSameTop(createdAtLocationIconView).Plus(6),
                    createdAtLocationTitle.AtRightOf(ContentView, 12),
                    createdAtLocationContent.Below(createdAtLocationTitle, 6),
                    createdAtLocationContent.WithSameLeft(createdAtLocationTitle),
                    createdAtLocationContent.WithSameRight(createdAtLocationTitle),

                    paymentDateIconView.Below(createdAtLocationIconView, 12),
                    paymentDateIconView.AtLeftOf(ContentView, 12),
                    paymentDateIconView.Width().EqualTo(60),
                    paymentDateIconView.Height().EqualTo(60),
                    paymentDateIconView.AtBottomOf(ContentView, 12),
                    paymentDateTitle.ToRightOf(paymentDateIconView, 12),
                    paymentDateTitle.WithSameTop(paymentDateIconView).Plus(6),
                    paymentDateTitle.AtRightOf(ContentView, 12),
                    paymentDateContent.Below(paymentDateTitle, 6),
                    paymentDateContent.WithSameLeft(paymentDateTitle),
                    paymentDateContent.WithSameRight(paymentDateTitle)
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FinanceDetailsFooterCell, FinanceDetailsFooterSection>();

                set.Bind(operationTypeContent)
                    .To(x => x.DataContext.Details.Type);

                set.Bind(createdDateContent)
                    .To(x => x.DataContext.Details.CreatedAt)
                    .WithConversion(new DateToTextValueConverter());

                set.Bind(createdDateAgo)
                    .To(x => x.DataContext.Details.CreatedAt)
                    .WithConversion(new HumanizedDateValueConverter());

                set.Bind(paymentDeadlineContent)
                    .To(x => x.DataContext.Details.PaymentDetails.DeadlineDate)
                    .WithConversion(new DateToTextValueConverter());

                set.Bind(paymentDeadlineWhen)
                    .To(x => x.DataContext.Details.PaymentDetails.DeadlineDate)
                    .WithConversion(new HumanizedDateValueConverter());

                set.Bind(paymentDateContent)
                    .To(x => x.DataContext.Details.PaymentDetails)
                    .WithConversion(new FinanceOperationPaymentDateTextValueConverter());
                
                set.Apply();
            });

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