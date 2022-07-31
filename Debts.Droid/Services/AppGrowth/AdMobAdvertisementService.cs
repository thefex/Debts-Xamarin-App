using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.DoubleClick;
using Android.Gms.Ads.Rewarded;
using Android.Runtime;
using Debts.Droid.Config;
using Debts.Services.AppGrowth;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace Debts.Droid.Services.AppGrowth
{
    public class AdMobAdvertisementService : AdvertisementService
    {
        private readonly Context _context;
        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;
        private TaskCompletionSource<bool> _adLoadTaskCompletionSource;
        private TaskCompletionSource<bool> _rewardedTaskCompletionSource;
        private RewardedAdLoadListener _rewardedAdLoadListener;

        public AdMobAdvertisementService(PremiumService premiumService, Context context) : base(premiumService)
        {
            _context = context;
        }
        public override bool IsRewardAdLoaded => (_rewardedAd?.IsLoaded ?? false);

        public override void PreloadFullScreenAd()
        {
            if (_interstitialAd == null)
            {
                _interstitialAd = new InterstitialAd(_context);    
                _interstitialAd.AdUnitId = AndroidAppConstants.FullScreenAdMobAdId; 
                _adLoadTaskCompletionSource = new TaskCompletionSource<bool>();
                _interstitialAd.AdListener = new AppAdListener(_adLoadTaskCompletionSource, _interstitialAd);
                _interstitialAd.LoadAd(new AdRequest.Builder().Build()); 
            } 
        }

        public override async Task ShowFullScreenAd()
        {
            if (_adLoadTaskCompletionSource == null)
                return;

            var task = await _adLoadTaskCompletionSource.Task;
            
            if (_interstitialAd.IsLoaded)
                _interstitialAd.Show();
        }

        public override void PreloadRewardedAd()
        {
            if (_rewardedAd == null)
            {
                _rewardedAd = new RewardedAd(_context, AndroidAppConstants.RewardedAdMobId);
                _rewardedTaskCompletionSource = new TaskCompletionSource<bool>();
                 _rewardedAdLoadListener = new RewardedAdLoadListener(_rewardedTaskCompletionSource, _rewardedAd);
                _rewardedAd.LoadAd(new AdRequest.Builder().Build(),
                    _rewardedAdLoadListener); 
            }
        }

        public override async Task<bool> ShowFullScreenRewardAd()
        {
            if (_rewardedTaskCompletionSource == null)
                return false;

            if (_rewardedAd == null)
                return false;
            
            bool isLoaded = _rewardedAd?.IsLoaded ?? false;
            
            if (!isLoaded) 
                isLoaded = await _rewardedTaskCompletionSource.Task;

            if (!isLoaded)
                return false;
             
            TaskCompletionSource<bool> hasRewardTaskCompletionSource = new TaskCompletionSource<bool>();
            _rewardedAd.Show(Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity, 
                new RewardedAdListener(hasRewardTaskCompletionSource, _rewardedAd, _rewardedAdLoadListener)
                );

            PreloadRewardedAd();
            return await hasRewardTaskCompletionSource.Task;
        }
  

        class AppAdListener : AdListener
        {
            private readonly TaskCompletionSource<bool> _taskCompletionSource;
            private readonly InterstitialAd _interstitialAd;
            private int retryCount = 0;
            private const int maxRetryCount = 3;

            public AppAdListener(TaskCompletionSource<bool> taskCompletionSource, InterstitialAd interstitialAd)
            {
                _taskCompletionSource = taskCompletionSource;
                _interstitialAd = interstitialAd;
            }
            
            protected AppAdListener(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public AppAdListener()
            {
            }

            public override void OnAdClicked()
            {
                base.OnAdClicked();
            }

            public override void OnAdClosed()
            {
                base.OnAdClosed();
                _interstitialAd.LoadAd(new AdRequest.Builder().Build());
            }

            public override void OnAdFailedToLoad(int p0)
            {
                base.OnAdFailedToLoad(p0);
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.SetResult(false);

                if (retryCount < maxRetryCount)
                {
                    _interstitialAd.LoadAd(new AdRequest.Builder().Build());
                    retryCount++;
                }
            }

            public override void OnAdImpression()
            {
                base.OnAdImpression();
            }

            public override void OnAdLeftApplication()
            {
                base.OnAdLeftApplication();
            }

            public override void OnAdLoaded()
            {
                base.OnAdLoaded();
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.SetResult(true);

                retryCount = 0;
            }

            public override void OnAdOpened()
            {
                base.OnAdOpened();
            }
        }

        class RewardedAdLoadListener : RewardedAdLoadCallback
        {
            private readonly TaskCompletionSource<bool> _taskCompletionSource;
            private readonly RewardedAd _rewardedAd;
            private int retryCount = 0;
            private const int maxRetryCount = 3;
            
            protected RewardedAdLoadListener(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public RewardedAdLoadListener(TaskCompletionSource<bool> taskCompletionSource, RewardedAd rewardedAd)
            {
                _taskCompletionSource = taskCompletionSource;
                _rewardedAd = rewardedAd;
            }

            public override void OnRewardedAdLoaded()
            {
                base.OnRewardedAdLoaded();
                
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.SetResult(true);
                
                retryCount = 0;
            }

            public override void OnRewardedAdFailedToLoad(int p0)
            {
                base.OnRewardedAdFailedToLoad(p0);
                
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.SetResult(false);
                    
                if (retryCount < maxRetryCount)
                {
                    _rewardedAd.LoadAd(new AdRequest.Builder().Build(), this);
                    retryCount++;
                }
            }
        }

        class RewardedAdListener : RewardedAdCallback
        {
            private readonly TaskCompletionSource<bool> _taskCompletionSource;
            private readonly RewardedAd _rewardedAd;
            private readonly RewardedAdLoadListener _rewardedAdLoadListener;

            protected RewardedAdListener(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public RewardedAdListener(TaskCompletionSource<bool> taskCompletionSource, RewardedAd rewardedAd, RewardedAdLoadListener rewardedAdLoadListener)
            {
                _taskCompletionSource = taskCompletionSource;
                _rewardedAd = rewardedAd;
                _rewardedAdLoadListener = rewardedAdLoadListener;
            }

            public override void OnUserEarnedReward(IRewardItem p0)
            {
                base.OnUserEarnedReward(p0);
                
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.SetResult(true);
            }

            public override void OnRewardedAdFailedToShow(int p0)
            {
                base.OnRewardedAdFailedToShow(p0);
                
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.SetResult(false);
            }

            public override void OnRewardedAdClosed()
            {
                base.OnRewardedAdClosed();
                
                if (!_taskCompletionSource.Task.IsCompleted)
                    _taskCompletionSource.SetResult(false);
                
                _rewardedAd.LoadAd(new AdRequest.Builder().Build(), _rewardedAdLoadListener);
            }
        }
    }
}