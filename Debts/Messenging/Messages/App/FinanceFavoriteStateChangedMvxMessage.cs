using Debts.Data;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages.App
{
    public class FinanceFavoriteStateChangedMvxMessage : MvxMessage
    {
        public FinanceOperation FinanceOperation { get; }

        public FinanceFavoriteStateChangedMvxMessage(object sender, FinanceOperation financeOperation) : base(sender)
        {
            FinanceOperation = financeOperation;
        }
    }
}