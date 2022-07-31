using System.Collections.Generic;
using Debts.Data;
using Debts.Model;
using MvvmCross.AdvancedRecyclerView.Data;

namespace Debts.Droid.Core.Grouping
{
    public class ContactListGroupingDataConverter : MvxExpandableDataConverter
    {
        private Dictionary<string, int> _groupItemUniqueMap = new Dictionary<string, int>();

        private int groupKeyId = 0;
        
        public override MvxGroupedData ConvertToMvxGroupedData(object item)
        {
            var contactGroup = item as ContactsGroup;

            long uniqueId = 0;

            if (!_groupItemUniqueMap.ContainsKey(contactGroup.GroupingLetter))
                _groupItemUniqueMap.Add(contactGroup.GroupingLetter, groupKeyId++);
            
            uniqueId = _groupItemUniqueMap[contactGroup.GroupingLetter];
            
            return new MvxGroupedData()
            {
                Key = contactGroup,
                GroupItems = contactGroup.GroupChilds,
                UniqueId = uniqueId + 99999999
            };
        }

        protected override long GetChildItemUniqueId(object item)
        {
            if (item is SelectableItem<ContactDetails> selectableItem)
                return selectableItem.Item.ContactPrimaryId;

            return (item as ContactDetails).ContactPrimaryId;
        }
    }
}