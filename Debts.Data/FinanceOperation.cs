using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Debts.Data.Annotations;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Debts.Data
{
    [Table("finance_operations")]
    public class FinanceOperation : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]	
        public long FinancePrimaryId { get; set; }
        
        public string Title { get; set; }
        
        [ManyToOne]
        public ContactDetails RelatedTo { get; set; }
        
        [ForeignKey(typeof(ContactDetails))]
        public long AssignedContactId { get; set; }
        
        [ForeignKey(typeof(PaymentDetails))]
        public long AssignedPaymentId { get; set; }

        [Indexed]
        public FinanceOperationType Type { get; set; }
        
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public PaymentDetails PaymentDetails { get; set; }

        [Indexed]
        public DateTime CreatedAt { get; set; }

        [Ignore]
        public bool IsDebt => Type == FinanceOperationType.Debt;

        [Ignore]
        public bool IsLoan => Type == FinanceOperationType.Loan;
        
        public bool IsFavourite { get; set; }

        public double? Latitude { get; set; }
        
        public double? Longitude { get; set; }
        
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Note> Notes { get; set; }

        public DateTime? RecentNotificationSentDate { get; set; }
        
        [Ignore] public bool HasLocation => Latitude.HasValue && Longitude.HasValue;

        [Ignore] public bool IsPaid => PaymentDetails?.PaymentDate.HasValue ?? false;

        [Ignore] public bool IsActive => !IsPaid;// && PaymentDetails?.DeadlineDate > DateTime.UtcNow;

        [Ignore] public bool IsDeadlineExceed => !IsPaid && PaymentDetails?.DeadlineDate <= DateTime.UtcNow;

        public override int GetHashCode() => FinancePrimaryId.GetHashCode();

        public override bool Equals(object obj) => obj is FinanceOperation operation && operation.FinancePrimaryId.Equals(FinancePrimaryId);

        public void OnPaymentStateChanged()
        { 
            OnPropertyChanged(nameof(PaymentDetails));
        }

        public void RaiseTitleAmountChanged()
        {
            OnPropertyChanged(nameof(Title));
            PaymentDetails?.RaiseAmountChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FinanceOperation Clone()
        {
            return new FinanceOperation()
            {
                Title = Title,
                PaymentDetails = PaymentDetails,
                AssignedPaymentId = AssignedPaymentId,
                AssignedContactId = AssignedContactId,
                FinancePrimaryId = FinancePrimaryId,
                Type = Type,
                Latitude = Latitude,
                Longitude = Longitude,
                Notes = Notes,
                RelatedTo = RelatedTo,
                CreatedAt = CreatedAt,
                IsFavourite = IsFavourite,
                RecentNotificationSentDate = RecentNotificationSentDate
            };
        }
    }
}