using System;
using System.Threading.Tasks;
using Debts.iOS.Cells.Contacts;
using Debts.iOS.Services.Walkthrough;
using Debts.iOS.Utilities.TableView;
using Debts.Resources;
using Debts.Services.Settings;
using Debts.ViewModel.Contacts;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace Debts.iOS.ViewControllers.Contacts
{
	[MvxPagePresentation]
    public class ContactsViewController : BaseContactsViewController<ContactListViewModel>
    {
	    public ContactsViewController()
	    {
	    }

	    public ContactsViewController(IntPtr handle) : base(handle)
	    {
	    }

	    public ContactsViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
	    {
	    }

	    public override void ViewDidLoad()
	    {
		    base.ViewDidLoad();
		    
		    Task.Delay(500);
		    var walkthroughService = new ContactListWalkthroughService(Mvx.IoCProvider.Resolve<WalkthroughService>());
		    walkthroughService.ShowIfPossible(); 
	    }

	    protected override string PageTitle => TextResources.ContactList_Title;
    }
}