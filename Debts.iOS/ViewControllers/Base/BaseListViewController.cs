using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Airbnb.Lottie;
using Cirrious.FluentLayouts.Touch;
using CoreFoundation;
using CoreGraphics;
using Debts.Converters;
using Debts.iOS.Config;
using Debts.iOS.Utilities;
using Debts.Messenging;
using Debts.Resources;
using Debts.Services;
using Debts.ViewModel;
using Foundation;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Views;
using UIKit;
using MvxBaseTableViewSource = Debts.iOS.Utilities.MvxBaseTableViewSource;

namespace Debts.iOS.ViewControllers.Base
{
    public abstract class BaseListViewController<TViewModel, TInitParams, TApiDataModel, TDataModel> :
        BaseViewController<TViewModel, TInitParams>
        where TViewModel : ListViewModel<TInitParams, TApiDataModel, TDataModel> where TInitParams : class where TDataModel : class
    {
        private readonly UIView _emptyListView = new UIView();
        private UIView _firstLoadingView;

        private UIView _loadMoreButton;
        private UIView _loadMoreProgress;
 
        private readonly UILabel _noActivityLabel = new UILabel
        {
            Lines = 0,
            LineBreakMode = UILineBreakMode.TailTruncation,
            TranslatesAutoresizingMaskIntoConstraints = false
        };
        
        protected bool IsEmptyListEnabled { get; set; } = true;
        
        public BaseListViewController()
        {
            
        }

        protected BaseListViewController(IntPtr handle) : base(handle)
        {
            
        }

        protected BaseListViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        { 
        }
 
        protected virtual UITableViewStyle TableViewStyle => UITableViewStyle.Plain;

        protected virtual bool IsGroupingSupported => false;

        protected virtual float TableViewYOffset => 0f;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
 
            TableView = new UITableView(TableFrame, TableViewStyle)
            {
                SeparatorColor = AppColors.GrayBackground,
                AllowsSelection = true,
                AllowsMultipleSelection = false,
                BackgroundColor = AppColors.GrayBackground,
                SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine,
                ContentInset = new UIEdgeInsets(0, 0, 82+16, 0),
                Alpha = 0
            };
            
            TableView.Source = GetTableViewSource();

            var tableViewContainer = new UIView();
            tableViewContainer.Add(TableView);
            View.Add(tableViewContainer);
            
            SetupFirstLoadingView();
            SetupEmptyListView();
            CreateBindings();

            TableView.Alpha = 0;
            _firstLoadingView.Alpha = 1;
            
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            tableViewContainer.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            tableViewContainer.AddConstraints(
                TableView.AtTopOf(tableViewContainer),
                TableView.AtBottomOf(tableViewContainer),
                TableView.AtRightOf(tableViewContainer),
                TableView.AtLeftOf(tableViewContainer)
            );
            
            View.AddConstraints(
                tableViewContainer.AtTopOf(View, TopTableViewMargin),
                tableViewContainer.WithSameLeft(View),
                tableViewContainer.WithSameRight(View),
                tableViewContainer.WithSameBottom(View)
            );
         
            TableView.ReloadData();
            TableView.DirectionalLockEnabled = true;

            (ViewModel as INotifyPropertyChanged).PropertyChanged += (e, a) =>
            {
                if (a.PropertyName == nameof(ViewModel.IsLoadingMoreDataEnabled))
                {
                    TableView.TableFooterView = GetFooterLoadMoreView();
                    UIView.Animate(0.3, 1,  UIViewAnimationOptions.CurveEaseOut, () =>
                        {
                            TableView.TableFooterView.Alpha = 1;
                        }, () => { });
                }
            };
        }

        protected virtual nfloat TopTableViewMargin { get; }= 0;

        private MvxTableViewSource GetTableViewSource()
        {
            if (IsGroupingSupported)
            {
                var groupedTableViewSource =  new MvxGroupedBaseTableViewSource(TableView, this);
                groupedTableViewSource.ParentViewControllerProvider = () => this;
                groupedTableViewSource.TemplateSelector = GetTemplateSelector() as MvxGroupedTemplateSelector;
                return groupedTableViewSource;
            }
            else
            {
                var tableViewSource =  new MvxBaseTableViewSource(TableView, this);
                tableViewSource.ParentViewControllerProvider = () => this;
                tableViewSource.TemplateSelector = GetTemplateSelector();
                return tableViewSource;
            }
        }  
        
        protected virtual CGRect TableFrame => View.Frame;

        protected virtual UIColor BackgroundListColor => UIColor.Gray;

        protected virtual bool HasSearchBar => false;

