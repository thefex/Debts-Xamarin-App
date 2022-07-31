using System;
using System.Globalization;
using System.Text;
using Android.OS;
using Android.Text;
using Debts.Droid.Core.Extensions;
using MvvmCross.Converters;

namespace Debts.Droid.Core.Converters
{
    public class SearchTextValueConverter : MvxValueConverter<string, ISpanned>
    {
        const string fontColorTagEnter = "<font color=\"hexcolorhere\">";
        const string fontColorTagClose = "</font>";

        public string ColorHex { get; set; } = "#FF4081";

        protected override ISpanned Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            var formattedHtmlValue = value;

            var startTag = fontColorTagEnter.Replace("hexcolorhere", ColorHex);
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                StringBuilder formattedHtmlStringBuilder = new StringBuilder();
                StringBuilder temporaryStringBuilder = new StringBuilder();
                int k = 0;

                for (int i = 0; i < value.Length; ++i)
                {
                    char valueLowerChar = char.ToLower(value[i]);
                    char searchQueryLowerChar = char.ToLower(SearchQuery[k]);

                    temporaryStringBuilder.Append(value[i]);

                    if (valueLowerChar != searchQueryLowerChar)
                    {
                        k = 0;
                        formattedHtmlStringBuilder.Append(temporaryStringBuilder);
                        temporaryStringBuilder.Clear();
                    } else
                        k++;
					
                    if (k == SearchQuery.Length)
                    {
                        k = 0;
                        formattedHtmlStringBuilder.Append(startTag);
                        formattedHtmlStringBuilder.Append(temporaryStringBuilder);
                        formattedHtmlStringBuilder.Append(fontColorTagClose);

                        temporaryStringBuilder.Clear();
                    }
                }

                if (temporaryStringBuilder.Length > 0)
                    formattedHtmlStringBuilder.Append(temporaryStringBuilder);

                formattedHtmlValue = formattedHtmlStringBuilder.ToString();
            }

            ISpanned htmlText = null;
            
            AndroidExtensions.OnSdk(BuildVersionCodes.N, () =>
            {
                htmlText = Html.FromHtml(formattedHtmlValue);
            }, () => htmlText = Html.FromHtml(formattedHtmlValue, FromHtmlOptions.ModeLegacy));

            return htmlText;
        }

        public string SearchQuery { get; set; } = string.Empty;
    }
}