// <copyright file="DateTimeExtensions.cs" company="InFund Technologies Ltd">
// Copyright (c) InFund Technologies Ltd. All rights reserved.
// </copyright>

namespace Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Extensions for the <see cref="DateTime"/> and <see cref="DateTimeOffset"/> objects.</summary>
    public static class DateTimeExtensions
    {
        /// <summary>Gets a value indicating whether the day is a working day or not.</summary>
        /// <param name="dateTime">DateTime to check if working day.</param>
        /// <param name="bankHolidays">Collection of bank holidays.</param>
        /// <returns>A <see cref="bool"/> indicating whether the day is a working day or not.</returns>
        public static bool IsWorkingDay(this DateTime dateTime, IEnumerable<DateTime> bankHolidays) =>
            dateTime.DayOfWeek != DayOfWeek.Saturday && dateTime.DayOfWeek != DayOfWeek.Sunday && !bankHolidays.Contains(dateTime.Date);

        /// <summary>Gets a value indicating whether the day is a working day or not.</summary>
        /// <param name="dateTimeOffset">DateTimeOffset to check if working day.</param>
        /// <param name="bankHolidays">Collection of bank holidays.</param>
        /// <returns>A <see cref="bool"/> indicating whether the day is a working day or not.</returns>
        public static bool IsWorkingDay(this DateTimeOffset dateTimeOffset, IEnumerable<DateTime> bankHolidays) =>
            dateTimeOffset.DayOfWeek != DayOfWeek.Saturday && dateTimeOffset.DayOfWeek != DayOfWeek.Sunday && !bankHolidays.Contains(dateTimeOffset.Date);

        /// <summary>Adds a number of working days to a date.</summary>
        /// <param name="dateTime">DateTime to add number of working days to.</param>
        /// <param name="numberOfWorkingDays">Number of working days to add.</param>
        /// <param name="bankHolidays">Collection of bank holidays.</param>
        /// <returns>A <see cref="DateTime"/> of the calculated working day.</returns>
        public static DateTime AddWorkingDays(this DateTime dateTime, int numberOfWorkingDays, IEnumerable<DateTime> bankHolidays) =>
            AddNumberOfWorkingDays(dateTime, numberOfWorkingDays, bankHolidays);

        /// <summary>Adds a number of working days to a date.</summary>
        /// <param name="dateTimeOffset">DateTimeOffset to add number of working days to.</param>
        /// <param name="numberOfWorkingDays">Number of working days to add.</param>
        /// <param name="bankHolidays">Collection of bank holidays.</param>
        /// <returns>A <see cref="DateTimeOffset"/> of the calculated working day.</returns>
        public static DateTimeOffset AddWorkingDays(this DateTimeOffset dateTimeOffset, int numberOfWorkingDays, IEnumerable<DateTime> bankHolidays) =>
            new DateTimeOffset(AddNumberOfWorkingDays(dateTimeOffset.Date, numberOfWorkingDays, bankHolidays), dateTimeOffset.Offset);

        /// <summary>Finds the first future working day after a specified date.</summary>
        /// <param name="dateTime">DateTime to skip to nearest future working day.</param>
        /// <param name="bankHolidays">Collection of bank holidays.</param>
        /// <returns>A <see cref="DateTime"/> of the calculated first future working day.</returns>
        public static DateTime FirstFutureWorkingDay(this DateTime dateTime, IEnumerable<DateTime> bankHolidays) =>
            GetFirstFutureWorkingDay(dateTime, bankHolidays);

        /// <summary>Finds the first future working day after a specified date.</summary>
        /// <param name="dateTimeOffset">DateTimeOffset to skip to nearest future working day.</param>
        /// <param name="bankHolidays">Collection of bank holidays.</param>
        /// <returns>A <see cref="DateTimeOffset"/> of the calculated first future working day.</returns>
        public static DateTimeOffset FirstFutureWorkingDay(this DateTimeOffset dateTimeOffset, IEnumerable<DateTime> bankHolidays) =>
            new DateTimeOffset(GetFirstFutureWorkingDay(dateTimeOffset.Date, bankHolidays), dateTimeOffset.Offset);

        /// <summary>Gets the end date of the previous month.</summary>
        /// <returns>A <see cref="DateTimeOffset"/> for the end date of the previous month.</returns>
        public static DateTimeOffset EndOfPreviousMonth()
        {
            var currentDate = DateTimeOffset.UtcNow.Date;
            return currentDate.AddDays(-currentDate.Day);
        }

        /// <summary>Gets the start date of the current month.</summary>
        /// <returns>A <see cref="DateTimeOffset"/> for the start date of the current month.</returns>
        public static DateTimeOffset StartOfCurrentMonth() =>
            new DateTimeOffset(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));

        private static DateTime AddNumberOfWorkingDays(DateTime dateTime, int numberOfWorkingDays, IEnumerable<DateTime> bankHolidays)
        {
            int leadDayCounter = numberOfWorkingDays;

            if (leadDayCounter > 0)
            {
                while (leadDayCounter > 0)
                {
                    dateTime = dateTime.AddDays(1);
                    if (dateTime.IsWorkingDay(bankHolidays))
                    {
                        leadDayCounter--;
                    }
                }
            }
            else if (leadDayCounter < 0)
            {
                while (leadDayCounter < 0)
                {
                    dateTime = dateTime.AddDays(-1);
                    if (dateTime.IsWorkingDay(bankHolidays))
                    {
                        leadDayCounter++;
                    }
                }
            }

            return dateTime;
        }

        private static DateTime GetFirstFutureWorkingDay(DateTime dateTime, IEnumerable<DateTime> bankHolidays)
        {
            while (!dateTime.IsWorkingDay(bankHolidays))
            {
                dateTime = dateTime.AddDays(1);
            }

            return dateTime;
        }
    }
}