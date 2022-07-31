using Debts.Data;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages.App
{
    public class BudgetCategorySelectedMvxMessage : MvxMessage
    {
        public BudgetCategory BudgetCategory { get; }

        public BudgetCategorySelectedMvxMessage(object sender, BudgetCategory budgetCategory) : base(sender)
        {
            BudgetCategory = budgetCategory;
        }
    }
}