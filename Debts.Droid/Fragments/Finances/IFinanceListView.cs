using Android.Views;

namespace Debts.Droid.Fragments.Finances
{
    public interface IFinanceListView
    {
        View GetSelectedTitleTextView();

        View GetAvatarImageView();
        
        View GetRootView();
    }
}