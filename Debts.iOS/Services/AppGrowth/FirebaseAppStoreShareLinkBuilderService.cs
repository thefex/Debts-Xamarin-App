using System;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Model;
using Debts.Resources;
using Debts.Services.AppGrowth;
using Debts.Services.Settings;
using Firebase.DynamicLinks;
using Foundation;

namespace Debts.iOS.Services.AppGrowth
{
    public class FirebaseAppStoreShareLinkBuilderService : ShareLinkBuilderService
    {
        public FirebaseAppStoreShareLinkBuilderService(SettingsService settingsService) : base(settingsService)
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
                var link = new NSUrl(AppPreviewPageUrl + "?operation=" + operationBase64);
                DynamicLinkComponents dynamicLinkComponents = new DynamicLinkComponents(link, AppLinkUrl);
                var socialTags = new DynamicLinkSocialMetaTagParameters();
                socialTags.Title = title;
                socialTags.DescriptionText = description;
                socialTags.ImageUrl = new NSUrl(imageUrl);

                dynamicLinkComponents.SocialMetaTagParameters = socialTags;
                dynamicLinkComponents.AndroidParameters =
                    DynamicLinkAndroidParameters.FromPackageName(AndroidPackageName);
                dynamicLinkComponents.AndroidParameters.MinimumVersion = AndroidMinimumVersion;
                dynamicLinkComponents.IOSParameters = DynamicLinkiOSParameters.FromBundleId(iOSAppStoreId);
                dynamicLinkComponents.IOSParameters.MinimumAppVersion = iOSMinimumVersion;


                var result = await dynamicLinkComponents.GetShortenUrlAsync();

                return new LayerResponse<string>(result.ShortUrl.ToString());
            }
            catch (System.Exception e)
            {
                return new LayerResponse<string>().AddErrorMessage(TextResources
                    .ErrorLayerResponse_BuildDeepLink_ErrorMessage);
            }
        }
    }
}