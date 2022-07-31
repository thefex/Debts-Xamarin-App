using System;
using Debts.Services.Settings;
using UIKit;

namespace Debts.iOS.Services
{
    public class iOSDisplayNameProvider : IDisplayNameProvider
    {
        public string GetDefaultDisplayName()
        {
            var deviceName = UIDevice.CurrentDevice.Name;
            int index = deviceName.IndexOf("iphone", StringComparison.OrdinalIgnoreCase);

            if (index == 0)
                return "Debts Shared Contact";
                
            if (index > 0) 
                deviceName = deviceName.Substring(0, index);

            return !string.IsNullOrEmpty(deviceName) ? deviceName : "Debts Shared Contact";
        }
    }
}