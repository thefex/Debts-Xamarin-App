using System;
using Android.Content;
using Android.Content.PM;
using Android.Icu.Util;
using Debts.Services;
using Debts.Services.AppGrowth;
using Java.Text;

namespace Debts.Droid.Services.AppGrowth
{
    public class AndroidAppRelatedStatisticsService : AppRelatedStatisticsService
    {
        private readonly Context _context;

        public AndroidAppRelatedStatisticsService(IStorageService storageService, Context context) : base(storageService)
        {
            _context = context;
        }

        public override DateTime GetInstallationDate()
        {
            var datetime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            var firstInstallInUnixEpochMs = _context
                .PackageManager
                .GetPackageInfo(_context.PackageName, PackageInfoFlags.Activities)
                .FirstInstallTime;
 
            return datetime.AddMilliseconds(firstInstallInUnixEpochMs);
        }
    }
}