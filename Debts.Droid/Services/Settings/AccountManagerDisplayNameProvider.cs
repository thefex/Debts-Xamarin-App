using System;
using System.Linq;
using Android.Accounts;
using Android.App;
using Debts.Services.Settings;

namespace Debts.Droid.Services.Settings
{
    public class AccountManagerDisplayNameProvider : IDisplayNameProvider
    {
        public string GetDefaultDisplayName()
        {
              AccountManager manager = AccountManager.Get(Application.Context);
              Account[] accounts = manager.GetAccountsByType("com.google");

              if (!accounts.Any())
                return "Debts Shared Contact";
              
              var name = accounts.First().Name;
              
              try
              {
                  return name.Split("@")[0];
              }
              catch (Exception)
              {
                  return name;
              }
        }
    }
}