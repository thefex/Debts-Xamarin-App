using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.TableView.Menu;
using Debts.iOS.ViewControllers.Menu;
using Debts.iOS.ViewModels;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace Debts.iOS.Core.TableView.Menu
{
    public class PremiumMenuItemCell  : MvxTableViewCell
    {
        public PremiumMenuItemCell(IntPtr ptr) : base(ptr)
        {
            InitializeCell();
        }

        void InitializeCell()
        {
             UILabel nameLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular),
                TextColor = AppColors.AppInfoOrange
            };
             
             var grayColor = UIColor.FromRGB(165, 165, 165);
             UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = grayColor,
                Lines = 0
            };
             
            iconImageView = new UIImageView()
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                TintColor = AppColors.AppInfoOrange
            };
            
            
            ContentView.Add(iconImageView);

            UIView container = new UIView();
            ContentView.Add(container);
            container.Add(nameLabel);
            container.Add(contentLabel);
            
            container.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            container.AddConstraints(
                    nameLabel.AtTopOf(container),
                    nameLabel.AtLeftOf(container),
                    nameLabel.AtRightOf(container),
                    
                    contentLabel.Below(nameLabel, 3),
                    contentLabel.AtLeftOf(container),
                    contentLabel.AtRightOf(container),
                    contentLabel.AtBottomOf(container)
                );
            
            ContentView.AddConstraints(
                iconImageView.WithSameCenterY(container),
                iconImageView.AtLeftOf(ContentView, 12),
                iconImageView.Width().EqualTo(48),
                
                container.AtTopOf(ContentView, 0),
                container.AtBottomOf(ContentView, 18),
                container.ToRightOf(iconImageView, 6),
                container.AtRightOf(ContentView, 18)
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<PremiumMenuItemCell, PremiumMenuViewController.PremiumMenuItem>();
                
                set.Bind(nameLabel)
                    .To(x => x.Title);

                set.Bind(contentLabel)
                    .To(x => x.Subtitle);
                
                set.Bind(this)
                    .For(x => x.AssetName)
                    .To(x =>x.Icon);
                
                set.Apply();
            });
        }

        private string _assetName;
        private UIImageView iconImageView;

        public string AssetName
        {
            get => _assetName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _assetName = value;
                    iconImageView.Image = UIImage.FromBundle(value).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                }
            }
        }
    }
}