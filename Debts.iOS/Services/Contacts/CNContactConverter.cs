using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contacts;
using Debts.Data;
using Foundation;

namespace Debts.iOS.Services
{
    public class CNContactConverter
    {
        public CNContactConverter()
        {
                
        }
        public IEnumerable<ContactDetails> Convert(IEnumerable<CNContact> fromContacts)
        {
            foreach (var contact in fromContacts)
                yield return Convert(contact);
        }
        
        public ContactDetails Convert(CNContact contact)
        {
            var contactDetails = new ContactDetails()
            {
                PhoneNumber = contact.PhoneNumbers.FirstOrDefault()?.Value?.StringValue ?? string.Empty, 
                FirstName = contact.GivenName, 
                LastName = contact.FamilyName,
                DeviceBasedId = contact.Identifier.GetHashCode() * 23 + contact.Nickname.GetHashCode() * 29,
            };

            if (contact.ImageDataAvailable)
            {
                string fileExtension = "png";
                string fileName = "photo_contacts_" + contact.Identifier + "." + fileExtension;
                var pathToFile =
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.MyDocuments), fileName);
                                    
                if (File.Exists(pathToFile))
                    File.Delete(pathToFile);
                
                contact.ImageData.Save(new NSUrl(pathToFile), true);
                contactDetails.AvatarUrl = pathToFile;
            }

            return contactDetails;
        }
    }
}