using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Debts.Droid.Fragments.AddFinance;
using Debts.Droid.Fragments.Onboard;
using Debts.Model;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.Finances;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.Design;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace Debts.Droid.Fragments
{
    [MvxFragmentPresentation(ActivityHostViewModelType = typeof(MainViewModel), AddToBackStack = false, FragmentContentId = Resource.Id.add_fragment_presenter, EnterAnimation = Resource.Animation.abc_grow_fade_in_from_bottom, ExitAnimation = Resource.Animation.abc_shrink_fade_out_from_bottom)]
    public class AddFinanceOperationFragment : MvxDialogFragment<AddFinanceOperationViewModel>
    {
        public AddFinanceOperationFragment()
        {
        }

        public AddFinanceOperationFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.fragment_add_finance_operation, container, false);
            
            var pager = view.FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.pager);
            pager.Adapter = new MvxViewPagerAddFinanceOperationFragmentAdapter(Activity, ChildFragmentManager, ViewModel);

            var tabLayout = view.FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.SetupWithViewPager(pager);

            var finishButton = view.FindViewById<Button>(Resource.Id.finishButton);
            
            pager.PageSelected += (e, a) =>
            {
                ViewModel.CurrentSubPage = a.Position;
                finishButton.Text = a.Position == 3 ? TextResources.AddFinanceOperation_Title : TextResources.AddFinanceOperation_NEXT;
            };
            finishButton.Click += (e, a) =>
            {
                if (pager.CurrentItem == 3)
                    ViewModel.Add.Execute();
                else
                    ViewModel.Next.Execute();
            };

            var set = this.CreateBindingSet<AddFinanceOperationFragment, AddFinanceOperationViewModel>();

            set.Bind(pager)
                .For(x => x.CurrentItem)
                .To(x => x.CurrentSubPage);
            
            set.Apply();
            
            return view;
        }
 
        //public override int Theme => Resource.Style.Widget_AppTheme_BottomSheet;
    }
}