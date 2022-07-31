using System;

namespace Debts.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetEndOfTheDayDate(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }
    }
}