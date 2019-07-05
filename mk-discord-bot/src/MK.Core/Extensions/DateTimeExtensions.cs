using System;
using System.Collections.Generic;
using System.Text;

namespace MK.Core
{
    public static class DateTimeExtensions
    {
        public static DateTime ClosestWeekDay(this DateTime date, DayOfWeek weekday, bool includeStartDate = true, bool? searchForward = true)
        {
            if (!searchForward.HasValue && !includeStartDate)
            {
                throw new ArgumentException("if searching in both directions, start date must be a valid result");
            }
            var day = date.DayOfWeek;
            int add = ((int)weekday - (int)day);
            if (searchForward.HasValue)
            {
                if (add < 0 && searchForward.Value)
                {
                    add += 7;
                }
                else if (add > 0 && !searchForward.Value)
                {
                    add -= 7;
                }
                else if (add == 0 && !includeStartDate)
                {
                    add = searchForward.Value ? 7 : -7;
                }
            }
            else if (add < -3)
            {
                add += 7;
            }
            else if (add > 3)
            {
                add -= 7;
            }
            return date.AddDays(add);
        }
    }
}
