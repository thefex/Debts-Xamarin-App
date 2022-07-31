using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Debts.Droid.Core.Extensions;
using FFImageLoading;
using FFImageLoading.Cross;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Uri = Android.Net.Uri;

namespace Debts.Droid.Core.Views
{
  [Register("com.debts.AvatarView")]
    public class AvatarView : FrameLayout
    {
        private MvxCachedImageView _avatarImageView;
        private TextView _avatarInitialsTextView;
        private string _imagePath;

        private int avatarInitialsBackgroundId;

        public AvatarView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AvatarView(Context context) : base(context)
        {
            Initialize();
        }

        public AvatarView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public AvatarView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize();
        }

        public AvatarView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize();
        }

        
        public Color InitialsTextColor
        {
            set
            {
                _avatarInitialsTextView.SetTextColor(value);
            }
        }

        public Drawable AvatarInitialsBackground
        {
            get { return _avatarImageView.Background; }
            set { _avatarImageView.SdkSafeSetBackground(value); }
        }

        private string avatarInitialsBackgroundPath;
        public string AvatarInitialsBackgroundPath
        {
            get { return avatarInitialsBackgroundPath; }
            set
            {
                if (avatarInitialsBackgroundPath == value)
                    return;

                avatarInitialsBackgroundPath = value;
                this.SetBackground(value);
            }
        }

        public int AvatarInitialsBackgroundId
        {
            get { return avatarInitialsBackgroundId; }
            set
            {
                if (avatarInitialsBackgroundId != value)
                {
                    avatarInitialsBackgroundId = value;
                    _avatarImageView.SetBackgroundResource(value);
                }
            }
        }

        public string Initials
        {
            get { return _avatarInitialsTextView.Text; }
            set { _avatarInitialsTextView.Text = value; }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value ?? string.Empty;

                if (string.IsNullOrEmpty(value))
                {
                    _avatarInitialsTextView.FadeIn();
                    _avatarImageView.FadeOut(ViewStates.Gone);
                }
                else
                {
                    RequestImageLoad();
                    _avatarInitialsTextView.FadeOut(ViewStates.Gone);
                    _avatarImageView.FadeIn();
                }
                
            }
        }
        
        private void Initialize()
        {
            Inflate(Context, Resource.Layout.AvatarImageView, this);
            _avatarInitialsTextView = FindViewById<TextView>(Resource.Id.AvatarInitials);
            var imageView = FindViewById<MvxCachedImageView>(Resource.Id.AvatarImageView);
            imageView.Transformations = new List<ITransformation>() { new CircleTransformation() };
            _avatarImageView = imageView; 

            if (Background != null)
            {
                _avatarInitialsTextView.SdkSafeSetBackground(Background);
                this.SdkSafeSetBackground(null);
            }
        }

        private void RequestImageLoad()
        { 
            _avatarImageView.ImagePath = ImagePath;
        }

     
    }
}