using System.Collections.Generic;
using Debts.Data;
using Debts.Model;
using MvvmCross.AdvancedRecyclerView.Data;

namespace Debts.Droid.Core.Grouping
{
    public class BudgetGroupingDataConverter : MvxExpandableDataConverter
    {
        private Dictionary<string, int> _childItemUniqueMap = new Dictionary<string, int>();
        private Dictionary<string, int> _groupItemUniqueMap = new Dictionary<string, int>();
        private int keyId = 0;
        private int groupKeyId = 0;
        
        public override MvxGroupedData ConvertToMvxGroupedData(object item)
        {
            var budgetItemGroup = item as BudgetItemGroup;

            int key = 0;

            if (_groupItemUniqueMap.ContainsKey(budgetItemGroup.GetGroupUniqueKey()))
                key = _groupItemUniqueMap[budgetItemGroup.GetGroupUniqueKey()];
            else
            {
                key = groupKeyId;
                _groupItemUniqueMap.Add(budgetItemGroup.GetGroupUniqueKey(), groupKeyId++);
            }

            var groupedData = new MvxGroupedData()
            {
                Key = budgetItemGroup.ForDate,
                GroupItems = budgetItemGroup.GroupChilds,
                UniqueId = key
            };

            return groupedData;
        }

        protected override long GetChildItemUniqueId(object item)
        {
            string key = string.Empty;

            key = "child" + (item as BudgetItem).Id;
                
            if (_childItemUniqueMap.ContainsKey(key))
                return _childItemUniqueMap[key];
            keyId++;
            _childItemUniqueMap.Add(key, keyId);
            return _childItemUniqueMap[key];
        }
    }
}