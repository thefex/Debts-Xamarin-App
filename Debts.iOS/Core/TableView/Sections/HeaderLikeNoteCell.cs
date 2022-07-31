using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.Resources;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Core.TableView.Sections
{
    public class HeaderLikeNoteCell : MvxBaseTableViewCell
    {
        public HeaderLikeNoteCell(IntPtr handle) : base(handle)
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
       
            ContentView.Add(noteTitle);  
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            ContentView.AddConstraints(
                noteTitle.AtLeftOf(ContentView, 18),
                noteTitle.AtTopOf(ContentView, 12),
                noteTitle.AtRightOf(ContentView, 12),
                noteTitle.AtBottomOf(ContentView, 12)
                
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<HeaderLikeNoteCell, HeaderNotesSection>();

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