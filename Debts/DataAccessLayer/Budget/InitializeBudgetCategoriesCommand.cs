using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Resources;
using SQLite;

namespace Debts.DataAccessLayer.Budget
{
    public class InitializeBudgetCategoriesCommand : IDataCommand
    {
        public async Task ExecuteCommand(SQLiteAsyncConnection connection)
        {
            var appSchema = await connection.Table<AppSchemaVersionData>().ToListAsync();

            if (appSchema.Any(x => x.Name == "budget"))
                return;

            await connection.InsertAsync(new AppSchemaVersionData() {Name = "budget", Number = 1});
            BudgetCategory foodDrinks = new BudgetCategory()
            {
                AssetName = "food_fork_drink",
                Name = TextResources.Budget_Category_Main_FoodAndDrinks,
                ColorHex = "#E60094"
            };

            BudgetCategory shopping = new BudgetCategory()
            {
                AssetName = "shopping",
                Name = TextResources.Budget_Category_Main_Shopping,
                ColorHex = "#570aeb"
            };

            BudgetCategory house = new BudgetCategory()
            {
                AssetName = "home_city",
                Name = TextResources.Budget_Category_Main_House,
                ColorHex = "#FF4081"
            };

            BudgetCategory transport = new BudgetCategory()
            {
                AssetName = "train_car",
                Name = TextResources.Budget_Category_Main_Transport,
                ColorHex = "#7ccfc9"
            };

            BudgetCategory car = new BudgetCategory()
            {
                AssetName = "car",
                Name = TextResources.Budget_Category_Main_Car,
                ColorHex = "#DC143C"
            };

            BudgetCategory entertaimentLife = new BudgetCategory()
            {
                AssetName = "human_handsup",
                Name = TextResources.Budget_Category_Main_Entertaiment,
                ColorHex = "#4422EE"
            };

            BudgetCategory communicationsElectronics = new BudgetCategory()
            {
                AssetName = "laptop_mac",
                Name = TextResources.Budget_Category_Electronics,
                ColorHex = "#e04f5f"
            };

            BudgetCategory bankRelated = new BudgetCategory()
            {
                AssetName = "bank",
                Name = TextResources.Budget_Category_Bank,
                ColorHex = "#21a9e2"
            };

            BudgetCategory party = new BudgetCategory()
            {
                AssetName = "party",
                Name = "Party",
                ColorHex = "#32CD32"
            };

            BudgetCategory kids = new BudgetCategory()
            {
                AssetName = "kids",
                Name = "Kids",
                ColorHex = "#12a360"
            };
            
            BudgetCategory investments = new BudgetCategory()
            {
                AssetName = "cash_multiple",
                Name = TextResources.Budget_Category_Investments,
                ColorHex = "#212121"
            };

            BudgetCategory income = new BudgetCategory()
            {
                Name = "Income",
                AssetName = "coin_outline",
                ColorHex = "#ff8800"
            };

            BudgetCategory regularExpenses = new BudgetCategory()
            {
                Name = "Regular Expenses",
                AssetName = "expenses_regular",
                ColorHex = "#385afd"
            };

            BudgetCategory healthAndBeauty = new BudgetCategory()
            {
                Name = "Health & Beauty",
                AssetName = "pill",
                ColorHex = "#a03c53"
            };

            BudgetCategory other = new BudgetCategory()
            {
                Name = "Other",
                AssetName = "Other",
                ColorHex = "#b73151"
            };

            var categories = new List<BudgetCategory>()
            {
                party,
                kids,
                foodDrinks,
                shopping,
                house,
                transport,
                car,
                entertaimentLife,
                communicationsElectronics,
                bankRelated,
                investments,
                income,
                other,
                regularExpenses,
                healthAndBeauty
            };
 
            foreach (var category in categories) 
                await connection.InsertAsync(category);
        }
    }
}