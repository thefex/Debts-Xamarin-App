using System.Collections.Generic;
using System.Linq;
using Debts.Data;
using Debts.Model.Sections;
using MvvmCross.AdvancedRecyclerView.Data;

namespace Debts.Droid.Core.Grouping
{
    public class StatisticsGroupingDataConverter : MvxExpandableDataConverter
    {
        private Dictionary<string, int> _childItemUniqueMap = new Dictionary<string, int>();
        private Dictionary<string, int> _groupItemUniqueMap = new Dictionary<string, int>();
        private int keyId = 0;
        private int groupKeyId = 0;
        
        public override MvxGroupedData ConvertToMvxGroupedData(object item)
        {
            var financeDetailsSection = item as DetailsSection;

            int key = 0;

            string keyName = financeDetailsSection.ImageName + financeDetailsSection.Title;
            if (_groupItemUniqueMap.ContainsKey(keyName))
                key = _groupItemUniqueMap[keyName];
            else
            {
                key = groupKeyId;
                _groupItemUniqueMap.Add(keyName, groupKeyId++);
            }

            var groupedData = new MvxGroupedData()
            {
                Key = financeDetailsSection,
                GroupItems = (financeDetailsSection as DetailsFinanceOperationsSection)?.Operations ?? Enumerable.Empty<object>(),
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