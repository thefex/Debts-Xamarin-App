using System;
using Android.Content; 
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Util;

namespace Debts.Droid.Core.Views
{
    [Register("com.debts.FixedListPreference")]
    public class FixedListPreference : ListPreference
    {
        protected FixedListPreference(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public FixedListPreference(Context context) : base(context)
        {
        }

        public FixedListPreference(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public FixedListPreference(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public FixedListPreference(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public override string Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                NotifyChanged();
            }
        }
    }
}