using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Debts.Data.Annotations;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Debts.Data
{
    [Table("contacts")]
    public class ContactDetails : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public long ContactPrimaryId {get; set;}
        
        public long? DeviceBasedId { get; set; }
        
        [Indexed]
        public string FirstName { get; set; }
        
        [Indexed]
        public string LastName { get; set; } 
        
        public string PhoneNumber { get; set; }
        
        public string AvatarUrl { get; set; }
     
        public bool IsSharedContact { get; set; }

        [Ignore] public string FullName => (FirstName ?? string.Empty) + " " + (LastName ?? string.Empty);

        public override string ToString() => FullName;

        public override bool Equals(object obj)
        {
            return obj is ContactDetails contactDetails &&
                   contactDetails.ContactPrimaryId.Equals(ContactPrimaryId);
        }

        public override int GetHashCode()
        {
            return ContactPrimaryId.GetHashCode();
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<FinanceOperation> Operations { get; set; }


        [Ignore]
        public decimal ActiveLoansAmount => Operations?
            .Where(x => x.IsLoan && !(x.PaymentDetails?.PaymentDate.HasValue ?? true))
            .Sum(x => x.PaymentDetails.Amount) ?? 0;
        
        [Ignore]
        public decimal ActiveDebtsAmount => Operations?
            .Where(x => x.IsDebt && !(x.PaymentDetails?.PaymentDate.HasValue ?? true))
            .Sum(x => x.PaymentDetails.Amount) ?? 0;
        
        public void RaiseContactNamePropertyChanged()
        {
            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(FullName));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}