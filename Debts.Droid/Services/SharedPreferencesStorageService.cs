using Android.App;
using Android.Content;
using Debts.Services;
using Newtonsoft.Json;

namespace Debts.Droid.Services
{
    public class SharedPreferencesStorageService : IStorageService
    {
        private readonly ISharedPreferences _preferences;

        public SharedPreferencesStorageService()
        {
            _preferences = Application.Context.GetSharedPreferences("storage_service", FileCreationMode.Private);
        }
        
        public void Store<TValue>(string key, TValue value)
        {
            _preferences.Edit()
                .PutString(key, JsonConvert.SerializeObject(value))
                .Commit();
        }

        public bool Contains(string key)
        {
            return _preferences.Contains(key);
        }

        public TValue Get<TValue>(string key, TValue defaultValue = default(TValue))
        {
            if (!Contains(key))
                return defaultValue;

            return JsonConvert.DeserializeObject<TValue>(_preferences.GetString(key, string.Empty));
        }
    }
}