using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Accounts;
using Android.App;
using Android.Gms.Extensions;
using Android.Gms.Tasks;
using Android.Runtime;
using Debts.Data;
using Debts.Model;
using Debts.Resources;
using Debts.Services.AppGrowth;
using Debts.Services.Settings;
using Firebase.DynamicLinks;
using Java.Interop;
using MvvmCross.Binding.BindingContext;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;

namespace Debts.Droid.Services.AppGrowth
{
    public class FirebaseShareLinkBuilderService : ShareLinkBuilderService
    {
        public FirebaseShareLinkBuilderService(SettingsService settingsService) : base(settingsService)
        {
        } 

        public override async Task<LayerResponse<string>> BuildDeepLinkForFinanceOperation(FinanceOperation operation)
        {
            string title = TextResources.DeepLink_SocialTags_Title;
            string description = TextResources.DeepLink_SocialTags_Body
                .Replace("{CONTACT_NAME}", operation.RelatedTo.ToString())
                .Replace("{TITLE}", operation.Title)
                .Replace("{TYPE}", operation.Type.ToString())
                .Replace("{AMOUNT}", operation.PaymentDetails.Amount + " " + operation.PaymentDetails.Currency)
                .Replace("{DEADLINE_DATE}", operation.PaymentDetails.DeadlineDate.ToString("f"));
            
            string imageUrl =
                "http://debts-app.com/img/app_mockups/web_promo.jpg";
        
            var operationBase64 = GetBase64HashedFinanceOperation(operation);
            try
            { 
                var tcs = new TaskCompletionSource<Java.Lang.Object>();
                var dynamicLink = await FirebaseDynamicLinks.Instance.CreateDynamicLink()
                    .SetLink(Uri.Parse(AppPreviewPageUrl + "?operation=" + operationBase64))
                    .SetDomainUriPrefix(AppLinkUrl)
                    .SetSocialMetaTagParameters(new DynamicLink.SocialMetaTagParameters.Builder()
                        .SetTitle(title)
                        .SetDescription(description)
                        .SetImageUrl(Uri.Parse(imageUrl))
                        .Build())
                    .SetAndroidParameters(new DynamicLink.AndroidParameters.Builder(AndroidPackageName)
                        .SetMinimumVersion(AndroidMinimumVersion)
                        .Build()
                    ) 
                    .SetIosParameters(new DynamicLink.IosParameters.Builder(iOSMinimumVersion)
                        .SetAppStoreId(iOSAppStoreId)
                        .Build())
                    .BuildShortDynamicLink()
                    .AddOnSuccessListener(new SuccessListener(tcs))
                    .AddOnFailureListener(new FailureListener(tcs));

           
                var shortDynamicLink = JavaObjectExtensions.JavaCast<IShortDynamicLink>(await tcs.Task);
                return new LayerResponse<string>(shortDynamicLink.ShortLink.ToString());
            }
            catch (System.Exception e)
            {
                return new LayerResponse<string>().AddErrorMessage(TextResources.ErrorLayerResponse_BuildDeepLink_ErrorMessage);
            }
        }
        
        class SuccessListener : Java.Lang.Object, IOnSuccessListener
        {
            private readonly TaskCompletionSource<Object> _taskCompletionSource;

            public SuccessListener(TaskCompletionSource<Java.Lang.Object> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public SuccessListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            public void OnSuccess(Object result)
            { 
                _taskCompletionSource.TrySetResult(result);
            }
        }
        
        class FailureListener : Java.Lang.Object, IOnFailureListener
        {
            private readonly TaskCompletionSource<Object> _completionSource;

            public FailureListener(TaskCompletionSource<Object> completionSource)
            {
                _completionSource = completionSource;
            }

            public FailureListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            public void OnFailure(Exception e)
            {
                _completionSource.TrySetException(e);
            }
        }
    }
}