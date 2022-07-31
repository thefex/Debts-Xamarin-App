using System;
using Foundation;
using MessageUI;

namespace Debts.iOS.ViewControllers.Finances.Details
{
    public class MFMessageComposeViewControllerExtended : MFMessageComposeViewController
    {
        public MFMessageComposeViewControllerExtended(MFMessageComposeViewController from)
        {
                
        }

        public MFMessageComposeViewControllerExtended(NSCoder coder) : base(coder)
        {
        }

        protected MFMessageComposeViewControllerExtended(NSObjectFlag t) : base(t)
        {
        }

        protected internal MFMessageComposeViewControllerExtended(IntPtr handle) : base(handle)
        {
        }
    }
}