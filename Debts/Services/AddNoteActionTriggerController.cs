using System;
using System.Reactive;
using Debts.Data;

namespace Debts.Services
{
    public class AddNoteActionTriggerController
    {
        public NoteType? RequestedNoteType { get; private set; }
        
        public void AddNoteFinished()
        {
            RequestedNoteType = null;
        }

        public void AddNoteStarted(NoteType noteType)
        {
            RequestedNoteType = noteType;
        }
        
    }
}