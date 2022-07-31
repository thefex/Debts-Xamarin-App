using System.Collections.Generic;
using Debts.Data;
using Debts.Messenging.Messages.App;
using Debts.Services;
using MvvmCross;

namespace Debts.Messenging
{
    
    public class MessageQueueInitialization
    {
        private MessageQueueInitialization()
        {
            
        }
        
        public static void Initialize()
        {
            ServicesLocation.MessageQueue.Init<BudgetCategorySelectedMvxMessage>();
            ServicesLocation.MessageQueue.Init<ContactSelectedMvxMessage>();
            ServicesLocation.MessageQueue.Init<ItemPublishedMessage<IEnumerable<ContactDetails>>>();
            ServicesLocation.MessageQueue.Init<ItemPublishedMessage<Note>>();
        }     
        
    }
}