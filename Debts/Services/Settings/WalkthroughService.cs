using System.Collections.Generic;
using System.Linq;
using Debts.Model.Walkthrough;

namespace Debts.Services.Settings
{
    public class WalkthroughService
    {
        private readonly IStorageService _storageService;
        private const string ImportContactsWalkthroughStorageKey = "ImportContactsWalkthroughStorageKey";
        private const string MainWalkthroughStorageKey = "MainWalkthroughStorageKey";
        private const string FinanceDetailsWalkthroughStorageKey = "FinanceDetailsWalkthroughStorageKey";
        private const string StatisticsWalkthroughStorageKey = "StatisticsWalkthroughStorageKey";
        private const string ListContactsWalkthroughStorageKey = "ListContactsWalkthroughStorageKey";
        private const string BudgetWalkthroughStorageKey = "BudgetWalkthroughStorageKey";

        public WalkthroughService(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        public IEnumerable<MainWalkthroughType> GetWalkthroughTypesToShowForMainView()
        {
            var mainWalkthroughData = _storageService.Get(MainWalkthroughStorageKey,
                GetAllMainWalkthroughTypes().Select(x => new MainWalkthroughData()
                {
                    HasBeenShown = false,
                    Type = x
                }));

            return mainWalkthroughData.Where(x => !x.HasBeenShown).Select(x => x.Type);
        }
        
        public void SetMainWalkthroughAsShown(IEnumerable<MainWalkthroughType> types)
        {
            var mainWalkthroughData = _storageService.Get(MainWalkthroughStorageKey,
                GetAllMainWalkthroughTypes().Select(x => new MainWalkthroughData()
                {
                    HasBeenShown = false,
                    Type = x
                })).ToList();

            foreach (var type in types)
            {
                var data = mainWalkthroughData.FirstOrDefault(x => x.Type == type);
                if (data != null)
                    data.HasBeenShown = true;
            }
            
           _storageService.Store(MainWalkthroughStorageKey, mainWalkthroughData);
        }
        
        public IEnumerable<FinanceWalkthroughType> GetWalkthroughTypesToShowForFinanceDetailsView(bool canBeFinalized, bool hasPhoneNumber)
        {
            var financeWalkthroughData = _storageService.Get(FinanceDetailsWalkthroughStorageKey,
                GetAllFinanceWalkthroughTypes().Select(x => new FinanceWalkthroughData()
                {
                    HasBeenShown = false,
                    Type = x
                }));

            var typesToShow = financeWalkthroughData.Where(x => !x.HasBeenShown).Select(x => x.Type);
            if (!canBeFinalized)
                typesToShow = typesToShow.Where(x => x != FinanceWalkthroughType.Finalize);
            if (!hasPhoneNumber)
                typesToShow = typesToShow.Where(x => x != FinanceWalkthroughType.Call &&
                                                     x != FinanceWalkthroughType.Sms);

            return typesToShow;
        }
        
        public void SetFinanceDetailsWalkthroughAsShown(IEnumerable<FinanceWalkthroughType> types)
        {
            var financeWalkthroughData = _storageService.Get(FinanceDetailsWalkthroughStorageKey,
                GetAllFinanceWalkthroughTypes().Select(x => new FinanceWalkthroughData()
                {
                    HasBeenShown = false,
                    Type = x
                })).ToList();

            foreach (var type in types)
            {
                var data = financeWalkthroughData.FirstOrDefault(x => x.Type == type);
                if (data != null)
                    data.HasBeenShown = true;
            }
            
            _storageService.Store(FinanceDetailsWalkthroughStorageKey, financeWalkthroughData);
        }

        public bool IsStatisticsTutorialShown()
        {
            return _storageService.Get<bool>(StatisticsWalkthroughStorageKey, false);
        }

        public void SetStatisticsTutorialAsShown()
        {
            _storageService.Store(StatisticsWalkthroughStorageKey, true);
        }

        public bool IsContactListTutorialShown()
        {
            return _storageService.Get(ListContactsWalkthroughStorageKey, false);
        }

        public void SetContactListTutorialAsShown()
        {
            _storageService.Store(ListContactsWalkthroughStorageKey, true);
        }

        public bool IsImportContactsTutorialShown()
        {
            return _storageService.Get<bool>(ImportContactsWalkthroughStorageKey, false);
        }

        public void SetImportContactsTutorialAsShown()
        {
            _storageService.Store<bool>(ImportContactsWalkthroughStorageKey, true);
        }

        public bool IsBudgetListTutorialShown()
        {
            return _storageService.Get(BudgetWalkthroughStorageKey, false);
        }

        public void SetBudgetListTutorialAsShown()
        {
            _storageService.Store(BudgetWalkthroughStorageKey, true);
        }

        private IEnumerable<MainWalkthroughType> GetAllMainWalkthroughTypes()
        {
            yield return MainWalkthroughType.Add;
            yield return MainWalkthroughType.Menu;
            yield return MainWalkthroughType.FilterByDates;
            yield return MainWalkthroughType.FilterByType;
        }
  
        IEnumerable<FinanceWalkthroughType> GetAllFinanceWalkthroughTypes()
        {
            yield return FinanceWalkthroughType.Call;
            yield return FinanceWalkthroughType.Finalize;
            yield return FinanceWalkthroughType.Share;
            yield return FinanceWalkthroughType.Sms;
            yield return FinanceWalkthroughType.AddNote;
            yield return FinanceWalkthroughType.Favorite;
        }

        class FinanceWalkthroughData
        {
            public FinanceWalkthroughType Type { get; set; }
            
            public bool HasBeenShown { get; set; }
        }

        class MainWalkthroughData
        {
            public MainWalkthroughType Type { get; set; }
            
            public bool HasBeenShown { get; set; }
        }
    }
}