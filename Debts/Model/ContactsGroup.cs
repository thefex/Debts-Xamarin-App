using System.Collections.Generic;

namespace Debts.Model
{
    public class ContactsGroup : IGroupedData
    {
        public string GroupingLetter { get; set; }
        public IEnumerable<object> GroupChilds { get; set; }

        public string GetGroupUniqueKey() => GroupingLetter;
    }
}