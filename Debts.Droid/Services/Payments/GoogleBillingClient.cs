using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.BillingClient.Api;
using Android.Content;
using Android.Runtime;
using Debts.Droid.Services.AppGrowth;
using Debts.Services;
using Debts.Services.Payments;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace Debts.Droid.Services.Payments
{
    public class GoogleBillingClient : Java.Lang.Object, IBillingService
    {
        private readonly Context _context; 
        private const string LifeTimeSubscriptionProductId = "lifetime_subscription";
        private const string MonthlySubscriptionProductId = "premium_access_subscription";
        private bool isInitialized;

        private BillingClient _billingClient;
        readonly TaskCompletionSource<bool> _initializerTaskCompletionSource = new TaskCompletionSource<bool>();
        private readonly PurchasesUpdatedListener _purchasesUpdatedListener = new PurchasesUpdatedListener();
        
        public GoogleBillingClient(Context context)
        {
            _context = context;
            _purchasesUpdatedListener.Purchased += async (x) =>
            {
                bool hasPremiumLifetime = await HandlePurchased(x, GetLifetimeProductId());
                bool hasSubscriptionLifetime = await HandlePurchased(x, GetMonthlyProductId());

                if (hasPremiumLifetime)
                    HasPremiumLifeTime = true;
                if (hasSubscriptionLifetime)
                    HasPremiumSubscription = true;
            };
        }

        public GoogleBillingClient(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public GoogleBillingClient()
        {
        }

        public async Task Initialize()
        {
            if (_billingClient==null)
            {  
                _billingClient = BillingClient.NewBuilder(_context)
                    .EnablePendingPurchases()
                    .SetListener(_purchasesUpdatedListener)
                    .Build();
                
                _billingClient.StartConnection(new BillingStateListener(_billingClient, _initializerTaskCompletionSource));
                
                var initializationState = await _initializerTaskCompletionSource.Task;
                await FetchAndUpdateState();
                isInitialized = true;
            }
        }
        
        public bool HasPremiumSubscription { get; private set; }
        
        public bool HasPremiumLifeTime { get; private set; }
        public bool CanMakePayments { get; } = true;
        public void Restore()
        { 
            
        }

        public event Action PremiumStateUpdated;

        async Task FetchAndUpdateState()
        {
            var subscriptionTaskRecords = new TaskCompletionSource<IList<PurchaseHistoryRecord>>();
            var inAppRecords = _billingClient.QueryPurchases(BillingClient.SkuType.Inapp);
            var subscriptionRecords = _billingClient.QueryPurchases(BillingClient.SkuType.Subs);

            if (inAppRecords?.PurchasesList != null)
            {
                var list = inAppRecords.PurchasesList.Where(x => x.PurchaseState == PurchaseState.Purchased).ToList();
                
                if (!list.Any())
                    HasPremiumLifeTime=false;
                else
                    HasPremiumLifeTime = await HandlePurchased(list, GetLifetimeProductId()); 
            }

            if (subscriptionRecords?.PurchasesList != null)
            {
                var list = subscriptionRecords.PurchasesList.Where(x => x.PurchaseState == PurchaseState.Purchased).ToList();

                if (!list.Any())
                    HasPremiumSubscription = false;
                else
                    HasPremiumSubscription = await HandlePurchased(subscriptionRecords.PurchasesList, GetMonthlyProductId());
            }
            
            PremiumStateUpdated?.Invoke();  
        }
        
        public async Task UpdateState()
        {
            if (!isInitialized)
                return;
            
            await FetchAndUpdateState();   
        }

        public Task BuyLifetimeSubscription()
        {
            return BuyProduct(GetLifetimeProductId(), BillingClient.SkuType.Inapp);
        }

        string GetLifetimeProductId()
        {
            var productId = LifeTimeSubscriptionProductId;

        #if DEBUG
            productId = "android.test.purchased";            
        #endif
        
            return productId;
        }
        
        public Task BuyMonthlySubscription()
        {
            var productId = GetMonthlyProductId();
            string skuType = BillingClient.SkuType.Subs;
            
            #if DEBUG
            skuType = BillingClient.SkuType.Inapp;
            #endif
            
            return BuyProduct(productId, skuType);
        }

        string GetMonthlyProductId()
        {
            #if DEBUG
            return "android.test.canceled";
            #endif

            return MonthlySubscriptionProductId;
        }

        async Task<bool> HandlePurchased(IList<Purchase> purchases, string productId)
        {
            bool isPurchased = false;
        
            foreach (var item in purchases)
            {
                if (item.PurchaseState != PurchaseState.Purchased)
                    continue;
                 
                if (!item.IsAcknowledged)
                {
                    var acknowledgePurchaseParams = AcknowledgePurchaseParams.NewBuilder()
                        .SetPurchaseToken(item.PurchaseToken)
                        .Build();
                        
                    TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                    _billingClient.AcknowledgePurchase(acknowledgePurchaseParams, new AcknowledgeListener(tcs));
                    await tcs.Task;
                }
             /*   else 
                {
                    var consumeParams = ConsumeParams.NewBuilder()
                        .SetPurchaseToken(item.PurchaseToken)
                        .Build();
                    _billingClient.ConsumeAsync(consumeParams, new ConsumeListener());
                } */

                 if (item.Sku.Equals(productId))
                     isPurchased = true; 
            }
            
            return isPurchased;
        }

        class ConsumeListener : Java.Lang.Object, IConsumeResponseListener
        {
            public void OnConsumeResponse(BillingResult p0, string p1)
            {
                
            }
        }

        async Task BuyProduct(string productId, string skuType)
        {
            await _initializerTaskCompletionSource.Task;

            var skuDetailsParams = SkuDetailsParams.NewBuilder()
                .SetSkusList(new List<string>() {productId})
                .SetType(skuType)
                .Build();
            TaskCompletionSource<List<SkuDetails>> tcs = new TaskCompletionSource<List<SkuDetails>>();
            _billingClient.QuerySkuDetailsAsync(skuDetailsParams, new SkuDetailsListener(tcs));

            var results = await tcs.Task;

            var lifeTimeSubscriptionDetails = results.FirstOrDefault(x => x.Sku == productId);
            var flowParams = BillingFlowParams.NewBuilder()
                .SetSkuDetails(lifeTimeSubscriptionDetails)
                .Build();
            
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            _billingClient.LaunchBillingFlow(currentActivity, flowParams);
        }
   
    }
    
    class AcknowledgeListener : Java.Lang.Object, IAcknowledgePurchaseResponseListener
    {
        private readonly TaskCompletionSource<bool> _taskCompletionSource;

        public AcknowledgeListener(TaskCompletionSource<bool> taskCompletionSource)
        {
            _taskCompletionSource = taskCompletionSource;
        }

        public AcknowledgeListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public void OnAcknowledgePurchaseResponse(BillingResult p0)
        {
            if (!_taskCompletionSource.Task.IsCompleted)
                _taskCompletionSource.SetResult(p0.ResponseCode == BillingResponseCode.Ok);
        }
    }
}