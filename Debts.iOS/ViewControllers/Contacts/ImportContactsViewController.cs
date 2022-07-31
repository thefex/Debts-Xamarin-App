using System;
using System.Collections.Generic;
using System.Linq;
using Contacts;
using ContactsUI;
using Debts.Data;
using Debts.iOS.Services;
using Debts.iOS.ViewControllers.Base;
using Debts.ViewModel.Contacts;
using Foundation;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Views.Base;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Contacts
{
    [MvxModalPresentation(Animated = true, ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen, ModalTransitionStyle = UIModalTransitionStyle.CoverVertical)]
    public class ImportContactsViewController : CNContactPickerViewController, IMvxEventSourceViewController, ICNContactPickerDelegate, 
        IMvxIosView<ImportContactsViewModel>, IMvxView<ImportContactsViewModel>  
    {
        private ImportContactsViewModel _viewModel;

        public ImportContactsViewController()
        {
            this.AdaptForBinding();
        }

        public ImportContactsViewController(IntPtr handle) : base(handle)
        {
            this.AdaptForBinding();
        }

        protected ImportContactsViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
            this.AdaptForBinding();
        }

        [Export("contactPicker:didSelectContacts:")]
        public void DidSelectContacts(CNContactPickerViewController picker, CNContact[] contacts)
        {
            CNContactConverter contactConverter = new CNContactConverter();
            var contactDetailsList = contactConverter.Convert(contacts).ToList();

            var importContactsViewModel = (ViewModel as ImportContactsViewModel);
            importContactsViewModel.SetContactsToImport(contactDetailsList);
            importContactsViewModel.Import.Execute();
        }

        [Export("contactPickerDidCancel:")]
        public void ContactPickerDidCancel(CNContactPickerViewController picker)
        {
            (ViewModel as ImportContactsViewModel).Close.Execute();
        }


        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewWillDisappearCalled.Raise(this, animated);
            ViewModel?.ViewDisappearing();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewDidAppearCalled.Raise(this, animated);
            ViewModel?.ViewAppeared();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ViewWillAppearCalled.Raise(this, animated);
            ViewModel?.ViewAppearing();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            ViewDidDisappearCalled.Raise(this, animated);
            ViewModel?.ViewDisappeared();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewDidLoadCalled.Raise(this);  
            ViewModel?.ViewCreated();
            Delegate = this;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            ViewDidLayoutSubviewsCalled.Raise(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeCalled.Raise(this);
            }
            base.Dispose(disposing);
        }


        public event EventHandler ViewDidLoadCalled;

        public event EventHandler ViewDidLayoutSubviewsCalled;

        public event EventHandler<MvxValueEventArgs<bool>> ViewWillAppearCalled;

        public event EventHandler<MvxValueEventArgs<bool>> ViewDidAppearCalled;

        public event EventHandler<MvxValueEventArgs<bool>> ViewDidDisappearCalled;

        public event EventHandler<MvxValueEventArgs<bool>> ViewWillDisappearCalled;

    

        public object DataContext
        {
            get { return BindingContext.DataContext; }
            set { BindingContext.DataContext = value; }
        }

        public IMvxViewModel ViewModel
        {
            get { return DataContext as IMvxViewModel; }
            set { DataContext = value; }
        }

        public MvxViewModelRequest Request { get; set; }

        public IMvxBindingContext BindingContext { get; set; }
    
        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);
            if (parent == null)
                ViewModel?.ViewDestroy();
        }
 
        public event EventHandler DisposeCalled;

        ImportContactsViewModel IMvxView<ImportContactsViewModel>.ViewModel
        {
            get => _viewModel;
            set => _viewModel = value;
        }
    }
}