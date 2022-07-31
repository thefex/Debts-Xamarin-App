using Debts.Model.Sections;

namespace Debts.iOS.Core.Data
{
    public class EmptyNotesSection
    {
        public DetailsNotesSection DetailsNotesSection { get; }

        public EmptyNotesSection(DetailsNotesSection detailsNotesSection)
        {
            DetailsNotesSection = detailsNotesSection;
        }
    }
}