        private void CreateBindings()
        {
            var bindingSet =
                this.CreateBindingSet<BaseListViewController<TViewModel, TInitParams, TApiDataModel, TDataModel>, TViewModel>();

            PresentFirstLoadingView = true;
            
            var tableViewSource = TableView.Source as MvxTableViewSource;
            bindingSet.Bind(tableViewSource)
                .For(x => x.ItemsSource)
                .To(x => x.Items);
            
            bindingSet.Bind(tableViewSource)
                .For(x => x.SelectionChangedCommand)
                .To(x => x.ItemTapped);
 
            bindingSet.Bind(this)
                .For(x => x.PresentEmptyView)
                .To(x => x.HasAnyItems)
                .WithConversion(new BooleanNegationValueConverter());

            bindingSet.Bind(this)
                .For(x => x.PresentFirstLoadingView)
                .To(x => x.IsListLoaded)
                .WithConversion(new BooleanNegationValueConverter());

            bindingSet.Bind(this)
                .For(x => x.IsLoadMoreInProgress)
                .To(x => x.IsLoadingMoreData);
            
            bindingSet.Apply();
        }

        private bool isLoadMoreInProgress;

        public bool IsLoadMoreInProgress
        {
            get => isLoadMoreInProgress;
            set
            {
                if (isLoadMoreInProgress != value)
                {
                    isLoadMoreInProgress = value;

                    if (_loadMoreButton == null)
                        return;

                    _loadMoreButton.UserInteractionEnabled = !value;
                    UIView.Animate(0.5, () =>
                    {
                        _loadMoreButton.Alpha = value ? 0 : 1;
                        _loadMoreProgress.Alpha = value ? 1 : 0;
                    });
                    
                }
            }
        }

