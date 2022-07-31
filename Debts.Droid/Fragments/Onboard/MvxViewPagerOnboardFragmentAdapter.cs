using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using Java.Lang;
using MvvmCross.Binding.Extensions;
using MvvmCross.ViewModels;

namespace Debts.Droid.Fragments.Onboard
{
    public class MvxViewPagerOnboardFragmentAdapter
        : FragmentStatePagerAdapter
    {
        private readonly Context _context;

        public IEnumerable<FragmentInfo> Fragments { get; private set; }

        public override int Count
        {
            get { return Fragments.Count(); }
        }

        protected MvxViewPagerOnboardFragmentAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public MvxViewPagerOnboardFragmentAdapter(
            Context context, FragmentManager fragmentManager, IEnumerable<FragmentInfo> fragments)
            : base(fragmentManager)
        {
            _context = context;
            Fragments = fragments;
        }

        public override Fragment GetItem(int position)
        {
            var fragmentInfo = Fragments.ElementAt(position) as FragmentInfo;
            var fragment = new OnboardFragment(fragmentInfo.AnimationAssetName,
                fragmentInfo.Title,
                fragmentInfo.Text);
            fragment.ViewModel = fragmentInfo.ViewModel;
            return fragment;
        }

        protected static string FragmentJavaName(Type fragmentType)
        {
            var namespaceText = fragmentType.Namespace ?? "";
            if (namespaceText.Length > 0)
                namespaceText = namespaceText.ToLowerInvariant() + ".";
            return namespaceText + fragmentType.Name;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(string.Empty);
        }

        public class FragmentInfo
        {
            public string Title { get; set; }

            public string Text { get; set; }

            public string AnimationAssetName { get; set; }

            public IMvxViewModel ViewModel { get; set; }
        }
    }
    
    
}