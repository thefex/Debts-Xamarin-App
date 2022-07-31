using System;
using CoreGraphics;
using UIKit;

namespace SpotlightXamarin.IOS
{
    public abstract class Target
    {
        public CGPoint Point { get; set; }
        public nfloat Radius { get; set; }
        public UIView View { get; set; }
    }
}
