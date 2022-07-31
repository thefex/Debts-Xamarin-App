using Debts.Data;
using Debts.Services.AppGrowth;
using Firebase.DynamicLinks;
using MvvmCross;
using Newtonsoft.Json;

namespace Debts.iOS.Services.AppGrowth
{
    public class FirebaseDynamicLinkHandler
    {
        public void HandleDynamicLink(DynamicLink link)
        {
            if (link.Url == null)
                return;
            
            var tappedLink = link.Url.ToString();

            var replacedLink = tappedLink
                .Replace("https://www.go-debts.com/#?operation=", "")
                .Replace("%3D", "=");

            var byteJson =
                System.Convert.FromBase64String(replacedLink);
            var jsonString = System.Text.Encoding.UTF8.GetString(byteJson);

            var financeOperation = JsonConvert.DeserializeObject<FinanceOperation>(jsonString);

            Mvx.IoCProvider.Resolve<DeepLinkHandler>().HandleDeepLink(financeOperation);
        }
    }
}