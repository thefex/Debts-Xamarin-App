using System.Collections.Generic;
using Debts.Data;
using Debts.Model;
using MvvmCross.AdvancedRecyclerView.Data;

namespace Debts.Droid.Core.Grouping
{
    public class FinanceOperationGroupingDataConverter : MvxExpandableDataConverter
    {
        private Dictionary<string, int> _childItemUniqueMap = new Dictionary<string, int>();
        private Dictionary<string, int> _groupItemUniqueMap = new Dictionary<string, int>();
        private int keyId = 0;
        private int groupKeyId = 0;
        
        public override MvxGroupedData ConvertToMvxGroupedData(object item)
        {
            var financesOperationGroup = item as FinancesOperationGroup;

            int key = 0;

            if (_groupItemUniqueMap.ContainsKey(financesOperationGroup.GetGroupUniqueKey()))
                key = _groupItemUniqueMap[financesOperationGroup.GetGroupUniqueKey()];
            else
            {
                key = groupKeyId;
                _groupItemUniqueMap.Add(financesOperationGroup.GetGroupUniqueKey(), groupKeyId++);
            }

            var groupedData = new MvxGroupedData()
            {
                Key = financesOperationGroup.GroupType == FinanceGroupType.Active ? financesOperationGroup.ForDate : (object)financesOperationGroup.GroupType,
                GroupItems = financesOperationGroup.GroupChilds,
                UniqueId = key
            };

            return groupedData;
        }

        protected override long GetChildItemUniqueId(object item)
        {
            string key = string.Empty;

            key = "child" + (item as FinanceOperation).FinancePrimaryId;
                
            if (_childItemUniqueMap.ContainsKey(key))
                return _childItemUniqueMap[key];
            keyId++;
            _childItemUniqueMap.Add(key, keyId);
            return _childItemUniqueMap[key];
        }
    }
}