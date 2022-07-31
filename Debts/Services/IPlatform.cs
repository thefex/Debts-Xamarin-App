namespace Debts.Services
{
    public interface IPlatform
    {
        bool IsIos();

        bool IsAndroid();
    }
}