using System;
using System.Threading.Tasks;
using Debts.iOS.Config;
using Debts.Services.AppGrowth;
using Foundation;
using Google.MobileAds;
using Google.MobileAds.DoubleClick;
using UIKit;
using Interstitial = Google.MobileAds.Interstitial;
using Request = Google.MobileAds.Request;

namespace Debts.iOS.Services.AppGrowth
{
    public class iOSAdvertisementService : AdvertisementService
    {
        private Interstitial _interstitialAd;
        private RewardedAd _rewardedAd;
        private TaskCompletionSource<bool> _adLoadTaskCompletionSource;
        private TaskCompletionSource<bool> _rewardedTaskCompletionSource;
        
        public iOSAdvertisementService(PremiumService premiumService) : base(premiumService)
        {
        }

        public override void PreloadFullScreenAd()
        {
            if (_interstitialAd == null) 
                BuildInterstialAd();
        }

        void BuildInterstialAd()
        {
            _interstitialAd = new Interstitial(iOSAppConstants.FullScreenAdMobAdId);
            _adLoadTaskCompletionSource = new TaskCompletionSource<bool>();
            _interstitialAd.Delegate = new InterstialAdDelegate(_adLoadTaskCompletionSource, BuildInterstialAd);
            var defaultRequest = Request.GetDefaultRequest();
            
            _interstitialAd.LoadRequest(defaultRequest);
        }

        class InterstialAdDelegate : InterstitialDelegate
        {
            private int retryCount = 0;
            private const int maxRetryCount = 3;
            private readonly TaskCompletionSource<bool> _taskCompletionSource;
            private readonly Action _buildNextInterstial;

            public InterstialAdDelegate(TaskCompletionSource<bool> taskCompletionSource, Action buildNextInterstial)
            {
                _taskCompletionSource = taskCompletionSource;
                _buildNextInterstial = buildNextInterstial;
            }

            protected InterstialAdDelegate(NSObjectFlag t) : base(t)
            {
            }

            protected internal InterstialAdDelegate(IntPtr handle) : base(handle)
            {
            }

            public override void DidDismissScreen(Interstitial ad)
            {
                _buildNextInterstial();
            }

            public override void DidFailToPresentScreen(Interstitial ad)
            {
                _buildNextInterstial();
            }

            public override void DidFailToReceiveAd(Interstitial sender, RequestError error)
            {
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.SetResult(false);
                    
                if (retryCount < maxRetryCount)
                {
                    sender.LoadRequest(Request.GetDefaultRequest());
                    retryCount++;
                }
            }

            public override void DidReceiveAd(Interstitial ad)
            {
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.SetResult(true);
                
                retryCount = 0;
            }

            public override void WillDismissScreen(Interstitial ad)
            {
            }

            public override void WillLeaveApplication(Interstitial ad)
            {
            }

            public override void WillPresentScreen(Interstitial ad)
            {
            }
        }

        public override async Task ShowFullScreenAd()
        {
            if (_adLoadTaskCompletionSource == null)
                return;

            var task = await _adLoadTaskCompletionSource.Task;
            
            if (_interstitialAd.IsReady)
                _interstitialAd.Present(UIApplication.SharedApplication.KeyWindow.RootViewController);
        }

        public override void PreloadRewardedAd()
        {
            if (_rewardedAd == null)
            {
                _rewardedAd = new RewardedAd(iOSAppConstants.RewardedAdMobId);
                _rewardedTaskCompletionSource = new TaskCompletionSource<bool>();
                var defaultRequest = Request.GetDefaultRequest();
                //defaultRequest.TestDevices = new[] {"ba1315325f7922619c05283abe12338e"};

                _rewardedAd.LoadRequest(defaultRequest, RewardedAdCOmpletionHandler);
            }
        }

        private int rewardedAdRetryCount = 0;
        private void RewardedAdCOmpletionHandler(RequestError error)
        {
            if (error == null)
            {
                if (!_rewardedTaskCompletionSource.Task.IsCompleted)
                    _rewardedTaskCompletionSource.TrySetResult(true);

                rewardedAdRetryCount = 0;
            }
            else
            {
                   
                if (!_rewardedTaskCompletionSource.Task.IsCompleted)
                    _rewardedTaskCompletionSource.TrySetResult(false);
                    
                if (rewardedAdRetryCount < 3)
                {
                    _rewardedAd.LoadRequest(Request.GetDefaultRequest(), RewardedAdCOmpletionHandler);
                    rewardedAdRetryCount++;
                }
            }
            
        }

        public override async Task<bool> ShowFullScreenRewardAd()
        {
            if (_rewardedTaskCompletionSource == null)
                return false;

            if (_rewardedAd == null)
                return false;
            
            bool isLoaded = _rewardedAd?.IsReady ?? false;
            
            if (!isLoaded) 
                isLoaded = await _rewardedTaskCompletionSource.Task;

            if (!isLoaded)
                return false;
             
            TaskCompletionSource<bool> hasRewardTaskCompletionSource = new TaskCompletionSource<bool>();
            _rewardedAd.Present(
                UIApplication.SharedApplication.KeyWindow.RootViewController,
                new RewardedAdDelegate(hasRewardTaskCompletionSource, RewardedAdCOmpletionHandler));

            PreloadRewardedAd();
            return await hasRewardTaskCompletionSource.Task;
        }

        class RewardedAdDelegate : NSObject, IRewardedAdDelegate
        {
            private readonly TaskCompletionSource<bool> _taskCompletionSource;
            private readonly RewardedAdLoadCompletionHandler _handler;

            public RewardedAdDelegate(TaskCompletionSource<bool> taskCompletionSource, RewardedAdLoadCompletionHandler handler)
            {
                _taskCompletionSource = taskCompletionSource;
                _handler = handler;
            }

            public RewardedAdDelegate(NSObjectFlag x) : base(x)
            {
            }

            public RewardedAdDelegate(IntPtr handle) : base(handle)
            {
            }

            public RewardedAdDelegate(IntPtr handle, bool alloced) : base(handle, alloced)
            {
            }

            public void UserDidEarnReward(RewardedAd rewardedAd, AdReward reward)
            {
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.TrySetResult(true);
            }

            [Export("rewardedAd:didFailToPresentWithError:")]
            public void DidFailToPresentWithError(RewardedAd rewardedAd, NSError error)
            {
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.TrySetResult(false);
            }

            [Export("rewardedAdDidPresent:")]
            public void DidPresent(RewardedAd ad)
            {
                
            }

            [Export(("rewardedAdDidDismiss:"))]
            public void DidDismiss(RewardedAd ad)
            {
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.TrySetResult(false);
                
                ad.LoadRequest(Request.GetDefaultRequest(), _handler);
            }
            
        }

        public override bool IsRewardAdLoaded => _rewardedAd?.IsReady ?? false;
    }
}