using System;
using Airbnb.Lottie;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Sections;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.Resources;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Core.TableView.Contacts
{
    public class FinanceOperationSectionHeaderLikeEmptyCell : MvxBaseTableViewCell
    {
        public FinanceOperationSectionHeaderLikeEmptyCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            
            UILabel headerTitle = new UILabel()
            {
                Font = UIFont.PreferredTitle2,
                TextColor = AppColors.BlackTextColor,
            };
            
            LOTAnimationView emptyNotesAnimation = LOTAnimationView.AnimationNamed("default_empty_list");
            emptyNotesAnimation.ContentMode = UIViewContentMode.ScaleAspectFit;
            emptyNotesAnimation.LoopAnimation = true;
            emptyNotesAnimation.ContentScaleFactor = 0.45f;
            emptyNotesAnimation.Play();
            
            ContentView.Add(headerTitle);
            ContentView.Add(emptyNotesAnimation);
             
            var noActivityLabel = new UILabel();
            noActivityLabel.Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular);
            noActivityLabel.Lines = 0;
            noActivityLabel.Text = TextResources.EmptyFinanceSection_Text;
            noActivityLabel.TextColor = UIColor.FromRGB(154, 160, 169);
            noActivityLabel.TextAlignment = UITextAlignment.Center;
            
            ContentView.Add(noActivityLabel);
            
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            ContentView.AddConstraints(
                    headerTitle.AtLeftOf(ContentView, 18),
                    headerTitle.AtTopOf(ContentView, 12),
                    headerTitle.AtRightOf(ContentView, 12),
                    
                    emptyNotesAnimation.Below(headerTitle, 18),
                    emptyNotesAnimation.WithSameCenterX(ContentView),
                    emptyNotesAnimation.Height().EqualTo(128),
                    
                    noActivityLabel.WithSameCenterX(ContentView),
                    noActivityLabel.Below(emptyNotesAnimation),
                    noActivityLabel.AtBottomOf(ContentView, 36)
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FinanceOperationSectionHeaderLikeEmptyCell, EmptyFinanceOperationSection>();

                set.Bind(headerTitle)
                    .To(x => x.OperationsSection.Title);
                set.Apply();
            });
        }
    }
}