using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Model;
using Debts.Services.Settings;
using Newtonsoft.Json;

namespace Debts.Services.AppGrowth
{
    public abstract class ShareLinkBuilderService
    {
        private readonly SettingsService _settingsService;

        public ShareLinkBuilderService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }
  
        public abstract Task<LayerResponse<string>> BuildDeepLinkForFinanceOperation(FinanceOperation operation);

        protected string GetBase64HashedFinanceOperation(FinanceOperation forOperation)
        {
            var clonedOperation = forOperation.Clone();
            clonedOperation.IsFavourite = true;
            clonedOperation.RecentNotificationSentDate = null;

            clonedOperation.Notes = new List<Note>(); // dont share notes
            clonedOperation.Type = clonedOperation.Type == FinanceOperationType.Debt
                ? FinanceOperationType.Loan
                : FinanceOperationType.Debt;
            
            clonedOperation.AssignedContactId = -1;
            clonedOperation.RelatedTo = new ContactDetails()
            {
                FirstName = _settingsService.DisplayName,
                IsSharedContact = true
            };
            
            var serializedObject = JsonConvert.SerializeObject(clonedOperation, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(serializedObject);
            string serializedOperationAsBase64String = System.Convert.ToBase64String(plainTextBytes);

            return serializedOperationAsBase64String;
        }
        
        public async Task<LayerResponse<string>> BuildShareMessage(FinanceOperation operation)
        {
            var deepLinkResponse = await BuildDeepLinkForFinanceOperation(operation);

            if (!deepLinkResponse.IsSuccess)
                return deepLinkResponse;
            
            var shareMessage = _settingsService.GetShareMessageTemplate(operation) +
                   "Sent via Debts - install to get details of operation inside application." + Environment.NewLine +
                    deepLinkResponse.Results;
            
            return new LayerResponse<string>(shareMessage);
        }

        protected string AndroidPackageName = "com.debts.fex";
        protected int AndroidMinimumVersion = 1;
        
        protected string iOSAppStoreId = "1497382962";
        protected string iOSMinimumVersion = "1";

        protected string AppPreviewPageUrl = "http://debts-app.com";
        protected string AppLinkUrl = "https://debtsmanager.page.link";
    }
}