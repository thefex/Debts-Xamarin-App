using System;
using System.Globalization;
using Debts.Data;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class NoteTypeToDrawableNameValueConverter : MvxValueConverter<NoteType, string>
    {
        protected override string Convert(NoteType value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == NoteType.Call)
                return "phone_log";
            else if (value == NoteType.Share)
                return "share_variant";
            else if (value == NoteType.Sms)
                return "message_text";
            else if (value == NoteType.Default)
                return "note_text";
            
            throw new InvalidOperationException(value.ToString());
        }
    }
}