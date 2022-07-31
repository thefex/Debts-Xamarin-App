using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Debts.Data.Annotations;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Debts.Data
{
    [Table("payments")]
    public class PaymentDetails : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int PaymentPrimaryId { get; set; }
        
        [ForeignKey(typeof(FinanceOperation))]
        public long AssignedToFinanceOperationId { get; set; }

        private DateTime? _paymentDate;
        public DateTime? PaymentDate
        {
            get => _paymentDate;
            set
            {
                if (_paymentDate != value)
                {
                    _paymentDate = value;
                    OnPropertyChanged(nameof(PaymentDate));
                }
            }
        }

        public DateTime DeadlineDate { get; set; }
        
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string FormattedAmount
        {
            get
            {
                if (Amount > 999)
                    return "$999+";

                return "$" + Amount;
            }
        }

        public void RaiseAmountChanged()
        {
            OnPropertyChanged(nameof(FormattedAmount));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}