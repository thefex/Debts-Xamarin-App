using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace Debts.iOS.TableView.Menu
{
    public sealed class MenuItemCell : MvxTableViewCell
    {
        public MenuItemCell(IntPtr ptr) : base(ptr)
        {
            InitializeCell();
        }

        void InitializeCell()
        {
            var grayColor = UIColor.FromRGB(165, 165, 165);
            UILabel nameLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = grayColor
            };
            iconImageView = new UIImageView()
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            iconImageView.TintColor = nameLabel.TextColor;
            
            ContentView.Add(iconImageView);
            ContentView.Add(nameLabel);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            ContentView.AddConstraints(
                iconImageView.AtTopOf(ContentView, 6),
                iconImageView.AtBottomOf( ContentView, 12),
                iconImageView.AtLeftOf(ContentView, 22),
                iconImageView.Width().LessThanOrEqualTo(36),
                
                nameLabel.WithSameCenterY(iconImageView),
                nameLabel.ToRightOf(iconImageView, 12),
                nameLabel.AtRightOf(ContentView, 36)
            );
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<MenuItemCell, MenuViewModel.MenuItem>();
                
                set.Bind(nameLabel)
                    .To(x => x.Name);
                
                set.Bind(this)
                    .For(x => x.AssetName)
                    .To(x =>x.Icon);
                
                set.Bind(nameLabel)
                    .For(x => x.TextColor)
                    .To(x => x.IsSelected)
                    .WithConversion(new BooleanToMethodProviderValueConverter<UIColor>(() => grayColor, () => AppColors.Primary));
                
                set.Bind(iconImageView)
                    .For(x => x.TintColor)
                    .To(x => x.IsSelected)
                    .WithConversion(new BooleanToMethodProviderValueConverter<UIColor>(() => grayColor, () => AppColors.Primary));
                
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
                    iconImageView.Image = UIImage.FromBundle(value);
                }
            }
        }
    }
}