using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Provider;
using Debts.Data;
using Debts.Services;
using Debts.Services.Contacts;
using Uri = Android.Net.Uri;

namespace Debts.Droid.Services
{
    public class PhoneContactsService : IPhoneContactsService
    {
        private readonly Context _context;
        private readonly IStorageService _storageService;

        public PhoneContactsService(Context context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }
        
        
        private const string HasImportedContactsInPastKey = "HasImportedContactsInPastKey";
        public async Task<IEnumerable<ContactDetails>> GetPhoneContacts()
        {
            var phoneContacts = new List<ContactDetails>();

            using (var phones = _context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null))
            {
                if (phones != null)
                {
                    int id = 0;
                    while (phones.MoveToNext())
                    {
                        try
                        {
                            string name = phones.GetString(phones.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                            string phoneNumber = phones.GetString(phones.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));
                            long deviceBasedId = 
                                phones.GetLong(phones.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id));
                            string avatarUri = 
                                phones.GetString(phones.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.PhotoUri));
                           
                            string[] words = name.Split(' ');
                            var contact = new ContactDetails();
                            
                            contact.FirstName = words[0];
                            if (words.Length > 1)
                                contact.LastName = words[1];
                            else
                                contact.LastName = ""; //no last name
                            contact.PhoneNumber = phoneNumber;
                            contact.DeviceBasedId = deviceBasedId;

                            if (!string.IsNullOrEmpty(avatarUri) && avatarUri.StartsWith("content:"))
                            { 
                                Stream avatarStream = null;
                                try
                                {
                                    avatarStream =
                                        _context.ContentResolver.OpenInputStream(Uri.Parse(avatarUri)); 
                                    
                                    string fileExtension = "png";
                                    string fileName = "photo_contacts_" + deviceBasedId + "." + fileExtension;
                                    var pathToFile =
                                        Path.Combine(
                                            Environment.GetFolderPath(
                                                Environment.SpecialFolder.LocalApplicationData), fileName);
                                    
                                    if (File.Exists(pathToFile))
                                        File.Delete(pathToFile);
                                        
                                    using (var pictureFile = File.Create(pathToFile))
                                    {
                                        await avatarStream.CopyToAsync(pictureFile);
                                    }
                                    
                                    contact.AvatarUrl = pathToFile;
                                }
                                catch (Exception e)
                                {

                                }
                                finally
                                {
                                    avatarStream?.Dispose();
                                }  
                            }

                            contact.ContactPrimaryId = id++;
                            phoneContacts.Add(contact);
                        }
                        catch (Exception ex)
                        {
                            //something wrong with one contact, may be display name is completely empty, decide what to do
                        }
                    }
                    phones.Close(); 
                }
                // if we get here, we can't access the contacts. Consider throwing an exception to display to the user
            }

            return phoneContacts;
        }

        public Task<IEnumerable<ContactDetails>> ImportContacts(IEnumerable<ContactDetails> contacts)
        {
            _storageService.Store(HasImportedContactsInPastKey, true);
            return Task.FromResult(contacts);
        }

        public Task<bool> CheckHasImportedContactsInPast() => Task.FromResult(_storageService.Get(HasImportedContactsInPastKey, false));
        
        public Task SetContactsAsImported()
        {
            _storageService.Store(HasImportedContactsInPastKey, true);
            return Task.FromResult(true);
        }
    }
}