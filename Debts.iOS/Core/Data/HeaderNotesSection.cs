using Debts.Model.Sections;

namespace Debts.iOS.Core.Data
{
    public class HeaderNotesSection
    {
        public DetailsNotesSection DetailsNotesSection { get; }
        
        public HeaderNotesSection(DetailsNotesSection detailsNotesSection)
        {
            DetailsNotesSection = detailsNotesSection;
        }
    }
}