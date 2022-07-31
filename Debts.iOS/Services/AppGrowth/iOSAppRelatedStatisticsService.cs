using System;
using System.Linq;
using Debts.Services;
using Debts.Services.AppGrowth;
using Foundation;
using MvvmCross.Platforms.Ios;

namespace Debts.iOS.Services.AppGrowth
{
    public class iOSAppRelatedStatisticsService : AppRelatedStatisticsService
    {
        public iOSAppRelatedStatisticsService(IStorageService storageService) : base(storageService)
        {
        }

        public override DateTime GetInstallationDate()
        {
            var urlToDocumentsFolder = NSFileManager.DefaultManager
                .GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).LastOrDefault();
            
            if (urlToDocumentsFolder == null)
                return DateTime.UtcNow;

            try
            {
                return NSFileManager.DefaultManager.GetAttributes(urlToDocumentsFolder.Path).CreationDate.ToDateTimeUtc();
            }
            catch (Exception)
            {
                return DateTime.UtcNow;
            } 
        }
    }
}