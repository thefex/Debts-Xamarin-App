using System;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V4.View.Animation;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Debts.Data;
using Debts.Resources;
using Debts.ViewModel.FinancesDetails;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.Presenter
{
    public class AddFinanceDetailsNoteSubPresenter : MvxSubpresenter
    {
        public AddFinanceDetailsNoteSubPresenter(MvxAppPresenter appPresenter) : base(appPresenter)
        {
        }

        public override Task<bool> HandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
        {
            var currentActivity = Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity as AppCompatActivity;
            var viewModelInstance = (request as MvxViewModelInstanceRequest)?.ViewModelInstance as AddFinanceDetailsNoteViewModel;
            new AddNoteController().Show(currentActivity, viewModelInstance); 
            return Task.FromResult(true);
        }


        public override bool ShouldHandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            return request.ViewModelType == typeof(AddFinanceDetailsNoteViewModel);
        }

        public override bool ShouldHandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            return false;
        }

        class AddNoteController
        {
            internal void Show(Activity activity, AddFinanceDetailsNoteViewModel noteViewModel)
            {
                var builder = new Android.Support.V7.App.AlertDialog.Builder(activity);
                var view = LayoutInflater.From(activity).Inflate(Resource.Layout.add_note_dialog, null);
                builder.SetView(view);
                builder.SetTitle(TextResources.AddFinanceOperationNote_Title);
                string content = TextResources.AddFinanceOperationNote_Body; 
                string buttonCancel = TextResources.AddFinanceOperationNote_Cancel;

                switch (noteViewModel.Type)
                {
                    case NoteType.Call:
                        content = "You can enter text note related to the call below.";
                        buttonCancel = "no, thanks";
                        break;
                    case NoteType.Share:
                        content = "You can enter text note related to the shared operation below.";
                        buttonCancel = "no, thanks";
                        break;
                    case NoteType.Sms:
                        content = "You can enter text note related to the sent sms below.";
                        buttonCancel = "no, thanks";
                        break;
                }
                
                builder.SetIcon(Resource.Drawable.ic_app_icon);

                var positiveButton = view.FindViewById<Button>(Resource.Id.add_note_ok);
                var negativeButton = view.FindViewById<Button>(Resource.Id.add_note_cancel);
                negativeButton.Text = buttonCancel;
                var noteField = view.FindViewById<EditText>(Resource.Id.add_note_edit_field);
                var contentText = view.FindViewById<TextView>(Resource.Id.title);

                contentText.Text = content;
                positiveButton.Text = "ok";
                negativeButton.Text ="no, thanks";
                positiveButton.Enabled = noteViewModel.Type != NoteType.Default;
                
                noteField.TextChanged += (sender, args) =>
                {
                    if (noteViewModel.Type == NoteType.Default)
                        positiveButton.Enabled = !string.IsNullOrWhiteSpace(noteField.Text);
                };
                
                var dialog = builder.Create();
                
                positiveButton.Click += (sender, args) =>
                {
                    noteField.Enabled = false;
                    negativeButton.Enabled = false;
                    noteViewModel.Note = noteField.Text; 
                    noteViewModel.AddNote.Execute(); 
                    
                    dialog.Dismiss();
                };
                negativeButton.Click += (sender, args) =>
                {
                    if (noteViewModel.Type != NoteType.Default)
                    {
                        noteViewModel.AddNote.Execute();
                    }

                    dialog.Dismiss(); 
                };
                
                
                dialog.Show();
            }
        }
    }
     
}