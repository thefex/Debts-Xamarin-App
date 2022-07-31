using System;
using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Android.Views;

namespace Debts.Droid.Core.ViewControllers
{
    public class FinanceOperationDetailsFavAndFabController
    {
        private readonly Func<FloatingActionButton> _fabProvider;

        public FinanceOperationDetailsFavAndFabController(Func<FloatingActionButton> fabProvider)
        {
            _fabProvider = fabProvider;
        }
 
        public IMenuItem FavoriteMenuItem { get; set; }

        private bool _isFavorite;

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
                FavoriteMenuItem?.SetIcon(value ? Resource.Drawable.ic_star_grey600_24dp : Resource.Drawable.ic_star_outline_grey600_24dp);
            }
        }


        private bool isOperationPaid;
        public bool IsOperationPaid
        {
            get => isOperationPaid;
            set
            {
                isOperationPaid = value;
                SetFloatingActionButtonVisibility(value);
            }
        }
     
        void SetFloatingActionButtonVisibility(bool isPaid)
        {
            var fab = _fabProvider();
            if (!isPaid && !fab.IsOrWillBeShown)
                fab.Post(fab.Show);
            else if (isPaid && !fab.IsOrWillBeHidden)
                fab.Post(fab.Hide);
        }
    }
}