        private bool isPresentingFirstLoadingView;
        public bool PresentFirstLoadingView
        {
            get { return isPresentingFirstLoadingView; }
            set
            {
                if (isPresentingFirstLoadingView == value)
                    return;

                isPresentingFirstLoadingView = value;
                if (!value)
                {
                    System.Diagnostics.Debug.WriteLine("Show");
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        UIView.Animate(0.6f, 0.35f, UIViewAnimationOptions.CurveEaseOut, () =>
                        {
                            TableView.ScrollEnabled = true;
                            TableView.Alpha = 1f;
                            _firstLoadingView.Alpha = 0.0f;
                        }, () => { });    
                    });
                    
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Hide");
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        UIView.Animate(0.6f, 0.35f, UIViewAnimationOptions.CurveEaseOut, () =>
                        {
                            TableView.ScrollEnabled = false;
                            TableView.Alpha = 0;
                            _firstLoadingView.Alpha = 1f;
                        }, () => { }); 
                    });
                }
            }
        }

        public bool PresentEmptyView
        {
            get { throw new NotImplementedException(); }
            set
            {
                //f (!ViewModel.IsListLoaded)
                //    return;
                
                if (value && IsEmptyListEnabled && !TableView.Subviews.Contains(_emptyListView))
                {
                    TableView.Add(_emptyListView);
                    
                    if (TableView.TableHeaderView != null)
                        TableView.BringSubviewToFront(TableView.TableHeaderView);
                }
                else if (!value || !IsEmptyListEnabled)
                {
                    _emptyListView.RemoveFromSuperview();
                }
            }
        }
  
        private void SetupFirstLoadingView()
        {
            var progressView = LOTAnimationView.AnimationNamed("loader_animation");
            progressView.ContentMode = UIViewContentMode.ScaleAspectFit;
            progressView.ContentScaleFactor = 0.1f;
            progressView.LoopAnimation = true;
            progressView.Alpha = 0;
            nfloat desiredHeight = 64f / 400.0f * 300.0f;
            
            View.Add(progressView);
            progressView.Play();
            
            View.AddConstraints(
                progressView.Width().EqualTo(64),
                progressView.Height().EqualTo(desiredHeight),
                progressView.WithSameCenterY(View).Minus(48),
                progressView.WithSameCenterX(View)
            );

            _firstLoadingView = progressView;
        }
        
        private LOTAnimationView emptyViewRefresherProgressView;
        private UIButton emptyListButton;
        private InkTouchController loadMoreInkTouchController;
        private InkTouchController _emptyListInkTouchController;

        protected virtual void SetupEmptyListView()
        {
            emptyViewRefresherProgressView = LOTAnimationView.AnimationNamed("loader_animation");
            emptyViewRefresherProgressView.ContentMode = UIViewContentMode.ScaleAspectFit;
            emptyViewRefresherProgressView.ContentScaleFactor = 0.1f;
            emptyViewRefresherProgressView.LoopAnimation = true;
            emptyViewRefresherProgressView.Alpha = 0;
            nfloat desiredHeight = 36f / 400.0f * 300.0f;
            
            _emptyListView.Frame = View.Frame;
            
            _noActivityLabel.Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular);
            _noActivityLabel.Lines = 0;
            _noActivityLabel.Text = EmptyListText;
            _noActivityLabel.TextColor = UIColor.FromRGB(154, 160, 169);
            _noActivityLabel.TextAlignment = UITextAlignment.Center;

            LOTAnimationView animationView = LOTAnimationView.AnimationNamed(EmptyListAssetName);
            animationView.ContentMode = UIViewContentMode.ScaleAspectFit;
            animationView.LoopAnimation = true;
            animationView.ContentScaleFactor = ScaleFactor;
            animationView.Play();
            
            emptyListButton = new UIButton();
            emptyListButton.Layer.CornerRadius = 12f;
            emptyListButton.Layer.MasksToBounds = true;
            emptyListButton.BackgroundColor = AppColors.Accent;
            emptyListButton.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
            emptyListButton.ContentEdgeInsets = new UIEdgeInsets(12f, 16f, 12f, 16f); 
            emptyListButton.SetTitle(EmptyListButtonText, UIControlState.Normal);
            emptyListButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            emptyListButton.TouchUpInside += (e, a) => EmptyListButtonAction.Invoke();
            
            _emptyListInkTouchController = new InkTouchController(emptyListButton);
            _emptyListInkTouchController.AddInkView();

            UIView subView = new UIView();
            subView.Add(animationView);
            subView.Add(_noActivityLabel);
            subView.Add(emptyListButton);

            _emptyListView.Add(subView);
            
            _emptyListView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            subView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            subView.AddConstraints(
                animationView.AtTopOf(subView),
                animationView.WithSameCenterX(subView),
                animationView.Height().EqualTo(HeightOfAnimation),
                
                _noActivityLabel.Below(animationView, 12),
                _noActivityLabel.AtLeftOf(subView,36),
                _noActivityLabel.AtRightOf(subView, 36),
                
                emptyListButton.Below(_noActivityLabel, 12f),
                emptyListButton.WithSameCenterX(subView),
                emptyListButton.AtBottomOf(subView)
            );
            
            _emptyListView.AddConstraints(
                    subView.AtLeftOf(_emptyListView),
                    subView.AtRightOf(_emptyListView),
                    subView.WithSameCenterY(_emptyListView).Minus(36)
                );

            if (!HasEmptyListButton)
            {
                emptyListButton.Hidden = true;
                emptyListButton.UserInteractionEnabled = false;
            }
 
        }

        protected virtual Action EmptyListButtonAction { get; } = new Action(() => { });

        protected virtual int HeightOfAnimation => 144;

        protected virtual float ScaleFactor => 1;

        public abstract string EmptyListText { get; } 

        public abstract string EmptyListAssetName { get; } 

        public virtual bool HasEmptyListButton { get; } = true;

        protected virtual string EmptyListButtonText { get; } = string.Empty;

        protected UITableView TableView { get; private set; }
        
        protected abstract MvxTemplateSelector GetTemplateSelector();
 
        
        public virtual string LoadMoreText => TextResources.BaseFinancesViewModel_LoadMore;
        
        UIView GetFooterLoadMoreView()
        {
            if (!ViewModel.IsLoadingMoreDataEnabled)
                return new UIView();

            var footer = new UIView(new CGRect(0, 0, View.Frame.Width, 64))
            {
                Alpha = 0
            };
            
            var loadMoreButton = new UIButton();
            loadMoreButton.Layer.CornerRadius = 18;
            loadMoreButton.Layer.MasksToBounds = true;
            loadMoreButton.BackgroundColor = StartViewController.Colors.Primary;
            loadMoreButton.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
            loadMoreButton.ContentEdgeInsets = new UIEdgeInsets(12f, 36, 12f, 36); 
            loadMoreButton.SetTitle(LoadMoreText, UIControlState.Normal);
            loadMoreButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            
            var progressView = LOTAnimationView.AnimationNamed("loader_animation");
            progressView.ContentMode = UIViewContentMode.ScaleAspectFit;
            progressView.ContentScaleFactor = 0.1f;
            progressView.LoopAnimation = true;
            progressView.Alpha = 0;
            nfloat desiredHeight = 48f / 400.0f * 300.0f;
            
            footer.Add(loadMoreButton);
            footer.Add(progressView);
            progressView.Play();
            
            footer.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            footer.AddConstraints(
                loadMoreButton.WithSameCenterX(footer),
                loadMoreButton.AtTopOf(footer, 12),
                
                progressView.Width().EqualTo(48),
                progressView.Height().EqualTo(desiredHeight),
                progressView.WithSameCenterY(footer),
                progressView.WithSameCenterX(footer)
            );
            loadMoreInkTouchController = new InkTouchController(loadMoreButton);
            loadMoreInkTouchController.AddInkView();

            loadMoreButton.TouchUpInside += (e, a) => { ViewModel.LoadMore.Execute(); };
            _loadMoreButton = loadMoreButton;
            _loadMoreProgress = progressView;
            
            return footer;
        }
    }
    
  
}