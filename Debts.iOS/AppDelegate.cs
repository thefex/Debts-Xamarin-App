using Debts.iOS.Config;
using Debts.iOS.Services;
using Debts.iOS.Services.AppGrowth;
using Debts.iOS.Services.Notifications;
using Debts.iOS.ViewControllers.Base;
using Firebase.DynamicLinks;
using Foundation;
using Google.MobileAds;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using MvvmCross.Platforms.Ios.Core;
using UIKit;
using UserNotifications;

namespace Debts.iOS
{
   [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate<AppSetup, iOSApplicationSetup>
    {
        // class-level declarations
        private CallDetectorController callDetectorController;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {           
            var result = base.FinishedLaunching(application, launchOptions);
            Window.BackgroundColor = UIColor.White;
            Window.MakeKeyAndVisible();
            if (Window.RootViewController is MainViewController mainViewController) 
                mainViewController.AdjustBottomAppBarHeight(Window.SafeAreaInsets);
            var z = 3;
            callDetectorController = new CallDetectorController();
            callDetectorController.Initialize();

            UNUserNotificationCenter.Current.Delegate = new AppNotificationDelegate();
            iOSNotificationsWorkScheduler.Register();
            iOSNotificationsWorkScheduler.Start();

            AppCenter.Start(iOSAppConstants.AppCenterId, typeof(Crashes));
            MobileAds.SharedInstance.Start(x =>
            {
                
            });
            Firebase.Core.App.Configure();

            return result;
        } 

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity,
            UIApplicationRestorationHandler completionHandler)
        {
            bool isHandled = false;

            if (userActivity.WebPageUrl != null)
            {
                var dynamicLinkHandler = new FirebaseDynamicLinkHandler();
                isHandled = DynamicLinks.SharedInstance.HandleUniversalLink(userActivity.WebPageUrl,
                    delegate(DynamicLink link, NSError error)
                    {
                        if (link != null)
                            dynamicLinkHandler.HandleDynamicLink(link);
                    });
            }

            return isHandled;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            string sourceApp = string.Empty;
            if (options.ContainsKey(UIApplication.LaunchOptionsSourceApplicationKey))
                sourceApp = options[UIApplication.LaunchOptionsSourceApplicationKey].ToString();
                
            return OpenUrl(app, url, sourceApp, new NSString(""));
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            var dynamicLink = DynamicLinks.SharedInstance.FromCustomSchemeUrl(url);
            if (dynamicLink != null)
            {
                var dynamicLinkHandler = new FirebaseDynamicLinkHandler();
                dynamicLinkHandler.HandleDynamicLink(dynamicLink);
                return true;
            }
            
            return false;
        }
    }
}