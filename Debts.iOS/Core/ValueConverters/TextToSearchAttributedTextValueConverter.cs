using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Debts.Extensions;
using Debts.iOS.Config;
using Foundation;
using MvvmCross.Converters;
using UIKit;

namespace Debts.iOS.Core.ValueConverters
{
    class TextToSearchAttributedTextValueConverter : MvxValueConverter<string, NSAttributedString>
    {
        protected override NSAttributedString Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            value = value.Trim('\n');		
            string searchWord = RecentSearchWord ?? string.Empty;
            if (string.IsNullOrEmpty(value))
                return new NSAttributedString(value);
			
            List<int> attributedTextStartIndexes = value.AllIndexOf(searchWord).ToList();

            NSMutableAttributedString attributedString = new NSMutableAttributedString(value ?? string.Empty);
            foreach (var attributedTextStartIndex in attributedTextStartIndexes)
            {
                var stringAttribute = new UIStringAttributes()
                {
                    ForegroundColor = AppColors.Accent
                };
                attributedString.SetAttributes(stringAttribute, new NSRange(attributedTextStartIndex, searchWord.Length));
            }

            return attributedString;
        }

        public string RecentSearchWord { get; set; } = string.Empty;
    }
}