using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages.App
{
    public class RequestFinalizeFinanceOperationMessage : MvxMessage
    {
        public RequestFinalizeFinanceOperationMessage(object sender) : base(sender)
        {
        }
    }
}