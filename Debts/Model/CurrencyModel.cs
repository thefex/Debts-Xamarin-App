namespace Debts.Model
{
    public class CurrencyModel
    {
        public string Currency { get; set; }
        
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is CurrencyModel currencyModel && currencyModel.Currency.Equals(Currency);
        }

        public override string ToString()
        {
            return Currency + " (" + Value + ")";
        }

        public override int GetHashCode() => Currency.GetHashCode();
    }
}