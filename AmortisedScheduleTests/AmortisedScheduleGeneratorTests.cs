namespace AmortisedScheduleTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xunit;

    public class AmortisedScheduleGeneratorTests
    {
        private static List<DateTime> BankHolidays() =>
            new List<DateTime>()
            {
                new DateTime(2020, 12, 25),
                new DateTime(2020, 12, 28),
                new DateTime(2021, 01, 01),
                new DateTime(2021, 04, 02),
                new DateTime(2021, 04, 05),
                new DateTime(2021, 05, 03),
                new DateTime(2021, 05, 31),
                new DateTime(2021, 08, 30),
                new DateTime(2021, 12, 27),
                new DateTime(2021, 12, 28),
                new DateTime(2022, 01, 03),
                new DateTime(2022, 04, 15),
                new DateTime(2022, 04, 18),
                new DateTime(2022, 05, 02),
                new DateTime(2022, 05, 30),
                new DateTime(2022, 08, 29),
                new DateTime(2022, 12, 26),
                new DateTime(2022, 12, 27),
            };

        private const int NumberOfPayments = 24;
        private static readonly DateTime startDate = new DateTime(2020, 9, 5);

        [Theory]
        [InlineData(100000, 0.28, 2, 0.0208)]
        [InlineData(100000, 0.30, 3, 0.0221)]
        [InlineData(100000, 0.33, 5, 0.0240)]
        [InlineData(100000, 0.36, 7, 0.0260)]
        public void MonthlyApr_ReturnsExpectedValue(decimal principal, double interestRate, int amortisationInYears, double expected)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.MonthlyApr(), 4)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData(100000, 0.28, 2)]
        [InlineData(100000, 0.30, 3)]
        [InlineData(100000, 0.33, 5)]
        [InlineData(100000, 0.36, 7)]
        public void MonthlyApr_ReverseCalculation_ReturnsOriginalValue(decimal principal, double interestRate, int amortisationInYears)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(Math.Pow(1 + test.MonthlyApr(), 12) - 1, 2)
                .Should().Be(interestRate);
        }

        [Theory]
        [InlineData(100000, 0.28, 2, 00.00)]
        [InlineData(100000, 0.30, 3, 33.33)]
        [InlineData(100000, 0.33, 5, 60.00)]
        [InlineData(100000, 0.36, 7, 71.43)]
        public void AmortisationPeriod_GeneratesExpectedBalloonPercentage(decimal principal, double interestRate, int amortisationInYears, decimal expected)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.BalloonPercentage() * 100, 2).Should().Be(expected);
        }

        [Theory]
        [InlineData(100000, 0.28, 2, 0)]
        [InlineData(100000, 0.30, 3, 33333.33)]
        [InlineData(100000, 0.33, 5, 60000.00)]
        [InlineData(100000, 0.36, 7, 71428.57)]
        public void Schedule_ReturnsExpectedBalloonAmount(decimal principal, double interestRate, int amortisationInYears, decimal expected)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.BalloonAmount(), 2)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData(100000, 0.28, 2, 5334.23)]
        [InlineData(100000, 0.30, 3, 4346.14)]
        [InlineData(100000, 0.33, 5, 3656.07)]
        [InlineData(100000, 0.36, 7, 3468.33)]
        public void Schedule_ReturnsExpectedPaymentAmount(decimal principal, double interestRate, int amortisationInYears, double expected)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.Payment(), 2)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData(100000, 0.28, 2, 2078.47)]
        [InlineData(100000, 0.30, 3, 2210.45)]
        [InlineData(100000, 0.33, 5, 2404.95)]
        [InlineData(100000, 0.36, 7, 2595.48)]
        public void Schedule_GeneratesExpectedFirstInterestAmount(decimal principal, double interestRate, int amortisationInYears, decimal expected)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.MonthlyScheduleItems[0].InterestAmount, 2)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData(100000, 0.28, 2)]
        [InlineData(100000, 0.30, 3)]
        [InlineData(100000, 0.33, 5)]
        [InlineData(100000, 0.36, 7)]
        public void Schedule_GeneratesExpectedTotalPrincipal(decimal principal, double interestRate, int amortisationInYears)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.MonthlyScheduleItems.Sum(x => x.PrincipalAmount), 2)
                .Should().Be(principal);
        }

        [Theory]
        [InlineData(100000, 0.28, 2, 28021.43)]
        [InlineData(100000, 0.30, 3, 37640.71)]
        [InlineData(100000, 0.33, 5, 47745.66)]
        [InlineData(100000, 0.36, 7, 54668.39)]
        public void Schedule_GeneratesExpectedTotalInterest(decimal principal, double interestRate, int amortisationInYears, decimal expected)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.MonthlyScheduleItems.Sum(x => x.InterestAmount), 2)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData(100000, 0.28, 2, 108.61)]
        [InlineData(100000, 0.30, 3, 814.87)]
        [InlineData(100000, 0.33, 5, 1494.95)]
        [InlineData(100000, 0.36, 7, 1894.76)]
        public void Schedule_GeneratesExpectedFinalInterest(decimal principal, double interestRate, int amortisationInYears, decimal expected)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.MonthlyScheduleItems[^1].InterestAmount, 2)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData(100000, 0.28, 2, 5225.61)]
        [InlineData(100000, 0.30, 3, 36864.60)]
        [InlineData(100000, 0.33, 5, 62161.12)]
        [InlineData(100000, 0.36, 7, 73002.14)]
        public void Schedule_GeneratesExpectedFinalPrincipal(decimal principal, double interestRate, int amortisationInYears, decimal expected)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.MonthlyScheduleItems[^1].PrincipalAmount, 2)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData(100000, 0.28, 2, 5119.21)]
        [InlineData(100000, 0.30, 3, 3454.90)]
        [InlineData(100000, 0.33, 5, 2110.37)]
        [InlineData(100000, 0.36, 7, 1533.76)]
        public void Schedule_GeneratesExpectedPenultimatePrincipal(decimal principal, double interestRate, int amortisationInYears, decimal expected)
        {
            var test = new AmortisedScheduleGenerator(principal, interestRate, NumberOfPayments, amortisationInYears, startDate, BankHolidays());
            Math.Round(test.MonthlyScheduleItems[^2].PrincipalAmount, 2)
                .Should().Be(expected);
        }

        [Fact]
        public void FirstPaymentDate_IsOverOneMonthAway()
        {
            var test = new AmortisedScheduleGenerator(100000, 0.3, NumberOfPayments, 3, startDate, BankHolidays());
            test.MonthlyScheduleItems[0].Date
                .Should().BeOnOrAfter(startDate.AddMonths(1).Date);
        }

        [Fact]
        public void FirstPaymentDate_IsInNextCalendarMonth()
        {
            var nextMonth = startDate.Date.AddMonths(1);
            var test = new AmortisedScheduleGenerator(100000, 0.3, NumberOfPayments, 3, startDate, BankHolidays());

            test.MonthlyScheduleItems[0].Date.Should().HaveMonth(nextMonth.Month);
            test.MonthlyScheduleItems[0].Date.Should().HaveYear(nextMonth.Year);
        }

        [Fact]
        public void FirstPaymentDate_DoesNotIncludeTimeOfDay()
        {
            var test = new AmortisedScheduleGenerator(100000, 0.3, NumberOfPayments, 3, startDate, BankHolidays());
            test.MonthlyScheduleItems[0].Date
                .Should().HaveHour(0);
        }

        [Fact]
        public void SecondPaymentDate_IsOverTwoMonthsAway()
        {
            var test = new AmortisedScheduleGenerator(100000, 0.3, NumberOfPayments, 3, startDate, BankHolidays());
            test.MonthlyScheduleItems[1].Date
                .Should().BeOnOrAfter(startDate.AddMonths(2).Date);
        }

        [Fact]
        public void PaymentDates_DoNotLandOnWeekends()
        {
            var test = new AmortisedScheduleGenerator(100000, 0.3, NumberOfPayments, 3, startDate, BankHolidays());
            foreach (var item in test.MonthlyScheduleItems)
            {
                item.Date.DayOfWeek.Should().NotBe(DayOfWeek.Saturday);
                item.Date.DayOfWeek.Should().NotBe(DayOfWeek.Sunday);
            }
        }

        [Fact]
        public void PaymentDates_DoNotLandOnBankHolidays()
        {
            var test = new AmortisedScheduleGenerator(100000, 0.3, NumberOfPayments, 3, startDate, BankHolidays());
            foreach (var item in test.MonthlyScheduleItems)
            {
                BankHolidays().Should().NotContain(item.Date);
            }
        }
    }
}
