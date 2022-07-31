using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.Converters.AppDomain;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Core.TableView.Sections
{
    public class NoteCell : MvxBaseTableViewCell
    {
        public NoteCell(IntPtr ptr) : base(ptr)
        {
            Initialize();
        }

        void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            var createdDateLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                TextColor = AppColors.StrongGray
            };
            var durationLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                TextColor = AppColors.StrongGray
            };
            var noteText = new UILabel()
            {
                Font = UIFont.PreferredBody,
                TextColor = AppColors.BlackTextColor,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            UIImageView typeIconImageView = new UIImageView()
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                TintColor = AppColors.StrongGray
            };
            ContentView.Add(createdDateLabel);
            ContentView.Add(durationLabel);
            ContentView.Add(typeIconImageView);
            ContentView.Add(noteText);
            
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            ContentView.AddConstraints(
                    createdDateLabel.AtTopOf(ContentView, 6),
                    createdDateLabel.AtLeftOf(ContentView, 18),
                    
                    typeIconImageView.AtRightOf(ContentView, 12),
                    typeIconImageView.Width().EqualTo(16),
                    typeIconImageView.Height().EqualTo(16),
                    typeIconImageView.WithSameCenterY(createdDateLabel),
                    
                    durationLabel.ToLeftOf(typeIconImageView, 4),
                    durationLabel.WithSameTop(createdDateLabel),
                    
                    noteText.Below(createdDateLabel, 2),
                    noteText.AtBottomOf(ContentView, 12),
                    noteText.WithSameLeading(createdDateLabel),
                    noteText.AtRightOf(ContentView, 12)
            );

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<NoteCell, Note>();

                set.Bind(createdDateLabel)
                    .To(x => x.CreatedAt)
                    .WithConversion(new HumanizedDateFullValueConverter());

                set.Bind(durationLabel)
                    .To(x => x.Duration)
                    .WithConversion(new DurationForNoteValueConverter());
            

                set.Bind(noteText)
                    .To(x => x.Text);

                set.Bind(typeIconImageView)
                    .For(x => x.Image)
                    .To(x => x.Type)
                    .WithConversion(new GenericValueConverter<NoteType, UIImage>(type =>
                    {
                        string assetName = string.Empty;
                        switch (type)
                        {
                            case NoteType.Call:
                                assetName = "phone";
                                break;
                            case NoteType.Default:
                                assetName = "note";
                                break;
                            case NoteType.Share:
                                assetName = "share";
                                break;
                            case NoteType.Sms:
                                assetName = "sms";
                                break;
                        }

                        return UIImage.FromBundle(assetName).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    }));
                
                set.Apply();
            });
        }
    }
}