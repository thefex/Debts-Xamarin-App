using System;
using Airbnb.Lottie;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.Resources;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using UIKit;

namespace Debts.iOS.Core.TableView.Sections
{
    public class HeaderLikeEmptyNoteCell : MvxBaseTableViewCell
    {
        public HeaderLikeEmptyNoteCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            
            UILabel noteTitle = new UILabel()
            {
                Font = UIFont.PreferredTitle2,
                TextColor = AppColors.BlackTextColor,
                Text = TextResources.ViewModel_FinancesDetailsViewModel_Notes
            };
            
            LOTAnimationView emptyNotesAnimation = LOTAnimationView.AnimationNamed("default_empty_list");
            emptyNotesAnimation.ContentMode = UIViewContentMode.ScaleAspectFit;
            emptyNotesAnimation.LoopAnimation = true;
            emptyNotesAnimation.ContentScaleFactor = 0.45f;
            emptyNotesAnimation.Play();
            
            ContentView.Add(noteTitle);
            ContentView.Add(emptyNotesAnimation);
             
            var noActivityLabel = new UILabel();
            noActivityLabel.Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular);
            noActivityLabel.Lines = 0;
            noActivityLabel.Text = TextResources.ViewModel_FinanceDetailsViewModel_NoNotes;
            noActivityLabel.TextColor = UIColor.FromRGB(154, 160, 169);
            noActivityLabel.TextAlignment = UITextAlignment.Center;
            
            ContentView.Add(noActivityLabel);
            
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            ContentView.AddConstraints(
                    noteTitle.AtLeftOf(ContentView, 18),
                    noteTitle.AtTopOf(ContentView, 12),
                    noteTitle.AtRightOf(ContentView, 12),
                    
                    emptyNotesAnimation.Below(noteTitle, 18),
                    emptyNotesAnimation.WithSameCenterX(ContentView),
                    emptyNotesAnimation.Height().EqualTo(128),
                    
                    noActivityLabel.WithSameCenterX(ContentView),
                    noActivityLabel.Below(emptyNotesAnimation),
                    noActivityLabel.AtBottomOf(ContentView, 36)
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<HeaderLikeEmptyNoteCell, EmptyNotesSection>();

                set.Bind(noteTitle)
                    .To(x => x.DetailsNotesSection.ItemsCount)
                    .WithConversion(new GenericValueConverter<int, string>(itemCount => itemCount == 0
                        ? TextResources.ViewModel_FinancesDetailsViewModel_Notes
                        : TextResources.ViewModel_FinancesDetailsViewModel_Notes + " (" + itemCount + ")"));
                
                set.Apply();
            });
        }
    }
}