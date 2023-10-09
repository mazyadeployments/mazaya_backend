using System;

namespace MMA.WebApi.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 0);
        }

        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        public static DateTime? SpecifyKind(this DateTime? date, DateTimeKind kind)
        {
            if (date.HasValue)
            {
                return DateTime.SpecifyKind(date.Value, kind);
            }

            return null;
        }

        public static DateTime SpecifyKind(this DateTime date, DateTimeKind kind)
        {
            return DateTime.SpecifyKind(date, kind);
        }

        public static DateTime Midday(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 12, 0, 0, 0);
        }

        public static DateTime? Midday(this DateTime? date)
        {
            if (date.HasValue)
            {
                return new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 12, 0, 0, 0);
            }

            return null;
        }
    }
}
