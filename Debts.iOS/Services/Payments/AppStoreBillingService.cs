using System;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;
using Debts.Services;
using Debts.Services.Payments;
using DynamicData.Kernel;
using Foundation;
using HealthKit;
using MvvmCross.Platforms.Ios;
using StoreKit;

namespace Debts.iOS.Services.Payments
{
    [Preserve(AllMembers = true)]
    public class AppStoreBillingService : IBillingService
    {
        private readonly IStorageService _storageService;
        public const string PremiumSubscriptionProductId = "premium_subscription";
        public const string RoyalProductId = "royal";

        public AppStoreBillingService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        private bool isInitialized = false;
        private SKProduct[] _products;
        private SKPaymentTransactionObserver skPaymentTransactionObserver;

        public static string PremiumSubscriptionPrice { get; private set; } 
        
        public async Task Initialize()
        {
            if (isInitialized)
                return;

            HasPremiumSubscription = _storageService.Get<DateTime>(PremiumSubscriptionProductId, DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30))) > DateTime.UtcNow;
            HasPremiumLifeTime = _storageService.Get<bool>(RoyalProductId);
            UpdateState();
            
            skPaymentTransactionObserver = new AppSKPaymentTransactionObserver(_storageService, () => isInitialized, productId =>
            {
                if (productId == PremiumSubscriptionProductId)
                    HasPremiumSubscription = true;
                else if (productId == RoyalProductId)
                {
                    _storageService.Store(RoyalProductId, true);
                    HasPremiumLifeTime = true;
                }

                PremiumStateUpdated?.Invoke();
            }, () => { });
            
            SKPaymentQueue.DefaultQueue.AddTransactionObserver(skPaymentTransactionObserver);
            isInitialized = true;
            SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();

            var tcsProducts = new TaskCompletionSource<SKProduct[]>();
            var request = new SKProductsRequest(new NSSet(PremiumSubscriptionProductId));
            request.Delegate = new SKProductRequestDelegate(tcsProducts);
            request.Start();

            var results = await tcsProducts.Task;

            if (results.Any())
            {
                var productDesc = results.FirstOrDefault();
                var formatter = new NSNumberFormatter();
                formatter.NumberStyle = NSNumberFormatterStyle.Currency;
                formatter.Locale = productDesc.PriceLocale;
                PremiumSubscriptionPrice = formatter.StringFromNumber(productDesc.Price);
            }
        }
        
        class SKProductRequestDelegate : NSObject, ISKProductsRequestDelegate
        {
            private readonly TaskCompletionSource<SKProduct[]> _tcs;

            public SKProductRequestDelegate(NSObjectFlag x) : base(x)
            {
            }

            public SKProductRequestDelegate(IntPtr handle) : base(handle)
            {
            }

            public SKProductRequestDelegate(IntPtr handle, bool alloced) : base(handle, alloced)
            {
            }

            public SKProductRequestDelegate(TaskCompletionSource<SKProduct[]> tcs)
            {
                _tcs = tcs;
            }
            
            public void ReceivedResponse(SKProductsRequest request, SKProductsResponse response)
            {
                _tcs.TrySetResult(response.Products);
            }

            [Export("requestDidFinish:")]
            public void RequestFinished(SKRequest request)
            { 
            }

            [Export("request:didFailWithError:")]
            public void RequestFailed(SKRequest request, NSError error)
            {
                if (error != null)
                    System.Diagnostics.Debug.WriteLine(error);
            }
        }

        public Task UpdateState()
        {
            SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();
            return Task.FromResult(true);
        }

        public async Task BuyLifetimeSubscription()
        {
            if (!isInitialized)
                await Initialize();
            
            var payment = SKPayment.CreateFrom(RoyalProductId);
            SKPaymentQueue.DefaultQueue.AddPayment(payment);
        }

        public async Task BuyMonthlySubscription()
        {
            if (!isInitialized)
                await Initialize();
            
            var payment = SKPayment.CreateFrom(PremiumSubscriptionProductId);
            SKPaymentQueue.DefaultQueue.AddPayment(payment);
        }

        public bool HasPremiumSubscription { get; private set; }
        public bool HasPremiumLifeTime { get; private set; }
        
        public bool CanMakePayments => SKPaymentQueue.CanMakePayments;

        public void Restore()
        {
            SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();
        }

        public event Action PremiumStateUpdated;
    }
    
    class AppSKPaymentTransactionObserver : SKPaymentTransactionObserver
    {
        private readonly IStorageService _storageService;
        private readonly Func<bool> _isInitialized;
        private readonly Action<string> _onSuccess;
        private readonly Action _onFailure;

        public AppSKPaymentTransactionObserver(IStorageService storageService, Func<bool> isInitialized, Action<string> onSuccess, Action onFailure)
        {
            _storageService = storageService;
            _isInitialized = isInitialized;
            _onSuccess = onSuccess;
            _onFailure = onFailure;
        }

        public AppSKPaymentTransactionObserver(NSObjectFlag x) : base(x)
        {
        }

        public AppSKPaymentTransactionObserver(IntPtr handle) : base(handle)
        {
        }
  
        public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions)
        {
            foreach (var transaction in transactions)
            {
                if (transaction.TransactionState == SKPaymentTransactionState.Purchased ||
                    transaction.TransactionState == SKPaymentTransactionState.Restored)
                    OnSuccess(transaction);
                else if (transaction.TransactionState == SKPaymentTransactionState.Failed)
                    OnFailure(transaction);
            }

            if (!_isInitialized())
                return;

            if (transactions.All(x => x.Payment.ProductIdentifier != AppStoreBillingService.PremiumSubscriptionProductId))
                _storageService.Store(AppStoreBillingService.PremiumSubscriptionProductId, DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(10)));

            if (transactions.Any(x =>
                x.Payment.ProductIdentifier == AppStoreBillingService.PremiumSubscriptionProductId &&
                (x.TransactionState == SKPaymentTransactionState.Restored ||
                 x.TransactionState == SKPaymentTransactionState.Purchased)))
            {
                var transaction = transactions.First(x =>
                    x.Payment.ProductIdentifier == AppStoreBillingService.PremiumSubscriptionProductId &&
                    (x.TransactionState == SKPaymentTransactionState.Restored ||
                     x.TransactionState == SKPaymentTransactionState.Purchased));
                
                _storageService.Store(AppStoreBillingService.PremiumSubscriptionProductId,
                    transaction.TransactionDate.ToDateTimeUtc() + TimeSpan.FromDays(31));
            }

        }

        void OnSuccess(SKPaymentTransaction transaction)
        {
            _onSuccess(transaction.Payment.ProductIdentifier);
            SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
        }

        void OnFailure(SKPaymentTransaction transaction)
        {
            _onFailure();
            SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
        }
        
        
    }
}