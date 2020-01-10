using System;
using System.Collections.Generic;
using Nager.Date;
using RoadTaxApi.Models;

namespace RoadTaxApi.Utilities
{
    public static class RateCalculator
    {

        public static int GetCurrentRate(DateTime timeStamp)
        {
            var rates = GetTimeRates();

            foreach (var item in rates)
            {
                if (IsTimeBetween(timeStamp, item.Start, item.End))
                    return item.Fee;
            }

            return 0;
        }

        internal static bool IsTimeBetween(DateTime timeStamp, TimeSpan startTime, TimeSpan endTime)
        {

            if (endTime == startTime)
            {
                return true;
            }
            else if (endTime < startTime)
            {
                return timeStamp.TimeOfDay <= endTime ||
                    timeStamp.TimeOfDay >= startTime;
            }

            else
            {
                return timeStamp.TimeOfDay >= startTime &&
                    timeStamp.TimeOfDay <= endTime;
            }
        }

        public static bool IsHoliday(DateTime date)
        {
            foreach (var item in GetHolidays())
            {
                if (date.ToShortDateString() == item.ToShortDateString())
                {
                    return true;
                }
            }

            return date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday;
        }

        public static IEnumerable<DateTime> GetHolidays()
        {
            List<DateTime> dates = new List<DateTime>();
            var holidays = DateSystem.GetPublicHoliday(DateTime.Now.Year, CountryCode.SE);

            foreach (var day in holidays)
            {
                var holiday = day.Date;
                var dayBefore = day.Date.AddDays(-1);
                dates.Add(holiday);
                dates.Add(dayBefore);
            }

            return dates;
        }

        internal static IEnumerable<TimeRates> GetTimeRates()
        {
            return new List<TimeRates>()
            {
                new TimeRates (new TimeSpan(6,0,0 ), new TimeSpan(6,29,0), 9),
                new TimeRates (new TimeSpan(6,30,0 ), new TimeSpan(6,59,0), 16),
                new TimeRates (new TimeSpan(7,0,0 ), new TimeSpan(7,59,0), 22),
                new TimeRates (new TimeSpan(8,0,0 ), new TimeSpan(8,29,0), 16),
                new TimeRates (new TimeSpan(8,3,0 ), new TimeSpan(14,59,0), 9),
                new TimeRates (new TimeSpan(15,0,0 ), new TimeSpan(15,29,0), 16),
                new TimeRates (new TimeSpan(15,30,0 ), new TimeSpan(16,59,0), 22),
                new TimeRates (new TimeSpan(17,0,0 ), new TimeSpan(17,59,0), 16),
                new TimeRates (new TimeSpan(18,0,0 ), new TimeSpan(18,29,0), 16)
            };
        }

    }
}
