namespace Debts.Data.Queries
{
    public class ContactListQueryParameter : LimitOffsetQueryParameter
    {
        public string SearchQuery { get; set; }
    }
}