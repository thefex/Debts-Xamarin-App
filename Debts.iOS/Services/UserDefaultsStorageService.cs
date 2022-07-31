using Debts.Services;
using Foundation;
using Newtonsoft.Json;

namespace Debts.iOS.Services
{
    public class UserDefaultsStorageService : IStorageService
    {
        private NSUserDefaults _userDefaults;

        public UserDefaultsStorageService()
        {
            _userDefaults = NSUserDefaults.StandardUserDefaults;
        }
        
        public void Store<TValue>(string key, TValue value)
        {
            _userDefaults.SetString(JsonConvert.SerializeObject(value), key);
        }

        public bool Contains(string key)
        {
            var stringForKey = _userDefaults.StringForKey(key);
            return !string.IsNullOrEmpty(stringForKey);
        }

        public TValue Get<TValue>(string key, TValue defaultValue = default(TValue))
        {
            if (!Contains(key))
                return defaultValue;

            var stringForKey = _userDefaults.StringForKey(key);
            return JsonConvert.DeserializeObject<TValue>(stringForKey);
        }
    }
}