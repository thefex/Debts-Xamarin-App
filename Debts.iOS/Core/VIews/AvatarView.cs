using System;
using System.IO;
using CoreFoundation;
using Debts.Extensions;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using JSQMessagesViewController;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Views;
using UIKit;

namespace Debts.iOS.Core.VIews
{
    internal class AvatarGenerator
    {
        public void GenerateAvatar(UIImageView destinationImageView, string personName, string imageUrl, int fontSize,
            int diameter, UIColor textColor, UIColor backgroundColor)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                destinationImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

                var avatarLiterals = GetAvatarLiteralsImage(personName, fontSize, diameter, textColor, backgroundColor);

                destinationImageView.Image = avatarLiterals;

                if (string.IsNullOrEmpty(imageUrl))
                {
                    destinationImageView.Image = avatarLiterals;
                    return;
                }

                var imageService = ImageService.Instance;

                TaskParameter imageTaskParameter = null;

                if (imageUrl.StartsWith("http") || imageUrl.StartsWith("https"))
                    imageTaskParameter = imageService.LoadUrl(imageUrl);
                else
                {
                    var uri = new Uri(imageUrl);
                    if (uri.IsFile)
                    {
                        var fileName = System.IO.Path.GetFileName(uri.LocalPath);
                        imageUrl = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
                    }
                    imageTaskParameter = imageService.LoadFile(imageUrl);
                }
                    

                imageTaskParameter
                    .Error(
                        ex =>
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message, ex.StackTrace);
                        })
                    .Success((image, result) => { 
                            
                        })
                    .WithCache(CacheType.All)
                    .Transform(new AvatarImageFactoryTransformation(diameter))
                    .CacheKey(Guid.NewGuid().ToString())
                    .FadeAnimation(true, false)
                    .Retry(3, 200)
                    .Into(destinationImageView);
            });   
        }

        private UIImage GetAvatarLiteralsImage(string fullName, int fontSize, int diameter, UIColor textColor,
            UIColor backgroundColor)
        {
            var initials = fullName?.GetInitials() ?? string.Empty;
            var avatar = MessagesAvatarImageFactory.CreateAvatarImage(initials, backgroundColor, textColor,
                UIFont.SystemFontOfSize(fontSize), (nuint) diameter);
            return avatar.AvatarImage;
        }

        private class AvatarImageFactoryTransformation : TransformationBase
        {
            public AvatarImageFactoryTransformation(int diameter)
            {
                Diameter = diameter;
            }

            protected override UIImage Transform(UIImage sourceBitmap, string path, ImageSource source,
                bool isPlaceholder, string key)
            {
                return MessagesAvatarImageFactory.CreateAvatarImage(sourceBitmap, (nuint) Diameter).AvatarImage;
            }

            public override string Key => $"AvatarImageFactoryTransformation,diameter={Diameter}";

            private int Diameter { get; }
        }
    }
}