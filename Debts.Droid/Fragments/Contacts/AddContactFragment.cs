using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Debts.ViewModel;
using Debts.ViewModel.Contacts;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments
{
    [MvxFragmentPresentation(ActivityHostViewModelType = typeof(MainViewModel), AddToBackStack = false, FragmentContentId = Resource.Id.add_fragment_presenter, EnterAnimation = Resource.Animation.abc_grow_fade_in_from_bottom, ExitAnimation = Resource.Animation.abc_shrink_fade_out_from_bottom)]
    public class AddContactFragment : MvxDialogFragment<AddContactViewModel>
    {
        public AddContactFragment()
        {
        }

        public AddContactFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.fragment_add_contact, container, false);
             
            
            
            return view;
        }
 
        //public override int Theme => Resource.Style.Widget_AppTheme_BottomSheet;
    }
}