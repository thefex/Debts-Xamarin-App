using Debts.Data;
using Debts.ViewModel;

namespace Debts.Services.Notifications
{
    public class FinanceOperationNotificationHandler
    {
        public void HandleNotification(FinanceOperation forFinanceOperation)
        {
            ServicesLocation.NavigationService.Navigate<FinanceDetailsViewModel, FinanceOperation>(forFinanceOperation);
        }
    }
}