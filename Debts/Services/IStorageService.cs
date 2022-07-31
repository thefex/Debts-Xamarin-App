namespace Debts.Services
{
    public interface IStorageService
    {
        void Store<TValue>(string key, TValue value);

        bool Contains(string key);
        TValue Get<TValue>(string key, TValue defaultValue = default(TValue));
    }
}