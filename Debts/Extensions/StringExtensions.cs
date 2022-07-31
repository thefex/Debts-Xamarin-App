using System;
using System.Collections.Generic;
using System.Linq;

namespace Debts.Extensions
{
    public static class StringExtensions
    {
        public static string GetFirstLettersFromEachWord(this string text)
        {
            var splittedText = text.Trim().Split(' ');

            return new string(splittedText.Where(x => x.Length > 0).Select(x => char.ToUpper(x[0])).ToArray());
        }

        public static string GetFirstAndLastLetter(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }
            return text.Length == 1 ? text : $"{text[0]}{text[text.Length - 1]}";
        }

        // CAUTION!
        // I am perfectly fine with fact that is method has ~ O(|Text|*|SubstringToSeekFor|) worst case complexity
        // I strongly doubt that this will ever cause any performance issues - in case you want to improve that, measure first.
        public static IEnumerable<int> AllIndexOf(this string text, string substringToSeekFor)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(substringToSeekFor))
                yield break;

            for (int i = 0; i < text.Length; i += substringToSeekFor.Length)
            {
                int indexOf = text.IndexOf(substringToSeekFor, i, StringComparison.OrdinalIgnoreCase);
                if (indexOf == -1)
                    yield break;

                yield return indexOf;
            }
        }

        public static string GetInitials(this string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.GetFirstLettersFromEachWord().GetFirstAndLastLetter();
        }
    }
}