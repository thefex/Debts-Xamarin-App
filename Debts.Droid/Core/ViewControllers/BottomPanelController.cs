using Android.Views;
using Android.Views.Animations;

namespace Debts.Droid.Core.ViewControllers
{
    public class BottomPanelController
    {
        private bool _isPanelVisible;
        public bool IsPanelVisible
        {
            get => _isPanelVisible;
            set
            {
                if (_isPanelVisible != value)
                {
                    _isPanelVisible = value;
                    
                    if (_isPanelVisible)
                        Show();
                    else
                        Hide();
                }
            }
        }

        private View _bottomPanel;
        public void Initialize(View bottomPanel)
        {
            _bottomPanel = bottomPanel;
            bottomPanel.Post(() => { bottomPanel.TranslationY = bottomPanel.Height; });
        }

        void Show()
        {
            _bottomPanel.Post(() =>
            {
                _bottomPanel.Alpha = 1;
                _bottomPanel.Animate()
                    .TranslationY(0)
                    .SetDuration(225)
                    .SetInterpolator(new AccelerateInterpolator(2f));    
            });
            
        }

        void Hide()
        {
            _bottomPanel.Post(() =>
            {
                _bottomPanel.Animate()
                    .TranslationY(_bottomPanel.Height)
                    .SetDuration(225)
                    .SetInterpolator(new AccelerateInterpolator(2f)); 
            });
        }
    }
}